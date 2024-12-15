using YuiLibrary.App.Records;

namespace YuiLibrary.App.Logic;
public class DialogManager
{
    internal readonly DialogFileInfo _DialogFileInfo;

    public DialogManager(DialogFileInfo dialogFileInfo)
    {
        _DialogFileInfo = dialogFileInfo;
    }
}
