namespace YuiGameSystems.DialogSystem;
public class DialogActions
{
    public Action?			 OnEndConversation					 { get; set; }
    public Action?			 OnStartConversation				 { get; set; }
	public Action<string>?   DisplayPrompt			 { get; set; }
	public Action<string[]>? DisplayPlayerPromptChoicesRequested { get; set; }
	public Func<string>?	 PlayerAnswerRequested				 { get; set; }

	public DialogActions(
		Action? onEndConversation,
		Action? onStartConversation,
		Action<string>? displayNpcPromptRequested,
		Action<string[]>? displayPlayerPromptChoicesRequested,
		Func<string>? playerAnswerRequested
	) {
		OnEndConversation					 = onEndConversation;
		OnStartConversation					 = onStartConversation;
		DisplayPrompt			 = displayNpcPromptRequested;
		DisplayPlayerPromptChoicesRequested  = displayPlayerPromptChoicesRequested;
		PlayerAnswerRequested				 = playerAnswerRequested;
	}
}
