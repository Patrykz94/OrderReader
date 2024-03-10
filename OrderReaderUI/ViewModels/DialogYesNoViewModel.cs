using Caliburn.Micro;

namespace OrderReaderUI.ViewModels
{
    public class DialogYesNoViewModel : Screen
    {
        #region Public Properties

        public double WindowMaxWidth { get; private set; }
        public double WindowMaxHeight { get; private set; }
        public double WindowMinWidth { get; private set; }
        public double WindowMinHeight { get; private set; }

        public string Title { get; set; } = "Message";
        public string Message { get; set; } = string.Empty;
        public string YesButtonText { get; set; } = "Yes";
        public string NoButtonText { get; set; } = "No";

        #endregion

        #region Constructor

        public DialogYesNoViewModel()
        {
            Initialize();
        }
        
        public DialogYesNoViewModel(string message, string title = "Message", string yesButtonText = "Yes", string noButtonText = "No")
        {
            Message = message;
            Title = title;
            YesButtonText = yesButtonText;
            NoButtonText = noButtonText;

            Initialize();
        }

        #endregion

        #region Public Methods

        public void YesButton()
        {
            TryCloseAsync(true);
        }

        public void NoButton()
        {
            TryCloseAsync(false);
        }

        #endregion
        
        #region Private Methods

        private void Initialize()
        {
            WindowMaxWidth = 600;
            WindowMaxHeight = 500;
            WindowMinWidth = 400;
            WindowMinHeight = 150;
        }

        #endregion
    }
}