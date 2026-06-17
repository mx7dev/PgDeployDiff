using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PgDeployDiff.App.Controls;

public partial class DiffViewer : UserControl
{
    public static readonly DependencyProperty OldTextProperty =
        DependencyProperty.Register(nameof(OldText), typeof(string), typeof(DiffViewer),
            new PropertyMetadata(string.Empty, OnTextChanged));

    public static readonly DependencyProperty NewTextProperty =
        DependencyProperty.Register(nameof(NewText), typeof(string), typeof(DiffViewer),
            new PropertyMetadata(string.Empty, OnTextChanged));

    public string? OldText
    {
        get => (string?)GetValue(OldTextProperty);
        set => SetValue(OldTextProperty, value);
    }

    public string? NewText
    {
        get => (string?)GetValue(NewTextProperty);
        set => SetValue(NewTextProperty, value);
    }

    public DiffViewer()
    {
        InitializeComponent();
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DiffViewer viewer)
            viewer.UpdateDiff();
    }

    private void UpdateDiff()
    {
        var diff = SideBySideDiffBuilder.Diff(OldText ?? string.Empty, NewText ?? string.Empty);
        OldLines.ItemsSource = diff.OldText.Lines.Select(l => new DiffLine(l.Text, l.Type)).ToList();
        NewLines.ItemsSource = diff.NewText.Lines.Select(l => new DiffLine(l.Text, l.Type)).ToList();
    }
}

public record DiffLine(string? Text, ChangeType Type)
{
    public string DisplayText => Text ?? string.Empty;

    public Brush Background => Type switch
    {
        ChangeType.Deleted   => new SolidColorBrush(Color.FromRgb(0xFF, 0xCD, 0xD2)),
        ChangeType.Inserted  => new SolidColorBrush(Color.FromRgb(0xC8, 0xE6, 0xC9)),
        ChangeType.Modified  => new SolidColorBrush(Color.FromRgb(0xFF, 0xF9, 0xC4)),
        ChangeType.Imaginary => new SolidColorBrush(Color.FromRgb(0xFA, 0xFA, 0xFA)),
        _                    => Brushes.Transparent
    };
}
