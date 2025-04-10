using DialogLibrary.App.DialogSystem.Datasets.loaders;
using YuiGameSystems.DialogSystem.FileLoading.DataFiles;
using YuiGameSystems.DialogSystem.FileLoading.ValidatedDataContainers;

namespace DialogLibrary.App.DialogSystem.Datasets;

public class DatasetManager
{
    public Datasets Datasets { get; } = new Datasets();

    public void LoadDtoDataIntoMemory(DatasetLocations datasetLocations) {
		LoadEvents(datasetLocations.events);
		LoadTraits(datasetLocations.traits);
		LoadCharacters(datasetLocations.characters);
		Datasets.Dialogs = new JsonDataManager<DialogContainer>().LoadJsonDataIntoDtoMemory(datasetLocations.dialogs);
	}

	private void LoadCharacters(string charactersLocation) {
		var loader = new Loader<CharacterContainer>();
		var characterContainer = loader.Load(charactersLocation, "Characters");

		foreach (var npc in characterContainer.NpcData) {
			Datasets.Characters.Add(npc.Id, npc);
		}
	}

	private void LoadTraits(string traitsLocation) {
		var loader = new Loader<TraitContainer>();
		var traitContainer = loader.Load(traitsLocation, "Traits");

		foreach (var trait in traitContainer.TraitData) {
			Datasets.Traits.Add(trait.Id, trait);
		}
	}

	private void LoadEvents(string eventsLocation) {
		var loader = new Loader<EventContainer>();
		var eventContainer = loader.Load(eventsLocation, "EventData");

		foreach (var eventDto in eventContainer.Events.EventData) {
			Datasets.Events.Add(eventDto.Id, new Event(eventDto.Id));
		}
	}

	public NpcPrompt? GetPromptById(DialogContainer dialog, string initial_prompt_id) {
		return dialog.Npc_prompts.FindAll(x => x.Id == dialog.Initial_prompt_id).FirstOrDefault()
			?? throw new Exception($"Dialog with id {initial_prompt_id} not found");
	}
}
