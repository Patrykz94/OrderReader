﻿using System.ComponentModel;

namespace OrderReaderUI.Models;

/// <summary>
/// Display version of the <see cref="OrderReader.Core.Product"/> model
/// </summary>
public class ProductDisplayModel : INotifyPropertyChanged
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
    public decimal Price { get; set; } = 0m;

    public ProductDisplayModel() { }
    public ProductDisplayModel(ProductDisplayModel other)
    {
        Id = other.Id;
        CustomerId = other.CustomerId;
        Name = other.Name;
        CSVName = other.CSVName;
        OrderName = other.OrderName;
        Price = other.Price;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void CallPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}