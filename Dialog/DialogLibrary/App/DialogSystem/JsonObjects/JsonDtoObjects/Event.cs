namespace YuiGameSystems.DialogSystem.FileLoading.DataFiles;

public class Event(string id) {
	public string Id = id;
	public Action? Action { get; set; }
}
