using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Caliburn.Micro;
using OrderReader.Core.Enums;
using OrderReader.Core.Interfaces;
using OrderReader.Pages.Customers;
using OrderReader.Pages.Orders;
using OrderReader.Pages.Settings;
using Velopack;

namespace OrderReader.Pages.Shell;

public class ShellViewModel : Conductor<object>
{
    private readonly INotificationService _notificationService;

    public string CurrentVersion { get; set; }

    private bool _updateInProgress;
    public bool UpdateInProgress
    {
        get => _updateInProgress;
        set
        {
            _updateInProgress = value;
            NotifyOfPropertyChange();
        }
    }

    private int _updateProgress = 0;
    public int UpdateProgress
    {
        get => _updateProgress;
        set
        {
            _updateProgress = value;
            NotifyOfPropertyChange();
        }
    }

    public ShellViewModel(INotificationService notificationService)
    {
        _notificationService = notificationService;
        // Get the current version of our app
        var assembly = Assembly.GetExecutingAssembly();
        var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        CurrentVersion = $" v{versionInfo.FileVersion}";
    }

    protected override async void OnViewLoaded(object view)
    {
        base.OnViewLoaded(view);
        _ = GetUpdates();
        await Orders();
    }

    private async Task GetUpdates()
    {
        var mgr = new UpdateManager(@"E:\Documents\Programming\OrderReader Resources\Updates");

        // Check for new version
        var newVersion = await mgr.CheckForUpdatesAsync();
        if (newVersion is null) return;

        // Download the new version
        UpdateInProgress = true;

        await mgr.DownloadUpdatesAsync(newVersion, ProgressHandler);

        UpdateInProgress = false;

        var result = await _notificationService.ShowUpdateNotification(newVersion.TargetFullRelease.Version.ToString());

        if (result == DialogResult.Yes) mgr.ApplyUpdatesAndRestart(newVersion);

        return;

        void ProgressHandler(int progress)
        {
            UpdateProgress = progress;
        }
    }

    public async Task Orders()
    {
        if (ActiveItem is OrdersViewModel) return;
        await ActivateItemAsync(IoC.Get<OrdersViewModel>());
    }

    public async Task Customers()
    {
        if (ActiveItem is CustomersViewModel) return;
        await ActivateItemAsync(IoC.Get<CustomersViewModel>());
    }

    public async Task Settings()
    {
        if (ActiveItem is SettingsViewModel) return;
        await ActivateItemAsync(IoC.Get<SettingsViewModel>());
    }

    public void ExitApplication()
    {
        TryCloseAsync();
    }
}
