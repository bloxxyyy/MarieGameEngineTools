using Xunit;
using YuiLibrary.App.Logic;
using YuiLibrary.App.Records;

namespace YuiTestProject.DialogTests;

/// <summary>
/// This is the example test class for this project. Made to test internal code testing.
/// </summary>
public class DialogTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDialogFileInfo()
    {
        var fileInfo = new DialogFileInfo("C:\\Dialogs", "example.json");

        var dialog = new DialogManager(fileInfo);

        Assert.Equal("C:\\Dialogs", dialog._DialogFileInfo.DialogFilePath);
        Assert.Equal("example.json", dialog._DialogFileInfo.DialogFileName);
    }
}
