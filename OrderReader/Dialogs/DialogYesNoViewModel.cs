using OrderReader.Dialogs.BaseViewModels;

namespace OrderReader.Dialogs
{
    public class DialogYesNoViewModel : DialogViewModelBase
    {
        #region Public Properties
        public string SecondaryButtonText { get; set; }

        #endregion

        #region Constructors

        public DialogYesNoViewModel()
        {
            PrimaryButtonText = "Yes";
            SecondaryButtonText = "No";
        }

        public DialogYesNoViewModel(string message, string title = "Message", string yesButtonText = "Yes", string noButtonText = "No")
        {
            Message = message;
            Title = title;
            PrimaryButtonText = yesButtonText;
            SecondaryButtonText = noButtonText;
        }

        #endregion

        #region Public Methods

        public void PrimaryButton()
        {
            Close(true);
        }

        public void SecondaryButton()
        {
            Close(false);
        }

        #endregion
    }
}