using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderReaderUI.DesignModels;

public class OrderListItemDesginModel
{
    public static DataView OrdersView => GetOrderItem().DefaultView;

    public static DataTable GetOrderItem()
    {
        DataTable dt = new();

        // Add required basic columns
        dt.Columns.Add("Depot", typeof(string));
        dt.Columns.Add("PO Number", typeof(string));
        dt.Columns.Add("Item Name 1", typeof(double));
        dt.Columns.Add("Item Name 2", typeof(double));
        dt.Columns.Add("Item Name 3", typeof(double));
        dt.Columns.Add("Total", typeof(double));

        for (int i = 0; i < 4; i++)
        {
            DataRow dr = dt.NewRow();
            dr["Depot"] = $"Depot Name {i+1}";
            dr["PO Number"] = $"012345678{i}";
            dr["Item Name 1"] = 25.0;
            dr["Item Name 2"] = 200.0;
            dr["Item Name 3"] = 75.0;
            dr["Total"] = 300.0;
            dt.Rows.Add(dr);
        }
        
        DataRow tr = dt.NewRow();
        tr["Depot"] = $"";
        tr["PO Number"] = $"Total";
        tr["Item Name 1"] = 100.0;
        tr["Item Name 2"] = 800.0;
        tr["Item Name 3"] = 300.0;
        tr["Total"] = 1200.0;
        dt.Rows.Add(tr);

        return dt;
    }
}
