using Caliburn.Micro;
using OrderReader.Core;
using System.Collections.ObjectModel;
using System.Linq;

namespace OrderReaderUI.ViewModels;

public class CustomersViewModel : Screen
{
    #region Constructor

    /// <summary>
    /// Default constructor that loads the customers information from the database
    /// </summary>
    public CustomersViewModel()
    {
        CustomersHandler = OrderReader.Core.IoC.Customers();
        LoadCustomers();
    }

    #endregion

    #region Properties

    /// <summary>
    /// The parent object that contains and manages all <see cref="Customer"/> objects
    /// </summary>
    public CustomersHandler CustomersHandler { get; set; }

    private ObservableCollection<Customer> _customers;
    /// <summary>
    /// A list of all <see cref="Customer"/> objects from <see cref="CustomersHandler"/>
    /// </summary>
    public ObservableCollection<Customer> Customers
    {
        get { return _customers; }
        set
        {
            _customers = value;
            NotifyOfPropertyChange(() => Customers);
        }
    }

    private Customer _selectedCustomer;
    /// <summary>
    /// The <see cref="Customer"/> that is currently selected in the UI
    /// </summary>
    public Customer SelectedCustomer
    {
        get { return _selectedCustomer; }
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
            
            NotifyOfPropertyChange(() => SelectedCustomer);
            NotifyOfPropertyChange(() => HasSelectedCustomer);
            NotifyOfPropertyChange(() => CanUpdateCustomer);
        }
    }

    /// <summary>
    /// Whether or not a <see cref="Customer"/> object has been selected in UI
    /// </summary>
    public bool HasSelectedCustomer
    {
        get
        {
            return SelectedCustomer != null;
        }
    }

    /// <summary>
    /// Whether or not a <see cref="Customer"/> object can be update via the UI
    /// </summary>
    public bool CanUpdateCustomer
    {
        get
        {
            return SelectedCustomer != null;
        }
    }

    private ObservableCollection<Depot> _depots;
    /// <summary>
    /// A list of all <see cref="Depot"/> objects belonging to the currently <see cref="SelectedCustomer"/> object
    /// </summary>
    public ObservableCollection<Depot> Depots
    {
        get { return _depots; }
        set
        {
            _depots = value;
            NotifyOfPropertyChange(() => Depots);
        }
    }

    private Depot _selectedDepot;
    /// <summary>
    /// The <see cref="Depot"/> object that is currently selected in the UI
    /// </summary>
    public Depot SelectedDepot
    {
        get { return _selectedDepot; }
        set
        {
            _selectedDepot = value;
            NotifyOfPropertyChange(() => SelectedDepot);
            NotifyOfPropertyChange(() => HasSelectedDepot);
            NotifyOfPropertyChange(() => CanUpdateDepot);
        }
    }

    /// <summary>
    /// Whether or not a <see cref="Depot"/> object has been selected in UI
    /// </summary>
    public bool HasSelectedDepot
    {
        get
        {
            return SelectedDepot != null;
        }
    }
    
    /// <summary>
    /// Whether or not a <see cref="Depot"/> object can be update via the UI
    /// </summary>
    public bool CanUpdateDepot
    {
        get
        {
            // TODO: Add more data validation here
            return SelectedDepot != null;
        }
    }

    private Depot _newDepot = new();
    /// <summary>
    /// An empty <see cref="Depot"/> object that the user can fill in with information when creating a new depot via the UI
    /// </summary>
    public Depot NewDepot
    {
        get { return _newDepot; }
        private set
        {
            _newDepot = value;
            NotifyOfPropertyChange(() => NewDepot);
        }
    }

    private ObservableCollection<Product> _products;
    /// <summary>
    /// A list of all <see cref="Product"/> objects belonging to the currently <see cref="SelectedCustomer"/> object
    /// </summary>
    public ObservableCollection<Product> Products
    {
        get { return _products; }
        set
        {
            _products = value;
            NotifyOfPropertyChange(() => Products);
        }
    }

    private Product _selectedProduct;
    /// <summary>
    /// The <see cref="Product"/> object that is currently selected in the UI
    /// </summary>
    public Product SelectedProduct
    {
        get { return _selectedProduct; }
        set
        {
            _selectedProduct = value;
            NotifyOfPropertyChange(() => SelectedProduct);
            NotifyOfPropertyChange(() => HasSelectedProduct);
            NotifyOfPropertyChange(() => CanUpdateProduct);
        }
    }

    private Product _newProduct = new();
    /// <summary>
    /// An empty <see cref="Product"/> object that the user can fill in with information when creating a new product via the UI
    /// </summary>
    public Product NewProduct
    {
        get { return _newProduct; }
        private set
        {
            _newProduct = value;
            NotifyOfPropertyChange(() => NewProduct);
        }
    }

    /// <summary>
    /// Whether or not a <see cref="Product"/> object has been selected in UI
    /// </summary>
    public bool HasSelectedProduct
    {
        get
        {
            return _selectedProduct != null;
        }
    }

    /// <summary>
    /// Whether or not a <see cref="Product"/> object can be update via the UI
    /// </summary>
    public bool CanUpdateProduct
    {
        get
        {
            // TODO: Add more data validation here
            return SelectedProduct != null;
        }
    }

    #endregion

    #region Public Functions

    /// <summary>
    /// Loads all <see cref="Customer"/>, <see cref="Depot"/> and <see cref="Product"/> information from the database
    /// and makes sure that, whenever possible, the same <see cref="SelectedCustomer"/> as before stays selected after the reload
    /// </summary>
    public void LoadCustomers()
    {
        int selectedCustomerID = SelectedCustomer?.Id ?? -1;
        CustomersHandler.LoadCustomers();
        Customers = CustomersHandler.Customers;
        if (Customers.Count == 0) return;

        if (CustomersHandler.HasCustomer(selectedCustomerID))
            SelectedCustomer = CustomersHandler.GetCustomerByID(selectedCustomerID);
        else
            SelectedCustomer = Customers.First();

        Depots = SelectedCustomer.Depots;
        Products = SelectedCustomer.Products;
    }

    /// <summary>
    /// Updates the currently selected <see cref="Customer"/>'s information in the <see cref="CustomersHandler"/> and in the database to the new values
    /// </summary>
    public void UpdateCustomer()
    {
        // Do stuff here
    }

    /// <summary>
    /// Updates the currently selected <see cref="Depot"/>'s information in the <see cref="CustomersHandler"/> and in the database to the new values
    /// </summary>
    public void UpdateDepot()
    {
        // Do stuff here
    }

    /// <summary>
    /// Updates the currently selected <see cref="Product"/>'s information in the <see cref="CustomersHandler"/> and in the database to the new values
    /// </summary>
    public void UpdateProduct()
    {
        // Do stuff here
    }

    /// <summary>
    /// Adds a new <see cref="Depot"/> to this <see cref="Customer"/>. Updates the <see cref="CustomersHandler"/> and the database.
    /// </summary>
    public void AddDepot()
    {
        // VALIDATE SHIT!
        Depots.Add(NewDepot);
        NewDepot = new();
    }

    /// <summary>
    /// Adds a new <see cref="Product"/> to this <see cref="Customer"/>. Updates the <see cref="CustomersHandler"/> and the database.
    /// </summary>
    public void AddProduct()
    {
        // VALIDATE SHIT!
        Products.Add(NewProduct);
        NewProduct = new();
    }

    #endregion
}
