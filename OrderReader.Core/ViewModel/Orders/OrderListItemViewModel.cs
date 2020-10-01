using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace OrderReader.Core
{
    /// <summary>
    /// A view model that represents a list of orders from a single customer for the same date
    /// </summary>
    public class OrderListItemViewModel : BaseViewModel
    {
        #region Public Properties

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

        #region Constructor

        /// <summary>
        /// A default constructor
        /// </summary>
        /// <param name="orderId">The ID of those orders</param>
        /// <param name="orders">A an <see cref="ObservableCollection{T}"/> of <see cref="Order"/> objects</param>
        public OrderListItemViewModel(string orderId, ObservableCollection<Order> orders)
        {
            OrderID = orderId;
            Orders = orders;
            if (Orders.Count > 0)
            {
                CustomerName = Orders[0].CustomerName;
                Date = Orders[0].Date;
            }

            OrdersTable = new DataTable();
            ReloadTable();
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
    }
}
