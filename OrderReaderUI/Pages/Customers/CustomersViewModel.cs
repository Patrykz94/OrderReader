using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AutoMapper;
using Caliburn.Micro;
using OrderReader.Core.DataAccess;
using OrderReader.Core.DataModels.Customers;
using OrderReader.Core.Enums;
using OrderReader.Core.Interfaces;
using OrderReaderUI.Helpers;
using OrderReaderUI.Models;

namespace OrderReaderUI.Pages.Customers;

public class CustomersViewModel : Screen
{
    #region Private Members

    /// <summary>
    /// An instance of the IMapper used to map data models from one to another
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    /// The parent object that contains and manages all <see cref="Customer"/> objects
    /// </summary>
    private CustomersHandler _customersHandler;

    /// <summary>
    /// An instance of INotificationService used to display notifications on screen
    /// </summary>
    private readonly INotificationService _notificationService;

    #endregion

    #region Constructor

    /// <summary>
    /// Default constructor that loads the customers information from the database
    /// </summary>
    public CustomersViewModel(IMapper mapper, CustomersHandler customersHandler, INotificationService notificationService)
    {
        _mapper = mapper;
        _customersHandler = customersHandler;
        _notificationService = notificationService;

        LoadCustomers();
        
        // Initialize Commands
        DeleteDepotCommand = new RelayCommand(DeleteDepot);
        DeleteProductCommand = new RelayCommand(DeleteProduct);
    }

    #endregion

    #region Properties

    private ObservableCollection<CustomerDisplayModel> _customers = [];
    /// <summary>
    /// A list of all <see cref="Customer"/> objects from <see cref="CustomersHandler"/>
    /// </summary>
    public ObservableCollection<CustomerDisplayModel> Customers
    {
        get => _customers;
        set
        {
            _customers = value;
            NotifyOfPropertyChange(() => Customers);
        }
    }

    private CustomerDisplayModel? _selectedCustomer;
    /// <summary>
    /// The <see cref="CustomerDisplayModel"/> that is currently selected in the UI
    /// </summary>
    public CustomerDisplayModel? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            _selectedCustomer = value;
            if (_selectedCustomer != null)
            {
                Depots = _selectedCustomer.Depots;
                Products = _selectedCustomer.Products;
            }
            else
            {
                Depots.Clear();
                Products.Clear();
            }

            CustomerToEdit = _selectedCustomer!;
            
