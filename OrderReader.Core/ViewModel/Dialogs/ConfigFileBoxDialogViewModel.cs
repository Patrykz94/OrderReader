using System.IO;

namespace OrderReader.Core
{
    /// <summary>
    /// Details for a config file box dialog
    /// </summary>
    public class ConfigFileBoxDialogViewModel : BaseDialogViewModel, IFilesDropped
    {
        #region Public properties

        /// <summary>
        /// Heading to be displayed
        /// </summary>
        public string Heading { get; set; }

        /// <summary>
        /// Message to be displayed
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The full path to the file
        /// </summary>
        public string FilePath { get; set; } = default;

        /// <summary>
        /// The error message if invalid file is used
        /// </summary>
        public string ErrorField { get; set; } = "File required";

        /// <summary>
        /// Text that will appear on the OK answer Button
        /// </summary>
        public string OKButtonText { get; set; } = "OK";

        /// <summary>
        /// Text that will appear on the Cancel answer Button
        /// </summary>
        public string CancelButtonText { get; set; } = "Cancel";

        /// <summary>
        /// Whether or not the file dropped is valid
        /// </summary>
        public bool FileValid { get; set; } = false;

        #endregion

        #region Public Helpers

        /// <summary>
        /// Process the file dropped
        /// </summary>
        /// <param name="files"></param>
        public void OnFilesDropped(string[] files)
        {
            string fPath = files[0];
            if (File.Exists(fPath))
            {
                // Determine if we are dealing with a correct file extension
                string ext = Path.GetExtension(fPath);

                if (ext.ToLower() == ".xml")
                {
                    FilePath = fPath;
                    FileValid = true;
                    ErrorField = "";

                    return;
                }
            }

            FilePath = "";
            ErrorField = "Invalid file provided";
            FileValid = false;
        }

        #endregion
    }
}
