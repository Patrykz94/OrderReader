using System.Windows;
using System.Windows.Controls;

namespace OrderReader
{
    /// <summary>
    /// The View Model for the custom flat window
    /// </summary>
    public class DialogWindowViewModel : WindowViewModel
    {
        #region Public Properties

        /// <summary>
        /// Title of this dialog window
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The content to host inside the dialog
        /// </summary>
        public Control Content { get; set; }

        /// <summary>
        /// The maximum width of a window
        /// </summary>
        public double WindowMaximumWidth { get; set; } = 500.0;

        /// <summary>
        /// Controls if the close button should be enabled or not
        /// Sometimes we may need to get an answer from the user and not allow them to just close the window
        /// </summary>
        public bool CloseButtonEnabled { get; set; } = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window"></param>
        public DialogWindowViewModel(Window window) : base(window)
        {
            // Make minimum size smaller
            WindowMinimumWidth = 250;
            WindowMinimumHeight = 100;

            // Make title bar smaller
            TitleHeight = 30;
        }

        #endregion
    }
}
