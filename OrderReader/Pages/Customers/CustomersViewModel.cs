using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AutoMapper;
using Caliburn.Micro;
using OrderReader.Core.DataAccess;
using OrderReader.Core.DataModels.Customers;
using OrderReader.Core.Enums;
using OrderReader.Core.Interfaces;
using OrderReader.Helpers;
using OrderReader.Models;

namespace OrderReader.Pages.Customers;

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
    private readonly CustomersHandler _customersHandler;

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

        LoadCustomerData();

        // Initialise Commands
        DeleteCustomerCommand = new RelayCommand(DeleteCustomer);
        DeleteDepotCommand = new RelayCommand(DeleteDepot);
        DeleteProductCommand = new RelayCommand(DeleteProduct);
    }

    #endregion

    #region Properties
    
    private ObservableCollection<CustomerProfileDisplayModel> _customerProfiles = [];
    public ObservableCollection<CustomerProfileDisplayModel> CustomerProfiles
    {
        get => _customerProfiles;
        set
        {
            _customerProfiles = value;
            NotifyOfPropertyChange(() => CustomerProfiles);
        }
    }
    
    private CustomerProfileDisplayModel? _selectedCustomerProfile;
    public CustomerProfileDisplayModel? SelectedCustomerProfile
    {
        get => _selectedCustomerProfile;
        set
        {
            _selectedCustomerProfile = value;
            if (_selectedCustomerProfile != null)
            {
                Customers = _selectedCustomerProfile.Customers;
                SelectedCustomer = Customers.FirstOrDefault();
                Products = _selectedCustomerProfile.Products;
            }
            else
            {
                Customers.Clear();
                Products.Clear();
            }

            CustomerProfileToEdit = _selectedCustomerProfile!;
            
            NotifyOfPropertyChange(() => SelectedCustomerProfile);
            NotifyOfPropertyChange(() => HasSelectedCustomerProfile);
        }
    }
    
    private CustomerProfileDisplayModel? _customerProfileToEdit;
    /// <summary>
    /// A copy of the <see cref="CustomerProfileDisplayModel"/> object that is currently selected in the UI, that is safe for editing,
    /// without affecting the original Customer object. This had to be created as a separate property because,
    /// if we set the value of <see cref="SelectedCustomerProfile"/> in the ViewModel to a copy of a <see cref="CustomerProfileDisplayModel"/> object,
    /// the ComboBox in WPF would not know which of the customers is selected, as the <see cref="SelectedCustomerProfile"/> would not be a reference
    /// to any of the <see cref="CustomerProfileDisplayModel"/> objects in the list.
    /// This is not necessary for <see cref="SelectedDepot"/> or <see cref="SelectedProduct"/> properties, because they are only ever selected
    /// directly in the UI, so UI know which one was selected.
    /// </summary>
    public CustomerProfileDisplayModel? CustomerProfileToEdit
    {
        get => _customerProfileToEdit;
        private set
        {
            _customerProfileToEdit = value != null ? new CustomerProfileDisplayModel(value) : null;

            NotifyOfPropertyChange(() => CustomerProfileToEdit);
            NotifyOfPropertyChange(() => CanUpdateCustomerProfile);
        }
    }
    
    /// <summary>
    /// Whether a <see cref="CustomerProfileDisplayModel"/> object has been selected in UI
    /// </summary>
    public bool HasSelectedCustomerProfile => SelectedCustomerProfile != null;

    /// <summary>
    /// Whether a <see cref="CustomerProfileDisplayModel"/> object can be updated via the UI
    /// </summary>
    public bool CanUpdateCustomerProfile
    {
        get
        {
            if (CustomerProfileToEdit == null) return false;
            // TODO: Look into validation here
            //if (!ValidateCustomerProfile()) return false;
            return true;
        }
    }
    
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
            }
            else
            {
                Depots.Clear();
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
    
    private CustomerDisplayModel _newCustomer = new();
    /// <summary>
    /// An empty <see cref="CustomerDisplayModel"/> object that the user can fill in with information when creating a new customer via the UI
    /// </summary>
    public CustomerDisplayModel NewCustomer
    {
        get => _newCustomer;
        private set
        {
            _newCustomer = value;
            NotifyOfPropertyChange(() => NewCustomer);
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
    
    /// <summary>
    /// Command that is used to delete the selected customer
    /// </summary>
    public ICommand DeleteCustomerCommand { get; private set; }

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
    public void LoadCustomerData()
    {
        // If a customer profile is selected, save the ID so that we can reselect it after reloading customer data
        var selectedCustomerProfileId = SelectedCustomerProfile?.Id ?? -1;
        var selectedCustomerId = SelectedCustomer?.Id ?? -1;

        // Load all customers from the database and map them over to their display models
        _customersHandler.LoadCustomerData();
        CustomerProfiles = _mapper.Map<ObservableCollection<CustomerProfileDisplayModel>>(_customersHandler.CustomerProfiles);
        
        // Make sure we have some customer profiles in the list
        if (CustomerProfiles.Count == 0)
        {
            SelectedCustomerProfile = null;
            SelectedCustomer = null;
            SelectedDepot = null;
            SelectedProduct = null;
            return;
        }

        // Attempt to reselect the same customer profile as before, if possible
        SelectedCustomerProfile = CustomerProfiles.FirstOrDefault(x => x.Id == selectedCustomerProfileId) ?? CustomerProfiles.First();
        
        Customers = SelectedCustomerProfile.Customers;
        Products = SelectedCustomerProfile.Products;
        SelectedProduct = null;
        
        if (Customers.Count == 0)
        {
            SelectedCustomer = null;
            SelectedDepot = null;
            return;
        }
        
        // Attempt to reselect the same customer as before, if possible
        SelectedCustomer = Customers.FirstOrDefault(x => x.Id == selectedCustomerId) ?? Customers.First();
        Depots = SelectedCustomer.Depots;
        SelectedDepot = null;
    }

    /// <summary>
    /// Updates the currently selected customer profiles information in the <see cref="CustomersHandler"/> and in the database to the new values
    /// </summary>
    public void UpdateCustomerProfile()
    {
        if (CustomerProfileToEdit is null || SelectedCustomerProfile is null) return;

        // Check if validation is passed
        if (!ValidateCustomerProfile(CustomerProfileToEdit)) return;
        
        // Get the selected customer profile
        var customerProfileToUpdate = _customersHandler.GetCustomerProfile(SelectedCustomerProfile.Id);
        if (customerProfileToUpdate is null) return;
        
        // Update the selected customer profile display model
        SelectedCustomerProfile.Name = CustomerProfileToEdit.Name;
        SelectedCustomerProfile.Identifier = CustomerProfileToEdit.Identifier;

        // Update the corresponding customer profile model
        customerProfileToUpdate.Update(CustomerProfileToEdit.Name, CustomerProfileToEdit.Identifier);

        // Update the customer in the database
        SqliteDataAccess.UpdateCustomerProfile(customerProfileToUpdate);
    }
    
    /// <summary>
    /// Updates the currently selected customers information in the <see cref="CustomersHandler"/> and in the database to the new values
    /// </summary>
    public void UpdateCustomer()
    {
        if (CustomerToEdit is null || SelectedCustomer is null || SelectedCustomerProfile is null) return;

        // Check if validation is passed
        if (!ValidateCustomer(CustomerToEdit)) return;
        
        var customerToUpdate = _customersHandler.GetCustomerProfile(SelectedCustomerProfile.Id)?.GetCustomer(SelectedCustomer.Id);
        if (customerToUpdate is null) return;

        // Update the selected customer display model
        SelectedCustomer.Name = CustomerToEdit.Name;
        SelectedCustomer.CsvName = CustomerToEdit.CsvName;
        SelectedCustomer.OrderName = CustomerToEdit.OrderName;

        // Update the corresponding customer model
        customerToUpdate.Update(CustomerToEdit.Name, CustomerToEdit.CsvName, CustomerToEdit.OrderName);

        // Update the customer in the database
        SqliteDataAccess.UpdateCustomer(customerToUpdate);
    }

    /// <summary>
    /// Updates the currently selected depots's information in the <see cref="CustomersHandler"/> and in the database to the new values
    /// </summary>
    public void UpdateDepot()
    {
        if (SelectedDepot is null || SelectedCustomer is null || SelectedCustomerProfile is null) return;

        var depotDisplayModel = Depots.FirstOrDefault(x => x.Id == SelectedDepot.Id);
        if (depotDisplayModel is null) return;

        var depotToUpdate = _customersHandler
            .GetCustomerProfile(SelectedCustomerProfile.Id)?
            .GetCustomer(SelectedCustomer.Id)?
            .GetDepot(SelectedDepot.Id);
        
        if (depotToUpdate is null) return;

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
        if (SelectedProduct is null || SelectedCustomerProfile is null) return;

        var productDisplayModel = Products.FirstOrDefault(x => x.Id == SelectedProduct.Id);
        if (productDisplayModel is null) return;

        var productToUpdate = _customersHandler
            .GetCustomerProfile(SelectedCustomerProfile.Id)?
            .GetProduct(SelectedProduct.Id);
        
        if (productToUpdate is null) return;

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
    /// Adds a new <see cref="Customer"/> to this <see cref="CustomerProfile"/>. Updates the <see cref="CustomersHandler"/> and the database.
    /// </summary>
    public void AddCustomer()
    {
        if (SelectedCustomerProfile is null) return;

        // Get the selected customer profile object
        var customerProfile = _customersHandler.GetCustomerProfile(SelectedCustomerProfile.Id);
        if (customerProfile is null) return;

        // Check if validation is passed
        if (!ValidateCustomer(NewCustomer, true)) return;

        // Create a new customer for this customer profile
        Customer newCustomer = new(customerProfile.Id, NewCustomer.Name, NewCustomer.CsvName, NewCustomer.OrderName);

        // Add the customer to the database and retrieve the unique ID
        var customerId = SqliteDataAccess.AddCustomer(newCustomer);

        // Update the ID of the customer and add it to the customerProfile
        newCustomer.UpdateId(customerId);
        customerProfile.AddCustomer(newCustomer);

        // Update the display model and add it to the list
        NewCustomer.Id = customerId;
        NewCustomer.CustomerProfileId = customerProfile.Id;
        Customers.Add(NewCustomer);

        // Clear the NewCustomer object
        NewCustomer = new CustomerDisplayModel();
    }
    
    /// <summary>
    /// Adds a new <see cref="Depot"/> to this <see cref="Customer"/>. Updates the <see cref="CustomersHandler"/> and the database.
    /// </summary>
    public void AddDepot()
    {
        if (SelectedCustomer is null || SelectedCustomerProfile is null) return;

        // Get the selected customer object
        var customer = _customersHandler.GetCustomerProfile(SelectedCustomer.Id)?.GetCustomer(SelectedCustomer.Id);
        
        if (customer is null) return;

        // Check if validation is passed
        if (!ValidateDepot(NewDepot, true)) return;

        // Create a new depot for this customer
        Depot newDepot = new(customer.Id, NewDepot.Name, NewDepot.CsvName, NewDepot.OrderName);

        // Add the depot to the database and retrieve the unique ID
        var depotId = SqliteDataAccess.AddDepot(newDepot);

        // Update the ID of the depot and add it to the customer
        newDepot.UpdateId(depotId);
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
        if (SelectedCustomerProfile is null) return;

        // Get the selected customer object
        var customerProfile = _customersHandler.GetCustomerProfile(SelectedCustomerProfile.Id);
        
        if (customerProfile is null) return;

        // Check if validation is passed
        if (!ValidateProduct(NewProduct, true)) return;

        // Create a new product for this customer
        Product newProduct = new(customerProfile.Id, NewProduct.Name, NewProduct.CsvName, NewProduct.OrderName, NewProduct.Price);

        // Add the product to the database and retrieve the unique ID
        var productId = SqliteDataAccess.AddProduct(newProduct);

        // Update the ID of the product and add it to the customer
        newProduct.UpdateId(productId);
        customerProfile.AddProduct(newProduct);

        // Update the display model and add it to the list
        NewProduct.Id = productId;
        NewProduct.CustomerProfileId = customerProfile.Id;
        Products.Add(NewProduct);

        // Clear the NewProduct object
        NewProduct = new ProductDisplayModel();
    }

    /// <summary>
    /// Delete the selected customer from this customer profile
    /// </summary>
    public async void DeleteCustomer()
    {
        // Make sure a customer is selected
        if (SelectedCustomer is null || SelectedCustomerProfile is null ) return;

        if (Customers.Count <= 1)
        {
            await _notificationService.ShowMessage("Cannot Remove Customer", $"Cannot remove {SelectedCustomer.Name} because it is the only customer on this profile.");
            return;
        }
        
        // Get the selected customer profile
        var customerProfile = _customersHandler.GetCustomerProfile(SelectedCustomerProfile.Id);
        if (customerProfile is null) return;
        
        var result = await _notificationService.ShowQuestion("Confirm Removing Customer", $"Are you sure you want to remove {SelectedCustomer.Name}?");
        if (result == DialogResult.No) return;

        // Remove the depot from the database
        SqliteDataAccess.RemoveCustomer(SelectedCustomer.Id);

        // Remove the depot from the customer
        customerProfile.DeleteCustomer(SelectedCustomer.Id);

        // Remove the depot from the list
        Customers.Remove(Customers.Single(x => x.Id == SelectedCustomer.Id));
        
        // Select another customer
        SelectedCustomer = Customers.First();
        Depots = SelectedCustomer.Depots;
        SelectedDepot = null;
    }
    
    /// <summary>
    /// Delete the selected depot from this customer
    /// </summary>
    public async void DeleteDepot()
    {
        // Make sure a depot is selected
        if (SelectedDepot is null || SelectedCustomer is null || SelectedCustomerProfile is null) return;

        var result = await _notificationService.ShowQuestion("Confirm Removing Depot", $"Are you sure you want to remove the {SelectedDepot.Name} depot?");
        if (result == DialogResult.No) return;

        // Get a selected customer
        var customer = _customersHandler.GetCustomerProfile(SelectedCustomerProfile.Id)?.GetCustomer(SelectedCustomer.Id);
        if (customer is null) return;

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
        if (SelectedProduct is null || SelectedCustomerProfile is null) return;

        var result = await _notificationService.ShowQuestion("Confirm Removing Product", $"Are you sure you want to remove {SelectedProduct.Name}?");
        if (result == DialogResult.No) return;

        // Get a selected customer profile
        var customerProfile = _customersHandler.GetCustomerProfile(SelectedCustomerProfile.Id);
        if (customerProfile is null) return;

        // Remove the product from the database
        SqliteDataAccess.RemoveProduct(SelectedProduct.Id);

        // Remove the product from the customer
        customerProfile.DeleteProduct(SelectedProduct.Id);

        // Remove the product from the list
        Products.Remove(Products.Single(x => x.Id == SelectedProduct.Id));
    }

    #endregion

    #region Private Functions

    /// <summary>
    /// Validate the customer profile's new or edited details to make sure all checks are passed
    /// </summary>
    /// <param name="customerProfile">The customer profile that we want to validate</param>
    /// <param name="isNew">Whether this is a new customer profile</param>
    /// <returns><see cref="bool"/> representing the validation result</returns>
    private bool ValidateCustomerProfile(CustomerProfileDisplayModel customerProfile, bool isNew = false)
    {
        // Make sure all fields have the required number of characters
        if (customerProfile.Name.Length is < 3 or > 50) return false;
        if (customerProfile.Identifier.Length is < 3 or > 50) return false;

        if (isNew)
        {
            // Make sure both the name and identifier are unique
            if (_customersHandler.HasCustomerProfileName(customerProfile.Name)) return false;
            if (_customersHandler.HasCustomerProfileIdentifier(customerProfile.Identifier)) return false;
        }
        else
        {
            if (SelectedCustomerProfile is null) return false;

            // Make sure changes have actually been made
            if (customerProfile.Name == SelectedCustomerProfile.Name &&
                customerProfile.Identifier == SelectedCustomerProfile.Identifier) return false;

            // Make sure both the name and order name are either the same as they were or they are unique
            if (customerProfile.Name != SelectedCustomerProfile.Name && _customersHandler.HasCustomerProfileName(customerProfile.Name)) return false;
            if (customerProfile.Identifier != SelectedCustomerProfile.Identifier && _customersHandler.HasCustomerProfileIdentifier(customerProfile.Identifier)) return false;
        }

        // Validation passed
        return true;
    }
    
    /// <summary>
    /// Validate the customer's new or edited details to make sure all checks are passed
    /// </summary>
    /// <param name="customer">The customer that we want to validate</param>
    /// <param name="isNew">Whether this is a new customer</param>
    /// <returns><see cref="bool"/> representing the validation result</returns>
    private bool ValidateCustomer(CustomerDisplayModel customer, bool isNew = false)
    {
        if (SelectedCustomerProfile is null) return false;
        
        var customerProfile = _customersHandler.GetCustomerProfile(SelectedCustomerProfile.Id);
        if (customerProfile is null) return false;
        
        // Make sure all fields have the required number of characters
        if (customer.Name.Length is < 3 or > 50) return false;
        if (customer.CsvName.Length is < 3 or > 50) return false;
        if (customer.OrderName.Length is < 3 or > 50) return false;

        if (isNew)
        {
            // Make sure both the name and order name are unique
            if (customerProfile.HasCustomerName(customer.Name)) return false;
            if (customerProfile.HasCustomerOrderName(customer.OrderName)) return false;
        }
        else
        {
            if (SelectedCustomer is null) return false;

            // Make sure changes have actually been made
            if (customer.Name == SelectedCustomer.Name &&
                customer.CsvName == SelectedCustomer.CsvName &&
                customer.OrderName == SelectedCustomer.OrderName) return false;

            // Make sure both the name and order name are either the same as they were or they are unique
            if (customer.Name != SelectedCustomer.Name && customerProfile.HasCustomerName(customer.Name)) return false;
            if (customer.OrderName != SelectedCustomer.OrderName && customerProfile.HasCustomerOrderName(customer.OrderName)) return false;
        }

        // Validation passed
        return true;
    }

    /// <summary>
    /// Validate the depot's new or edited details to make sure all checks are passed
    /// </summary>
    /// <param name="depot">The depot we want to validate</param>
    /// <param name="isNew">Whether this is a new depot</param>
    /// <returns><see cref="bool"/> representing the validation result</returns>
    private bool ValidateDepot(DepotDisplayModel depot, bool isNew = false)
    {
        if (SelectedCustomer is null || SelectedCustomerProfile is null) return false;

        // Get the customer model
        var customer = _customersHandler.GetCustomerProfile(SelectedCustomerProfile.Id)?.GetCustomer(SelectedCustomer.Id);
        if (customer is null) return false;

        // Make sure all fields have the required number of characters
        if (depot.Name.Length is < 3 or > 50) return false;
        if (depot.CsvName.Length is < 3 or > 50) return false;
        if (depot.OrderName.Length < 3) return false;

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
        if (SelectedCustomerProfile is null) return false;

        // Get the customer model
        var customerProfile = _customersHandler.GetCustomerProfile(SelectedCustomerProfile.Id);
        if (customerProfile is null) return false;

        // Make sure all fields have the required number of characters
        if (product.Name.Length is < 3 or > 50) return false;
        if (product.CsvName.Length is < 3 or > 50) return false;
        if (product.OrderName.Length is < 3 or > 50) return false;

        // Make sure the price is within bounds
        if (product.Price < 0m) return false;

        if (isNew)
        {
            // Make sure both the name and order name are unique
            if (customerProfile.HasProductName(product.Name)) return false;
            if (customerProfile.HasProductOrderName(product.OrderName)) return false;
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
            if (product.Name != productDisplayModel.Name && customerProfile.HasProductName(product.Name)) return false;
            if (product.OrderName != productDisplayModel.OrderName && customerProfile.HasProductOrderName(product.OrderName)) return false;
        }

        // Validation passed
        return true;
    }

    #endregion
}
