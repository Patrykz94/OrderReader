using System.Threading.Tasks;
using Caliburn.Micro;
using OrderReader.Core;
using OrderReader.Core.Enums;
using OrderReader.Core.Interfaces;
using OrderReader.Dialogs;

namespace OrderReader.Services;

public class NotificationService(IWindowManager windowManager) : INotificationService
{
    public async Task<DialogResult> ShowMessage(string title, string message, string button)
    {
        await windowManager.ShowDialogAsync(new DialogMessageViewModel(message, title, button));
        return DialogResult.Ok;
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

    public async Task<DialogResult> ShowUpdateNotification(string updatedVersion)
    {
        var notificationWindow = new ToastUpdateNotificationViewModel(updatedVersion);
        await windowManager.ShowWindowAsync(notificationWindow);
        return notificationWindow.Result ? DialogResult.Yes : DialogResult.No;
    }
}