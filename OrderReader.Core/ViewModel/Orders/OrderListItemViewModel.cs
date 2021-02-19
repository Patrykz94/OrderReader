using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

namespace OrderReader.Core
{
    /// <summary>
    /// A view model that represents a list of orders from a single customer for the same date
    /// </summary>
    public class OrderListItemViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// Reference to the list containing this item view model
        /// </summary>
        public ObservableCollection<OrderListItemViewModel> OrdersList { get; private set; }

        /// <summary>
        /// The ID of this order
        /// This is mainly used to group orders together by customer and date
        /// </summary>
        public string OrderID { get; private set; }

        /// <summary>
        /// A list of orders with the same order ID
        /// </summary>
        public ObservableCollection<Order> Orders { get; private set; }

        /// <summary>
        /// Gets the name of the Customer
        /// </summary>
        public string CustomerName { get; private set; }

        /// <summary>
        /// A delivery date for this order
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// A data table that will be used for displaying DataGrid
        /// </summary>
        public DataTable OrdersTable { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// Command that processes this order
        /// </summary>
        public ICommand ProcessCommand { get; set; }

        /// <summary>
        /// Commad that removes this order from the list of orders
        /// </summary>
        public ICommand DeleteCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// A default constructor
        /// </summary>
        /// <param name="orderId">The ID of those orders</param>
        /// <param name="orders">A an <see cref="ObservableCollection{T}"/> of <see cref="Order"/> objects</param>
        public OrderListItemViewModel(ObservableCollection<OrderListItemViewModel> orderList, string orderId, ObservableCollection<Order> orders)
        {
            OrdersList = orderList;
            OrderID = orderId;
            Orders = orders;
            if (Orders.Count > 0)
            {
                CustomerName = Orders[0].CustomerName;
                Date = Orders[0].Date;
            }

            OrdersTable = new DataTable();
            ReloadTable();

            // Command definitions
            ProcessCommand = new RelayCommand(ProcessOrder);
            DeleteCommand = new RelayCommand(DeleteOrder);
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Reload all data in the <see cref="DataTable"/>
        /// </summary>
        public void ReloadTable()
        {
            // Clear all data first
            OrdersTable.Clear();
            OrdersTable.Columns.Clear();

            // Add order details columns
            OrdersTable.Columns.Add("Depot", typeof(string));
            OrdersTable.Columns.Add("PO Number", typeof(string));

            // Add product columns
            // First make a list of all unique product ID's in those orders
            Dictionary<int, string> uniqueProducts = new Dictionary<int, string>();
            foreach (Order order in Orders)
            {
                foreach (OrderProduct product in order.Products)
                {
                    if (!uniqueProducts.ContainsKey(product.ProductID))
                    {
                        uniqueProducts.Add(product.ProductID, product.ProductName);
                    }
                }
            }

            // Then create columns for each of the product IDs and name them with the product names
            foreach (int id in uniqueProducts.Keys)
            {
                OrdersTable.Columns.Add(uniqueProducts[id], typeof(string));
            }

            // Add total column
            OrdersTable.Columns.Add("Total", typeof(string));

            // Add rows with data
            foreach (Order order in Orders)
            {
                DataRow row = OrdersTable.NewRow();

                row["Depot"] = order.DepotName;
                row["PO Number"] = order.OrderReference;

                foreach (int productID in uniqueProducts.Keys)
                {
                    row[uniqueProducts[productID]] = order.GetQuantityOfProduct(productID);
                }

                row["Total"] = order.GetTotalProductQuantity();

                OrdersTable.Rows.Add(row);
            }

            // Add the total row
            DataRow totalRow = OrdersTable.NewRow();

            totalRow["Depot"] = "";
            totalRow["PO Number"] = "Total";

            foreach (int productID in uniqueProducts.Keys)
            {
                double total = 0.0;
                foreach (Order order in Orders)
                {
                    total += order.GetQuantityOfProduct(productID);
                }
                totalRow[uniqueProducts[productID]] = total;
            }

            double totalProducts = 0.0;

            foreach (Order order in Orders)
            {
                totalProducts += order.GetTotalProductQuantity();
            }

            totalRow["Total"] = totalProducts;

            OrdersTable.Rows.Add(totalRow);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Process current order using settings specified by the user
        /// </summary>
        private void ProcessOrder()
        {
            // Load the user settings
            UserSettings settings = Settings.LoadSettings();

            // Perform all the order processing tasks based on users settings
            if (settings.ExportCSV) CSVExport.ExportOrdersToCSV(OrderID);

            if (settings.PrintOrders || settings.ExportPDF) PDFExport.ExportOrderToPDF(this);

            // Once processed, remove the order
            DeleteOrder();
        }

        /// <summary>
        /// Delete the current order by removing it from orders library and then reloading the page
        /// </summary>
        private void DeleteOrder()
        {
            // TODO: ask user to confirm whether this order should be removed.
            // Need to create a confirmation message box with multiple possible answers
            if (OrdersList.Contains(this)) OrdersList.Remove(this);
            // Remove the orders first
            IoC.Get<OrdersLibrary>().RemoveAllOrdersWithID(OrderID);
        }

        #endregion
    }
}
