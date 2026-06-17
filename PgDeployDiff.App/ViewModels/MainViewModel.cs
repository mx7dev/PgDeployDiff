using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PgDeployDiff.App.Models;
using PgDeployDiff.Core;
using PgDeployDiff.Data;
using System.Collections.ObjectModel;

namespace PgDeployDiff.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SchemaReader _reader = new();

    public ObservableCollection<ConnectionProfile> SavedConnections { get; } =
    [
        new() { Server = "localhost", Port = 5433, Database = "lagrama",      Username = "lagrama", Password = "lagrama" },
        new() { Server = "localhost", Port = 5433, Database = "lagramanuevo", Username = "lagrama", Password = "lagrama" }
    ];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CompareCommand))]
    private ConnectionProfile? _selectedSourceConnection;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CompareCommand))]
    private ConnectionProfile? _selectedTargetConnection;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CompareCommand))]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private ObjectDifference? _selectedDifference;

    public ObservableCollection<ObjectDifference> Differences { get; } = [];
    public ObservableCollection<DiffGroup> DiffGroups { get; } = [];

    [RelayCommand(CanExecute = nameof(CanCompare))]
    private async Task CompareAsync()
    {
        Differences.Clear();
        DiffGroups.Clear();
        SelectedDifference = null;
        IsBusy = true;
        StatusMessage = "Leyendo base origen...";

        try
        {
            var sourceObjects = await _reader.ReadObjectsAsync(SelectedSourceConnection!.ToConnectionString());
            StatusMessage = "Leyendo base destino...";
            var targetObjects = await _reader.ReadObjectsAsync(SelectedTargetConnection!.ToConnectionString());

            var diffs = SchemaComparer.Compare(sourceObjects, targetObjects);
            foreach (var diff in diffs)
                Differences.Add(diff);

            var modified = diffs.Where(d => d.ChangeType == DiffChangeType.Modified).OrderBy(d => d.FullName).ToList();
            var added    = diffs.Where(d => d.ChangeType == DiffChangeType.Added).OrderBy(d => d.FullName).ToList();
            var removed  = diffs.Where(d => d.ChangeType == DiffChangeType.Removed).OrderBy(d => d.FullName).ToList();

            if (modified.Count > 0) DiffGroups.Add(new DiffGroup { Label = "Modificado", ChangeType = DiffChangeType.Modified, Items = modified });
            if (added.Count > 0)    DiffGroups.Add(new DiffGroup { Label = "Nuevo",      ChangeType = DiffChangeType.Added,    Items = added });
            if (removed.Count > 0)  DiffGroups.Add(new DiffGroup { Label = "Eliminado",  ChangeType = DiffChangeType.Removed,  Items = removed });

            StatusMessage = diffs.Count == 0
                ? "Sin diferencias — los esquemas son identicos."
                : $"{diffs.Count} objeto(s) con diferencias.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanCompare() =>
        SelectedSourceConnection is not null &&
        SelectedTargetConnection is not null &&
        !IsBusy;
}
