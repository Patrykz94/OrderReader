using System.Threading.Tasks;
using Caliburn.Micro;
using OrderReader.Core;
using OrderReader.Core.Interfaces;
using OrderReaderUI.ViewModels.Dialogs;

namespace OrderReaderUI.Services;

public class NotificationService(IWindowManager windowManager) : INotificationService
{
    public async Task<DialogResult> ShowMessage(string title, string message, string button = "OK")
    {
        await windowManager.ShowDialogAsync(new DialogMessageViewModel(message, title, button));
        return DialogResult.OK;
    }

    public async Task<DialogResult> ShowQuestion(string title, string message, string primaryButton = "Yes", string secondaryButton = "No")
    {
        var result = await windowManager.ShowDialogAsync(new DialogYesNoViewModel(message, title, primaryButton, secondaryButton));
        return result == true ? DialogResult.Yes : DialogResult.No;
    }
}