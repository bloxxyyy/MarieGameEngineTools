using DialogLibrary.App.DialogSystem.Datasets.loaders;
using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoContainers;
using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoObjects;

using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

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
        CharacterContainer characterContainer = Loader<CharacterContainer>.Load(charactersLocation, "Characters");

		foreach (Npc npc in characterContainer.NpcData) {
			Datasets.Characters.Add(npc.Id, npc);
		}
	}

	private void LoadTraits(string traitsLocation) {
		TraitContainer traitContainer = Loader<TraitContainer>.Load(traitsLocation, "Traits");

		foreach (Trait trait in traitContainer.TraitData) {
			Datasets.Traits.Add(trait.Id, trait);
		}
	}

	private void LoadEvents(string eventsLocation) {
		EventContainer eventContainer = Loader<EventContainer>.Load(eventsLocation, "EventData");

		foreach (Event eventDto in eventContainer.EventData) {
			Datasets.Events.Add(eventDto.Id, new Event(eventDto.Id));
		}
	}
}
