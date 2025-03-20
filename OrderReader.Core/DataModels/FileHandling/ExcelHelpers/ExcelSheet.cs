namespace OrderReader.Core.DataModels.FileHandling.ExcelHelpers;

public class ExcelSheet
{
    #region Private Variables
    
    private const int TableColumnCountPosition = 0;
    private const int TableRowCountPosition = 1;

    #endregion
    
    #region Public Properties

    public string SheetName { get; set; }
    public string[] TableText { get; init; }
    public int ColumnCount { get; init; }
    public int RowCount { get; init; }

    #endregion
    
    #region Initialisation

    public ExcelSheet(string name, string[] text)
    {
        SheetName = name;
        TableText = text;
        
        ColumnCount = int.Parse(TableText[TableColumnCountPosition]);
        RowCount = int.Parse(TableText[TableRowCountPosition]);
    }

    #endregion
    
    #region Public Functions

    /// <summary>
    /// Selects the cell from a text based table
    /// </summary>
    /// <param name="column">Column number (starting at index 1)</param>
    /// <param name="row">Row number (starting at index 1)</param>
    /// <returns>Cell value string or null</returns>
    public string? GetCell(int column, int row)
    {
        string? result = null;

        var c = column - 1;
        var r = row - 1;

        if (column > 0 && column <= ColumnCount && row > 0 && row <= RowCount)
        {
            result = TableText[c + r * ColumnCount + 2];
            if (result == "") result = null;
        }

        return result;
    }

    /// <summary>
    /// Selects the cell from a text based table
    /// </summary>
    /// <param name="cellReference">Cell address (e.g. 'A1')</param>
    /// <returns>Cell value string or null</returns>
    public string? GetCell(string cellReference)
    {
        var cell = new Cell(cellReference);
        return GetCell(cell.Column, cell.Row);
    }

    /// <summary>
    /// Selects the cell from a text based table
    /// </summary>
    /// <param name="cell">A <see cref="Cell"/> object</param>
    /// <returns>Cell value string or null</returns>
    public string? GetCell(Cell cell)
    {
        return GetCell(cell.Column, cell.Row);
    }

    #endregion
}