using System.Windows.Input;

namespace OrderReader.Core
{
    /// <summary>
    /// The View Model for customer settings page
    /// </summary>
    public class CustomersViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// An enum to help keep track of which index has changed and which fields need to be update on the view
        /// </summary>
        private enum IndexChangedFor
        {
            Customer = 0,
            Depot = 1,
            Product = 2
        }

        /// <summary>
        /// The index of currently selected customer
        /// </summary>
        private int _customerIndex = 0;

        /// <summary>
        /// The index of currently selected depot
        /// </summary>
        private int _depotIndex = -1;

        /// <summary>
        /// The index of currently selected product
        /// </summary>
        private int _productIndex = -1;

        #endregion
        
        #region Public Properties

        /// <summary>
        /// An instance of the <see cref="CustomersHandler"/> class storing all our customers
        /// </summary>
        public CustomersHandler Customers { get; set; }

        /// <summary>
        /// Currently selected customer ID
        /// </summary>
        public int SelectedCustomerIndex
        {
            get => _customerIndex;
            set
            {
                _customerIndex = value;
                UpdateFields(IndexChangedFor.Customer);
            }
        }

        /// <summary>
        /// Currently selected depot ID
        /// </summary>
        public int SelectedDepotIndex
        {
            get => _depotIndex;
            set
            {
                _depotIndex = value;
                UpdateFields(IndexChangedFor.Depot);
            }
        }

        /// <summary>
        /// Currently selected product ID
        /// </summary>
        public int SelectedProductIndex
        {
            get => _productIndex;
            set
            {
                _productIndex = value;
                UpdateFields(IndexChangedFor.Product);
            }
        }


        /// <summary>
        /// Name of currently selected customer
        /// </summary>
        public string SelectedCustomerName { get; set; }

        /// <summary>
        /// CSVName of currently selected customer
        /// </summary>
        public string SelectedCustomerCSVName { get; set; }

        /// <summary>
        /// OrderName of currently selected customer
        /// </summary>
        public string SelectedCustomerOrderName { get; set; }


        /// <summary>
        /// Name of currently selected depot
        /// </summary>
        public string SelectedDepotName { get; set; }

        /// <summary>
        /// CSVName of currently selected depot
        /// </summary>
        public string SelectedDepotCSVName { get; set; }

        /// <summary>
        /// OrderName of currently selected depot
        /// </summary>
        public string SelectedDepotOrderName { get; set; }


        /// <summary>
        /// Name of currently selected product
        /// </summary>
        public string SelectedProductName { get; set; }

        /// <summary>
        /// CSVName of currently selected product
        /// </summary>
        public string SelectedProductCSVName { get; set; }

        /// <summary>
        /// OrderName of currently selected product
        /// </summary>
        public string SelectedProductOrderName { get; set; }

        /// <summary>
        /// Price of currently selected product
        /// </summary>
        public string SelectedProductPrice { get; set; }


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

        /// <summary>
        /// The new product price that user has entered
        /// </summary>
        public string NewProductPrice { get; set; } = "";

        #endregion

        #region Commands

        /// <summary>
        /// Commad that discards all changed data by reloading the form
        /// </summary>
        public ICommand ReloadCommand { get; set; }

        /// <summary>
        /// Command updates the customer information
        /// </summary>
        public ICommand UpdateCustomerCommand { get; set; }

        /// <summary>
        /// Command that updates the depot information
        /// </summary>
        public ICommand UpdateDepotCommand { get; set; }

        /// <summary>
        /// Command that updates the product information
        /// </summary>
        public ICommand UpdateProductCommand { get; set; }

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
        public CustomersViewModel()
        {
            // Initialize the customers property
            Customers = new CustomersHandler();
            UpdateFields(IndexChangedFor.Customer);

            // Command definitions
            ReloadCommand = new RelayCommand(() => ReloadCustomers());
            UpdateCustomerCommand = new RelayCommand(() => UpdateCustomer(SelectedCustomerIndex));
            UpdateDepotCommand = new RelayCommand(() => UpdateDepot(SelectedCustomerIndex, SelectedDepotIndex));
            UpdateProductCommand = new RelayCommand(() => UpdateProduct(SelectedCustomerIndex, SelectedProductIndex));
            RemoveDepotCommand = new RelayCommand(() => RemoveDepot(SelectedCustomerIndex, SelectedDepotIndex));
            RemoveProductCommand = new RelayCommand(() => RemoveProduct(SelectedCustomerIndex, SelectedProductIndex));
            AddNewDepotCommand = new RelayCommand(() => AddNewDepot(SelectedCustomerIndex));
            AddNewProductCommand = new RelayCommand(() => AddNewProduct(SelectedCustomerIndex));
        }

        #endregion

        #region Private Helpers

        // TODO: More customer details entry validation

