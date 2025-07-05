namespace YuiGameSystems.DialogSystem.FileLoading.DataFiles;

public record PlayerPrompt(
    string Id,
    List<string> Text,
    List<string> Target_npc_prompt_chance_ids,
    int? Added_Attitude_Towards_target_interlocutor
);
