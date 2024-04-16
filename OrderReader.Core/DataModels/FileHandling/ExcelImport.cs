using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using OrderReader.Core.Interfaces;

namespace OrderReader.Core
{
    public class ExcelImport : IFileImport
    {
        #region Private Variables
        
        private readonly IUserNotificationService _userNotificationService;

        #endregion

        #region Public Properties

        /// <summary>
        /// DataSet containing the data from excel file
        /// </summary>
        private readonly DataSet excelData;

        /// <summary>
        /// All text extracted from a PDF file, separated by lines
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

        #endregion

        #region Constructor

        // Default constructor
        public ExcelImport(string filePath, IUserNotificationService userNotificationService)
        {
            _userNotificationService = userNotificationService;
            fileExtension = Path.GetExtension(filePath).ToLower();
            fileName = Path.GetFileName(filePath);
            excelData = ReadExcelData(filePath);
            orderText = DataSetToDictionary(excelData);
        }

        #endregion

        #region Public Helpers

        public async Task<bool> ProcessFileAsync()
        {
            // List of available processors - TODO: Populate this list with actual parsers
            List<IParseOrder> orderParsers = new List<IParseOrder>();
            orderParsers.Add(new LidlExcelParser(_userNotificationService));
            orderParsers.Add(new LidlNewExcelParser(_userNotificationService));

            CustomersHandler customers = IoC.Customers();
            customers.LoadCustomers();

            foreach (IParseOrder parser in orderParsers)
            {
                if (parser.FileExtension == fileExtension)
                {
                    Customer customer = parser.GetCustmer(orderText, customers);

                    if (customer != null)
                    {
                        await parser.ParseOrderAsync(orderText, fileName, customer);
                        return true;
                    }
                }
            }

            string errorMessage = $"Could not identify customer information in file {fileName}\n\n" +
                    "Please double check the Excel file to make sure it contains a valid order.\n";

            // Display error message to the user
            await _userNotificationService.ShowMessage("File Processing Error", errorMessage);

            return false;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Extracts all data from an Excel file as a <see cref="DataSet"/> object
        /// </summary>
        /// <param name="filePath">The location of Excel file to read from</param>
        /// <returns>A <see cref="DataSet"/> object containing all Excel data</returns>
        private DataSet ReadExcelData(string filePath)
        {
            // Make sure the file exists
            if (File.Exists(filePath))
            {
                // Make sure we are looking at an Excel file
                string ext = Path.GetExtension(filePath);
                if (ext.ToLower() == ".xlsx")
                {
                    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            return reader.AsDataSet();
                        }
                    }
                }
            }

            // If something went wrong, return a blank DataSet
            return new DataSet();
        }

        /// <summary>
        /// A function that converts a <see cref="DataSet"/> into a <see cref="Dictionary{TKey, TValue}"/> of <see cref="string"/> and <see cref="string[]"/> that can be procesed further
        /// Each <see cref="KeyValuePair{TKey, TValue}"/> in the dictionary represents a <see cref="DataTable"/> from the <see cref="DataSet"/>
        /// Each Key represents a table name and each value is an array of strings representing values from each cell
        /// </summary>
        /// <param name="data">A <see cref="DataSet"/> containing data from the excel spreadsheet we loaded</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> representing the <see cref="DataSet"/> we loaded</returns>
        private Dictionary<string, string[]> DataSetToDictionary(DataSet data)
        {
            Dictionary<string, string[]> orderData = new Dictionary<string, string[]>();

            foreach (DataTable table in data.Tables)
            {
                int columnCount = table.Columns.Count;
                int rowCount = table.Rows.Count;
                string allText = columnCount.ToString() + Environment.NewLine + rowCount.ToString();

                foreach (DataRow row in table.Rows)
                {
                    for (int c = 0; c < row.ItemArray.Length; c++)
                    {
                        allText += Environment.NewLine + row.ItemArray[c].ToString();
                    }
                }

                string[] tableData = allText.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                );

                orderData.Add(table.TableName, tableData);
            }

            return orderData;
        }

        #endregion
    }
}
