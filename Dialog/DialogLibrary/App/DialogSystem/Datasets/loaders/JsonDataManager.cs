using DialogLibrary.App.Helpers;
using Newtonsoft.Json;

namespace DialogLibrary.App.DialogSystem.Datasets.loaders;

public class JsonDataManager<CONTAINER>
{
    public Dictionary<string, CONTAINER> LoadJsonDataIntoDtoMemory(string fileLocation)
    {
        Guard.NotNullOrEmpty(fileLocation);

        Dictionary<string, CONTAINER> fileNameContainerPairs = [];

        foreach (string file in Directory.GetFiles(fileLocation, "*.json", SearchOption.AllDirectories))
        {
            string json = File.ReadAllText(file) ?? throw new Exception($"File {file} is null");
            CONTAINER container = JsonConvert.DeserializeObject<CONTAINER>(json) ?? throw new Exception($"File {file} could not be deserialized");

            string fileName = Path.GetFileNameWithoutExtension(file) ?? throw new Exception($"File {file} has no name");

            if (fileNameContainerPairs.ContainsKey(fileName))
            {
                throw new Exception($"Duplicate dialog file name {fileName}");
            }

            fileNameContainerPairs.Add(fileName, container);
        }

        return fileNameContainerPairs;
    }
}
