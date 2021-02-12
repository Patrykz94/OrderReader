using System;

namespace OrderReader.Core
{
    /// <summary>
    /// The View Model for orders panel
    /// </summary>
    public class OrdersViewModel : BaseViewModel, IFilesDropped
    {
        #region Public Properties

        /// <summary>
        /// A <see cref="OrderListViewModel"/> that holds all customer orders
        /// </summary>
        public OrderListViewModel OrdersHandler { get; set; }

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

        #region Public Helpers

        public void OnFilesDropped(string[] files)
        {
            // TODO: Implement the handling logic here
            throw new NotImplementedException();
        }

        #endregion
    }
}