        /// <summary>
        /// Reload the list of customers from the database
        /// </summary>
        private void ReloadCustomers()
        {
            Customers.LoadCustomers();
            // The steps below seem to be required in order to successfully refresh the page and keep the same customer selected
            int tempCustomerIndex = SelectedCustomerIndex;
            OnPropertyChanged(nameof(Customers));
            SelectedCustomerIndex = -1;
            SelectedCustomerIndex = tempCustomerIndex;

            UpdateFields(IndexChangedFor.Customer);
        }

        /// <summary>
        /// Update all editing fields to make sure they have the correct data
        /// </summary>
        /// <param name="changedFor"></param>
        private void UpdateFields(IndexChangedFor changedFor)
        {
            // Make sure that we have selected a valid customer
            if (SelectedCustomerIndex < 0 || SelectedCustomerIndex >= Customers.Customers.Count)
            {
                SelectedCustomerName = "";
                SelectedCustomerCSVName = "";
                SelectedCustomerOrderName = "";

                SelectedDepotName = "";
                SelectedDepotCSVName = "";
                SelectedDepotOrderName = "";

                SelectedProductName = "";
                SelectedProductCSVName = "";
                SelectedProductOrderName = "";
                SelectedProductPrice = "";

                return;
            }

            // Update all customer fields
            Customer customer = Customers.Customers[SelectedCustomerIndex];
            
            SelectedCustomerName = customer.Name;
            SelectedCustomerCSVName = customer.CSVName;
            SelectedCustomerOrderName = customer.OrderName;

            
            if (changedFor == IndexChangedFor.Depot || changedFor == IndexChangedFor.Customer)
            {
                // Now make sure we selected a valid depot
                if (SelectedDepotIndex < 0 || SelectedDepotIndex >= customer.Depots.Count)
                {
                    SelectedDepotName = "";
                    SelectedDepotCSVName = "";
                    SelectedDepotOrderName = "";
                }
                else
                {
                    // Update all depot fields
                    Depot depot = customer.Depots[SelectedDepotIndex];

                    SelectedDepotName = depot.Name;
                    SelectedDepotCSVName = depot.CSVName;
                    SelectedDepotOrderName = depot.OrderName;
                }
            }

            if (changedFor == IndexChangedFor.Product || changedFor == IndexChangedFor.Customer)
            {
                // Now make sure we selected a valid product
                if (SelectedProductIndex < 0 || SelectedProductIndex >= customer.Products.Count)
                {
                    SelectedProductName = "";
                    SelectedProductCSVName = "";
                    SelectedProductOrderName = "";
                    SelectedProductPrice = "";
                }
                else
                {
                    // Update all product fields
                    Product product = customer.Products[SelectedProductIndex];

                    SelectedProductName = product.Name;
                    SelectedProductCSVName = product.CSVName;
                    SelectedProductOrderName = product.OrderName;
                    SelectedProductPrice = product.Price.ToString();
                }
            }
        }

        /// <summary>
        /// Update the customer information
        /// </summary>
        /// <param name="customerIndex">The index of currently selected customer</param>
        private void UpdateCustomer(int customerIndex)
        {
            // Select the correct customer based on the index provided
            Customer customer = Customers.Customers[customerIndex];

            // Make sure none of the requeired fields are empty
            if (SelectedCustomerName == "" || SelectedCustomerCSVName == "" || SelectedCustomerOrderName == "") return;

            // Make sure that some changes have actually been made
            if (SelectedCustomerName == customer.Name && SelectedCustomerCSVName == customer.CSVName && SelectedCustomerOrderName == customer.OrderName) return;

            // Change the customer on currently loaded customers list
            customer.Update(SelectedCustomerName, SelectedCustomerCSVName, SelectedCustomerOrderName);

            // Update the customer in the database
            SqliteDataAccess.UpdateCustomer(customer);

            ReloadCustomers();
        }

        /// <summary>
        /// Update the depot information
        /// </summary>
        /// <param name="customerIndex">The index of currently selected customer</param>
        /// <param name="depotIndex">The index of currently selected depot</param>
        private void UpdateDepot(int customerIndex, int depotIndex)
        {
            // Select the correct customer and depot based on the index provided
            Customer customer = Customers.Customers[customerIndex];
            Depot depot = customer.Depots[depotIndex];

            // Make sure none of the requeired fields are empty
            if (SelectedDepotName == "" || SelectedDepotCSVName == "" || SelectedDepotOrderName == "") return;

            // Make sure that some changes have actually been made
            if (SelectedDepotName == depot.Name && SelectedDepotCSVName == depot.CSVName && SelectedDepotOrderName == depot.OrderName) return;

            // Change the depot on currently loaded customers list
            depot.Update(SelectedDepotName, SelectedDepotCSVName, SelectedDepotOrderName);

            // Update the depot in the database
            SqliteDataAccess.UpdateDepot(depot);

            ReloadCustomers();
        }

