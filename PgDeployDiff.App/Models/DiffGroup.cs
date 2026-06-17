using PgDeployDiff.Core;
using System.Windows.Media;

namespace PgDeployDiff.App.Models;

public class DiffGroup
{
    public required string Label { get; init; }
    public required DiffChangeType ChangeType { get; init; }
    public required IReadOnlyList<ObjectDifference> Items { get; init; }

    public string Header => $"{Label}  ({Items.Count})";

    public Brush GroupColor => ChangeType switch
    {
        DiffChangeType.Added   => new SolidColorBrush(Color.FromRgb(0x2E, 0x7D, 0x32)),
        DiffChangeType.Removed => new SolidColorBrush(Color.FromRgb(0xC6, 0x28, 0x28)),
        _                      => new SolidColorBrush(Color.FromRgb(0xF5, 0x7F, 0x17))
    };
}
