using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace YuiGameSystems.DialogSystem.FileLoading.ValidatedDataContainers;

public class DialogContainer()
{
	public string             Initial_prompt_id  = "";
	public List<NpcPrompt>    Npc_prompts        = [];
	public List<PlayerPrompt> Player_prompts     = [];
	public List<PromptChance> Npc_prompt_chances = [];
}