        /// <summary>
        /// Update the product information
        /// </summary>
        /// <param name="customerIndex">The index of currently selected customer</param>
        /// <param name="productIndex">The index of currently selected product</param>
        private void UpdateProduct(int customerIndex, int productIndex)
        {
            // Select the correct customer and product based on the index provided
            Customer customer = Customers.Customers[customerIndex];
            Product product = customer.Products[productIndex];

            // Make sure none of the requeired fields are empty
            if (SelectedProductName == "" || SelectedProductCSVName == "" || SelectedProductOrderName == "") return;

            // Make sure that some changes have actually been made
            if (SelectedProductName == product.Name && SelectedProductCSVName == product.CSVName && SelectedProductOrderName == product.OrderName) return;

            // TODO: Integrate the price into the form

            // Change the product on currently loaded customers list
            product.Update(SelectedProductName, SelectedProductCSVName, SelectedProductOrderName, product.Price);

            // Update the product in the database
            SqliteDataAccess.UpdateProduct(product);

            ReloadCustomers();
        }

        /// <summary>
        /// A function that removes a depot from the list of depots for specified customer
        /// </summary>
        /// <param name="customerIndex">The index of the currently selected customer</param>
        /// <param name="depotIndex">The index of the currently selected depot</param>
        private void RemoveDepot(int customerIndex, int depotIndex)
        {
            // Select the correct customer and depot based on selected index provided
            Customer customer = Customers.Customers[customerIndex];
            Depot depot = customer.Depots[depotIndex];

            // Delete the depot from database first
            SqliteDataAccess.RemoveDepot(depot.Id);

            // Then delete the object from currently loaded customers list
            customer.DeleteDepot(depot.Id);
        }

        /// <summary>
        /// A function that removes a product form the list of products for specified customer
        /// </summary>
        /// <param name="customerIndex">The index of the currently selected customer</param>
        /// <param name="productIndex">The index of the currently selected product</param>
        private void RemoveProduct(int customerIndex, int productIndex)
        {
            // Select the correct customer and product based on selected index provided
            Customer customer = Customers.Customers[customerIndex];
            Product product = customer.Products[productIndex];

            // Delete the product from database first
            SqliteDataAccess.RemoveProduct(product.Id);

            // Then delete the object from currently loaded customers list
            customer.DeleteProduct(product.Id);
        }

        /// <summary>
        /// A function that adds a new <see cref="Depot"/> to the list of depots for specified customer
        /// </summary>
        /// <param name="name">Name of the depot</param>
        /// <param name="csvName">Name of the depot as appears on CSV files</param>
        /// <param name="orderName">Name of the depot as appears on orders</param>
        /// <param name="customerIndex">The index of the currently selected <see cref="Customer"/></param>
        private void AddNewDepot(int customerIndex)
        {
            // Find the selected customer object
            Customer customer = Customers.Customers[customerIndex];

            // Validate that user has entered all necessary data
            if (NewDepotName == "" || NewDepotCSVName == "" || NewDepotOrderName == "") return;

            // TODO: Show an error message and maybe highlight the border around the field

            // Create a depot object to be inserted into the database
            Depot depot = new Depot(customer.Id, NewDepotName, NewDepotCSVName, NewDepotOrderName);

            // First make sure that there isn't already a depot with exactly the same CSVName & OrderName combination
            if (!customer.SameDepotExists(depot))
            {
                // Insert the depot into the database and get a new depot object with the new depot ID
                Depot newDepot = SqliteDataAccess.AddDepot(depot);

                // If the new depot object is not null (so it was created successfully) add it to the depots list of our customer
                if (newDepot != null)
                {
                    customer.AddDepot(newDepot);

                    // Clear data that was on the form
                    NewDepotName = "";
                    NewDepotCSVName = "";
                    NewDepotOrderName = "";
                }
            }
        }

        /// <summary>
        /// A function that adds a new <see cref="Product"/> to the list of products for specified customer
        /// </summary>
        /// <param name="customerIndex">The index of the currently selected <see cref="Customer"/></param>
        private void AddNewProduct(int customerIndex)
        {
            // Find the selected customer object
            Customer customer = Customers.Customers[customerIndex];

            // Validate that user has entered all necessary data
            if (NewProductName == "" || NewProductCSVName == "" || NewProductOrderName == "") return;

            // TODO: Show an error message and maybe highlight the border around the field

            // Create a product object to be inserted into the database
            Product product = new Product(customer.Id, NewProductName, NewProductCSVName, NewProductOrderName);

            // First make sure that there isn't already a product with exactly the same CSVName & OrderName combination
            if (!customer.SameProductExists(product))
            {
                // Insert the product into the database and get a new product object with the new product ID
                Product newProduct = SqliteDataAccess.AddProduct(product);

                // If the new product object is not null (so it was created successfully) add it to the products list of our customer
                if (newProduct != null)
                {
                    customer.AddProduct(newProduct);

                    // Clear data that was on the form
                    NewProductName = "";
                    NewProductCSVName = "";
                    NewProductOrderName = "";
                }
            }
        }

        #endregion
    }
}
