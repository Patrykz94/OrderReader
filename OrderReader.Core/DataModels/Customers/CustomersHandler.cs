using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Forms.VisualStyles;

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
        /// Default constructor that loads the customers from database
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
        /// Checks if a customer with that ID exists
        /// </summary>
        /// <param name="id">ID number of the customer</param>
        /// <returns>True or false</returns>
        public bool HasCustomer(int id)
        {
            foreach (Customer customer in Customers)
            {
                if (customer.Id == id) return true;
            }

            return false;
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

        /// <summary>
        /// Gets the customer object by ID number
        /// </summary>
        /// <param name="customerId">The ID number of required customer</param>
        /// <returns><see cref="Customer"/> object or Null</returns>
        public Customer GetCustomerByID(int customerId)
        {
            foreach (Customer customer in Customers)
            {
                if (customer.Id == customerId) return customer;
            }

            return null;
        }

        #endregion
    }
}
