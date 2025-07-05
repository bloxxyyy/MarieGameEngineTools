using MongoDB.Driver;
using MongoDB.Entities;

namespace MarieBaseLibrary.Database.DialogDB.Db;
public class DialogDatabase
{
    public DialogDatabase()
    {
        Task.Run(async () =>
        {
            string? mongoPort = Environment.GetEnvironmentVariable("MARIE_MONGO_PORT", EnvironmentVariableTarget.User);
            string? mongoUser = Environment.GetEnvironmentVariable("MARIE_MONGO_USER", EnvironmentVariableTarget.User);
            string? mongoPassword = Environment.GetEnvironmentVariable("MARIE_MONGO_PASSWORD", EnvironmentVariableTarget.User);
            string? mongoHost = Environment.GetEnvironmentVariable("MARIE_MONGO_HOST", EnvironmentVariableTarget.User);

            if (mongoPort == null || mongoUser == null || mongoPassword == null || mongoHost == null)
                throw new Exception("Missing environment variables for MongoDB connection");

            var connectionString = $"mongodb://{mongoUser}:{mongoPassword}@{mongoHost}:{mongoPort}/?authSource=admin";
            await DB.InitAsync("DialogDB", MongoClientSettings.FromConnectionString(connectionString));
        }).GetAwaiter().GetResult();
    }
}
