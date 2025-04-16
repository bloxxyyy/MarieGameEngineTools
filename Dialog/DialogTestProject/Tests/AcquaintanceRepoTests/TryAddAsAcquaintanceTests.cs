using DialogLibrary.App.DialogSystem.Repositories;
using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogTests.Tests.AcquaintanceRepoTests;

public class TryAddAsAcquaintanceTests {
    [Fact]
    public void AddsNewAcquaintance() {
        // Arrange
        Npc toNpc   = new("A", "", [], []);
        Npc findNpc = new("B", "", [], []);

        // Act
        AcquaintanceRepo.TryAddAsAcquaintance(toNpc, findNpc);
        toNpc.Acquaintances ??= [];

        // Assert
        Assert.Single(toNpc.Acquaintances);
    }

    [Fact]
    public void DoesNotAddDuplicateAcquaintance() {
        // Arrange
        Npc toNpc   = new("A", "", [], []);
        Npc findNpc = new("B", "", [], []);
        AcquaintanceRepo.TryAddAsAcquaintance(toNpc, findNpc);

        // Act
        AcquaintanceRepo.TryAddAsAcquaintance(toNpc, findNpc);
        toNpc.Acquaintances ??= [];

        // Assert
        Assert.Single(toNpc.Acquaintances);
    }
}
