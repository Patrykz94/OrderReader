using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderReader.Core.DataModels.Customers;
using OrderReader.Core.DataModels.FileHandling.ExcelHelpers;
using OrderReader.Core.DataModels.Orders;
using OrderReader.Core.Enums;
using OrderReader.Core.Interfaces;

namespace OrderReader.Core.DataModels.FileHandling.OrderParsers;

public class MarketExcelParser(INotificationService notificationService) : IParseOrder
{
    private struct CollectedDataToProcess
    {
        public string Date { get; set; }
        public string Customer { get; set; }
        public string? Depot { get; set; }
        public Dictionary<string, Tuple<double, double>> Products { get; set; }
        public List<OrderWarning> Warnings { get; set; }
        public List<string> Errors { get; set; }

        public double GetTotalProductQty(string productName)
        {
            Products.TryGetValue(productName, out var quantity);
            return quantity?.Item1 ?? 0;
        }
    }
    
    private List<CollectedDataToProcess> _collectedData = [];
    private Dictionary<int, string> _productNames = [];
    private Dictionary<string, double> _totalQuantities = [];
    private Dictionary<string, double> _totalUnusedQuantities = [];

    private HashSet<string> _usedProductNames = [];

    // Variables that will be required
    private List<string> _criticalErrors = [];

    // Data locations
    private const int DateCol = 2;
    private const int CustomerCol = 3;
    private const int DepotCol = 4;

    private const int ProductColStart = 6;
    private const int OrderRowStart = 6;
    
    public string FileExtension => ".xlsx";
    
    public CustomerProfile? GetCustomerProfile(Dictionary<string, string[]> orderText, CustomersHandler customers)
    {
        foreach (var sheetText in orderText)
        {
            var sheet = new ExcelSheet(sheetText.Key, sheetText.Value);
            
            var customerIdCell = sheet.GetCell("A2");

            if (!string.IsNullOrEmpty(customerIdCell) && customers.HasCustomerProfileIdentifier(customerIdCell))
            {
                return customers.GetCustomerProfile(customerIdCell);
            }
        }

        return null;
    }

