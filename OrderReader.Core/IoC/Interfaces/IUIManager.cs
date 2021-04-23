using System.Threading.Tasks;

namespace OrderReader.Core
{
    /// <summary>
    /// The UI manager that handles any UI interactions in the application
    /// </summary>
    public interface IUIManager
    {
        /// <summary>
        /// Displays a single message box to the user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        Task<DialogResult> ShowMessage(MessageBoxDialogViewModel viewModel);

        /// <summary>
        /// Asks user a question with two possible answers
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        Task<DialogResult> ShowMessage(YesNoBoxDialogViewModel viewModel);

        /// <summary>
        /// Ask user to drag and drop a file
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        Task<string> ShowMessage(ConfigFileBoxDialogViewModel viewModel);
    }
}
