namespace PgDeployDiff.Core;

public class TableDifference
{
    public required string SchemaName { get; init; }
    public required string TableName { get; init; }
    public required DiffChangeType ChangeType { get; init; }
    public IReadOnlyList<ColumnDifference> ColumnDifferences { get; init; } = [];

    public string FullName => $"{SchemaName}.{TableName}";
}