    public async Task ParseOrderAsync(Dictionary<string, string[]> orderText, string fileName, CustomerProfile customerProfile, OrdersLibrary ordersLibrary)
    {
        foreach (var table in orderText)
        {
            // Load the sheet
            var sheet = new ExcelSheet(table.Key, table.Value);
            
            // Get a list of all products and the columns they are in
            _productNames = GetProductInfo(sheet);
            _totalQuantities = GetTotalQuantities(sheet);

            foreach (var product in _usedProductNames)
            {
                if (!customerProfile.HasProductOrderName(product))
                {
                    _criticalErrors.Add($"Unknown Product '{product}' is used in the file. If this is a real product, please add it to the list of products in the customer profile and try again.\n");
                }
            }
            
            // Make sure the products are not duplicated anywhere
            HashSet<string> uniqueProductNames = [];
            foreach (var product in _productNames)
            {
                if (uniqueProductNames.Add(product.Value)) continue;
                var columnLetter = Cell.GetColumnLetter(product.Key);
                _criticalErrors.Add($"Product '{product.Value}' found in column '{columnLetter}' is duplicated. Please remove the duplicate and try again.");
            }
            
            // Show an error message if any critical errors were found
            if (_criticalErrors.Count > 0)
            {
                var errorString = $"The following errors were found in file '{fileName}':\n\n";
                errorString = _criticalErrors.Aggregate(errorString, (current, error) => current + $"{error}\n");
                
                errorString += "\nThis file could not be read and will not be processed.";
                await notificationService.ShowMessage("Errors Found", errorString, "Cancel");
                return;
            }
            
            // Iterate over all rows in the sheet, starting from the 6th row
            for (var r = OrderRowStart; r <= sheet.RowCount; r++)
            {
                var dateString = sheet.GetCell(DateCol, r);
                var customerString = sheet.GetCell(CustomerCol, r);
                var depotString = sheet.GetCell(DepotCol, r);
                
                var productsFound = new Dictionary<string, Tuple<double, double>>();
                
                List<string> recordErrors = [];
                
                // Iterate over all product columns to get products that have quantities and prices
                for (var i = ProductColStart; i <= sheet.ColumnCount; i += 2)
                {
                    var qtyString = sheet.GetCell(i, r);
                    var priceString = sheet.GetCell(i + 1, r);
                    
                    var qtyValid = double.TryParse(qtyString, out var quantity);
                    var priceValid = double.TryParse(priceString, out var price);
                    
                    // If a quantity string was not null or empty, it means something was entered but not a number
                    // if the quantity was simply missing, that's fine, we'll just ignore it
                    if (!qtyValid && !string.IsNullOrWhiteSpace(qtyString))
                        recordErrors.Add($"Quantity in cell {Cell.GetCellReference(i, r)} is not a number: {qtyString}.");
                    
                    // If a price string was not null or empty, it means something was entered but not a number
                    // if the price was simply missing, that's fine, we'll just ignore it
                    if (!priceValid && !string.IsNullOrWhiteSpace(priceString))
                        recordErrors.Add($"Price in cell {Cell.GetCellReference(i + 1, r)} is not a number: {priceString}.");
                    
                    if (!_productNames.TryGetValue(i, out var productName)) continue;
                    
                    if (quantity == 0)
                    {
                        if (price > 0)
                        {
                            var cellRef = Cell.GetCellReference(i, r);
                            recordErrors.Add($"Product {productName} in cell {cellRef} has a price but no quantity.");
                        }
                        continue;
                    }
                    
                    productsFound.Add(productName, Tuple.Create(quantity, price));
                }
                
                // If we have some products ordered, make sure the customer and date info exists
                if (productsFound.Count > 0)
                {
                    List<string> missingFields = [];
                    
                    if (string.IsNullOrWhiteSpace(customerString)) missingFields.Add("Customer Name");
                    if (string.IsNullOrWhiteSpace(dateString)) missingFields.Add("Required Date");

                    var allDataValid = true;
                    
                    if (missingFields.Count > 0)
                    {
                        recordErrors.Add($"Missing Information: {string.Join(", ", missingFields)} fields are missing in row {r}.");
                        allDataValid = false;
                    }
                    
                    // Make sure the date is in the correct format
                    if (!DateTime.TryParse(dateString, out _))
                    {
                        recordErrors.Add($"Date in cell {Cell.GetCellReference(DateCol, r)} is not in the correct format.");
                        allDataValid = false;
                    }

                    if (customerProfile.HasCustomerOrderName(customerString!))
                    {
                        if (!string.IsNullOrWhiteSpace(depotString) &&
                            !(customerProfile.GetCustomer(customerString!)?.HasDepotOrderName(depotString) ?? false))
                        {
                            recordErrors.Add($"Unknown Depot '{depotString}' for customer '{customerString}' in cell {Cell.GetCellReference(DepotCol, r)}. If this is a new depot, please add it to the customer and try again.");
                            
                            allDataValid = false;
                        }
                    }
                    else
                    {
                        recordErrors.Add($"Unknown Customer '{customerString}' in cell {Cell.GetCellReference(CustomerCol, r)}. If this is a new customer, please add them to the list of customers and try again.");
                        allDataValid = false;
                    }

                    if (!allDataValid)
                    {
                        var errorMessage = $"We could not parse order in row {r}. Following errors were found:\n\n";
                        foreach (var error in recordErrors)
                        {
                            errorMessage += $"* {error}\n";
                        }
                        errorMessage += "\n This particular line will not be processed. Would you like to continue without it?";
                        var result = await notificationService.ShowQuestion("Error Parsing Order", errorMessage, "Continue", "Cancel");
                        
                        if (result == DialogResult.Yes)
                        {
                            // If we are continuing without this order, remove the product quantities found
                            foreach (var product in productsFound)
                            {
                                if (!_totalUnusedQuantities.TryAdd(product.Key, product.Value.Item1))
                                {
                                    _totalUnusedQuantities[product.Key] += product.Value.Item1;
                                }
                            }
                            continue;
                        }
                        
                        return;
                    }
                    
                    _collectedData.Add(new CollectedDataToProcess
                    {
                        Date = dateString ?? string.Empty,
                        Customer = customerString ?? string.Empty,
                        Depot = depotString,
                        Products = productsFound,
                        Errors = recordErrors,
                        Warnings = []
                    });
                }
            }
            
            // Once all orders were read, make sure that the sum of product quantities is equal to the total quantity row
            foreach (var product in _productNames)
            {
                _totalQuantities.TryGetValue(product.Value, out var totalQuantity);
                _totalUnusedQuantities.TryGetValue(product.Value, out var unusedQuantity);
                var totalReadQuantity = _collectedData.Sum(x => x.GetTotalProductQty(product.Value));

                if (Math.Abs(totalQuantity - unusedQuantity - totalReadQuantity) > 0.001)
                {
                    var columnName = Cell.GetColumnLetter(product.Key);
                    _criticalErrors.Add($"Total product quantity of '{product.Value}' in column '{columnName}' is not equal to sum of all quantities in file '{fileName}'.");
                }
            }
            
            // Create all orders that have been validated
            List<Order> orders = [];
            foreach (var record in _collectedData)
            {
                var dateValid = DateTime.TryParse(record.Date, out var date);
                var customer = customerProfile.GetCustomer(record.Customer);
                var depot = customer?.GetDepot(record.Depot ?? string.Empty);

                if (customer is null || !dateValid)
                {
                    var errorMessage = "Failed to process the following order:\n\n" +
                                       $"Date: {record.Date}\n" +
                                       $"Customer: {record.Customer}\n" +
                                       $"Depot: {record.Depot}\n" +
                                       "\nPlease contact Patryk Z and provide the order file.";
                    var result = await notificationService.ShowQuestion("Unknown Processing Error", errorMessage, "Continue", "Cancel");
                    if (result == DialogResult.Yes) continue;
                    return;
                }
                
                if (date.Date < DateTime.Today)
                {
                    record.Warnings.Add(new OrderWarning(OrderWarning.WarningType.UnusualDate, $"Date is in the past: {date.ToShortDateString()}"));
                    var errorMessage = "The date for the following order is in the past:\n\n" +
                                       $"Date: {record.Date}\n" +
                                       $"Customer: {record.Customer}\n" +
                                       $"Depot: {record.Depot}\n" +
                                       "\nWould you like to process it anyways?";
                    var result = await notificationService.ShowQuestion("Date Warning", errorMessage);
                    if (result == DialogResult.No) continue;
                }
                
                var order = new Order(string.Empty, date, customer.Id, depot?.Id ?? -1, customerProfile, customer);

                foreach (var productRecord in record.Products)
                {
                    var product = customerProfile.GetProduct(productRecord.Key);
                    if (product is null)
                    {
                        var errorMessage = "Failed to process the following order:\n\n" +
                                           $"Date: {record.Date}\n" +
                                           $"Customer: {record.Customer}\n" +
                                           $"Depot: {record.Depot}\n" +
                                           "\nPlease contact Patryk Z and provide the order file.";
                        var result = await notificationService.ShowQuestion("Unknown Processing Error", errorMessage, "Continue", "Cancel");
                        if (result == DialogResult.Yes) continue;
                        return;
                    }
                    
                    order.AddProduct(product.Id, productRecord.Value.Item1, productRecord.Value.Item2);
                }

                foreach (var warning in record.Warnings)
                {
                    order.AddWarning(warning);
                }
                
                orders.Add(order);
            }
            
            foreach (var order in orders) ordersLibrary.AddOrder(order);

            // Only process the first sheet
            break;
        }
    }

