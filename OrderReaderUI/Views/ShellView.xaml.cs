using OrderReaderUI.Helpers;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace OrderReaderUI.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            // Check user settings to see which theme we should use
            var userSettings = OrderReader.Core.Settings.LoadSettings();
            ThemeManager.ChangeTheme(userSettings.Theme);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Detect when the theme changed
            var source = (HwndSource?)PresentationSource.FromVisual(this);

            source?.AddHook((IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
            {
                const int wmSettingChange = 0x001A;
                if (msg != wmSettingChange) return IntPtr.Zero;

                if (wParam != IntPtr.Zero || Marshal.PtrToStringUni(lParam) != "ImmersiveColorSet") return IntPtr.Zero;

                if (OrderReader.Core.Settings.LoadSettings().Theme != "Auto") return IntPtr.Zero;
                
                var isLightTheme = ThemeManager.IsOsLightTheme();
                
                ThemeManager.ChangeTheme(isLightTheme ? "Light" : "Dark");

                return IntPtr.Zero;
            });
        }
    }
}
