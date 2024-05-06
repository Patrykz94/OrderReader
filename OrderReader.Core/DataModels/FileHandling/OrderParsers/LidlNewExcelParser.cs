using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderReader.Core.DataModels.Customers;
using OrderReader.Core.DataModels.Orders;
using OrderReader.Core.Interfaces;

namespace OrderReader.Core.DataModels.FileHandling.OrderParsers;

public class LidlNewExcelParser(INotificationService notificationService) : IParseOrder
{
    #region Private Variables

    /// <summary>
    /// Where in a table string array we can find the column count
    /// </summary>
    private static readonly int tableColumntCountPosition = 0;

    /// <summary>
    /// Where in a table string array we can find the row count
    /// </summary>
    private static readonly int tableRowCountPosition = 1;

    private readonly INotificationService _notificationService = notificationService;

    #endregion

    #region Public Properties

    /// <summary>
    /// The file extention that this parser works with
    /// </summary>
    public string FileExtension { get; } = ".xlsx";

    #endregion

    #region Private Structs

    /// <summary>
    /// A struct that represents the cell position in a table
    /// </summary>
    public struct Cell
    {
        /// <summary>
        /// Constructs a cell out of column and row numbers
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        public Cell(int column, int row)
        {
            this.column = column;
            this.row = row;
        }

        /// <summary>
        /// Constructs a cell out of cell name string
        /// </summary>
        /// <param name="cell"></param>
        public Cell(string cell)
        {
            bool foundNums = false;

            List<int> col = new List<int>();
            string row = "";

            foreach (char character in cell)
            {
                if (character >= 'A' && character <= 'Z')
                {
                    if (foundNums) { this.column = -1; this.row = -1; };
                    col.Add(character - 'A');
                }
                else if (character >= 'a' && character <= 'z')
                {
                    if (foundNums) { this.column = -1; this.row = -1; };
                    col.Add(character - 'a');
                }
                else if (character >= '0' && character <= '9')
                {
                    foundNums = true;
                    row += character;
                }
                else
                {
                    this.column = -1;
                    this.row = -1;
                }
            }

            int c = 0;
            for (int i = 0; i < col.Count; i++)
            {
                c += ((col[i] + 1) * (int)Math.Pow(26.0, i));
            }

            if (!int.TryParse(row, out int r)) { this.column = -1; this.row = -1; };

            this.column = c;
            this.row = r;
        }

        public bool IsSameAs(Cell otherCell)
        {
            return column == otherCell.column && row == otherCell.row;
        }

        public int column;
        public int row;
    }

    #endregion

    #region Public Helpers

    public Customer GetCustomer(Dictionary<string, string[]> orderText, CustomersHandler customers)
    {
        foreach (KeyValuePair<string, string[]> valuePair in orderText)
        {
            string CellD1 = GetCell(new Cell("D1"), valuePair.Value);

            if (customers.HasCustomerOrderName(CellD1))
            {
                return customers.GetCustomerByOrderName(CellD1);
            }
        }

        return null;
    }

