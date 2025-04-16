using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoContainers;
using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoObjects;

using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogLibrary.App.DialogSystem.Datasets;

public sealed class Datasets
{
	public Dictionary<string, DialogContainer> Dialogs { get; set; } = [];
    public Dictionary<string, Npc> Characters { get; set; } = [];
	public Dictionary<string, Trait> Traits { get; set; } = [];
	public Dictionary<string, Event> Events { get; set; } = [];
}