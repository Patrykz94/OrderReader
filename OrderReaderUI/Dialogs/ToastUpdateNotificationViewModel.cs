using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using OrderReader.Core;
using Screen = Caliburn.Micro.Screen;

namespace OrderReaderUI.Dialogs;

public class ToastUpdateNotificationViewModel : Screen
{
    #region Public Properties
    
    public DateTime NotificationTime { get; } = DateTime.Now;
    public string UpdatedVersion { get; set; }

    public bool Result { get; set; }

    #endregion

    #region Constructors

    public ToastUpdateNotificationViewModel(string updatedVersion)
    {
        UpdatedVersion = updatedVersion;
    }

    #endregion

    #region Public Methods

    public void CloseNotification()
    {
        TryCloseAsync();
    }
    
    public void RestartApplication()
    {
        Result = true;
        TryCloseAsync();
    }

    #endregion
}