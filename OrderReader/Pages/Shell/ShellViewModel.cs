using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Extensions.Logging;
using OrderReader.Core.Interfaces;
using OrderReader.Pages.Customers;
using OrderReader.Pages.Orders;
using OrderReader.Pages.Settings;
using Velopack;
using Velopack.Sources;

namespace OrderReader.Pages.Shell;

public class ShellViewModel : Conductor<object>
{
    #region Private Variables

    private readonly INotificationService _notificationService;
    private readonly ILogger _logger;
    private UpdateInfo? _updateInfo;

    #endregion

    #region Public Propertes

    public string CurrentVersion { get; }

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

    #endregion

    #region Initialization
    
    public ShellViewModel(INotificationService notificationService, ILogger logger)
    {
        _notificationService = notificationService;
        _logger = logger;
        
        // Get the current version of our app
        var assembly = Assembly.GetExecutingAssembly();
        var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        CurrentVersion = $"v{versionInfo.FileVersion}";
        _logger.LogDebug("Application {version} launched and displayed the ShellViewModel", CurrentVersion);
    }

    protected override async void OnViewLoaded(object view)
    {
        base.OnViewLoaded(view);
        _ = GetUpdates();
        await Orders();
    }

    #endregion

    #region Navigation Button Handlers

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
        Environment.Exit(0);
    }

    #endregion

    #region Private Functions

    private async Task GetUpdates()
    {
        var mgr = new UpdateManager(new GithubSource(@"https://github.com/Patrykz94/OrderReader", null, true));

        // Check for new version
        try
        {
            _updateInfo = await mgr.CheckForUpdatesAsync();
        }
        catch (Exception e)
        {
            _logger.LogDebug("Error when checking for updates: {message}", e.Message);
            throw;
        }
        if (_updateInfo is null) return;
        
        // Download the new version
        UpdateInProgress = true;

        await mgr.DownloadUpdatesAsync(_updateInfo, ProgressHandler);

        UpdateInProgress = false;

        await _notificationService.ShowUpdateNotification(_updateInfo.TargetFullRelease.Version.ToString(), ApplyUpdates);

        // Return from the function here but there are still function definitions below
        return;

        void ApplyUpdates(bool userChoice)
        {
            if (userChoice == false) return;

            if (_updateInfo is null) return;
            try
            {
                _logger.LogDebug("Update is being installed...");
                mgr.ApplyUpdatesAndRestart(_updateInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured while trying to install the update and restart: {ex}", ex.Message);
                throw;
            }
        }

        void ProgressHandler(int progress)
        {
            UpdateProgress = progress;
        }
    }

    #endregion
}
