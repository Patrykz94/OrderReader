using Caliburn.Micro;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace OrderReaderUI.ViewModels;

public class ShellViewModel : Conductor<object>
{
    public string CurrentVersion { get; set; }

    public ShellViewModel()
    {
        // Get the current version of our app
        Assembly assembly = Assembly.GetExecutingAssembly();
        FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        CurrentVersion = $" v{versionInfo.FileVersion}";
    }

    protected async override void OnViewLoaded(object view)
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
