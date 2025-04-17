using DialogLibrary.App.DialogSystem.Repositories;
using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogTests.Tests.AcquaintanceRepoTests;

public class DoesHaveAcquaintanceTests {
    [Fact]
    public void ReturnsTrueWhenExists() {
        // Arrange
        Npc findNpc = new("B", "", null, null);
        Npc toNpc   = new("A", "", null, [new (findNpc.Id, 0)]);

        // Act
        bool result = AcquaintanceRepo.DoesNpcHaveAcquaintance(toNpc, findNpc);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ReturnsFalseWhenNotNotAsAcquaintance() {
        // Arrange
        Npc findNpc = new("B", "", null, null);
        Npc toNpc   = new("A", "", null, [new("C", 0)]);

        // Act
        bool result = AcquaintanceRepo.DoesNpcHaveAcquaintance(toNpc, findNpc);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ReturnsFalseWhenAcquaintancesNull() {
        // Arrange
        Npc findNpc = new("B", "", null, null);
        Npc toNpc   = new("A", "", null, null);

        // Act
        bool result = AcquaintanceRepo.DoesNpcHaveAcquaintance(toNpc, findNpc);

        // Assert
        Assert.False(result);
    }
}
