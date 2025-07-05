using MarieBaseLibrary.Database.DialogDB.Db;
using MongoDB.Entities;

if (args.Length == 0 || !args[0].Equals("migrate", StringComparison.OrdinalIgnoreCase))
    Console.WriteLine("No valid arguments provided. Use 'migrate' to run migrations.");

_ = new DialogDatabase();
await DB.MigrateAsync();
