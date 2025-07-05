using MongoDB.Entities;

namespace MarieBaseLibrary.Database.DialogDB.Entities;

public class Dialog : Entity
{
    public string Title { get; set; }

    static Dialog()
    {
        DB.Index<Dialog>()
          .Key(b => b.Title, KeyType.Ascending)
          .CreateAsync();
    }
}
