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

        /// <summary>
        /// Loads the customers from database into the Customers property
        /// </summary>
        public void LoadCustomers()
        {
            Customers = SqliteDataAccess.LoadCustomers();
        }

        /// <summary>
        /// Check if a customer with that name already exists
        /// </summary>
        /// <param name="name">Name as it appears in the UI</param>
        /// <returns>true or false</returns>
        public bool HasCustomerName(string name)
        {
            foreach (Customer customer in Customers)
            {
                if (customer.Name == name) return true;
            }

            return false;
        }

        #endregion
    }
}
