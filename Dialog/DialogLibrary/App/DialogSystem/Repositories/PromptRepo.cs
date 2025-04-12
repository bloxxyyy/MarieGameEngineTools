using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogLibrary.App.DialogSystem.Repositories;
public class PromptRepo() {
    public static NpcPrompt? GetPromptById(List<NpcPrompt> npcPrompts, string initial_prompt_id) {
        return npcPrompts.FindAll(x => x.Id == initial_prompt_id).FirstOrDefault();
    }
}
