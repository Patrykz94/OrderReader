using System;
using System.Linq;
using Caliburn.Micro;
using OrderReader.Core;
using OrderReader.Core.Interfaces;

namespace OrderReaderUI.ViewModels.Controls.Orders;

public class OrderListViewModel(OrdersLibrary ordersLibrary, INotificationService notificationService) : Conductor<IScreen>.Collection.AllActive
{
    
    public bool CanRemoveItem => Items.Count > 0;

    public void AddItem(string orderId)
    {
        // Make sure we don't have an order 
        if (Items.OfType<OrderListItemViewModel>().Any(x => x.OrderId == orderId)) return;
        
        OrderListItemViewModel newItem = new(orderId, ordersLibrary, notificationService);
        
        Items.Add(newItem);
        NotifyOfPropertyChange(() => Items);
        NotifyOfPropertyChange(() => CanRemoveItem);
    }

    public void RemoveItem(OrderListItemViewModel itemToRemove)
    {
        if (!Items.Remove(itemToRemove)) return;
        
        NotifyOfPropertyChange(() => Items);
        NotifyOfPropertyChange(() => CanRemoveItem);
    }
}