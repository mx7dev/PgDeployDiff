namespace PgDeployDiff.Core;

public record DbObjectInfo(
    string SchemaName,
    string ObjectName,
    DbObjectType ObjectType,
    IReadOnlyList<ColumnInfo> Columns,
    string? ViewDefinition = null)
{
    public string FullName => $"{SchemaName}.{ObjectName}";
}
