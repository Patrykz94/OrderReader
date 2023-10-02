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
            NotifyOfPropertyChange(() => SelectedCustomer);
        }
    }

    #endregion

    #region Public Functions

    public void LoadCustomers()
    {
        CustomersHandler.LoadCustomers();
        Customers = CustomersHandler.Customers;
        SelectedCustomer = Customers.FirstOrDefault();
    }

    #endregion
}
