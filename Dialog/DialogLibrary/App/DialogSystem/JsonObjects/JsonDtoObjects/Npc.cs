namespace YuiGameSystems.DialogSystem.FileLoading.DataFiles;

public class Npc {
    public string Id;
	public string Name;
	public List<string>? Trait_Ids;
	public List<Acquaintance>? Acquaintances;

	public Npc(string id, string name, List<string>? traitIds, List<Acquaintance>? acquaintances) {
		Id = id;
		Name = name;
		Trait_Ids = traitIds;
		Acquaintances = acquaintances;
	}
}
