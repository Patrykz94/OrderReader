using Caliburn.Micro;
using OrderReader.Core;
using System.Collections.ObjectModel;
using System.Linq;

namespace OrderReaderUI.ViewModels;

public class CustomersViewModel : Screen
{
    #region Constructor

    public CustomersViewModel()
    {
        CustomersHandler = OrderReader.Core.IoC.Customers();
        LoadCustomers();
    }

    #endregion

    #region Properties

    public CustomersHandler CustomersHandler { get; set; }

    private ObservableCollection<Customer> _customers;
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
        }
    }

    private ObservableCollection<Depot> _depots;
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

    public bool HasSelectedDepot
    {
        get
        {
            return SelectedDepot != null;
        }
    }
    public bool CanUpdateDepot
    {
        get
        {
            // Add more data validation here
            return SelectedDepot != null;
        }
    }

    private Depot _newDepot = new();
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
    public Product NewProduct
    {
        get { return _newProduct; }
        private set
        {
            _newProduct = value;
            NotifyOfPropertyChange(() => NewProduct);
        }
    }

    public bool HasSelectedProduct
    {
        get
        {
            return _selectedProduct != null;
        }
    }
    public bool CanUpdateProduct
    {
        get
        {
            // Add more data validation here
            return SelectedProduct != null;
        }
    }

    #endregion

    #region Public Functions

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

    public void UpdateDepot()
    {
        // Do stuff here
    }

    public void UpdateProduct()
    {
        // Do stuff here
    }

    public void AddDepot()
    {
        // VALIDATE SHIT!
        Depots.Add(NewDepot);
        NewDepot = new();
    }

    public void AddProduct()
    {
        // VALIDATE SHIT!
        Products.Add(NewProduct);
        NewProduct = new();
    }

    #endregion
}
