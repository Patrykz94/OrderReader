using System.ComponentModel;
using OrderReader.Core.DataModels.Customers;

namespace OrderReader.Models;

/// <summary>
/// Display version of the <see cref="Product"/> model
/// </summary>
public class ProductDisplayModel : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _csvName = string.Empty;
    private string _orderName = string.Empty;
    private decimal _price;

    public int Id { get; set; } = -1;
    public int CustomerProfileId { get; set; } = -1;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            CallPropertyChanged(nameof(Name));
        }
    }

    public string CsvName
    {
        get => _csvName;
        set
        {
            _csvName = value;
            CallPropertyChanged(nameof(CsvName));
        }
    }

    public string OrderName
    {
        get => _orderName;
        set
        {
            _orderName = value;
            CallPropertyChanged(nameof(OrderName));
        }
    }
    public decimal Price
    {
        get => _price;
        set
        {
            _price = value;
            CallPropertyChanged(nameof(Price));
        }
    }

    public ProductDisplayModel() { }
    public ProductDisplayModel(ProductDisplayModel other)
    {
        Id = other.Id;
        CustomerProfileId = other.CustomerProfileId;
        Name = other.Name;
        CsvName = other.CsvName;
        OrderName = other.OrderName;
        Price = other.Price;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void CallPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}