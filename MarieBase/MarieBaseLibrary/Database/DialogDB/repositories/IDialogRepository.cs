using MarieBaseLibrary.Database.DialogDB.Entities;

namespace MarieBaseLibrary.Database.DialogDB.repositories;

public interface IDialogRepository
{
    public Task SaveDialog(Dialog bookToSave);
    public Task<Dialog> GetByTitle(string title);
    public Task<List<Dialog>> GetAll();
}
