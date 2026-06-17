namespace PgDeployDiff.Core;

public class ObjectDifference
{
    public required string SchemaName { get; init; }
    public required string ObjectName { get; init; }
    public required DbObjectType ObjectType { get; init; }
    public required DiffChangeType ChangeType { get; init; }
    public IReadOnlyList<ColumnDifference> ColumnDifferences { get; init; } = [];
    public string? OldDefinition { get; init; }
    public string? NewDefinition { get; init; }

    public string FullName => $"{SchemaName}.{ObjectName}";
    public string TypeLabel => ObjectType == DbObjectType.View ? "Vista" : "Tabla";

    public string Summary => (ObjectType, ChangeType) switch
    {
        (DbObjectType.View, DiffChangeType.Added)    => "Vista nueva",
        (DbObjectType.View, DiffChangeType.Removed)  => "Vista eliminada",
        (DbObjectType.View, DiffChangeType.Modified) => "Definicion modificada",
        (DbObjectType.Table, DiffChangeType.Added)   => $"Tabla nueva ({ColumnDifferences.Count} col.)",
        (DbObjectType.Table, DiffChangeType.Removed) => $"Tabla eliminada ({ColumnDifferences.Count} col.)",
        _                                            => $"{ColumnDifferences.Count} columna(s) afectada(s)"
    };
}
