using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OrderReader.Models;

public class CustomerProfileDisplayModel : INotifyPropertyChanged
{
    public int Id { get; set; } = -1;
    
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            CallPropertyChanged(nameof(Name));
        }
    }
    public string Identifier { get; set; } = string.Empty;
    
    public ObservableCollection<CustomerDisplayModel> Customers { get; set; } = [];
    public ObservableCollection<ProductDisplayModel> Products { get; set; } = [];

    public CustomerProfileDisplayModel()
    { }
    
    public CustomerProfileDisplayModel(CustomerProfileDisplayModel other)
    {
        Id = other.Id;
        Name = other.Name;
        Identifier = other.Identifier;
        Customers = other.Customers;
        Products = other.Products;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    public void CallPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}