using DialogLibrary.App.DialogSystem.Datasets;
using DialogLibrary.App.Helpers;

using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogLibrary.App.DialogSystem.Repositories;
public class TraitsRepo(DatasetManager datasetManager) {
	private readonly DatasetManager _DatasetManager = datasetManager;

	public Trait[] GetTraitsByIds(string[] ids) {
        if (Guard.IsNullOrEmpty(ids)) return [];

		HashSet<string> idSet    = new(ids);
		List<Trait>     matching = [];

		foreach (KeyValuePair<string, Trait> trait in _DatasetManager.Datasets.Traits) {
			if (idSet.Contains(trait.Key)) {
				matching.Add(trait.Value);
			}
		}

		return [.. matching];
	}
}
