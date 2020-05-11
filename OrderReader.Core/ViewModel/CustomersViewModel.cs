using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace OrderReader.Core
{
    /// <summary>
    /// The View Model for customer settings
    /// </summary>
    public class CustomersViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// An instance of the <see cref="CustomersHandler"/> class storing all our customers
        /// </summary>
        public CustomersHandler Customers { get; set; }

        /// <summary>
        /// Currently selected customer ID
        /// </summary>
        public int SelectedCustomerID { get; set; } = 0;

        /// <summary>
        /// Currently selected depot ID
        /// </summary>
        public int SelectedDepotID { get; set; } = 0;

        /// <summary>
        /// Currently selected product ID
        /// </summary>
        public int SelectedProductID { get; set; } = 0;

        /// <summary>
        /// The new depot name that user has entered
        /// </summary>
        public string NewDepotName { get; set; } = "";

        /// <summary>
        /// The new depot CSV name that user has entered
        /// </summary>
        public string NewDepotCSVName { get; set; } = "";

        /// <summary>
        /// The new depot order name that user has entered
        /// </summary>
        public string NewDepotOrderName { get; set; } = "";

        /// <summary>
        /// The new product name that user has entered
        /// </summary>
        public string NewProductName { get; set; } = "";

        /// <summary>
        /// The new product CSV name that user has entered
        /// </summary>
        public string NewProductCSVName { get; set; } = "";

        /// <summary>
        /// The new product order name that user has entered
        /// </summary>
        public string NewProductOrderName { get; set; } = "";

        #endregion

        #region Commands

        /// <summary>
        /// Commad that discards all changed data by reloading the form
        /// </summary>
        public ICommand DiscardCommand { get; set; }

        /// <summary>
        /// Command that saves all changed data to a file
        /// </summary>
        public ICommand SaveCommand { get; set; }
        
        /// <summary>
        /// Command that removes the selected depot
        /// </summary>
        public ICommand RemoveDepotCommand { get; set; }

        /// <summary>
        /// Command that removes the selected depot
        /// </summary>
        public ICommand RemoveProductCommand { get; set; }

        /// <summary>
        /// Command that adds a new depot to the selected customer
        /// </summary>
        public ICommand AddNewDepotCommand { get; set; }

        /// <summary>
        /// Command that adds a new product to the selected customer
        /// </summary>
        public ICommand AddNewProductCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window"></param>
        public CustomersViewModel()
        {
            Customers = new CustomersHandler();

            DiscardCommand = new RelayCommand(() => {
                Customers.LoadCustomers();
                OnPropertyChanged(nameof(Customers));
            });
            SaveCommand = new RelayCommand(() => Customers.SaveCustomers());
            RemoveDepotCommand = new RelayCommand(() => RemoveDepot(SelectedCustomerID, SelectedDepotID));
            RemoveProductCommand = new RelayCommand(() => RemoveProduct(SelectedCustomerID, SelectedProductID));
            AddNewDepotCommand = new RelayCommand(() => AddNewDepot(NewDepotName, NewDepotCSVName, NewDepotOrderName, SelectedCustomerID));
            AddNewProductCommand = new RelayCommand(() => AddNewProduct(NewProductName, NewProductCSVName, NewProductOrderName, SelectedCustomerID));
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// A function that removes a depot from the list of depots for specified customer
        /// </summary>
        /// <param name="customerID">The index of the currently selected customer</param>
        /// <param name="depotID">The index of the currently selected depot</param>
        private void RemoveDepot(int customerID, int depotID)
        {
            Customer customer = Customers.Customers[customerID];
            Depot depot = customer.Depots[depotID];

            customer.DeleteDepot(depot.Name);
        }

        /// <summary>
        /// A function that removes a product form the list of products for specified customer
        /// </summary>
        /// <param name="customerID">The index of the currently selected customer</param>
        /// <param name="productID">The index of the currently selected product</param>
        private void RemoveProduct(int customerID, int productID)
        {
            Customer customer = Customers.Customers[customerID];
            Product product = customer.Products[productID];

            customer.DeleteProduct(product.Name);
        }

        /// <summary>
        /// A function that adds a new <see cref="Depot"/> to the list of depots for specified customer
        /// </summary>
        /// <param name="name">Name of the depot</param>
        /// <param name="csvName">Name of the depot as appears on CSV files</param>
        /// <param name="orderName">Name of the depot as appears on orders</param>
        /// <param name="customerID">The index of the currently selected <see cref="Customer"/></param>
        private void AddNewDepot(string name, string csvName, string orderName, int customerID)
        {
            Customer customer = Customers.Customers[customerID];

            if (name == "" || csvName == "" || orderName == "") return;

            // TODO: Show an error message and maybe highlight the border around the field

            if (!customer.HasDepot(name)) customer.AddDepot(name, csvName, orderName);
        }

        /// <summary>
        /// A function that adds a new <see cref="Product"/> to the list of products for specified customer
        /// </summary>
        /// <param name="name">Name of the product</param>
        /// <param name="csvName">Name of the product as appears on CSV files</param>
        /// <param name="orderName">Name of the product as appears on orders</param>
        /// <param name="customerID">The index of the currently selected <see cref="Customer"/></param>
        private void AddNewProduct(string name, string csvName, string orderName, int customerID)
        {
            Customer customer = Customers.Customers[customerID];

            if (name == "" || csvName == "" || orderName == "") return;

            // TODO: Show an error message and maybe highlight the border around the field

            if (!customer.HasProduct(name)) customer.AddProduct(name, csvName, orderName);
        }

        #endregion
    }
}
