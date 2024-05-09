using System;
using Caliburn.Micro;

namespace OrderReader.Dialogs;

public class ToastUpdateNotificationViewModel : Screen
{
    #region Callbacks
    
    private readonly Action<bool>? _windowCloseAction;
    
    #endregion
    
    #region Public Properties

    public DateTime NotificationTime { get; } = DateTime.Now;
    public string UpdatedVersion { get; set; }

    #endregion

    #region Constructors

    public ToastUpdateNotificationViewModel(string updatedVersion, Action<bool>? windowCloseAction = null)
    {
        _windowCloseAction = windowCloseAction;
        UpdatedVersion = updatedVersion;
    }

    #endregion

    #region Public Methods

    public void CloseNotification()
    {
        _windowCloseAction?.Invoke(false);
        TryCloseAsync();
    }

    public void RestartApplication()
    {
        _windowCloseAction?.Invoke(true);
        TryCloseAsync();
    }

    #endregion
}