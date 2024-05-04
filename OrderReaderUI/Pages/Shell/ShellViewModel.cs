using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Caliburn.Micro;
using OrderReaderUI.Pages.Customers;
using OrderReaderUI.Pages.Orders;
using OrderReaderUI.Pages.Settings;

namespace OrderReaderUI.Pages.Shell;

public class ShellViewModel : Conductor<object>
{
    public string CurrentVersion { get; set; }

    public ShellViewModel()
    {
        // Get the current version of our app
        var assembly = Assembly.GetExecutingAssembly();
        var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        CurrentVersion = $" v{versionInfo.FileVersion}";
    }

    protected override async void OnViewLoaded(object view)
    {
        base.OnViewLoaded(view);
        await Orders();
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
