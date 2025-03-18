using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OrderReader.Controls.Orders;

public partial class OrderListItemView : UserControl
{
    public OrderListItemView()
    {
        InitializeComponent();
    }

    private void OrdersView_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var scrollViewer = FindAncestor<ScrollViewer>(sender as DependencyObject);
        if (scrollViewer != null)
        {
            var scrollStep = e.Delta / SystemParameters.WheelScrollLines;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - scrollStep);
            e.Handled = true; // Prevents the DataGrid from handling the event
        }
    }

    private static T? FindAncestor<T>(DependencyObject? child) where T : DependencyObject
    {
        if (child is null) return null;
        var parent = VisualTreeHelper.GetParent(child);
        while (parent != null)
        {
            if (parent is T correctlyTyped)
                return correctlyTyped;

            parent = VisualTreeHelper.GetParent(parent);
        }

        return null;
    }
}