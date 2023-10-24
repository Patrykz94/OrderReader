using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OrderReaderUI.Models;

/// <summary>
/// Display version of the <see cref="OrderReader.Core.Customer"/> model
/// </summary>
public class CustomerDisplayModel : INotifyPropertyChanged
{
    private string _name = string.Empty;

    public int Id { get; set; } = -1;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            CallPropertyChanged(nameof(Name));
        }
    }
    public string CSVName { get; set; } = string.Empty;
    public string OrderName { get; set; } = string.Empty;
    public ObservableCollection<DepotDisplayModel> Depots { get; set; } = new();
    public ObservableCollection<ProductDisplayModel> Products { get; set; } = new();

    public CustomerDisplayModel() { }
    public CustomerDisplayModel(CustomerDisplayModel other)
    {
        Id = other.Id;
        Name = other.Name;
        CSVName = other.CSVName;
        OrderName = other.OrderName;
        Depots = other.Depots;
        Products = other.Products;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void CallPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}