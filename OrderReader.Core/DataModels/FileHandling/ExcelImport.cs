using ExcelDataReader;
using System.Data;
using System.IO;

namespace OrderReader.Core
{
    public class ExcelImport
    {
        #region Public Properties

        /// <summary>
        /// DataSet containing the data from excel file
        /// </summary>
        public DataSet ExcelData { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="filePath"></param>
        public ExcelImport(string filePath)
        {
            ExcelData = ReadExcelData(filePath);
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Get the name of customer from an order file
        /// </summary>
        /// <returns>Name of customer or <see cref="null"/></returns>
        public int GetCustomerId()
        {
            // Load the customers
            CustomersHandler customers = IoC.Customers();
            customers.LoadCustomers();

            foreach (DataTable table in ExcelData.Tables)
            {
                string CellA1 = table.Rows[0][0].ToString();
                string CellB1 = table.Rows[0][1].ToString();

                if (customers.HasCustomerOrderName(CellA1 + CellB1))
                {
                    return customers.GetCustomerByOrderName(CellA1 + CellB1).Id;
                }
            }
            return -1;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Extracts all data from an Excel file as a <see cref="DataSet"/> object
        /// </summary>
        /// <param name="filePath">The location of PDF file to read from</param>
        /// <returns>A <see cref="DataSet"/> object containing all Excel data</returns>
        private DataSet ReadExcelData(string filePath)
        {
            // Make sure the file exists
            if (File.Exists(filePath))
            {
                // Make sure we are looking at a PDF file
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

        #endregion
    }
}
