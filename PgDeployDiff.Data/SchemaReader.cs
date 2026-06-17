using Npgsql;
using PgDeployDiff.Core;

namespace PgDeployDiff.Data;

public class SchemaReader
{
    private const string ColumnsQuery = """
        SELECT
            t.table_schema,
            t.table_name,
            t.table_type,
            c.column_name,
            c.data_type,
            c.is_nullable,
            c.column_default,
            c.ordinal_position,
            c.character_maximum_length,
            c.numeric_precision,
            c.numeric_scale
        FROM information_schema.tables t
        LEFT JOIN information_schema.columns c
            ON c.table_schema = t.table_schema
            AND c.table_name = t.table_name
        WHERE t.table_schema NOT IN ('information_schema', 'pg_catalog')
            AND t.table_type IN ('BASE TABLE', 'VIEW')
        ORDER BY t.table_schema, t.table_name, c.ordinal_position
        """;

    private const string ViewsQuery = """
        SELECT table_schema, table_name, view_definition
        FROM information_schema.views
        WHERE table_schema NOT IN ('information_schema', 'pg_catalog')
        """;

    public async Task<IReadOnlyList<DbObjectInfo>> ReadObjectsAsync(string connectionString, CancellationToken ct = default)
    {
        var objects = new Dictionary<(string Schema, string Name), (string TableType, List<ColumnInfo> Columns)>();
        var viewDefinitions = new Dictionary<(string, string), string>();

        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync(ct);

        await using (var cmd = new NpgsqlCommand(ColumnsQuery, conn))
        await using (var reader = await cmd.ExecuteReaderAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                var schema = reader.GetString(0);
                var name = reader.GetString(1);
                var tableType = reader.GetString(2);
                var key = (schema, name);

                if (!objects.TryGetValue(key, out var entry))
                {
                    entry = (tableType, []);
                    objects[key] = entry;
                }

                if (!reader.IsDBNull(3))
                {
                    entry.Columns.Add(new ColumnInfo(
                        ColumnName: reader.GetString(3),
                        DataType: reader.GetString(4),
                        IsNullable: reader.GetString(5) == "YES",
                        ColumnDefault: reader.IsDBNull(6) ? null : reader.GetString(6),
                        OrdinalPosition: reader.GetInt32(7),
                        CharacterMaximumLength: reader.IsDBNull(8) ? null : reader.GetInt32(8),
                        NumericPrecision: reader.IsDBNull(9) ? null : reader.GetInt32(9),
                        NumericScale: reader.IsDBNull(10) ? null : reader.GetInt32(10)
                    ));
                }
            }
        }

        await using (var cmd = new NpgsqlCommand(ViewsQuery, conn))
        await using (var reader = await cmd.ExecuteReaderAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                var key = (reader.GetString(0), reader.GetString(1));
                if (!reader.IsDBNull(2))
                    viewDefinitions[key] = reader.GetString(2);
            }
        }

        return objects
            .Select(kvp =>
            {
                var objectType = kvp.Value.TableType == "VIEW" ? DbObjectType.View : DbObjectType.Table;
                viewDefinitions.TryGetValue(kvp.Key, out var viewDef);
                return new DbObjectInfo(kvp.Key.Schema, kvp.Key.Name, objectType, kvp.Value.Columns, viewDef);
            })
            .ToList();
    }
}
