using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoObjects;

namespace YuiGameSystems.DialogSystem.FileLoading.DataFiles;

public record NpcPrompt(
    string Id,
    List<string> Text,
    List<string> Player_prompt_ids,
    List<string> Target_npc_prompt_chance_ids,
    int? Added_Attitude_Towards_target_interlocutor,
    List<PossibleAction> Possible_Actions
);
