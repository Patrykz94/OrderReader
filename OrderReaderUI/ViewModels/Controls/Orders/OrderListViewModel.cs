using System.Collections.ObjectModel;
using Caliburn.Micro;

namespace OrderReaderUI.ViewModels.Controls.Orders;

public class OrderListViewModel : Conductor<IScreen>.Collection.AllActive
{
    public ObservableCollection<OrderListItemViewModel> OrderItemControl { get; set; }

    public OrderListViewModel()
    {
        OrderItemControl = [];
    }

    public string Field { get; set; } = "Order List Control";
    public bool CanRemoveItem => OrderItemControl.Count > 0;

    public void AddItem(string name)
    {
        OrderListItemViewModel newItem = new ()
        {
            Field = name
        };

        OrderItemControl.Add(newItem);
        Items.Add(newItem);
        NotifyOfPropertyChange(() => OrderItemControl);
        NotifyOfPropertyChange(() => CanRemoveItem);
    }

    public void RemoveItem()
    {
        if (OrderItemControl.Count > 0)
        {
            OrderItemControl.RemoveAt(0);
            NotifyOfPropertyChange(() => OrderItemControl);
            NotifyOfPropertyChange(() => CanRemoveItem);
        }
    }
}