    private Dictionary<int, string> GetProductInfo(ExcelSheet sheet)
    {
        Dictionary<int, string> products = [];

        var lastColWithProductType = -1;
        
        for (var i = ProductColStart; i <= sheet.ColumnCount; i += 2)
        {
            var productType = sheet.GetCell(i, 2);
            var productDetail = sheet.GetCell(i, 3);

            if (productType is null)
            {
                if (lastColWithProductType == -1) continue;

                if (lastColWithProductType >= i - 6)
                    productType = sheet.GetCell(lastColWithProductType, 2);
            }
            else lastColWithProductType = i;
            
            if (productType is not null && productDetail is not null)
            {
                products.Add(i, $"{productType} {productDetail}");
            }
        }

        return products;
    }

    private Dictionary<string, double> GetTotalQuantities(ExcelSheet sheet)
    {
        Dictionary<string, double> totalQty = [];
        
        // Row with total quantities
        const int row = 1;
        
        // Iterate over all product columns to get the total quantities for each product
        for (var i = ProductColStart; i <= sheet.ColumnCount; i += 2)
        {
            var qtyString = sheet.GetCell(i, row);
            var qtyValid = double.TryParse(qtyString, out var quantity);

            // If a quantity string was not null or empty, it means something was entered but not a number
            // if the quantity was simply missing, that's fine, we'll just ignore it
            if (!qtyValid && !string.IsNullOrWhiteSpace(qtyString))
                _criticalErrors.Add($"Quantity in cell {Cell.GetCellReference(i, row)} is not a number: {qtyString}.");
            
            var hasProductName = _productNames.TryGetValue(i, out var productName);
            if (!hasProductName)
            {
                if (quantity > 0)
                {
                    var columnLetter = Cell.GetColumnLetter(i);
                    _criticalErrors.Add($"Product in column {columnLetter} is used but doesn't have a valid name.");
                }
                continue;
            }

            if (!_usedProductNames.Add(productName!))
            {
                _criticalErrors.Add($"Product '{productName}' is used more than once in the file.");
            }
            else
            {
                totalQty.Add(productName!, quantity);
            }
        }
        
        return totalQty;
    }
}