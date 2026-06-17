using PgDeployDiff.Core;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PgDeployDiff.App.Selectors;

public class ObjectDiffTemplateSelector : DataTemplateSelector
{
    public DataTemplate? AddedTemplate    { get; set; }
    public DataTemplate? RemovedTemplate  { get; set; }
    public DataTemplate? ModifiedTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container) =>
        item is ObjectDifference diff ? diff.ChangeType switch
        {
            DiffChangeType.Added   => AddedTemplate,
            DiffChangeType.Removed => RemovedTemplate,
            _                      => ModifiedTemplate
        } : null;
}

public class ObjectTypeLabelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value switch
        {
            DbObjectType.Table => "Tablas",
            DbObjectType.View  => "Vistas",
            _                  => value?.ToString() ?? string.Empty
        };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
