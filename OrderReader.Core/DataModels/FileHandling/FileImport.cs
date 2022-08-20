using System.IO;
using System.Threading.Tasks;

namespace OrderReader.Core
{
    public static class FileImport
    {
        #region Public Helpers

        /// <summary>
        /// Start processing this file
        /// </summary>
        /// <param name="filePath">A path to the file that we want to process</param>
        /// <returns>True or false whether the file could be processed</returns>
        public static async Task<bool> ProcessFileAsync(string filePath)
        {
            // Make sure the file exists
            if (File.Exists(filePath))
            {
                // Determine what file extension we are dealing with and call appropriate import class
                string fileExtension = Path.GetExtension(filePath);

                switch (fileExtension.ToLower())
                {
                    case ".xlsx":
                        return await ReadExcelFileAsync(filePath);
                    case ".pdf":
                        return await ReadPDFFileAsync(filePath);
                    default:
                        // Display error message to the user
                        await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                        {
                            Title = "File Processing Error",
                            Message = "Unsupported file type.",
                            ButtonText = "OK"
                        });
                        return false;
                }
            }

            // Display error message to the user
            await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
            {
                Title = "File Processing Error",
                Message = "Could not process this file.",
                ButtonText = "OK"
            });

            return false;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Process an Excel file
        /// </summary>
        /// <param name="filePath">A path to the file that we want to process</param>
        /// <returns>Whether or not processing was successful</returns>
        private static async Task<bool> ReadExcelFileAsync(string filePath)
        {
            ExcelImport excelImporter = new ExcelImport(filePath);

            return await excelImporter.ProcessFileAsync();
        }

        /// <summary>
        /// Read a PDF file
        /// </summary>
        /// <param name="filePath">A path to the file that we want to process</param>
        /// <returns>Whether or not processing was successful</returns>
        private static async Task<bool> ReadPDFFileAsync(string filePath)
        {
            PDFImport pdfImporter = new PDFImport(filePath);

            return await pdfImporter.ProcessFileAsync();
        }

        #endregion
    }
}
