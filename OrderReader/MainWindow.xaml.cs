using System.Windows;

namespace OrderReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new WindowViewModel(this);
        }

        private void AppWindow_Deactivated(object sender, System.EventArgs e)
        {
            // Show overlay if we are not focused
            (DataContext as WindowViewModel).DimmableOverlayVisible = true;
        }

        private void AppWindow_Activated(object sender, System.EventArgs e)
        {
            // Hide overlay if we are focused
            (DataContext as WindowViewModel).DimmableOverlayVisible = false;
        }
    }
}
