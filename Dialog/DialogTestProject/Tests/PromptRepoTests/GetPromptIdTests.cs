using DialogLibrary.App.DialogSystem.Repositories;

using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogTests.Tests.PromptRepoTests;

public class GetPromptIdTests {
    [Fact]
    public void ReturnsPromptWhenPromptIdFound() {
        // Arrange
        const string promptId = "SomeIdToTest";
        NpcPrompt findPrompt = new(promptId, [], [], [], null, []);

        // Act
        NpcPrompt? result = PromptRepo.GetPromptById([findPrompt], promptId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(promptId, result?.Id);
    }

    [Fact]
    public void ReturnsNullWhenPromptIdNotFoundButOtherPromptExist() {
        // Arrange
        const string existingId = "SomeIdToTest";
        const string tryFindId  = "NonExistentId";
        NpcPrompt findPrompt = new(existingId, [], [], [], null, []);

        // Act
        NpcPrompt? result = PromptRepo.GetPromptById([findPrompt], tryFindId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ReturnsNullWhenNoPromptsExist() {
        // Arrange
        const string tryFindId = "NonExistentId";

        // Act
        NpcPrompt? result = PromptRepo.GetPromptById([], tryFindId);

        // Assert
        Assert.Null(result);
    }
}
