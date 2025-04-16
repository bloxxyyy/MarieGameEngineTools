using DialogLibrary.App.Helpers;

namespace DialogTests.Tests.GuardTests;

public class ListOrNullToArrayTests {
    [Fact]
    public void OnEmptyListReturnEmptyArray() {
        // Arrange
        List<string> value = [];

        // Act
        string[] result = Guard.ListOrNullToArray(value);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void OnListIsNullReturnEmptyArray() {
        // Arrange
        List<string>? value = null;

        // Act
        string[] result = Guard.ListOrNullToArray(value);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void OnFilledListChangeIntoFilledArray() {
        // Arrange
        List<string> value = ["1", "", "3"];

        // Act
        string[] result = Guard.ListOrNullToArray(value);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(value.Count, result.Length);
        Assert.Equal(value[0], result[0]);
        Assert.Equal(value[1], result[1]);
    }
}
