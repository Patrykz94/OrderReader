using FluentAssertions;
using OrderReader.Core.DataModels.FileHandling.ExcelHelpers;

namespace OrderReader.Core.Tests;

public class CellTests
{
    [Theory]
    [InlineData(1, "A")]
    [InlineData(26, "Z")]
    [InlineData(8, "H")]
    [InlineData(16, "P")]
    public void GetColumnLetter_WithSingleColumn_ReturnsAtoZ(int inputColumn, string? expectedLetter)
    {
        // Act
        var columnLetter = Cell.GetColumnLetter(inputColumn);
        
        // Assert
        columnLetter.Should().NotBeNullOrEmpty();
        columnLetter.Should().Be(expectedLetter);
    }
    
    [Theory]
    [InlineData(27, "AA")]
    [InlineData(34, "AH")]
    [InlineData(42, "AP")]
    public void GetColumnLetter_WithBeyondZ_ReturnsAAStyle(int inputColumn, string? expectedLetter)
    {
        // Act
        var columnLetter = Cell.GetColumnLetter(inputColumn);
        
        // Assert
        columnLetter.Should().NotBeNullOrEmpty();
        columnLetter.Should().Be(expectedLetter);
    }
    
    [Theory]
    [InlineData(26, "Z")]
    [InlineData(52, "AZ")]
    [InlineData(78, "BZ")]
    [InlineData(104, "CZ")]
    [InlineData(130, "DZ")]
    [InlineData(703, "AAA")]
    [InlineData(729, "ABA")]
    [InlineData(4343, "FKA")]
    [InlineData(16380, "XEZ")]
    public void GetColumnLetter_WithExactMultipleOf26_ReturnsAAStyle(int inputColumn, string? expectedLetter)
    {
        // Act
        var columnLetter = Cell.GetColumnLetter(inputColumn);
        
        // Assert
        columnLetter.Should().NotBeNullOrEmpty();
        columnLetter.Should().Be(expectedLetter);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void GetColumnLetter_WithNegativeOrZero_ReturnsNull(int inputColumn)
    {
        // Act
        var columnLetter = Cell.GetColumnLetter(inputColumn);
        
        // Assert
        columnLetter.Should().BeNull();
    }
    
    [Theory]
    [InlineData(1, 1, "A1")]
    [InlineData(26, 20, "Z20")]
    [InlineData(8, 25, "H25")]
    [InlineData(34, 21, "AH21")]
    [InlineData(42, 99, "AP99")]
    [InlineData(104, 16668, "CZ16668")]
    [InlineData(130, 4, "DZ4")]
    public void GetCellReference_WithValidInputs_ReturnsA1StyleReference(int inputColumn, int inputRow, string? expectedLetter)
    {
        // Act
        var cellReference = Cell.GetCellReference(inputColumn, inputRow);
        
        // Assert
        cellReference.Should().NotBeNullOrEmpty();
        cellReference.Should().Be(expectedLetter);
    }
    
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(0, 0)]
    [InlineData(-27, -4)]
    [InlineData(42, -21)]
    [InlineData(-6, 16668)]
    public void GetCellReference_WithInvalidInputs_ReturnsNull(int inputColumn, int inputRow)
    {
        // Act
        var cellReference = Cell.GetCellReference(inputColumn, inputRow);
        
        // Assert
        cellReference.Should().BeNull();
    }
}