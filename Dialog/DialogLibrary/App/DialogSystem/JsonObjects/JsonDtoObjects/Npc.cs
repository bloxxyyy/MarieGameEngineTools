using DialogLibrary.App.DialogSystem.TraitManagement;

namespace YuiGameSystems.DialogSystem.FileLoading.DataFiles;

public class Npc(string id, string name, List<string>? traitIds, List<Acquaintance>? acquaintances) {
	public string              Id            = id;
	public string              Name          = name;
	public List<string>?       Trait_Ids     = traitIds;
	public List<Acquaintance>? Acquaintances = acquaintances;
}
