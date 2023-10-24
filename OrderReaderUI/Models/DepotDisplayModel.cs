using System.ComponentModel;

namespace OrderReaderUI.Models;

/// <summary>
/// Display version of the <see cref="OrderReader.Core.Depot"/> model
/// </summary>
public class DepotDisplayModel : INotifyPropertyChanged
{
    private string _name = string.Empty;

    public int Id { get; set; } = -1;
    public int CustomerId { get; set; } = -1;
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

    public DepotDisplayModel() { }
    public DepotDisplayModel(DepotDisplayModel other)
    {
        Id = other.Id;
        CustomerId = other.CustomerId;
        Name = other.Name;
        CSVName = other.CSVName;
        OrderName = other.OrderName;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void CallPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}