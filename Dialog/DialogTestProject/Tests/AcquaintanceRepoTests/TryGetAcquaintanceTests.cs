using DialogLibrary.App.DialogSystem.Repositories;
using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogTests.Tests.AcquaintanceRepoTests;

public class TryGetAcquaintanceTests {
    [Fact]
    public void ReturnsAcquaintanceWhenExists() {
        // Arrange
        Npc findNpc = new("B", "", [], []);
        Npc toNpc   = new("A", "", [], [new("C", 0), new (findNpc.Id, 0)]);

        // Act
        Acquaintance? result = AcquaintanceRepo.TryGetAcquaintance(toNpc, findNpc);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(findNpc.Id, result?.Id);
    }

    [Fact]
    public void ReturnsNullWhenAcquaintancesNotInList() {
        // Arrange
        Npc findNpc = new("B", "", [], []);
        Npc toNpc   = new("A", "", [], [new("C", 0)]);

        // Act
        Acquaintance? result = AcquaintanceRepo.TryGetAcquaintance(toNpc, findNpc);

        // Assert
        Assert.Null(result);
    }
}
