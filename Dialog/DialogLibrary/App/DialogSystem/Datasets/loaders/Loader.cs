using Newtonsoft.Json;

namespace DialogLibrary.App.DialogSystem.Datasets.loaders;

internal static class Loader<T>
{
	public static T Load(string path, string fileName) {
		string[] files = Directory.GetFiles(path, fileName + ".json");
		if (files.Length == 0)
			throw new Exception($"No file found for {fileName}");
		if (files.Length > 1)
			throw new Exception($"Multiple files found for {fileName}");
		string json = File.ReadAllText(files[0]) ?? throw new Exception($"File {files[0]} is null");

		return JsonConvert.DeserializeObject<T>(json)
			?? throw new Exception($"File {fileName} could not be deserialized");
	}
}