using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderReader.Core.DataModels.Customers;
using OrderReader.Core.DataModels.Orders;
using OrderReader.Core.Interfaces;

namespace OrderReader.Core.DataModels.FileHandling.OrderParsers;

public class FppExcelParser(INotificationService notificationService) : IParseOrder
{
    #region Private Variables

    /// <summary>
    /// Where in a table string array we can find the column count
    /// </summary>
    private const int TableColumnCountPosition = 0;

    /// <summary>
    /// Where in a table string array we can find the row count
    /// </summary>
    private const int TableRowCountPosition = 1;

    private readonly INotificationService _notificationService = notificationService;

    #endregion

    #region Public Properties

    /// <summary>
    /// The file extension that this parser works with
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
        foreach (var valuePair in orderText)
        {
            var customerIdCell = GetCell(new Cell("A1"), valuePair.Value);

            if (customers.HasCustomerOrderName(customerIdCell))
            {
                return customers.GetCustomerByOrderName(customerIdCell);
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
    /// <param name="ordersLibrary">The library containing all order data</param>
    public async Task ParseOrderAsync(Dictionary<string, string[]> orderText, string fileName, Customer customer, OrdersLibrary ordersLibrary)
    {
        foreach (var table in orderText.Values)
        {
            // Table information
            var columnCount = int.Parse(table[TableColumnCountPosition]);
            var rowCount = int.Parse(table[TableRowCountPosition]);
            
            const int firstOrderRow = 7;
            const int depotCol = 2;
            const int productCol = 3;
            const int qtyPerPalletCol = 8;
            var belowOrdersRow = firstOrderRow;
            
            // Required data to extract in string form
            Dictionary<int, string?> depotStrings = [];
            Dictionary<int, string?> productStrings = [];
            Dictionary<int, string?> qtyPerPalletStrings = [];

            while (true)
            {
                var cellContent = GetCell(1, belowOrdersRow, table);
                if (string.IsNullOrEmpty(cellContent)) break;
                
                // Get the depot, product and qty per pallet
                depotStrings.Add(belowOrdersRow, GetCell(depotCol, belowOrdersRow, table));
                productStrings.Add(belowOrdersRow, GetCell(productCol, belowOrdersRow, table));
                qtyPerPalletStrings.Add(belowOrdersRow, GetCell(qtyPerPalletCol, belowOrdersRow, table));

                belowOrdersRow += 2;
            }

            var dateRow = belowOrdersRow + 2;
            var totalRow = dateRow + 1;
            const int dateColumnStart = 9;
            const int dateColumnEnd = 9 + 6;

            for (var c = dateColumnStart; c <= dateColumnEnd; c++)
            {
                Dictionary<int, decimal> palletQuantities = [];
                DateTime deliveryDate;
                
                var dateString = GetCell(c, dateRow, table);
                var orderReference = GetCell(c, belowOrdersRow, table);
                var totalQtyString = GetCell(c, totalRow, table);
                var totalQty = 0m;
                
                // Variables that will be required
                List<OrderWarning> depotWarnings = [];
                List<string> readQtyErrors = [];
                
                // Get total quantities
                if (totalQtyString != null)
                {
                    if (!decimal.TryParse(totalQtyString, out totalQty))
                    {
                        readQtyErrors.Add($"Could not read total quantity from the \"{dateString}\" column.\n");
                    }
                }

                // Get pallet quantities
                for (var r = firstOrderRow; r < belowOrdersRow; r += 2)
                {
                    var quantity = 0m;
                    var cell = new Cell(c, r);
                    var cellValue = GetCell(cell, table);
                    if (cellValue != null)
                    {
                        if (!decimal.TryParse(cellValue, out quantity))
                        {
                            readQtyErrors.Add($"Depot: {depotStrings[r]}, Date: {dateString}, Product: {productStrings[r]}\n");
                        }
                    }
                    
                    palletQuantities.Add(r, quantity);
                }
                
                // Validation
                
                // First just make sure there are some kind of orders on this day, if there are no orders and the total quantity is 0, skip to the next day
                var sumOfPalletQuantities = palletQuantities.Values.Sum();
                if (sumOfPalletQuantities == 0 && totalQty == 0)
                {
                    // Just skip further validation and processing, since there are no quantities to add
                    continue;
                }
                
                // Make sure that we have found all necessary data
                if (palletQuantities.Count == 0 || string.IsNullOrEmpty(orderReference) || string.IsNullOrEmpty(dateString))
                {
                    var errorMessage = $"Following issues have been found in file {fileName}:\n";
                    if (palletQuantities.Count == 0) errorMessage += "* No orders have been found\n";
                    if (string.IsNullOrEmpty(orderReference)) errorMessage += "* PO reference number was not found\n";
                    if (string.IsNullOrEmpty(dateString)) errorMessage += "* Delivery date was not found\n";
                    errorMessage += "\nThis order was not processed.\n";
                    errorMessage += "If you think this file has all the correct data, please contact Patryk Z.";

                    // Display error message to the user
                    await _notificationService.ShowMessage("File Processing Error", errorMessage);
                }
                else
                {
                    // Process and validate delivery date
                    if (!DateTime.TryParse(dateString.Trim(), out deliveryDate))
                    {
                        var errorMessage = $"Could not read the date from file {fileName}, in column {c}.\n\n" +
                                           "Please check the date in Excel file. If the date format looks correct then please contact Patryk Z.\n" +
                                           "\nThis order was not processed.";

                        // Display error message to the user
                        await _notificationService.ShowMessage("File Processing Error", errorMessage);
                    }
                    else if (deliveryDate != DateTime.Today.AddDays(1))
                    {
                        var errorMessage = $"Delivery date ({deliveryDate.ToShortDateString()}) in file {fileName} is not tomorrow.";

                        // Create a new warning object for this order
                        depotWarnings.Add(new OrderWarning(OrderWarning.WarningType.UnusualDate, errorMessage));

                        // Display error message to the user
                        //await _notificationService.ShowMessage("Unusual Date Warning", errorMessage);
                    }

                    // Display all the cells that could not be read from
                    if (readQtyErrors.Count > 0)
                    {
                        var errorMessage = $"Not all data could be read from file {fileName}.\n" +
                                           "Please see below which orders could not be read:\n\n";
                        foreach (var error in readQtyErrors)
                        {
                            errorMessage += error;
                        }
                        errorMessage += "\nThis order was not processed.";

                        // Display error message to the user
                        await _notificationService.ShowMessage("File Processing Error", errorMessage);
                        continue;
                    }

                    // Create the order objects
                    for (var row = firstOrderRow; row < belowOrdersRow; row += 2)
                    {
                        Depot? depot = customer.GetDepot(depotStrings[row]);
                        Product? product = customer.GetProduct(productStrings[row]);
                        if (!decimal.TryParse(qtyPerPalletStrings[row], out var qtyPerPallet))
                        {
                            var errorMessage = $"Could not read quantity per pallet for product: {productStrings[row]}.\n";
                            errorMessage += "\nThis order was not processed.";

                            // Display error message to the user
                            await _notificationService.ShowMessage("File Processing Error", errorMessage);
                            continue;
                        }

                        if (depot is null || product is null)
                        {
                            string errorMessage;
                            if (depot is null)
                            {
                                errorMessage = $"Unknown depot name found: {depotStrings[row]}, in file {fileName}.\n";
                            }
                            else
                            {
                                errorMessage = $"Unknown product name found: {productStrings[row]}, in file {fileName}.\n";
                            }
                            errorMessage += "\nThis order was not processed.";

                            // Display error message to the user
                            await _notificationService.ShowMessage("File Processing Error", errorMessage);
                            continue;
                        }

                        var updatingExistingOrder = false;

                        var order = ordersLibrary.Orders.FirstOrDefault(x =>
                            x.Customer.Id == customer.Id &&
                            x.DepotID == depot.Id &&
                            x.Date == deliveryDate);

                        if (order is not null) updatingExistingOrder = true;
                        
                        order ??= new Order(orderReference, deliveryDate, customer.Id, depot.Id, customer);
                        order.AddProduct(product.Id, (double)(palletQuantities[row] * qtyPerPallet));
                        
                        // Add warnings
                        if (depotWarnings.Count > 0)
                        {
                            foreach (var warning in depotWarnings)
                            {
                                order.AddWarning(warning);
                            }
                        }
                        
                        if (updatingExistingOrder) continue;
                        
                        // Check if the same order already exists
                        if (ordersLibrary.HasOrder(order))
                        {
                            var errorMessage = $"The order in file {fileName} for depot {order.DepotName} could not be processed. The same order already exists.";

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
    private static string? GetCell(int column, int row, string[] tableText)
    {
        string? result = null;

        var columnCount = int.Parse(tableText[TableColumnCountPosition]);
        var rowCount = int.Parse(tableText[TableRowCountPosition]);

        var c = column - 1;
        var r = row - 1;

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
    private static string? GetCell(Cell cell, string[] tableText)
    {
        return GetCell(cell.column, cell.row, tableText);
    }

    #endregion
}