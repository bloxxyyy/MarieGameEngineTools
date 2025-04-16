using DialogLibrary.App.Helpers;

namespace DialogTests.Tests.GuardTests;

public class IsNullOrEmptyTests {
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void IsNullOrEmptyReturnsTrueOnNullOrEmpty(string? nullOrEmptyString) {
        // Act
        bool result = Guard.IsNullOrEmpty(nullOrEmptyString);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNullOrEmptyReturnsFalseValue() {
        // Arrange
        const string value = "SomeExampleValue";

        // Act
        bool result = Guard.IsNullOrEmpty(value);

        // Assert
        Assert.False(result);
    }
}
