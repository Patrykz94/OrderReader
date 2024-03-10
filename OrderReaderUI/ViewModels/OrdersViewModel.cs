using Caliburn.Micro;
using System.Threading.Tasks;

namespace OrderReaderUI.ViewModels;

public class OrdersViewModel : Screen
{
    private readonly IWindowManager _windowManager;

    public OrdersViewModel(IWindowManager windowManager)
    {
        _windowManager = windowManager;
    }

    public async Task ShowDialog()
    {
        // Message Dialog
        var messageDialog = new DialogMessageViewModel("Test Message Dialog Box");
        var messageResult = await _windowManager.ShowDialogAsync(messageDialog);
        
        // YesNo Dialog
        var yesNoDialog = new DialogYesNoViewModel("Test Yes/No Dialog Box");
        var yesNoResult = await _windowManager.ShowDialogAsync((yesNoDialog));
        
        // Config File Dialog
        var configDialog = new DialogConfigFileViewModel("Welcome", "Configure");
        var configResult = await _windowManager.ShowDialogAsync(configDialog);
    }
}
 