using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogLibrary.App.DialogSystem.Repositories;
public static class PromptRepo {
    public static NpcPrompt? GetPromptById(List<NpcPrompt> npcPrompts, string promptId) {
        return npcPrompts.FindAll(x => x.Id == promptId).FirstOrDefault();
    }
}
