using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderReader.Core.DataModels.Customers;
using OrderReader.Core.DataModels.FileHandling.ExcelHelpers;
using OrderReader.Core.DataModels.Orders;
using OrderReader.Core.Interfaces;

namespace OrderReader.Core.DataModels.FileHandling.OrderParsers;

public class LidlExcelParser(INotificationService notificationService) : IParseOrder
{
    #region Private Variables

    private readonly INotificationService _notificationService = notificationService;

    #endregion

    #region Public Properties

    /// <summary>
    /// The file extension that this parser works with
    /// </summary>
    public string FileExtension => ".xlsx";

    #endregion

    #region Public Helpers

    public Customer? GetCustomer(Dictionary<string, string[]> orderText, CustomersHandler customers)
    {
        foreach (var sheetText in orderText)
        {
            var sheet = new ExcelSheet(sheetText.Key, sheetText.Value);
            
            var customerIdCell = sheet.GetCell("D1");

            if (!string.IsNullOrEmpty(customerIdCell) && customers.HasCustomerOrderName(customerIdCell))
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
    /// <param name="ordersLibrary"></param>
    public async Task ParseOrderAsync(Dictionary<string, string[]> orderText, string fileName, Customer customer, OrdersLibrary ordersLibrary)
    {
        foreach (var table in orderText)
        {
            // Load the sheet
            var sheet = new ExcelSheet(table.Key, table.Value);

            // Variables that will be required
            Dictionary<int, List<OrderWarning>> depotWarnings = [];
            List<string> readQtyErrors = [];

            // Required data to extract in string form
            Dictionary<int, string?> depotStrings = [];
            Dictionary<int, string?> productStrings = [];
            Dictionary<Cell, double> orderQuantities = [];

            // Data locations
            var dateCell = new Cell("B1");
            const int productCol1 = 4;
            const int productCol2 = 11;
            const int truckCol = 3;
            const int productRowStart = 8;
            const int depotRow = 6;
            const int depotColStart = 31;
            var depotColEnd = sheet.ColumnCount - 85;
            var refCell = new Cell("B8");

            // Extract the data
            var dateString = sheet.GetCell(dateCell);
            var orderReference = sheet.GetCell(refCell);

            // Get list of depots
            for (var c = depotColStart; c <= depotColEnd; c++)
            {
                depotStrings.Add(c, sheet.GetCell(c, depotRow));
            }

            // Get list of products
            for (var r = productRowStart; r < productRowStart + 30; r++)
            {
                var c1 = sheet.GetCell(productCol1, r);
                var c2 = sheet.GetCell(productCol2, r);
                var truck = sheet.GetCell(truckCol, r);
                if (c1 == null || c2 == null || truck is not "1") break;
                productStrings.Add(r, $"{c1}{c2}");
            }

            // Get all cells which contain ordered product
            for (var c = depotColStart; c <= depotColEnd; c++)
            {
                for (var r = productRowStart; r <= productRowStart+productStrings.Count-1; r++)
                {
                    var cell = new Cell(c, r);
                    var cellValue = sheet.GetCell(cell);
                    if (cellValue != null)
                    {
                        if (double.TryParse(cellValue, out var quantity))
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

            List<int> usedDepotColumns = [];

            // Get all the used depot information
            foreach (var orderQuantity in orderQuantities)
            {
                foreach (var depotString in depotStrings)
                {
                    if (orderQuantity.Key.Column == depotString.Key)
                    {
                        if (!string.IsNullOrEmpty(depotString.Value) && customer.HasDepotOrderName(depotString.Value))
                        {
                            if (!usedDepotColumns.Contains(depotString.Key)) usedDepotColumns.Add(depotString.Key);
                        }
                        else
                        {
                            var errorMessage = $"Unknown depot \"{depotString.Value}\" was found in file {fileName}.\n" +
                                               "If this is a new depot, please add it to the list.\n" +
                                               $"\nThis file will be processed without depot \"{depotString.Value}\". You will need to add order for that depot manually.";

                            // Display error message to the user
                            await _notificationService.ShowMessage("File Processing Error", errorMessage);
                        }
                    }
                }
            }

            List<int> usedProductRows = [];

            // Get all the used product information
            foreach (var orderQuantity in orderQuantities)
            {
                foreach (var productString in productStrings)
                {
                    if (orderQuantity.Key.Row == productString.Key)
                    {
                        if (!string.IsNullOrEmpty(productString.Value) && customer.HasProductOrderName(productString.Value))
                        {
                            if (!usedProductRows.Contains(productString.Key)) usedProductRows.Add(productString.Key);
                        }
                        else
                        {
                            var errorMessage = $"Unknown product \"{productString.Value}\" was found in file {fileName}.\n" +
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
                var errorMessage = $"Following issues have been found in file {fileName}:\n";
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
                if (!DateTime.TryParse(dateString.Trim(), out var deliveryDate))
                {
                    var errorMessage = $"Could not read the date from file {fileName}.\n" +
                                       "Please check the date in Excel file. If the date format looks correct then please contact Patryk Z.\n" +
                                       "\nThis file was not processed.";

                    // Display error message to the user
                    await _notificationService.ShowMessage("File Processing Error", errorMessage);
                }
                else if (deliveryDate != DateTime.Today.AddDays(1))
                {
                    var errorMessage = $"Delivery date ({deliveryDate.ToShortDateString()}) in file {fileName} is not tomorrow.";

                    // Create a new warning object for this order
                    foreach (var col in usedDepotColumns)
                    {
                        if (!depotWarnings.ContainsKey(col)) depotWarnings.Add(col, []);
                        depotWarnings[col].Add(new OrderWarning(OrderWarning.WarningType.UnusualDate, errorMessage));
                    }

                    // Display error message to the user
                    await _notificationService.ShowMessage("Unusual Date Warning", errorMessage);
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
                    errorMessage += "\nThis file was not processed.";

                    // Display error message to the user
                    await _notificationService.ShowMessage("File Processing Error", errorMessage);
                }

                // Create the order objects
                foreach (var col in usedDepotColumns)
                {
                    var depot = customer.GetDepot(depotStrings[col] ?? string.Empty);
                    var order = new Order(orderReference, deliveryDate, customer.Id, depot.Id, customer);
                    // Add products
                    foreach (var row in usedProductRows)
                    {
                        var cell = new Cell(col, row);
                        foreach (var orderQuantity in orderQuantities)
                        {
                            if (orderQuantity.Key.Equals(cell) && orderQuantity.Value > 0.0)
                            {
                                var product = customer.GetProduct(productStrings[row] ?? string.Empty);
                                order.AddProduct(product.Id, orderQuantity.Value);
                            }
                        }
                    }
                    // Add warnings
                    if (depotWarnings.TryGetValue(col, out var value))
                    {
                        foreach (var warning in value)
                        {
                            order.AddWarning(warning);
                        }
                    }

                    if (order.Products.Count > 0)
                    {
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

            break;
        }
    }

    #endregion
}