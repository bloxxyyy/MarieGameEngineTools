using DialogLibrary.App.DialogSystem.Repositories;
using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogTests.Tests.AcquaintanceRepoTests;

public class TryAddAsAcquaintanceTests {
    [Fact]
    public void AddsNewAcquaintance() {
        // Arrange
        Npc toNpc   = new("A", "", null, []);
        Npc findNpc = new("B", "", null, []);

        // Act
        AcquaintanceRepo.TryAddAsAcquaintance(toNpc, findNpc);
        toNpc.Acquaintances ??= [];

        // Assert
        Assert.Single(toNpc.Acquaintances);
        Assert.Equal(findNpc.Id, toNpc.Acquaintances[0].Id);
    }

    [Fact]
    public void DoesNotAddDuplicateAcquaintance() {
        // Arrange
        Npc findNpc = new("B", "", null, []);
        Npc toNpc   = new("A", "", null, [new Acquaintance(findNpc.Id, 0)]);

        // Act
        AcquaintanceRepo.TryAddAsAcquaintance(toNpc, findNpc);
        toNpc.Acquaintances ??= [];

        // Assert
        Assert.Single(toNpc.Acquaintances);
    }
}
