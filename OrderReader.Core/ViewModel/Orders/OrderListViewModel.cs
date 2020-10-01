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
                    Orders.Add(new OrderListItemViewModel(orderId, IoC.Get<OrdersLibrary>().GetAllOrdersWithID(orderId)));
                }
            }
        }
    }
}
