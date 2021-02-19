using OrderReader.Core;
using WinForms = System.Windows.Forms;

namespace OrderReader
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : BasePage<SettingsViewModel>
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        #region Events

        /// <summary>
        /// Button click event that displays a dialog to the user and processes the result
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseCSVButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string selectedPath = BrowseFolder();

            if (selectedPath != null)
            {
                CSVExportDir.Text = selectedPath;
            }
        }

        /// <summary>
        /// Button click event that displays a dialog to the user and processes the result
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowsePDFButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string selectedPath = BrowseFolder();

            if (selectedPath != null)
            {
                PDFExportDir.Text = selectedPath;
            }
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Open a folder browser dialog that lets the user pick a folder and return the result
        /// </summary>
        /// <returns></returns>
        private string BrowseFolder()
        {
            // TODO: replace this with a custom forlder browser window so that we don't need to use WinForms
            WinForms.FolderBrowserDialog folderBrowserDialog = new WinForms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                return folderBrowserDialog.SelectedPath;
            }
            return null;
        }

        #endregion
    }
}
