namespace DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoObjects;

public record PromptChance(
    string Id,
    string? Exclusive_to_npc,
    string? Exclusive_to_trait,
    int Exclusive_to_attitude,
    string Npc_prompt_id,
    int Base_chance_percentage,
    string Prompt_type
);
