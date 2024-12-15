using MarieBaseLibrary.Database.DialogDB.repositories;

namespace MarieBaseTestProject;

internal class App(IDialogRepository dialogRepository)
{
    private IDialogRepository _dialogRepository = dialogRepository;

    internal void Run()
    {
        var x = _dialogRepository.GetAll().GetAwaiter().GetResult();
        var y = 1;
    }
}