namespace PgDeployDiff.Core;

public static class SchemaComparer
{
    public static IReadOnlyList<ObjectDifference> Compare(
        IEnumerable<DbObjectInfo> source,
        IEnumerable<DbObjectInfo> target)
    {
        var sourceMap = source.ToDictionary(o => (o.SchemaName, o.ObjectName));
        var targetMap = target.ToDictionary(o => (o.SchemaName, o.ObjectName));

        var results = new List<ObjectDifference>();

        foreach (var key in sourceMap.Keys.Except(targetMap.Keys))
        {
            var obj = sourceMap[key];
            results.Add(new ObjectDifference
            {
                SchemaName = key.SchemaName,
                ObjectName = key.ObjectName,
                ObjectType = obj.ObjectType,
                ChangeType = DiffChangeType.Removed,
                OldDefinition = obj.ObjectType == DbObjectType.Table
                    ? ScriptGenerator.CreateTable(obj)
                    : ScriptGenerator.CreateView(obj),
                ColumnDifferences = obj.ObjectType == DbObjectType.Table
                    ? [.. obj.Columns.Select(c => new ColumnDifference { ColumnName = c.ColumnName, ChangeType = DiffChangeType.Removed, OldDefinition = Format(c) })]
                    : []
            });
        }

        foreach (var key in targetMap.Keys.Except(sourceMap.Keys))
        {
            var obj = targetMap[key];
            results.Add(new ObjectDifference
            {
                SchemaName = key.SchemaName,
                ObjectName = key.ObjectName,
                ObjectType = obj.ObjectType,
                ChangeType = DiffChangeType.Added,
                NewDefinition = obj.ObjectType == DbObjectType.Table
                    ? ScriptGenerator.CreateTable(obj)
                    : ScriptGenerator.CreateView(obj),
                ColumnDifferences = obj.ObjectType == DbObjectType.Table
                    ? [.. obj.Columns.Select(c => new ColumnDifference { ColumnName = c.ColumnName, ChangeType = DiffChangeType.Added, NewDefinition = Format(c) })]
                    : []
            });
        }

        foreach (var key in sourceMap.Keys.Intersect(targetMap.Keys))
        {
            var src = sourceMap[key];
            var tgt = targetMap[key];

            if (src.ObjectType == DbObjectType.View)
            {
                if (Normalize(src.ViewDefinition) != Normalize(tgt.ViewDefinition))
                    results.Add(new ObjectDifference
                    {
                        SchemaName = key.SchemaName,
                        ObjectName = key.ObjectName,
                        ObjectType = DbObjectType.View,
                        ChangeType = DiffChangeType.Modified,
                        OldDefinition = ScriptGenerator.CreateView(src),
                        NewDefinition = ScriptGenerator.CreateView(tgt)
                    });
            }
            else
            {
                var colDiffs = CompareColumns(src.Columns, tgt.Columns);
                if (colDiffs.Count > 0)
                    results.Add(new ObjectDifference
                    {
                        SchemaName = key.SchemaName,
                        ObjectName = key.ObjectName,
                        ObjectType = DbObjectType.Table,
                        ChangeType = DiffChangeType.Modified,
                        OldDefinition = ScriptGenerator.CreateTable(src),
                        NewDefinition = ScriptGenerator.CreateTable(tgt),
                        ColumnDifferences = colDiffs
                    });
            }
        }

        return [.. results.OrderBy(o => o.ObjectType).ThenBy(o => o.SchemaName).ThenBy(o => o.ObjectName)];
    }

    private static IReadOnlyList<ColumnDifference> CompareColumns(
        IReadOnlyList<ColumnInfo> source,
        IReadOnlyList<ColumnInfo> target)
    {
        var sourceMap = source.ToDictionary(c => c.ColumnName);
        var targetMap = target.ToDictionary(c => c.ColumnName);

        var results = new List<ColumnDifference>();

        foreach (var name in sourceMap.Keys.Except(targetMap.Keys))
            results.Add(new ColumnDifference { ColumnName = name, ChangeType = DiffChangeType.Removed, OldDefinition = Format(sourceMap[name]) });

        foreach (var name in targetMap.Keys.Except(sourceMap.Keys))
            results.Add(new ColumnDifference { ColumnName = name, ChangeType = DiffChangeType.Added, NewDefinition = Format(targetMap[name]) });

        foreach (var name in sourceMap.Keys.Intersect(targetMap.Keys))
        {
            var src = sourceMap[name];
            var tgt = targetMap[name];
            if (src.FullType != tgt.FullType || src.IsNullable != tgt.IsNullable || src.ColumnDefault != tgt.ColumnDefault)
                results.Add(new ColumnDifference { ColumnName = name, ChangeType = DiffChangeType.Modified, OldDefinition = Format(src), NewDefinition = Format(tgt) });
        }

        return results;
    }

    private static string? Normalize(string? def) =>
        def?.Trim().Replace("\r\n", "\n").Replace("\r", "\n");

    private static string Format(ColumnInfo col)
    {
        var nullable = col.IsNullable ? "NULL" : "NOT NULL";
        var def = col.ColumnDefault is not null ? $" DEFAULT {col.ColumnDefault}" : "";
        return $"{col.FullType} {nullable}{def}";
    }
}
