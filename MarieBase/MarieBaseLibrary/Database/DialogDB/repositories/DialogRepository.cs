using MarieBaseLibrary.Database.DialogDB.Entities;
using MongoDB.Entities;

namespace MarieBaseLibrary.Database.DialogDB.repositories;
public class DialogRepository : IDialogRepository
{
    public Task SaveDialog(Dialog dialog)
    {
        return dialog.SaveAsync();
    }

    public Task<List<Dialog>> GetAll()
    {
        return DB.Find<Dialog>()
                 .Match(_ => true)
                 .ExecuteAsync();
    }

    public Task<Dialog> GetByTitle(string title) =>
        DB.Find<Dialog>()
          .Match(b => b.Title == title)
          .ExecuteSingleAsync();
}