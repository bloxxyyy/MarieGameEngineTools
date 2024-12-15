using MarieBaseLibrary.Database.DialogDB.Entities;
using MongoDB.Entities;

public class _001_add_dialog : IMigration
{
    public async Task UpgradeAsync()
    {
        var dialog = new Dialog { Title = "The Power Of Now" };
        await dialog.SaveAsync();
    }
}