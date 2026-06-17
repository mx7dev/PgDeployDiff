namespace PgDeployDiff.Core;

public record TableInfo(
    string SchemaName,
    string TableName,
    IReadOnlyList<ColumnInfo> Columns);
