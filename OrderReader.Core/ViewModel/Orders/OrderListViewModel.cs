using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OrderReader.Core
{
    /// <summary>
    /// A view model responsible for holding a list of customer orders
    /// </summary>
    public class OrderListViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// A list of all <see cref="OrderListItemViewModel"/>
        /// </summary>
        public ObservableCollection<OrderListItemViewModel> Orders { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrderListViewModel()
        {
            // Load the orders from OrdersLibrary if any exist
            Orders = new ObservableCollection<OrderListItemViewModel>();

            UpdateAllOrders();
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Checks if orders with this order ID exist
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <returns>Ture or false</returns>
        public bool HasOrdersWithID(string orderId)
        {
            foreach (var order in Orders)
            {
                if (order.OrderID == orderId)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Remove all deleted orders from the list and add all new orders to the list
        /// </summary>
        public void UpdateAllOrders()
        {
            // Remove all deleted orders first
            for (int i = Orders.Count-1; i >= 0; i--)
            {
                var OrderVM = Orders[i];
                if (IoC.Orders().HasOrdersWithID(OrderVM.OrderID))
                {
                    for (int c = OrderVM.Orders.Count-1; c >= 0; c--)
                    {
                        Order o = OrderVM.Orders[c];
                        if (!IoC.Orders().HasOrder(o))
                        {
                            OrderVM.Orders.Remove(o);
                            OrderVM.ReloadTable();
                        }
                    }
                }
                else
                {
                    Orders.Remove(OrderVM);
                }
            }

            // Add all new orders
            List<string> OrderIDs = IoC.Orders().GetUniqueOrderIDs();

            foreach (string orderId in OrderIDs)
            {
                if (!HasOrdersWithID(orderId))
                {
                    Orders.Add(new OrderListItemViewModel(Orders, orderId, IoC.Orders().GetAllOrdersWithID(orderId)));
                }
                else
                {
                    foreach (var OrderVM in Orders)
                    {
                        if (OrderVM.OrderID == orderId)
                        {
                            foreach (Order o in IoC.Orders().GetAllOrdersWithID(orderId))
                            {
                                if (!OrderVM.HasOrder(o))
                                {
                                    OrderVM.Orders.Add(o);
                                    OrderVM.ReloadTable();
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
