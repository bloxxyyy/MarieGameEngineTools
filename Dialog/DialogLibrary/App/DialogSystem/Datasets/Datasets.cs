using YuiGameSystems.DialogSystem.FileLoading.DataFiles;
using YuiGameSystems.DialogSystem.FileLoading.ValidatedDataContainers;

namespace DialogLibrary.App.DialogSystem.Datasets;

public sealed class Datasets
{
	public Dictionary<string, DialogContainer> Dialogs { get; set; } = new();
    public Dictionary<string, Npc> Characters { get; set; } = new();
	public Dictionary<string, Trait> Traits { get; set; } = new();
	public Dictionary<string, Event> Events { get; set; } = new();
}