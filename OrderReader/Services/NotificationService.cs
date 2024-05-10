using System;
using System.Media;
using System.Threading.Tasks;
using Caliburn.Micro;
using OrderReader.Core.DataModels;
using OrderReader.Core.Enums;
using OrderReader.Core.Interfaces;
using OrderReader.Dialogs;

namespace OrderReader.Services;

public class NotificationService(IWindowManager windowManager) : INotificationService
{
    public async Task<DialogResult> ShowMessage(string title, string message, string button)
    {
        if (Settings.LoadSettings().EnableSounds) SystemSounds.Asterisk.Play();
        await windowManager.ShowDialogAsync(new DialogMessageViewModel(message, title, button));
        return DialogResult.Ok;
    }

    public async Task<DialogResult> ShowQuestion(string title, string message, string primaryButton, string secondaryButton)
    {
        if (Settings.LoadSettings().EnableSounds) SystemSounds.Asterisk.Play();
        var result = await windowManager.ShowDialogAsync(new DialogYesNoViewModel(message, title, primaryButton, secondaryButton));
        return result == true ? DialogResult.Yes : DialogResult.No;
    }

    public async Task<string> ShowConfigMessage(string title, string message)
    {
        var dialogViewModel = new DialogConfigFileViewModel(title)
        {
            Message = message
        };
        if (Settings.LoadSettings().EnableSounds) SystemSounds.Asterisk.Play();
        var result = await windowManager.ShowDialogAsync(dialogViewModel);
        return result == true ? dialogViewModel.ConfigFileLocation : string.Empty;
    }

    public async Task<string> ShowConfigMessage()
    {
        var dialogViewModel = new DialogConfigFileViewModel();
        if (Settings.LoadSettings().EnableSounds) SystemSounds.Asterisk.Play();
        var result = await windowManager.ShowDialogAsync(dialogViewModel);
        return result == true ? dialogViewModel.ConfigFileLocation : string.Empty;
    }

    public async Task ShowUpdateNotification(string updatedVersion, Action<bool>? windowCloseAction)
    {
        if (Settings.LoadSettings().EnableSounds) SystemSounds.Asterisk.Play();
        await windowManager.ShowWindowAsync(new ToastUpdateNotificationViewModel(updatedVersion, windowCloseAction));
    }
}