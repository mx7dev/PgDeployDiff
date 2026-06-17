using PgDeployDiff.App.Models;
using PgDeployDiff.App.ViewModels;
using System.Windows;

namespace PgDeployDiff.App.Views;

public partial class ConnectionDialog : Window
{
    public ConnectionProfile? Result { get; private set; }

    public ConnectionDialog()
    {
        InitializeComponent();
        DataContext = new ConnectionDialogViewModel();
    }

    private void PwdBox_PasswordChanged(object sender, RoutedEventArgs e) =>
        ((ConnectionDialogViewModel)DataContext).Password = PwdBox.Password;

    private void OnOkClick(object sender, RoutedEventArgs e)
    {
        Result = ((ConnectionDialogViewModel)DataContext).ToProfile();
        DialogResult = true;
    }

    private void OnCancelClick(object sender, RoutedEventArgs e) =>
        DialogResult = false;
}
