using DialogLibrary.App.Helpers;

namespace DialogTests.Tests.GuardTests;
public class ListOrNullToListTests {
    [Fact]
    public void OnValuesReturnValues() {
        // Arrange
        List<string>? value = ["1", "2"];

        // Act
        List<string> result = Guard.ListOrNullToList(value);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(value.Count, result.Count);
        Assert.Equal(value[0], result[0]);
        Assert.Equal(value[1], result[1]);
    }

    [Fact]
    public void OnEmptyListReturnEmptyList() {
        // Arrange
        List<string> value = [];

        // Act
        List<string> result = Guard.ListOrNullToList(value);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void OnListIsNullReturnEmptyList() {
        // Arrange
        List<string>? value = null;

        // Act
        List<string> result = Guard.ListOrNullToList(value);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
