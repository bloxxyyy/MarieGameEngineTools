using DialogLibrary.App.Helpers;

namespace DialogTests.Tests.GuardTests;

public class NotNullOrEmptyTests {
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void OnNullOrEmptyValueThrowArgumentException(string? value) {
        // Act
        Exception? exception = Record.Exception(() => Guard.NotNullOrEmpty(value));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void OnNotNullOrEmptyValueNoExceptionIsThrown() {
        // Arrange
        const string value = "myExampleValue";

        // Act
        Exception? exception = Record.Exception(() => Guard.NotNullOrEmpty(value));

        // Assert
        Assert.Null(exception);
    }
}
