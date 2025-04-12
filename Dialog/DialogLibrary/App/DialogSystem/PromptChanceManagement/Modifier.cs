namespace DialogLibrary.App.DialogSystem.PromptChanceManagement;
public struct Modifier(string promptId, int startModifier, int endModifier) {
    public string NpcPromptId   = promptId;
    public int    StartModifier = startModifier;
    public int    EndModifier   = endModifier;
}
