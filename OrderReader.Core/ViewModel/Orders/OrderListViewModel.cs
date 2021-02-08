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

            List<string> OrderIDs = IoC.Get<OrdersLibrary>().GetUniqueOrderIDs();

            foreach (string orderId in OrderIDs)
            {
                if (IoC.Get<OrdersLibrary>().HasOrdersWithID(orderId))
                {
                    Orders.Add(new OrderListItemViewModel(Orders, orderId, IoC.Get<OrdersLibrary>().GetAllOrdersWithID(orderId)));
                }
            }
        }

        #endregion

        #region Public Helpers

        public void UpdateAllOrders()
        {
            //List<string> OrderIDs = IoC.Get<OrdersLibrary>().GetUniqueOrderIDs();

            for (int i = Orders.Count-1; i >= 0; i--)
            {
                var OrderVM = Orders[i];
                if (IoC.Get<OrdersLibrary>().HasOrdersWithID(OrderVM.OrderID))
                {
                    for (int c = OrderVM.Orders.Count-1; c >= 0; c--)
                    {
                        Order o = OrderVM.Orders[c];
                        if (!IoC.Get<OrdersLibrary>().HasOrder(o))
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
        }

        #endregion
    }
}
