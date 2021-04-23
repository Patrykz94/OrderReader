using OrderReader.Core;
using System.Threading.Tasks;

namespace OrderReader
{
    /// <summary>
    /// The applications implementation of the <see cref="IUIManager"/>
    /// </summary>
    public class UIManager : IUIManager
    {
        /// <summary>
        /// Displays a single message box to the user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        public async Task<DialogResult> ShowMessage(MessageBoxDialogViewModel viewModel)
        {
            return await new DialogMessageBox().ShowDialog(viewModel);
        }

        /// <summary>
        /// Displays a question box to the user and asks for the answer
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        public async Task<DialogResult> ShowMessage(YesNoBoxDialogViewModel viewModel)
        {
            return await new DialogYesNoBox().ShowDialog(viewModel);
        }

        /// <summary>
        /// Displays a 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<string> ShowMessage(ConfigFileBoxDialogViewModel viewModel)
        {
            return await new DialogConfigFileBox().ShowStringDialog(viewModel);
        }
    }
}
