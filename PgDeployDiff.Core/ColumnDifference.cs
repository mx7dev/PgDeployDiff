namespace PgDeployDiff.Core;

public class ColumnDifference
{
    public string ColumnName { get; init; } = string.Empty;
    public DiffChangeType ChangeType { get; init; }
    public string? OldDefinition { get; init; }
    public string? NewDefinition { get; init; }
}
