using Newtonsoft.Json;

// KokoScript

public class FileUpdater
{
    public static string UpdateData(string path, string value)
    {
        if (!File.Exists(path))          return $"Error: File not found at '{path}'";
        if (string.IsNullOrEmpty(value)) return "Error: Value to append is null or empty";

        try
        {
            string fileContent    = File.ReadAllText(path);
            string updatedContent = fileContent + value;
            var    jsonData       = new { FilePath = path, UpdatedContent = updatedContent };
            return JsonConvert.SerializeObject(jsonData);
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }
}