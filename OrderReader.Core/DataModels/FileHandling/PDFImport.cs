using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using OrderReader.Core.DataModels.Customers;
using OrderReader.Core.DataModels.FileHandling.OrderParsers;
using OrderReader.Core.DataModels.Orders;
using OrderReader.Core.Interfaces;

namespace OrderReader.Core.DataModels.FileHandling;

public class PDFImport : IFileImport
{
    #region Private Variables

    /// <summary>
    /// All text extracted from a PDF file, separated by lines and pages
    /// </summary>
    private readonly Dictionary<string, string[]> orderText;

    /// <summary>
    /// The extension of the file that we are working on
    /// </summary>
    private readonly string fileExtension;

    /// <summary>
    /// Name of the file that we are working on including extension
    /// </summary>
    private readonly string fileName;
        
    private readonly INotificationService _notificationService;

    #endregion

    #region Constructor

    // Default constructor that opens the PDF file and reads from it
    public PDFImport(string filePath, INotificationService notificationService)
    {
        _notificationService = notificationService;
        fileExtension = Path.GetExtension(filePath).ToLower();
        fileName = Path.GetFileName(filePath);
        orderText = ReadAllLines(filePath);
    }

    #endregion

    #region Public Helpers

    public async Task<bool> ProcessFileAsync(CustomersHandler customers, OrdersLibrary ordersLibrary)
    {
        // List of available processors - TODO: Populate this list automatically
        List<IParseOrder> orderParsers = new List<IParseOrder>();
        orderParsers.Add(new KeelingsPDFParser(_notificationService));
        orderParsers.Add(new FppPdfParser(_notificationService));

        foreach (IParseOrder parser in orderParsers)
        {
            if (parser.FileExtension == fileExtension)
            {
                Customer customer = parser.GetCustomer(orderText, customers);

                if (customer != null)
                {
                    await parser.ParseOrderAsync(orderText, fileName, customer, ordersLibrary);
                    return true;
                }
            }
        }

        string errorMessage = $"Could not identify customer information in file {fileName}\n\n" +
                              "Please double check the PDF file to make sure it contains a valid order.\n";

        // Display error message to the user
        await _notificationService.ShowMessage("File Processing Error", errorMessage);

        return false;
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Extracts all text from PDF file and separates it by pages and lines
    /// </summary>
    /// <param name="filePath">The location of PDF file to read from</param>
    /// <returns>A <see cref="Dictionary{TKey, TValue}"/> of <see cref="string"/> and <see cref="T:string[]"/>, representing pages and lines of text</returns>
    private Dictionary<string, string[]> ReadAllLines(string filePath)
    {
        // Create the return array first
        Dictionary<string, string[]> text = new Dictionary<string, string[]>();

        // Make sure the file exists
        if (File.Exists(filePath))
        {
            List<string> allExtractedText = new List<string>();

            // Load the PDF file and extract all text
            using (PdfReader reader = new PdfReader(filePath))
            {
                using (PdfDocument doc = new PdfDocument(reader))
                {
                    for (int page = 1; page <= doc.GetNumberOfPages(); page++)
                    {
                        ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                        string extractedText = "";
                        extractedText = string.Concat(extractedText, PdfTextExtractor.GetTextFromPage(doc.GetPage(page), strategy));
                        allExtractedText.Add(extractedText);
                    }
                }
            }

            for (int i = 0; i < allExtractedText.Count; i++)
            {
                text.Add((i + 1).ToString(), allExtractedText[i].Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                ));
            }
        }

        return text;
    }

    #endregion
}