using System.IO;
using System.Threading.Tasks;
using OrderReader.Core.DataModels.Customers;
using OrderReader.Core.DataModels.Orders;
using OrderReader.Core.Interfaces;

namespace OrderReader.Core.DataModels.FileHandling;

public static class FileImport
{
    #region Public Properties

    public static INotificationService NotificationService { get; set; }
    public static OrdersLibrary OrdersLibrary { get; set; }
    public static CustomersHandler CustomersHandler { get; set; }

    #endregion

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
                    await NotificationService.ShowMessage("File Processing Error", "Unsupported file type.");
                    return false;
            }
        }

        // Display error message to the user
        await NotificationService.ShowMessage("File Processing Error", "Could not process this file.");

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
        ExcelImport excelImporter = new ExcelImport(filePath, NotificationService);

        return await excelImporter.ProcessFileAsync(CustomersHandler, OrdersLibrary);
    }

    /// <summary>
    /// Read a PDF file
    /// </summary>
    /// <param name="filePath">A path to the file that we want to process</param>
    /// <returns>Whether or not processing was successful</returns>
    private static async Task<bool> ReadPDFFileAsync(string filePath)
    {
        PDFImport pdfImporter = new PDFImport(filePath, NotificationService);

        return await pdfImporter.ProcessFileAsync(CustomersHandler, OrdersLibrary);
    }

    #endregion
}