using OrderReader.Core;
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
            UserSettings userSettings = OrderReader.Core.Settings.LoadSettings();
            ThemeManager.ChangeTheme(userSettings.Theme);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Detect when the theme changed
            HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
            source.AddHook((IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
            {
                const int WM_SETTINGCHANGE = 0x001A;
                if (msg == WM_SETTINGCHANGE)
                {
                    if (wParam == IntPtr.Zero && Marshal.PtrToStringUni(lParam) == "ImmersiveColorSet")
                    {
                        if (OrderReader.Core.Settings.LoadSettings().Theme == "Auto")
                        {
                            var isLightTheme = ThemeManager.IsOSLightTheme();
                            if (isLightTheme)
                            {
                                ThemeManager.ChangeTheme("Light");
                            }
                            else
                            {
                                ThemeManager.ChangeTheme("Dark");
                            }
                        }
                    }
                }

                return IntPtr.Zero;
            });
        }
    }
}
