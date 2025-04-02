using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderReader.Core.DataModels.Customers;
using OrderReader.Core.DataModels.FileHandling.ExcelHelpers;
using OrderReader.Core.DataModels.Orders;
using OrderReader.Core.Interfaces;

namespace OrderReader.Core.DataModels.FileHandling.OrderParsers;

public class FppExcelParser(INotificationService notificationService) : IParseOrder
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
        foreach (var valuePair in orderText)
        {
            var sheet = new ExcelSheet(valuePair.Key, valuePair.Value);
            
            var customerIdCell = sheet.GetCell("A1");

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
    /// <param name="ordersLibrary">The library containing all order data</param>
    public async Task ParseOrderAsync(Dictionary<string, string[]> orderText, string fileName, Customer customer, OrdersLibrary ordersLibrary)
    {
        List<DateTime> missingReferenceDates = [];
        
        foreach (var table in orderText)
        {
            // Create the sheet
            var sheet = new ExcelSheet(table.Key, table.Value);
            
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
                var cellContent = sheet.GetCell(1, belowOrdersRow);
                if (string.IsNullOrEmpty(cellContent)) break;
                
                // Get the depot, product and qty per pallet
                depotStrings.Add(belowOrdersRow, sheet.GetCell(depotCol, belowOrdersRow));
                productStrings.Add(belowOrdersRow, sheet.GetCell(productCol, belowOrdersRow));
                qtyPerPalletStrings.Add(belowOrdersRow, sheet.GetCell(qtyPerPalletCol, belowOrdersRow));

                belowOrdersRow += 2;
            }

            var totalRow = belowOrdersRow + 1;
            const int dateRow = 6;
            const int dateColumnStart = 9;
            const int dateColumnEnd = 9 + 6;

            for (var c = dateColumnStart; c <= dateColumnEnd; c++)
            {
                Dictionary<int, decimal> palletQuantities = [];

                var dateString = sheet.GetCell(c, dateRow);
                var orderReference = sheet.GetCell(c, belowOrdersRow);
                var totalQtyString = sheet.GetCell(c, totalRow);
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
                    var cellValue = sheet.GetCell(c, r);
                    if (cellValue != null)
                    {
                        if (!decimal.TryParse(cellValue, out quantity))
                        {
                            readQtyErrors.Add($"Depot: {depotStrings[r]}, Collection Date: {dateString}, Product: {productStrings[r]}\n");
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
                if (palletQuantities.Count == 0 || string.IsNullOrEmpty(dateString))
                {
                    var errorMessage = $"Following issues have been found in file {fileName} on sheet {table.Key}:\n";
                    if (palletQuantities.Count == 0) errorMessage += "* No orders have been found\n";
                    if (string.IsNullOrEmpty(dateString)) errorMessage += "* Delivery date was not found\n";
                    errorMessage += "\nOrder for this day was not processed.\n";
                    errorMessage += "If you think this file has all the correct data, please contact Patryk Z.";

                    // Display error message to the user
                    await _notificationService.ShowMessage("File Processing Error", errorMessage);
                }
                else
                {
                    // Process and validate delivery date
                    if (!DateTime.TryParse(dateString.Trim(), out var deliveryDate))
                    {
                        var errorMessage = $"Could not read the date from sheet {table.Key}, in column {c}.\n\n" +
                                           "Please check the date in Excel file. If the date format looks correct then please contact Patryk Z.\n" +
                                           "\nOrder for this day was not processed.";

                        // Display error message to the user
                        await _notificationService.ShowMessage("File Processing Error", errorMessage);
                        continue;
                    }
                    
                    // Convert the date from collection date to delivery date by adding 1 day
                    deliveryDate = deliveryDate.AddDays(1);

                    // Only read orders for deliveries from tomorrow to 7 days ahead. Ignore all other orders
                    if (deliveryDate <= DateTime.Today || deliveryDate > DateTime.Today.AddDays(7)) continue;

                    if (deliveryDate != DateTime.Today.AddDays(1))
                    {
                        var errorMessage = $"Delivery date ({deliveryDate.ToShortDateString()}) on sheet {table.Key} is not tomorrow.";

                        // Create a new warning object for this order
                        depotWarnings.Add(new OrderWarning(OrderWarning.WarningType.UnusualDate, errorMessage));
                    }

                    // Display all the cells that could not be read from
                    if (readQtyErrors.Count > 0)
                    {
                        var errorMessage = $"Not all data could be read from file {fileName} on sheet {table.Key}.\n" +
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
                    
                    if (string.IsNullOrEmpty(orderReference))
                    {
                        var errorMessage = $"Delivery for ({deliveryDate.ToShortDateString()}) on sheet {table.Key} has no reference number.";

                        // Create a new warning object for this order
                        depotWarnings.Add(new OrderWarning(OrderWarning.WarningType.MissingReference, errorMessage));
                        
                        missingReferenceDates.Add(deliveryDate);
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
                            errorMessage += "\nOrder for this day was not processed.";

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
                        
                        order ??= new Order(orderReference ?? "N/A", deliveryDate, customer.Id, depot.Id, customer);
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

        if (missingReferenceDates.Count > 0)
        {
            var errorMessage = "Orders with the following dates were processed but have no reference numbers:\n";

            foreach (var missingRefDate in missingReferenceDates)
            {
                errorMessage += $"\n• {missingRefDate.ToShortDateString()}";
            }

            // Display error message to the user
            await _notificationService.ShowMessage("Missing References", errorMessage);
        }
    }
    
    #endregion
}