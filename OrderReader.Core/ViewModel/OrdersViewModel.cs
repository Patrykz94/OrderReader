using System;

namespace OrderReader.Core
{
    /// <summary>
    /// The View Model for orders panel
    /// </summary>
    public class OrdersViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// A <see cref="OrderListViewModel"/> that holds all customer orders
        /// </summary>
        public OrderListViewModel OrdersHandler { get; set; }

        /// <summary>
        /// Whether or not any orders exist
        /// </summary>
        public bool HasOrders => OrdersExist();

        #endregion

        #region Commands

        // Add commands here

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window"></param>
        public OrdersViewModel()
        {
            OrdersHandler = new OrderListViewModel();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Check if any orders exist
        /// </summary>
        /// <returns>True or false</returns>
        private bool OrdersExist()
        {
            return IoC.Get<OrdersLibrary>().Orders.Count > 0;
        }

        #endregion
    }
}
