namespace DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoObjects;

public record PossibleAction(
    int Needed_Attitude_Of_Target_Interlocutor,
    int Chance,
    string Event_Id
);