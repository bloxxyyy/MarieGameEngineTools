using Newtonsoft.Json;

namespace DialogLibrary.App.DialogSystem.Datasets.loaders;

internal class Loader<T>
{
	public T Load(string path, string fileName) {
		var files = Directory.GetFiles(path, fileName + ".json");
		if (files.Length == 0)
			throw new Exception($"No file found for {fileName}");
		if (files.Length > 1)
			throw new Exception($"Multiple files found for {fileName}");
		string json = File.ReadAllText(files[0]) ?? throw new Exception($"File {files[0]} is null");

		return JsonConvert.DeserializeObject<T>(json)
			?? throw new Exception($"File {fileName} could not be deserialized");
	}
}