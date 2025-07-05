using DialogLibrary.App.DialogSystem.Datasets;
using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoObjects;
using DialogLibrary.App.Helpers;

namespace DialogLibrary.App.DialogSystem.Repositories;
public class TraitsRepo(DatasetManager datasetManager) {
	private readonly DatasetManager _DatasetManager = datasetManager;

	public Trait[] GetTraitsByIds(string[] ids) {
        if (Guard.IsNullOrEmpty(ids)) return [];

		HashSet<string> idSet    = [.. ids];
		List<Trait>     matching = [];

		foreach (KeyValuePair<string, Trait> trait in _DatasetManager.Datasets.Traits) {
			if (idSet.Contains(trait.Key)) {
				matching.Add(trait.Value);
			}
		}

		return [.. matching];
	}
}
