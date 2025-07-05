using YuiGameSystems.DialogSystem.FileLoading.DataFiles;
using DialogLibrary.App.DialogSystem.Datasets;
using DialogLibrary.App.Helpers;
using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoObjects;

namespace DialogLibrary.App.DialogSystem.ActionManagement;
public class ActionHandler
{
    private readonly Random _Random = new();

    public void DoPossibleAction(Acquaintance acquaintance, NpcPrompt currentNpcPrompt, DatasetManager datasetManager)
    {
        if (Guard.IsNullOrEmpty(currentNpcPrompt.Possible_Actions)) return;

        List<PossibleActionModifier> possibleActions = [];
        int minActionChance = 0;
        int maxActionChance = 0;

        foreach (PossibleAction action in currentNpcPrompt.Possible_Actions)
        {
            if (IsNotHostileEnough(acquaintance, action) && IsNotFriendlyEnough(acquaintance, action))
                continue;

            maxActionChance = minActionChance + action.Chance;
            possibleActions.Add(new PossibleActionModifier
            {
                Action = datasetManager.Datasets.Events[action.Event_Id],
                ChanceFrom = minActionChance,
                ChanceTo = maxActionChance
            });

            minActionChance = maxActionChance + 1;
        }

        int highestChance = possibleActions.Max(x => x.ChanceTo);
        int diceRoll = _Random.Next(0, highestChance);

        PossibleActionModifier actionToFire = possibleActions.Find(x => x.ChanceFrom <= diceRoll && x.ChanceTo >= diceRoll);
        actionToFire.Action.Action?.Invoke();
    }

    private static bool IsNotFriendlyEnough(Acquaintance acquaintance, PossibleAction action)
    {
        if (action.Needed_Attitude_Of_Target_Interlocutor <= 0) return false;
        bool isFriendlyAction = action.Needed_Attitude_Of_Target_Interlocutor > 0;
        bool acquaintanceAttitudeNotHighEnough = action.Needed_Attitude_Of_Target_Interlocutor > acquaintance.Attitude;
        return isFriendlyAction && acquaintanceAttitudeNotHighEnough;
    }

    private static bool IsNotHostileEnough(Acquaintance acquaintance, PossibleAction action)
    {
        if (action.Needed_Attitude_Of_Target_Interlocutor >= 0) return false;
        bool isHostileAction = action.Needed_Attitude_Of_Target_Interlocutor < 0;
        bool acquaintanceAttitudeNotLowEnough = action.Needed_Attitude_Of_Target_Interlocutor < acquaintance.Attitude;
        return isHostileAction && acquaintanceAttitudeNotLowEnough;
    }
}
