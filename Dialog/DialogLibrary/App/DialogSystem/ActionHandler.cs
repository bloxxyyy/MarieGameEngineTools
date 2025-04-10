using YuiGameSystems.DialogSystem.FileLoading.DataFiles;
using DialogLibrary.App.DialogSystem.Datasets;
using DialogLibrary.App.Helpers;

namespace YuiGameSystems.DialogSystem;
public class ActionHandler {

	private readonly Random _Random = new();

	private struct PossibleActionStruct {
		public Event Action;
		public int ChanceFrom;
		public int ChanceTo;
	}

	public void DoPossibleAction(Acquaintance acquaintance, NpcPrompt currentNpcPrompt, DatasetManager datasetManager) {
		if (Guard.IsNullOrEmpty(currentNpcPrompt.Possible_Actions)) return;

		List<PossibleActionStruct> possibleActions = [];
		int minActionChance = 0;
		int maxActionChance = 0;

		foreach (var action in currentNpcPrompt.Possible_Actions) {
			if (isNotHostileEnough(acquaintance, action) && isNotFriendlyEnough(acquaintance, action))
				continue;

			maxActionChance = minActionChance + action.Chance;
			possibleActions.Add(new PossibleActionStruct {
				Action = datasetManager.Datasets.Events[action.Event_Id],
				ChanceFrom = minActionChance,
				ChanceTo = maxActionChance
			});

			minActionChance = maxActionChance + 1;
		}

		var highestChance = possibleActions.Max(x => x.ChanceTo);
		int diceRoll = _Random.Next(0, highestChance);

		var actionToFire = possibleActions.Find(x => x.ChanceFrom <= diceRoll && x.ChanceTo >= diceRoll);
		actionToFire.Action.Action?.Invoke();
	}

	private bool isNotFriendlyEnough(Acquaintance acquaintance, PossibleAction action) {
		if (action.Needed_Attitude_Of_Target_Interlocutor <= 0) return false;
		bool isFriendlyAction = action.Needed_Attitude_Of_Target_Interlocutor > 0;
		bool acquaintanceAttitudeNotHighEnough = action.Needed_Attitude_Of_Target_Interlocutor > acquaintance.Attitude;
		return isFriendlyAction && acquaintanceAttitudeNotHighEnough;
	}

	private bool isNotHostileEnough(Acquaintance acquaintance, PossibleAction action) {
		if (action.Needed_Attitude_Of_Target_Interlocutor >= 0) return false;
		bool isHostileAction = action.Needed_Attitude_Of_Target_Interlocutor < 0;
		bool acquaintanceAttitudeNotLowEnough = action.Needed_Attitude_Of_Target_Interlocutor < acquaintance.Attitude;
		return isHostileAction && acquaintanceAttitudeNotLowEnough;
	}
}
