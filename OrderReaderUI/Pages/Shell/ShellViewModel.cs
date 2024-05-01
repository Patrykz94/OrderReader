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

    public Task Orders()
    {
        return ActivateItemAsync(IoC.Get<OrdersViewModel>());
    }

    public Task Customers()
    {
        return ActivateItemAsync(IoC.Get<CustomersViewModel>());
    }

    public Task Settings()
    {
        return ActivateItemAsync(IoC.Get<SettingsViewModel>());
    }

    public void ExitApplication()
    {
        TryCloseAsync();
    }
}
