using System.Threading.Tasks;
using Caliburn.Micro;
using OrderReader.Core;
using OrderReader.Core.Interfaces;
using OrderReaderUI.Dialogs;

namespace OrderReaderUI.Services;

public class NotificationService(IWindowManager windowManager) : INotificationService
{
    public async Task<DialogResult> ShowMessage(string title, string message, string button)
    {
        await windowManager.ShowDialogAsync(new DialogMessageViewModel(message, title, button));
        return DialogResult.OK;
    }

    public async Task<DialogResult> ShowQuestion(string title, string message, string primaryButton, string secondaryButton)
    {
        var result = await windowManager.ShowDialogAsync(new DialogYesNoViewModel(message, title, primaryButton, secondaryButton));
        return result == true ? DialogResult.Yes : DialogResult.No;
    }
    
    public async Task<string> ShowConfigMessage(string title, string message)
    {
        var dialogViewModel = new DialogConfigFileViewModel(title)
        {
            Message = message
        };
        var result = await windowManager.ShowDialogAsync(dialogViewModel);
        return result == true ? dialogViewModel.ConfigFileLocation : string.Empty;
    }
    
    public async Task<string> ShowConfigMessage()
    {
        var dialogViewModel = new DialogConfigFileViewModel();
        var result = await windowManager.ShowDialogAsync(dialogViewModel);
        return result == true ? dialogViewModel.ConfigFileLocation : string.Empty;
    }

    public Task ShowUpdateNotification(string updatedVersion)
    {
        return windowManager.ShowWindowAsync(new ToastUpdateNotificationViewModel(updatedVersion));
    }
}