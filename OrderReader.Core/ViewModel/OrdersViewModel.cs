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

        /// <summary>
        /// When a file is dropped, process the file
        /// </summary>
        /// <param name="files"></param>
        public async void OnFilesDropped(string[] files)
        {
            foreach (string file in files)
            {
                await FileImport.ProcessFileAsync(file);
            }

            // Once files have been processed, refresh the orders page
            OrdersHandler.UpdateAllOrders();
        }

        #endregion
    }
}
