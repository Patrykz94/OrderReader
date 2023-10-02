using System.Windows;
using System.Windows.Input;

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
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
