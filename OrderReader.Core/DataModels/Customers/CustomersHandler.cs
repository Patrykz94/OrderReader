using System.Collections.ObjectModel;

namespace OrderReader.Core
{
    /// <summary>
    /// A class that manages the customers
    /// </summary>
    public class CustomersHandler
    {
        #region Public Properties
        
        /// <summary>
        /// A list of all known customers
        /// </summary>
        public ObservableCollection<Customer> Customers { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// A default constructor that loads a list of customers
        /// </summary>
        public CustomersHandler()
        {
            LoadCustomers();
        }

        #endregion

        #region Public Helpers

        public void LoadCustomers()
        {
            Customers = SqliteDataAccess.LoadCustomers();
        }

        #endregion
    }
}
