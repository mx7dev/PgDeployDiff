using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PgDeployDiff.App.Controls;

public partial class DiffViewer : UserControl
{
    private bool _syncingScroll;

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
        OldLines.ItemsSource = diff.OldText.Lines.Select(l => new DiffLine(l.Text, l.Type, l.Position)).ToList();
        NewLines.ItemsSource = diff.NewText.Lines.Select(l => new DiffLine(l.Text, l.Type, l.Position)).ToList();
    }

    private void OnOldScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (_syncingScroll) return;
        _syncingScroll = true;
        NewScroll.ScrollToVerticalOffset(e.VerticalOffset);
        NewScroll.ScrollToHorizontalOffset(e.HorizontalOffset);
        _syncingScroll = false;
    }

    private void OnNewScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (_syncingScroll) return;
        _syncingScroll = true;
        OldScroll.ScrollToVerticalOffset(e.VerticalOffset);
        OldScroll.ScrollToHorizontalOffset(e.HorizontalOffset);
        _syncingScroll = false;
    }
}

public record DiffLine(string? Text, ChangeType Type, int? LineNumber)
{
    public string DisplayText => Text ?? string.Empty;
    public string LineNumberText => LineNumber.HasValue ? LineNumber.Value.ToString() : string.Empty;

    public Brush Background => Type switch
    {
        ChangeType.Deleted   => new SolidColorBrush(Color.FromRgb(0xFF, 0xCD, 0xD2)),
        ChangeType.Inserted  => new SolidColorBrush(Color.FromRgb(0xC8, 0xE6, 0xC9)),
        ChangeType.Modified  => new SolidColorBrush(Color.FromRgb(0xFF, 0xF9, 0xC4)),
        ChangeType.Imaginary => new SolidColorBrush(Color.FromRgb(0xFA, 0xFA, 0xFA)),
        _                    => Brushes.Transparent
    };
}
