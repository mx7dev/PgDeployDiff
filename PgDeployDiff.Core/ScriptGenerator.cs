using System.Text;

namespace PgDeployDiff.Core;

public static class ScriptGenerator
{
    public static string CreateTable(DbObjectInfo table)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"CREATE TABLE {table.SchemaName}.{table.ObjectName}");
        sb.AppendLine("(");

        var cols = table.Columns.OrderBy(c => c.OrdinalPosition).ToList();
        for (int i = 0; i < cols.Count; i++)
        {
            var c = cols[i];
            sb.Append($"    {c.ColumnName} {c.FullType}");
            if (!c.IsNullable) sb.Append(" NOT NULL");
            if (c.ColumnDefault is not null) sb.Append($" DEFAULT {c.ColumnDefault}");
            if (i < cols.Count - 1) sb.Append(",");
            sb.AppendLine();
        }

        sb.Append(");");
        return sb.ToString();
    }

    public static string CreateView(DbObjectInfo view) =>
        $"CREATE OR REPLACE VIEW {view.SchemaName}.{view.ObjectName} AS\n{view.ViewDefinition?.Trim()}";
}
