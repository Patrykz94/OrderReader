using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace OrderReader.Core
{
    public class FileImport
    {
        #region Private Enums

        /// <summary>
        /// In the future, make each customer processing logic a separate optional plugin
        /// </summary>
        private enum CustomerNames
        {
            Keelings_Coop = 1,
            LidlOld = 2,
            Lidl = 3
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Lines of text that have been read from a PDF file
        /// </summary>
        private string[] LinesOfText = null;

        /// <summary>
        /// Data set which has been read from an Excel file
        /// </summary>
        private DataSet ExcelData = new DataSet();

        #endregion

        #region Public Properties

        /// <summary>
        /// The path to the file we are working on
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// The name of the file we are working on
        /// </summary>
        public string FileName { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="filePath"></param>
        public FileImport(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(FilePath);
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Process the file
        /// </summary>
        /// <returns>True or False whether the file was processed successfully</returns>
        public async Task<bool> ProcessFileAsync()
        {
            // Make sure the file exists
            if (File.Exists(FilePath))
            {
                // Determine what file extension we are dealing with and call appropriate import class
                string ext = Path.GetExtension(FilePath);
                switch (ext.ToLower())
                {
                    case ".xlsx":
                        return await ProcessExcelFileAsync();
                    case ".pdf":
                        return await ProcessPDFFileAsync();
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
        /// <returns>Whether or not processing was successful</returns>
        private async Task<bool> ProcessExcelFileAsync()
        {
            ExcelImport importedExcelFile = new ExcelImport(FilePath);

            int customerId = importedExcelFile.GetCustomerId();

            if (customerId == -1)
            {
                string errorMessage = $"Could not identify customer information in file {FileName}\n\n" +
                    "Please double check the Excel file to make sure it contains a valid order.\n";

                // Display error message to the user
                await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                {
                    Title = "File Processing Error",
                    Message = errorMessage,
                    ButtonText = "OK"
                });

                return false;
            }

            Customer customer = IoC.Customers().GetCustomerByID(customerId);

            ExcelData = importedExcelFile.ExcelData;

            switch ((CustomerNames)customer.Id)
            {
                case CustomerNames.LidlOld:
                    LidlExcelParser.ParseOrderAsync(ExcelData, FileName, customer);
                    break;
                case CustomerNames.Lidl:
                    LidlNewExcelParser.ParseOrderAsync(ExcelData, FileName, customer);
                    break;
                default:
                    // Display error message to the user
                    await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                    {
                        Title = "File Processing Error",
                        Message = "Could not process order for this customer.",
                        ButtonText = "OK"
                    });
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Process a PDF file
        /// </summary>
        /// <returns>Whether or not processing was successful</returns>
        private async Task<bool> ProcessPDFFileAsync()
        {
            PDFImport importedPDFFile = new PDFImport(FilePath);

            int customerId = importedPDFFile.GetCustomerId();

            if (customerId == -1)
            {
                string errorMessage = $"Could not identify customer information in file {FileName}\n\n" +
                    "Please double check the PDF file to make sure it contains a valid order.\n";

                // Display error message to the user
                await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                {
                    Title = "File Processing Error",
                    Message = errorMessage,
                    ButtonText = "OK"
                });

                return false;
            }

            Customer customer = IoC.Customers().GetCustomerByID(customerId);

            LinesOfText = importedPDFFile.Lines;

            switch ((CustomerNames)customer.Id)
            {
                case CustomerNames.Keelings_Coop:
                    KeelingsPDFParser.ParseOrderAsync(LinesOfText, FileName, customer);
                    break;
                default:
                    // Display error message to the user
                    await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                    {
                        Title = "File Processing Error",
                        Message = "Could not process order for this customer.",
                        ButtonText = "OK"
                    });
                    return false;
            }

            return true;
        }

        #endregion
    }
}
