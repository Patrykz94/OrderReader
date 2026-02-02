using System.Collections.ObjectModel;
using System.ComponentModel;
using OrderReader.Core.DataModels.Customers;

namespace OrderReader.Models;

/// <summary>
/// Display version of the <see cref="Customer"/> model
/// </summary>
public class CustomerDisplayModel : INotifyPropertyChanged
{
    public int Id { get; set; } = -1;
    public int CustomerProfileId { get; set; } = -1;
    
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
    public string CsvName { get; set; } = string.Empty;
    public string OrderName { get; set; } = string.Empty;
    public ObservableCollection<DepotDisplayModel> Depots { get; set; } = [];

    public CustomerDisplayModel() { }
    public CustomerDisplayModel(CustomerDisplayModel other)
    {
        Id = other.Id;
        CustomerProfileId = other.CustomerProfileId;
        Name = other.Name;
        CsvName = other.CsvName;
        OrderName = other.OrderName;
        Depots = other.Depots;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void CallPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}