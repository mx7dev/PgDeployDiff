using PgDeployDiff.App.ViewModels;
using PgDeployDiff.App.Views;
using PgDeployDiff.Core;
using System.Windows;
using System.Windows.Controls;

namespace PgDeployDiff.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    private void OnTreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        var vm = (MainViewModel)DataContext;
        vm.SelectedDifference = e.NewValue as ObjectDifference;
    }

    private void OnSourceBrowseClick(object sender, RoutedEventArgs e) =>
        OpenConnectionDialog(vm => vm.SelectedSourceConnection = vm.SavedConnections[^1]);

    private void OnTargetBrowseClick(object sender, RoutedEventArgs e) =>
        OpenConnectionDialog(vm => vm.SelectedTargetConnection = vm.SavedConnections[^1]);

    private void OpenConnectionDialog(Action<MainViewModel> onAdded)
    {
        var dialog = new ConnectionDialog { Owner = this };
        if (dialog.ShowDialog() != true || dialog.Result is not { } profile)
            return;

        var vm = (MainViewModel)DataContext;
        vm.SavedConnections.Add(profile);
        onAdded(vm);
    }
}
