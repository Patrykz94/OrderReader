using OrderReader.Core;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OrderReader
{
    /// <summary>
    /// The base class for any content that is being used inside of a <see cref="DialogWindow"/>
    /// </summary>
    public class BaseDialogUserControl : UserControl
    {
        #region Private Members

        /// <summary>
        /// The dialog window we will be contained within
        /// </summary>
        private DialogWindow mDialogWindow;

        #endregion

        #region Public Properties

        /// <summary>
        /// The minimum width of this dialog
        /// </summary>
        public int WindowMinimumWidth { get; set; } = 250;

        /// <summary>
        /// The minimum height of this dialog
        /// </summary>
        public int WindowMinimumHeight { get; set; } = 100;

        /// <summary>
        /// The height of the title bar
        /// </summary>
        public int TitleHeight { get; set; } = 30;

        /// <summary>
        /// Title for this dialog
        /// </summary>
        public string Title { get; set; }

        #endregion

        #region Public Commands

        /// <summary>
        /// Closes this dialog and returns NONE
        /// </summary>
        public ICommand NoneCommand { get; private set; }

        /// <summary>
        /// Closes this dialog and returns OK
        /// </summary>
        public ICommand OKCommand { get; private set; }

        /// <summary>
        /// Closes this dialog and returns CANCEL
        /// </summary>
        public ICommand CancelCommand { get; private set; }

        /// <summary>
        /// Closes this dialog and returns ABORT
        /// </summary>
        public ICommand AbortCommand { get; private set; }

        /// <summary>
        /// Closes this dialog and returns RETRY
        /// </summary>
        public ICommand RetryCommand { get; private set; }

        /// <summary>
        /// Closes this dialog and returns IGNORE
        /// </summary>
        public ICommand IgnoreCommand { get; private set; }

        /// <summary>
        /// Closes this dialog and returns YES
        /// </summary>
        public ICommand YesCommand { get; private set; }

        /// <summary>
        /// Closes this dialog and returns NO
        /// </summary>
        public ICommand NoCommand { get; private set; }

        /// <summary>
        /// Closes this dialog and returns a string parameter
        /// </summary>
        public ICommand ReturnStringCommand { get; private set; }

        #endregion

        #region Contructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseDialogUserControl()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                // Create a new dialog window
                mDialogWindow = new DialogWindow();
                mDialogWindow.ViewModel = new DialogWindowViewModel(mDialogWindow);

                // Create None command
                NoneCommand = new RelayCommand(() => {
                    mDialogWindow.DialogResult = DialogResult.None;
                    CloseDialog(mDialogWindow);
                });

                // Create OK command
                OKCommand = new RelayCommand(() => {
                    mDialogWindow.DialogResult = DialogResult.OK;
                    CloseDialog(mDialogWindow);
                });
                // Create Cancel command
                CancelCommand = new RelayCommand(() => {
                    mDialogWindow.DialogResult = DialogResult.Cancel;
                    CloseDialog(mDialogWindow);
                });
                // Create Abort command
                AbortCommand = new RelayCommand(() => {
                    mDialogWindow.DialogResult = DialogResult.Abort;
                    CloseDialog(mDialogWindow);
                });
                // Create Retry command
                RetryCommand = new RelayCommand(() => {
                    mDialogWindow.DialogResult = DialogResult.Retry;
                    CloseDialog(mDialogWindow);
                });
                // Create Ignore command
                IgnoreCommand = new RelayCommand(() => {
                    mDialogWindow.DialogResult = DialogResult.Ignore;
                    CloseDialog(mDialogWindow);
                });
                // Create Yes command
                YesCommand = new RelayCommand(() => {
                    mDialogWindow.DialogResult = DialogResult.Yes;
                    CloseDialog(mDialogWindow);
                });
                // Create No command
                NoCommand = new RelayCommand(() => {
                    mDialogWindow.DialogResult = DialogResult.No;
                    CloseDialog(mDialogWindow);
                });
                // Create Return String command
                ReturnStringCommand = new RelayParameterizedCommand((parameter) =>
                {
                    mDialogWindow.StringResult = (string)parameter;
                    CloseDialog(mDialogWindow);
                });
            }
        }

        #endregion

        #region Public Dialog Show Methods

        /// <summary>
        /// Displays a single dialog box to the user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <typeparam name="T">The view model type for this control</typeparam>
        /// <returns></returns>
        public Task<DialogResult> ShowDialog<T>(T viewModel)
            where T : BaseDialogViewModel
        {
            // Creates a task await dialog closing
            var tcs = new TaskCompletionSource<DialogResult>();

            // Run on UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Create the result variable for use later
                DialogResult result = DialogResult.None;
                try
                {
                    // Match controls expected sizes to the dialog windows view model
                    mDialogWindow.ViewModel.WindowMinimumWidth = WindowMinimumWidth;
                    mDialogWindow.ViewModel.WindowMinimumHeight = WindowMinimumHeight;
                    mDialogWindow.ViewModel.TitleHeight = TitleHeight;
                    mDialogWindow.ViewModel.Title = string.IsNullOrEmpty(viewModel.Title) ? Title : viewModel.Title;

                    // Set this control to the dialog window content
                    mDialogWindow.ViewModel.Content = this;

                    // Setup this controls data context binding to the view model
                    DataContext = viewModel;

                    // Show dialog
                    mDialogWindow.ShowDialog();

                    // Update the result variable
                    result = mDialogWindow.DialogResult;
                }
                finally
                {
                    // Let caller know we finished and return the result
                    tcs.TrySetResult(result);
                }
            });

            return tcs.Task;
        }


        /// <summary>
        /// Displays a single dialog box to the user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <typeparam name="T">The view model type for this control</typeparam>
        /// <returns>A <see cref="string"/> result</returns>
        public Task<string> ShowStringDialog<T>(T viewModel)
            where T : BaseDialogViewModel
        {
            // Creates a task await dialog closing
            var tcs = new TaskCompletionSource<string>();

            // Run on UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Create the result variable for use later
                string result = default;
                try
                {
                    // Match controls expected sizes to the dialog windows view model
                    mDialogWindow.ViewModel.WindowMinimumWidth = WindowMinimumWidth;
                    mDialogWindow.ViewModel.WindowMinimumHeight = WindowMinimumHeight;
                    mDialogWindow.ViewModel.TitleHeight = TitleHeight;
                    mDialogWindow.ViewModel.Title = string.IsNullOrEmpty(viewModel.Title) ? Title : viewModel.Title;

                    // Set this control to the dialog window content
                    mDialogWindow.ViewModel.Content = this;

                    // Setup this controls data context binding to the view model
                    DataContext = viewModel;

                    // Show dialog
                    mDialogWindow.ShowDialog();

                    // Update the result variable
                    result = mDialogWindow.StringResult;
                }
                finally
                {
                    // Let caller know we finished and return the result
                    tcs.TrySetResult(result);
                }
            });

            return tcs.Task;
        }

        public void CloseDialog(DialogWindow dialog)
        {
            dialog.Close();
        }

        #endregion
    }
}
