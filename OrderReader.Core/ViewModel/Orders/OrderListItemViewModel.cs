using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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

        /// <summary>
        /// The data view that will be displayed in the orders page
        /// </summary>
        public DataView OrdersView
        {
            get { return OrdersTable.DefaultView; }
        }

        /// <summary>
        /// The number of warnings
        /// </summary>
        public ObservableCollection<OrderWarning> WarningsList
        {
            get
            {
                ObservableCollection<OrderWarning> list = new ObservableCollection<OrderWarning>();
                foreach (Order order in Orders)
                {
                    foreach (OrderWarning warning in order.Warnings)
                    {
                        list.Add(warning);
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// Whether or not there are any warnings
        /// </summary>
        public bool HasWarnings => WarningsList.Count > 0;

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
            ProcessCommand = new RelayCommand(() => { ProcessOrder(); });
            DeleteCommand = new RelayCommand(() => { DeleteOrder(); });
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Check if that order exists
        /// </summary>
        /// <param name="orderIn">An <see cref="Order"/> object</param>
        /// <returns>True or false</returns>
        public bool HasOrder(Order orderIn)
        {
            foreach (Order order in Orders)
            {
                if (order == orderIn)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Reload all data in the <see cref="DataTable"/>
        /// </summary>
        public void ReloadTable()
        {
            // Get a clean new data table
            DataTable tempTable = new DataTable();

            // Add order details columns
            tempTable.Columns.Add("Depot", typeof(string));
            tempTable.Columns.Add("PO Number", typeof(string));

            // Add product columns
            // First make a list of all unique products in those orders
            SortedDictionary<string, int> uniqueProducts = new SortedDictionary<string,int>();
            foreach (Order order in Orders)
            {
                foreach (OrderProduct product in order.Products)
                {
                    if (!uniqueProducts.ContainsKey(product.ProductName))
                    {
                        uniqueProducts.Add(product.ProductName, product.ProductID);
                    }
                }
            }

            // Then create columns for each product and name them with the product names
            foreach (string name in uniqueProducts.Keys)
            {
                tempTable.Columns.Add(name, typeof(string));
            }

            // Add total column
            tempTable.Columns.Add("Total", typeof(string));

            // Add rows with data
            foreach (Order order in Orders)
            {
                DataRow row = tempTable.NewRow();

                row["Depot"] = order.DepotName;
                row["PO Number"] = order.OrderReference;

                foreach (string productName in uniqueProducts.Keys)
                {
                    row[productName] = order.GetQuantityOfProduct(uniqueProducts[productName]);
                }

                row["Total"] = order.GetTotalProductQuantity();

                tempTable.Rows.Add(row);
            }

            // Add the total row
            DataRow totalRow = tempTable.NewRow();

            totalRow["Depot"] = "";
            totalRow["PO Number"] = "Total";

            foreach (string productName in uniqueProducts.Keys)
            {
                double total = 0.0;
                foreach (Order order in Orders)
                {
                    total += order.GetQuantityOfProduct(uniqueProducts[productName]);
                }
                totalRow[productName] = total;
            }

            double totalProducts = 0.0;

            foreach (Order order in Orders)
            {
                totalProducts += order.GetTotalProductQuantity();
            }

            totalRow["Total"] = totalProducts;

            tempTable.Rows.Add(totalRow);

            OrdersTable = tempTable;
            OnPropertyChanged(nameof(OrdersView));
            OnPropertyChanged(nameof(WarningsList));
        }

        /// <summary>
        /// Sorts the orders alphabetically by the depot name
        /// </summary>
        public void SortOrders()
        {
            Orders = new ObservableCollection<Order>(Orders.OrderBy(o => o.DepotName));
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Process current order using settings specified by the user
        /// </summary>
        private async void ProcessOrder()
        {
            // Load the user settings
            UserSettings settings = Settings.LoadSettings();

            try
            {

                // Perform all the order processing tasks based on users settings
                //if (settings.ExportCSV) await CSVExport.ExportOrdersToCSV(OrderID);
                
                
                if (settings.PrintOrders || settings.ExportPDF) PDFExport.ExportOrderToPDF(OrdersTable, new Customer(), OrderID, Date);

                // Once processed, remove the order without requiring confirmation
                DeleteOrder(false);
            }
            catch (Exception ex)
            {
                // If an error occurs, we want to show the error message
                // await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                // {
                //     Title = "Processing Error",
                //     Message = $"This order could not processed due to the following error:\n\n{ex.Message}",
                //     ButtonText = "OK"
                // });
            }
        }

        /// <summary>
        /// Delete the current order by removing it from orders library and then reloading the page
        /// </summary>
        private async void DeleteOrder(bool promptUser = true)
        {
            /// Prompt user to confirm whether this order should be removed.
            if (promptUser)
            {
                // var result = await IoC.UI.ShowMessage(new YesNoBoxDialogViewModel
                // {
                //     Title = "Confirm Deleting Order",
                //     Question = "Are you sure you want to delete this order?"
                // });
                //
                // if (result == DialogResult.No) return;
            }
            // Need to create a confirmation message box with multiple possible answers
            if (OrdersList.Contains(this)) OrdersList.Remove(this);
            // Remove the orders first
            // IoC.Orders().RemoveAllOrdersWithID(OrderID);
        }

        #endregion
    }
}
