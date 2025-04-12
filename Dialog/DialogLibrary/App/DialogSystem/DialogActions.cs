namespace YuiGameSystems.DialogSystem;
public class DialogActions(
    Action?           onEndConversation,
    Action?           onStartConversation,
    Action<string>?   displayNpcPromptRequested,
    Action<string[]>? displayPlayerPromptChoicesRequested,
    Func<string>?     playerAnswerRequested
) {
    public Action? OnEndConversation                             { get; set; } = onEndConversation;
    public Action? OnStartConversation                           { get; set; } = onStartConversation;
    public Action<string>? DisplayPrompt                         { get; set; } = displayNpcPromptRequested;
    public Action<string[]>? DisplayPlayerPromptChoicesRequested { get; set; } = displayPlayerPromptChoicesRequested;
    public Func<string>? PlayerAnswerRequested                   { get; set; } = playerAnswerRequested;
}