            NotifyOfPropertyChange(() => SelectedCustomer);
            NotifyOfPropertyChange(() => HasSelectedCustomer);
        }
    }

    private CustomerDisplayModel? _customerToEdit;
    /// <summary>
    /// A copy of the <see cref="CustomerDisplayModel"/> object that is currently selected in the UI, that is safe for editing,
    /// without affecting the original Customer object. This had to be created as a separate property because,
    /// if we set the value of <see cref="SelectedCustomer"/> in the ViewModel to a copy of a <see cref="CustomerDisplayModel"/> object,
    /// the ComboBox in WPF would not know which of the customers is selected, as the <see cref="SelectedCustomer"/> would not be a reference
    /// to any of the <see cref="CustomerDisplayModel"/> objects in the list.
    /// This is not necessary for <see cref="SelectedDepot"/> or <see cref="SelectedProduct"/> properties, because they are only ever selected
    /// directly in the UI, so UI know which one was selected.
    /// </summary>
    public CustomerDisplayModel? CustomerToEdit
    {
        get => _customerToEdit;
        private set
        {
            _customerToEdit = value != null ? new CustomerDisplayModel(value) : null;

            NotifyOfPropertyChange(() => CustomerToEdit);
            NotifyOfPropertyChange(() => CanUpdateCustomer);
        }
    }

    /// <summary>
    /// Whether or not a <see cref="CustomerDisplayModel"/> object has been selected in UI
    /// </summary>
    public bool HasSelectedCustomer => SelectedCustomer != null;

    /// <summary>
    /// Whether or not a <see cref="CustomerDisplayModel"/> object can be update via the UI
    /// </summary>
    public bool CanUpdateCustomer
    {
        get
        {
            if (CustomerToEdit == null) return false;
            // TODO: Look into validation here
            //if (!ValidateCustomer()) return false;
            return true;
        }
    }

    private ObservableCollection<DepotDisplayModel> _depots = [];
    /// <summary>
    /// A list of all <see cref="DepotDisplayModel"/> objects belonging to the currently <see cref="SelectedCustomer"/> object
    /// </summary>
    public ObservableCollection<DepotDisplayModel> Depots
    {
        get => _depots;
        set
        {
            _depots = value;
            NotifyOfPropertyChange(() => Depots);
        }
    }

    private DepotDisplayModel? _selectedDepot;
    /// <summary>
    /// The <see cref="DepotDisplayModel"/> object that is currently selected in the UI
    /// </summary>
    public DepotDisplayModel? SelectedDepot
    {
        get => _selectedDepot;
        set
        {
            _selectedDepot = value != null ? new DepotDisplayModel(value) : null;
            NotifyOfPropertyChange(() => SelectedDepot);
            NotifyOfPropertyChange(() => HasSelectedDepot);
            NotifyOfPropertyChange(() => CanUpdateDepot);
        }
    }

    private DepotDisplayModel _newDepot = new();
    /// <summary>
    /// An empty <see cref="DepotDisplayModel"/> object that the user can fill in with information when creating a new depot via the UI
    /// </summary>
    public DepotDisplayModel NewDepot
    {
        get => _newDepot;
        private set
        {
            _newDepot = value;
            NotifyOfPropertyChange(() => NewDepot);
        }
    }
    /// <summary>
    /// Whether or not a <see cref="DepotDisplayModel"/> object has been selected in UI
    /// </summary>
    public bool HasSelectedDepot => SelectedDepot != null;

    /// <summary>
    /// Whether or not a <see cref="DepotDisplayModel"/> object can be update via the UI
    /// </summary>
    public bool CanUpdateDepot
    {
        get
        {
            if (SelectedDepot == null) return false;
            // TODO: Look into validation here
            //if (!ValidateDepot()) return false;
            return true;
        }
    }

    /// <summary>
    /// Command that is used to delete the selected depot
    /// </summary>
    public ICommand DeleteDepotCommand { get; private set; }

    private ObservableCollection<ProductDisplayModel> _products = [];
    /// <summary>
    /// A list of all <see cref="ProductDisplayModel"/> objects belonging to the currently <see cref="SelectedCustomer"/> object
    /// </summary>
    public ObservableCollection<ProductDisplayModel> Products
    {
        get => _products;
        set
        {
            _products = value;
            NotifyOfPropertyChange(() => Products);
        }
    }

    private ProductDisplayModel? _selectedProduct;
    /// <summary>
    /// The <see cref="ProductDisplayModel"/> object that is currently selected in the UI
    /// </summary>
    public ProductDisplayModel? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            _selectedProduct = value != null ? new ProductDisplayModel(value) : null;
            NotifyOfPropertyChange(() => SelectedProduct);
            NotifyOfPropertyChange(() => HasSelectedProduct);
            NotifyOfPropertyChange(() => CanUpdateProduct);
        }
    }

    private ProductDisplayModel _newProduct = new();
    /// <summary>
    /// An empty <see cref="ProductDisplayModel"/> object that the user can fill in with information when creating a new product via the UI
    /// </summary>
    public ProductDisplayModel NewProduct
    {
        get => _newProduct;
        private set
        {
            _newProduct = value;
            NotifyOfPropertyChange(() => NewProduct);
        }
    }

    /// <summary>
    /// Whether or not a <see cref="ProductDisplayModel"/> object has been selected in UI
    /// </summary>
    public bool HasSelectedProduct => _selectedProduct != null;

    /// <summary>
    /// Whether or not a <see cref="ProductDisplayModel"/> object can be update via the UI
    /// </summary>
    public bool CanUpdateProduct
    {
        get
        {
            if (SelectedProduct == null) return false;
            // TODO: Look into validation here
            //if (!ValidateProduct()) return false;
            return true;
        }
    }

    /// <summary>
    /// Command that is used to delete the selected product
    /// </summary>
    public ICommand DeleteProductCommand { get; private set; }

    #endregion

    #region Public Functions

    /// <summary>
    /// Loads all <see cref="Customer"/>, <see cref="Depot"/> and <see cref="Product"/> information from the database,
    /// maps them to the corresponding <see cref="CustomerDisplayModel"/>, <see cref="DepotDisplayModel"/> and <see cref="ProductDisplayModel"/>
    /// and makes sure that, whenever possible, the same <see cref="SelectedCustomer"/> as before stays selected after the reload
    /// </summary>
    public void LoadCustomers()
    {
        // If customer is selected, save the Id so that we can reselect it after reloading customers
        var selectedCustomerId = SelectedCustomer?.Id ?? -1;

        // Load all customers from the database and map them over to their display models
        _customersHandler.LoadCustomers();
        Customers = _mapper.Map<ObservableCollection<CustomerDisplayModel>>(_customersHandler.Customers);

        // Make sure we have some customers in the list
        if (Customers.Count == 0) return;

        // If we managed to save a selected customer Id before, reselect that customer now
        if (_customersHandler.HasCustomer(selectedCustomerId))
            SelectedCustomer = Customers.FirstOrDefault(x => x.Id == selectedCustomerId)!;
        else
            SelectedCustomer = Customers.First();

        // Populate the Depot and Product lists with data from the selected customer
        Depots = SelectedCustomer.Depots;
        Products = SelectedCustomer.Products;
    }

    /// <summary>
    /// Updates the currently selected customers information in the <see cref="CustomersHandler"/> and in the database to the new values
    /// </summary>
    public void UpdateCustomer()
    {
        if (CustomerToEdit is null || SelectedCustomer is null) return;
        
        // Check if validation is passed
        if (!ValidateCustomer(CustomerToEdit)) return;

        // Update the selected customer display model
        SelectedCustomer.Name = CustomerToEdit.Name;
        SelectedCustomer.CsvName = SelectedCustomer.CsvName;
        SelectedCustomer.OrderName = SelectedCustomer.OrderName;

        // Update the corresponding customer model
        var customerToUpdate = _customersHandler.GetCustomerByID(SelectedCustomer.Id);
        customerToUpdate.Update(CustomerToEdit.Name, CustomerToEdit.CsvName, CustomerToEdit.OrderName);

        // Update the customer in the database
        SqliteDataAccess.UpdateCustomer(customerToUpdate);
    }

    /// <summary>
    /// Updates the currently selected depots's information in the <see cref="CustomersHandler"/> and in the database to the new values
    /// </summary>
    public void UpdateDepot()
    {
        if (SelectedDepot is null) return;
        
        var depotDisplayModel = Depots.FirstOrDefault(x => x.Id == SelectedDepot.Id);
        if (depotDisplayModel is null) return;

        var depotToUpdate = _customersHandler.GetCustomerByID(SelectedDepot.CustomerId).GetDepot(SelectedDepot.Id);
        
        // Check if validation is passed
        if (!ValidateDepot(SelectedDepot)) return;

        // Update the selected depot display model
        depotDisplayModel.Name = SelectedDepot.Name;
        depotDisplayModel.CsvName = SelectedDepot.CsvName;
        depotDisplayModel.OrderName = SelectedDepot.OrderName;

        // Update the corresponding depot model
        depotToUpdate.Update(SelectedDepot.Name, SelectedDepot.CsvName, SelectedDepot.OrderName);

        // Update the depot in the database
        SqliteDataAccess.UpdateDepot(depotToUpdate);

        // Deselect the depot
        SelectedDepot = null;
    }

    /// <summary>
    /// Updates the currently selected product's information in the <see cref="CustomersHandler"/> and in the database to the new values
    /// </summary>
    public void UpdateProduct()
    {
        if (SelectedProduct is null) return;
        
        var productDisplayModel = Products.FirstOrDefault(x => x.Id == SelectedProduct.Id);
        if (productDisplayModel is null) return;
        
        var productToUpdate = _customersHandler.GetCustomerByID(SelectedProduct.CustomerId).GetProduct(SelectedProduct.Id);
        
        // Check if validation is passed
        if (!ValidateProduct(SelectedProduct)) return;

        // Update the selected product display model
        productDisplayModel.Name = SelectedProduct.Name;
        productDisplayModel.CsvName = SelectedProduct.CsvName;
        productDisplayModel.OrderName = SelectedProduct.OrderName;
        productDisplayModel.Price = SelectedProduct.Price;

        // Update the corresponding product model
        productToUpdate.Update(SelectedProduct.Name, SelectedProduct.CsvName, SelectedProduct.OrderName, SelectedProduct.Price);

        // Update the product in the database
        SqliteDataAccess.UpdateProduct(productToUpdate);

        // Deselect the product
        SelectedProduct = null!;
    }

    /// <summary>
    /// Adds a new <see cref="Depot"/> to this <see cref="Customer"/>. Updates the <see cref="CustomersHandler"/> and the database.
    /// </summary>
    public void AddDepot()
    {
        if (SelectedCustomer is null) return;
        
        // Get the selected customer object
        var customer = _customersHandler.GetCustomerByID(SelectedCustomer.Id);

        // Check if validation is passed
        if (!ValidateDepot(NewDepot, true)) return;

        // Create a new depot for this customer
        Depot newDepot = new(customer.Id, NewDepot.Name, NewDepot.CsvName, NewDepot.OrderName);

        // Add the depot to the database and retrieve the unique ID
        var depotId = SqliteDataAccess.AddDepot(newDepot);

        // Update the ID of the depot and add it to the customer
        newDepot.UpdateID(depotId);
        customer.AddDepot(newDepot);

        // Update the display model and add it to the list
        NewDepot.Id = depotId;
        NewDepot.CustomerId = customer.Id;
        Depots.Add(NewDepot);

        // Clear the NewDepot object
        NewDepot = new DepotDisplayModel();
    }

    /// <summary>
    /// Adds a new <see cref="Product"/> to this <see cref="Customer"/>. Updates the <see cref="CustomersHandler"/> and the database.
    /// </summary>
    public void AddProduct()
    {
        if (SelectedCustomer is null) return;
        
        // Get the selected customer object
        var customer = _customersHandler.GetCustomerByID(SelectedCustomer.Id);

        // Check if validation is passed
        if (!ValidateProduct(NewProduct, true)) return;

        // Create a new product for this customer
        Product newProduct = new(customer.Id, NewProduct.Name, NewProduct.CsvName, NewProduct.OrderName, NewProduct.Price);

        // Add the product to the database and retrieve the unique ID
        var productId = SqliteDataAccess.AddProduct(newProduct);

        // Update the ID of the product and add it to the customer
        newProduct.UpdateID(productId);
        customer.AddProduct(newProduct);

        // Update the display model and add it to the list
        NewProduct.Id = productId;
        NewProduct.CustomerId = customer.Id;
        Products.Add(NewProduct);

        // Clear the NewProduct object
        NewProduct = new ProductDisplayModel();
    }

    /// <summary>
    /// Delete the selected depot from this customer
    /// </summary>
    public async void DeleteDepot()
    {
        // Make sure a depot is selected
        if (SelectedDepot is null || SelectedCustomer is null) return;

        var result = await _notificationService.ShowQuestion("Confirm Removing Depot", $"Are you sure you want to remove the {SelectedDepot.Name} depot?");
        if (result == DialogResult.No) return;
        
        // Get selected customer
        var customer = _customersHandler.GetCustomerByID(SelectedCustomer.Id);

        // Remove the depot from the database
        SqliteDataAccess.RemoveDepot(SelectedDepot.Id);

        // Remove the depot from the customer
        customer.DeleteDepot(SelectedDepot.Id);

        // Remove the depot from the list
        Depots.Remove(Depots.Single(x => x.Id == SelectedDepot.Id));
    }
    
    /// <summary>
    /// Delete the selected product from this customer
    /// </summary>
    public async void DeleteProduct()
    {
        // Make sure a product is selected
        if (SelectedProduct is null || SelectedCustomer is null) return;
        
        var result = await _notificationService.ShowQuestion("Confirm Removing Product", $"Are you sure you want to remove {SelectedProduct.Name}?");
        if (result == DialogResult.No) return;
        
        // Get selected customer
        var customer = _customersHandler.GetCustomerByID(SelectedCustomer.Id);

        // Remove the product from the database
        SqliteDataAccess.RemoveProduct(SelectedProduct.Id);

        // Remove the product from the customer
        customer.DeleteProduct(SelectedProduct.Id);

        // Remove the product from the list
        Products.Remove(Products.Single(x => x.Id == SelectedProduct.Id));
    }

    #endregion

    #region Private Functions

    /// <summary>
    /// Validate the customer's new or edited details to make sure all checks are passed
    /// </summary>
    /// <param name="customer">The customer that we want to validate</param>
    /// <param name="isNew">Whether or not this is a new customer</param>
    /// <returns><see cref="bool"/> representing the validation result</returns>
    private bool ValidateCustomer(CustomerDisplayModel customer, bool isNew = false)
    {
        // Make sure all fields have the required number of characters
        if (customer.Name.Length is < 3 or > 50) return false;
        if (customer.CsvName.Length is < 3 or > 50) return false;
        if (customer.OrderName.Length is < 3 or > 50) return false;

        if (isNew)
        {
            // Make sure both the name and order name are unique
            if (_customersHandler.HasCustomerName(customer.Name)) return false;
            if (_customersHandler.HasCustomerOrderName(customer.OrderName)) return false;
        }
        else
        {
            if (SelectedCustomer is null) return false;
            
            // Make sure changes have actually been made
            if (customer.Name == SelectedCustomer.Name &&
                customer.CsvName == SelectedCustomer.CsvName &&
                customer.OrderName == SelectedCustomer.OrderName) return false;

            // Make sure both the name and order name are either the same as they were or they are unique
            if (customer.Name != SelectedCustomer.Name && _customersHandler.HasCustomerName(customer.Name)) return false;
            if (customer.OrderName != SelectedCustomer.OrderName && _customersHandler.HasCustomerOrderName(customer.OrderName)) return false;
        }

        // Validation passed
        return true;
    }

    /// <summary>
    /// Validate the depot's new or edited details to make sure all checks are passed
    /// </summary>
    /// <param name="depot">The depot we want to validate</param>
    /// <param name="isNew">Whether or not this is a new depot</param>
    /// <returns><see cref="bool"/> representing the validation result</returns>
    private bool ValidateDepot(DepotDisplayModel depot, bool isNew = false)
    {
        if (SelectedCustomer is null) return false;
        
        // Get the customer model
        var customer = _customersHandler.GetCustomerByID(SelectedCustomer.Id);

        // Make sure all fields have the required number of characters
        if (depot.Name.Length is < 3 or > 50) return false;
        if (depot.CsvName.Length is < 3 or > 50) return false;
        if (depot.OrderName.Length is < 3 or > 50) return false;

        if (isNew)
        {
            // Make sure both the name and order name are unique
            if (customer.HasDepotName(depot.Name)) return false;
            if (customer.HasDepotOrderName(depot.OrderName)) return false;
        }
        else
        {
            // Get the display model for comparison
            var depotDisplayModel = Depots.FirstOrDefault(x => x.Id == depot.Id)!;

            // Make sure changes have actually been made
            if (depot.Name == depotDisplayModel.Name &&
                depot.CsvName == depotDisplayModel.CsvName &&
                depot.OrderName == depotDisplayModel.OrderName) return false;

            // Make sure both the name and order name are either the same as they were or they are unique
            if (depot.Name != depotDisplayModel.Name && customer.HasDepotName(depot.Name)) return false;
            if (depot.OrderName != depotDisplayModel.OrderName && customer.HasDepotOrderName(depot.OrderName)) return false;
        }

        // Validation passed
        return true;
    }

    /// <summary>
    /// Validate the product's new or edited details to make sure all checks are passed
    /// </summary>
    /// <param name="product">The Product we want to validate</param>
    /// <param name="isNew">Whether or not this is a new product</param>
    /// <returns><see cref="bool"/> representing the validation result</returns>
    private bool ValidateProduct(ProductDisplayModel product, bool isNew = false)
    {
        if (SelectedCustomer is null) return false;
        
        // Get the customer model
        var customer = _customersHandler.GetCustomerByID(SelectedCustomer.Id);

        // Make sure all fields have the required number of characters
        if (product.Name.Length is < 3 or > 50) return false;
        if (product.CsvName.Length is < 3 or > 50) return false;
        if (product.OrderName.Length is < 3 or > 50) return false;

        // Make sure the price is within bounds
        if (product.Price < 0m) return false;

        if (isNew)
        {
            // Make sure both the name and order name are unique
            if (customer.HasProductName(product.Name)) return false;
            if (customer.HasProductOrderName(product.OrderName)) return false;
        }
        else
        {
            // Get the display model for comparison
            var productDisplayModel = Products.FirstOrDefault(x => x.Id == product.Id)!;

            // Make sure changes have actually been made
            if (product.Name == productDisplayModel.Name &&
                product.CsvName == productDisplayModel.CsvName &&
                product.OrderName == productDisplayModel.OrderName &&
                product.Price == productDisplayModel.Price) return false;

            // Make sure both the name and order name are either the same as they were or they are unique
            if (product.Name != productDisplayModel.Name && customer.HasProductName(product.Name)) return false;
            if (product.OrderName != productDisplayModel.OrderName && customer.HasProductOrderName(product.OrderName)) return false;
        }

        // Validation passed
        return true;
    }

    #endregion
}
