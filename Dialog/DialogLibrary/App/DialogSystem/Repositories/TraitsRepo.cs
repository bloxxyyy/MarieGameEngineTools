using DialogLibrary.App.DialogSystem.Datasets;
using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogLibrary.App.DialogSystem.Repositories;
public class TraitsRepo(DatasetManager datasetManager) {
	private readonly DatasetManager _DatasetManager = datasetManager;

	public Trait[] GetTraitsByIds(string[] ids) {
		if (ids == null || ids.Length == 0) return [];

		HashSet<string> idSet          = new HashSet<string>(ids);
		List<Trait>     matchingTraits = [];

		foreach (var trait in _DatasetManager.Datasets.Traits) {
			if (idSet.Contains(trait.Key)) {
				matchingTraits.Add(trait.Value);
			}
		}

		return [.. matchingTraits];
	}
}
