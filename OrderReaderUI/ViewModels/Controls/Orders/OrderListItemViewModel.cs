using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using OrderReader.Core;
using OrderReader.Core.Interfaces;

namespace OrderReaderUI.ViewModels.Controls.Orders;

public class OrderListItemViewModel : Screen
{

    #region Private Variables
    
    private readonly OrdersLibrary _ordersLibrary;
    private readonly INotificationService _notificationService;

    private DataTable _ordersTable = new ();

    #endregion
    
    #region Public Properties

    public string OrderId { get; }
    
    public string Customer { get; }
    
    public DateTime Date { get; }

    public ObservableCollection<Order> Orders { get; private set; } = [];

    public DataView OrdersView => _ordersTable.DefaultView;

    #endregion

    #region Constructors

    public OrderListItemViewModel(string orderId, OrdersLibrary ordersLibrary, INotificationService notificationService)
    {
        OrderId = orderId;
        _ordersLibrary = ordersLibrary;
        _notificationService = notificationService;
        var order = _ordersLibrary.GetAllOrdersWithID(OrderId)[0];
        Date = order.Date;
        Customer = order.CustomerName;

        UpdateOrders();
    }

    #endregion

    #region Public Methods
    
    public void UpdateOrders()
    {
        // Get the orders with an ID that we want
        Orders = _ordersLibrary.GetAllOrdersWithID(OrderId);
        
        // TODO - if there are no orders with this Id, this view model should not exist, therefore we need to destroy it
        
        // Update the data table with the new data
        DataTable tempTable = new();

        // Add required basic columns
        tempTable.Columns.Add("Depot", typeof(string));
        tempTable.Columns.Add("PO Number", typeof(string));
        
        // Get a list of all unique products
        var uniqueProducts = Orders.SelectMany(x => x.Products).DistinctBy(x => x.ProductID).OrderBy(x => x.ProductName).ToList();
        
        // Add a column for each product name
        foreach (var uniqueProduct in uniqueProducts)
        {
            // Add a new column with a default value of 0
            var newColumn = new DataColumn(uniqueProduct.ProductName, typeof(double))
            {
                DefaultValue = 0.0
            };
            tempTable.Columns.Add(newColumn);
        }
        
        // Add the totals column at the end
        tempTable.Columns.Add("Total", typeof(double));
        
        // Add records to the table
        foreach (var order in Orders)
        {
            var newRow = tempTable.NewRow();
            newRow["Depot"] = order.DepotName;
            newRow["PO Number"] = order.OrderReference;

            var totalQuantity = 0.0;
            foreach (var product in order.Products)
            {
                newRow[product.ProductName] = product.Quantity;
                totalQuantity += product.Quantity;
            }

            newRow["Total"] = totalQuantity;
            
            tempTable.Rows.Add(newRow);
        }
        
        // Add total row
        var totalRow = tempTable.NewRow();
        totalRow["Depot"] = string.Empty;
        totalRow["PO Number"] = "Totals";

        for (var i = 2; i < tempTable.Columns.Count; i++)
        {
            totalRow[i] = tempTable.Rows.Cast<DataRow>().Sum(row => (double)row[i]);
        }
        
        tempTable.Rows.Add(totalRow);

        _ordersTable = tempTable;
        NotifyOfPropertyChange(() => OrdersView);
    }

    public async Task ProcessOrder()
    {
        // Make sure there are orders to process
        if (Orders.Count == 0) return;
        
        // Load the user settings
        var settings = Settings.LoadSettings();
        
        try
        {
            var customer = Orders[0].Customer;

            // Perform all the order processing tasks based on users settings
            if (settings.ExportCSV) await CSVExport.ExportOrdersToCSV(Orders, customer);
                
                
            if (settings.PrintOrders || settings.ExportPDF) PDFExport.ExportOrderToPDF(_ordersTable, customer, OrderId, Date);

            // Once processed, remove the order
            Delete();
        }
        catch (Exception ex)
        {
            // If an error occurs, we want to show the error message
            await _notificationService.ShowMessage("Processing Error", $"This order could not processed due to the following error:\n\n{ex.Message}");
        }
    }
    
    public async Task DeleteOrder()
    {
        // Prompt user to confirm whether this order should be removed.
        var result = await _notificationService.ShowQuestion("Confirm Deleting Order", "Are you sure you want to delete this order?");
        
        if (result == DialogResult.No) return;
        
        Delete();
    }

    #endregion

    #region Private Methods

    public void Delete()
    {
        // Remove the orders from orders library first
        _ordersLibrary.RemoveAllOrdersWithID(OrderId);
        
        // Then remove this view model from the list
        if (Parent is OrderListViewModel parentViewModel)
        {
            parentViewModel.RemoveItem(this);
        }
    }

    #endregion
}