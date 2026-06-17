namespace PgDeployDiff.Core;

public record ColumnInfo(
    string ColumnName,
    string DataType,
    bool IsNullable,
    string? ColumnDefault,
    int OrdinalPosition,
    int? CharacterMaximumLength,
    int? NumericPrecision,
    int? NumericScale)
{
    public string FullType => CharacterMaximumLength.HasValue
        ? $"{DataType}({CharacterMaximumLength})"
        : NumericPrecision.HasValue && NumericScale.HasValue
            ? $"{DataType}({NumericPrecision},{NumericScale})"
            : DataType;
}
