using OrderReader.Core;
using System.Windows;

namespace OrderReader
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        #region Private Memebers

        /// <summary>
        /// The view model for this window
        /// </summary>
        private DialogWindowViewModel mViewModel;

        #endregion

        #region Public Properties

        /// <summary>
        /// This view model for this window
        /// </summary>
        public DialogWindowViewModel ViewModel
        {
            get => mViewModel;
            set
            {
                // Set new value
                mViewModel = value;

                // Update data context
                DataContext = mViewModel;
            }
        }

        /// <summary>
        /// Dialog result that will be returned by a dialog window
        /// </summary>
        public DialogResult DialogResult { get; set; } = DialogResult.None;

        /// <summary>
        /// An alternative result which will be returned for some dialog boxes
        /// e.g. a DragDropBox
        /// </summary>
        public string StringResult { get; set; } = default;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DialogWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}
