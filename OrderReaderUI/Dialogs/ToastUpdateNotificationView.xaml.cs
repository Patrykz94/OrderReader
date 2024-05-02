using System.Windows;
using System.Windows.Controls;

namespace OrderReaderUI.Dialogs;

public partial class ToastUpdateNotificationView : Window
{
    public ToastUpdateNotificationView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var desktopWorkingArea = SystemParameters.WorkArea;
        Left = desktopWorkingArea.Right - ActualWidth;
        Top = desktopWorkingArea.Bottom - ActualHeight;
    }
}