    /// <summary>
    /// A function that reads the order and extracts all required information
    /// </summary>
    /// <param name="orderText">The data that we should read from</param>
    /// <param name="fileName">Name of the order file</param>
    /// <param name="customer">Customer associated with this order</param>
    public async Task ParseOrderAsync(Dictionary<string, string[]> orderText, string fileName, Customer customer, OrdersLibrary ordersLibrary)
    {
        foreach (string[] table in orderText.Values)
        {
            // TODO: Future improvement: consolidate errors of the same type together and display at once

            // Table information
            int columnCount = int.Parse(table[tableColumntCountPosition]);
            int rowCount = int.Parse(table[tableRowCountPosition]);

            // Variables that will be required
            Dictionary<int, List<OrderWarning>> depotWarnings = new Dictionary<int, List<OrderWarning>>();
            List<string> readQtyErrors = new List<string>();

            // Required data to extract in string form
            string dateString = null;
            string orderReference = null;
            Dictionary<int, string> depotStrings = new Dictionary<int, string>();
            Dictionary<int, string> productStrings = new Dictionary<int, string>();
            Dictionary<Cell, double> orderQuantities = new Dictionary<Cell, double>();

            // Required data
            List<Depot> depots = new List<Depot>();
            List<Product> products = new List<Product>();
            DateTime deliveryDate = DateTime.MinValue;

            // Data locations
            Cell dateCell = new Cell("B1");
            int productCol1 = 4;
            int productCol2 = 11;
            int truckCol = 3;
            int productRowStart = 8;
            int depotRow = 6;
            int depotColStart = 31;
            int depotColEnd = columnCount - 85;
            Cell refCell = new Cell("B8");

            // Extract the data
            dateString = GetCell(dateCell, table);
            orderReference = GetCell(refCell, table);

            // Get list of depots
            for (int c = depotColStart; c <= depotColEnd; c++)
            {
                depotStrings.Add(c, GetCell(c, depotRow, table));
            }

            // Get list of products
            for (int r = productRowStart; r < productRowStart + 30; r++)
            {
                string c1 = GetCell(productCol1, r, table);
                string c2 = GetCell(productCol2, r, table);
                string truck = GetCell(truckCol, r, table);
                if (c1 == null || c2 == null || truck == null || truck != "1") break;
                productStrings.Add(r, $"{c1}{c2}");
            }

            // Get all cells wich contain ordered product
            for (int c = depotColStart; c <= depotColEnd; c++)
            {
                for (int r = productRowStart; r <= productRowStart+productStrings.Count-1; r++)
                {
                    Cell cell = new Cell(c, r);
                    string cellValue = GetCell(cell, table);
                    if (cellValue != null)
                    {
                        if (double.TryParse(cellValue, out double quantity))
                        {
                            if (quantity > 0.0)
                            {
                                orderQuantities.Add(cell, quantity);
                            }
                        }
                        else
                        {
                            readQtyErrors.Add($"Depot: {depotStrings[c]}, Product: {productStrings[r]}\n");
                        }
                    }
                }
            }

            List<int> usedDepotColumns = new List<int>();

            // Get all the used depot information
            foreach (var orderQuantity in orderQuantities)
            {
                foreach (var depotString in depotStrings)
                {
                    if (orderQuantity.Key.column == depotString.Key)
                    {
                        if (customer.HasDepotOrderName(depotString.Value))
                        {
                            if (!usedDepotColumns.Contains(depotString.Key)) usedDepotColumns.Add(depotString.Key);
                        }
                        else
                        {
                            string errorMessage = $"Unknown depot \"{depotString.Value}\" was found in file {fileName}.\n" +
                                                  "If this is a new depot, please add it to the list.\n" +
                                                  $"\nThis file will be processed without depot \"{depotString.Value}\". You will need to add order for that depot manually.";

                            // Display error message to the user
                            await _notificationService.ShowMessage("File Processing Error", errorMessage);
                        }
                    }
                }
            }

            List<int> usedProductRows = new List<int>();

            // Get all the used product information
            foreach (var orderQuantity in orderQuantities)
            {
                foreach (var productString in productStrings)
                {
                    if (orderQuantity.Key.row == productString.Key)
                    {
                        if (customer.HasProductOrderName(productString.Value))
                        {
                            if (!usedProductRows.Contains(productString.Key)) usedProductRows.Add(productString.Key);
                        }
                        else
                        {
                            string errorMessage = $"Unknown product \"{productString.Value}\" was found in file {fileName}.\n" +
                                                  "If this is a new product, please add it to the list.\n" +
                                                  $"\nThis file will be processed without product \"{productString.Value}\". You will need to add that product to order manually if required.";

                            // Display error message to the user
                            await _notificationService.ShowMessage("File Processing Error", errorMessage);
                        }
                    }
                }
            }

            // Validation
            // Make sure that we have found all necessary data
            if (orderQuantities.Count == 0 || orderReference == null || dateString == null)
            {
                string errorMessage = $"Following issues have been found in file {fileName}:\n";
                if (orderQuantities.Count == 0) errorMessage += "* No orders have been found\n";
                if (orderReference == null) errorMessage += "* PO reference number was not found\n";
                if (dateString == null) errorMessage += "* Delivery date was not found\n";
                errorMessage += "\nThis file was not processed.\n";
                errorMessage += "If you think this file has all the correct data, please contact Patryk Z.";

                // Display error message to the user
                await _notificationService.ShowMessage("File Processing Error", errorMessage);
            }
            else
            {
                // Process and validate delivery date
                if (!DateTime.TryParse(dateString.Trim(), out deliveryDate))
                {
                    string errorMessage = $"Could not read the date from file {fileName}.\n" +
                                          "Please check the date in Excel file. If the date format looks correct then please contact Patryk Z.\n" +
                                          "\nThis file was not processed.";

                    // Display error message to the user
                    await _notificationService.ShowMessage("File Processing Error", errorMessage);
                }
                else if (deliveryDate != DateTime.Today.AddDays(1))
                {
                    string errorMessage = $"Delivery date ({deliveryDate.ToShortDateString()}) in file {fileName} is not tomorrow.";

                    // Create a new warning object for this order
                    foreach (int col in usedDepotColumns)
                    {
                        if (!depotWarnings.ContainsKey(col)) depotWarnings.Add(col, new List<OrderWarning>());
                        depotWarnings[col].Add(new OrderWarning(OrderWarning.WarningType.UnusualDate, errorMessage));
                    }

                    // Display error message to the user
                    await _notificationService.ShowMessage("Unusual Date Warning", errorMessage);
                }

                // Display all the cells that could not be read from
                if (readQtyErrors.Count > 0)
                {
                    string errorMessage = $"Not all data could be read from file {fileName}.\n" +
                                          "Please see below which orders could not be read:\n\n";
                    foreach (string error in readQtyErrors)
                    {
                        errorMessage += error;
                    }
                    errorMessage += "\nThis file was not processed.";

                    // Display error message to the user
                    await _notificationService.ShowMessage("File Processing Error", errorMessage);
                }

                // Create the order objects
                foreach (int col in usedDepotColumns)
                {
                    Depot depot = customer.GetDepot(depotStrings[col]);
                    Order order = new Order(orderReference, deliveryDate, customer.Id, depot.Id, customer);
                    // Add products
                    foreach (int row in usedProductRows)
                    {
                        Cell cell = new Cell(col, row);
                        foreach (var orderQuantity in orderQuantities)
                        {
                            if (orderQuantity.Key.IsSameAs(cell) && orderQuantity.Value > 0.0)
                            {
                                Product product = customer.GetProduct(productStrings[row]);
                                order.AddProduct(product.Id, orderQuantity.Value);
                            }
                        }
                    }
                    // Add warnings
                    if (depotWarnings.ContainsKey(col))
                    {
                        foreach (OrderWarning warning in depotWarnings[col])
                        {
                            order.AddWarning(warning);
                        }
                    }

                    if (order.Products.Count > 0)
                    {
                        // Check if the same order already exists
                        if (ordersLibrary.HasOrder(order))
                        {
                            string errorMessage = $"The order in file {fileName} for depot {order.DepotName} could not be processed. The same order already exists.";

                            // Display error message to the user
                            await _notificationService.ShowMessage("Order Processing Error", errorMessage);
                        }
                        else
                        {
                            // Add this order to the list
                            ordersLibrary.AddOrder(order);
                        }
                    }
                }
            }

            break;
        }
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Selects the cell from a text based table
    /// </summary>
    /// <param name="column">Column number (starting at index 1)</param>
    /// <param name="row">Row number (starting at index 1)</param>
    /// <param name="tableText">A <see cref="string[]"/> from which we want to get the cell value</param>
    /// <returns>Cell value string or null</returns>
    private static string GetCell(int column, int row, string[] tableText)
    {
        string result = null;

        int columnCount = int.Parse(tableText[tableColumntCountPosition]);
        int rowCount = int.Parse(tableText[tableRowCountPosition]);

        int c = column - 1;
        int r = row - 1;

        if (column > 0 && column <= columnCount && row > 0 && row <= rowCount)
        {
            result = tableText[c + r * columnCount + 2];
            if (result == "") result = null;
        }

        return result;
    }

    /// <summary>
    /// Selects the cell from a text based table
    /// </summary>
    /// <param name="cell">A <see cref="Cell"/> object</param>
    /// <param name="tableText">A <see cref="string[]"/> from which we want to get the cell value</param>
    /// <returns></returns>
    private static string GetCell(Cell cell, string[] tableText)
    {
        return GetCell(cell.column, cell.row, tableText);
    }

    #endregion
}