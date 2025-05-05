using DialogLibrary.App.DialogSystem.Datasets;
using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoObjects;
using DialogLibrary.App.DialogSystem.Repositories;

namespace DialogTests.Tests.TraitsRepoTests;

public class GetTraitsByIdsTests {
    private static DatasetManager CreateDatasetManagerWithTraits(Dictionary<string, Trait> traits) {
        DatasetManager datasetManager = new();
        datasetManager.Datasets.Traits = traits;
        return datasetManager;
    }

    [Fact]
    public void ReturnsMatchingTraits() {
        // Arrange
        DatasetManager datasetManager = CreateDatasetManagerWithTraits(new()
        {
            { "t1", new Trait("t1", "t1", [], []) },
            { "t2", new Trait("t2", "t2", [], []) },
            { "t3", new Trait("t3", "t3", [], []) }
        });

        TraitsRepo repo = new(datasetManager);

        // Act
        Trait[] result = repo.GetTraitsByIds(["t1", "t3", "missing"]);

        // Assert
        Assert.Collection(result, // Asserts that the collection contains the exact expected elements in the specified order.
            trait => Assert.Equal("t1", trait.Id),
            trait => Assert.Equal("t3", trait.Id)
        );
    }

    [Theory]
    [InlineData("missing")]
    [InlineData(null)]
    public void ReturnsEmptyArrayWhenNoMatchingTraits(string? data) {
        // Arrange
        string[] ids = data is null ? [] : [data];
        DatasetManager datasetManager = CreateDatasetManagerWithTraits(new()
        {
            { "t1", new Trait("t1", "t1", [], []) },
            { "t2", new Trait("t2", "t2", [], []) }
        });

        TraitsRepo repo = new(datasetManager);

        // Act
        Trait[] result = repo.GetTraitsByIds(ids);

        // Assert
        Assert.Empty(result);
    }
}
