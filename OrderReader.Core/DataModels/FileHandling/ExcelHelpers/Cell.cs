using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OrderReader.Core.DataModels.FileHandling.ExcelHelpers;

public struct Cell : IEquatable<Cell>
{
    #region Properties

    public int Column { get;set;}
    public int Row { get; set; }

    #endregion

    #region Initialisation

    /// <summary>
    /// Constructs a cell out of column and row numbers
    /// </summary>
    public Cell(int column, int row)
    {
        Column = column;
        Row = row;
    }

    /// <summary>
    /// Constructs a cell out of cell name string
    /// </summary>
    public Cell(string cellReference)
    {
        var foundNums = false;
        var characters = cellReference.ToUpper();

        List<int> col = [];
        var row = string.Empty;

        foreach (var character in characters)
        {
            switch (character)
            {
                case >= 'A' and <= 'Z':
                {
                    if (foundNums) { Column = -1; Row = -1; };
                    col.Add(character - 'A');
                    break;
                }
                case >= '0' and <= '9':
                    foundNums = true;
                    row += character;
                    break;
                default:
                    Column = -1;
                    Row = -1;
                    break;
            }
        }

        var c = 0;
        for (var i = 0; i < col.Count; i++)
        {
            c += (col[i] + 1) * (int)Math.Pow(26.0, i);
        }

        if (!int.TryParse(row, out var r)) { Column = -1; Row = -1; };

        Column = c;
        Row = r;
    }

    #endregion

    #region Public Functions

    public override bool Equals([NotNullWhen(true)] object? other)
    {
        if (other is Cell otherCell)
        {
            return Column == otherCell.Column && Row == otherCell.Row;
        }

        return false;
    }

    public bool Equals(Cell other)
    {
        return Column == other.Column && Row == other.Row;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Column, Row);
    }

    #region Operators

    public static bool operator ==(Cell left, Cell right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Cell left, Cell right)
    {
        return !(left == right);
    }

    #endregion
    
    #endregion
}