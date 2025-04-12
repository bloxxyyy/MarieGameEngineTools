using YuiGameSystems.DialogSystem.FileLoading.DataFiles;
using YuiGameSystems.DialogSystem.FileLoading.ValidatedDataContainers;

namespace YuiGameSystems.DialogSystem;
public class PlayerChoiceHandler(DialogActions _DialogActions) {
	private readonly DialogActions _DialogActions = _DialogActions;

	public PlayerPrompt MakeResponseChoice(DialogContainer dialog, string[] playerPromptIds) {
		if (_DialogActions.DisplayPlayerPromptChoicesRequested == null)
			throw new Exception("DisplayPlayerPromptChoicesRequested is null");
		if (_DialogActions.PlayerAnswerRequested == null)
			throw new Exception("PlayerAnswerRequested is null");

		string[] playerChoices = GetPlayerChoices(playerPromptIds, dialog);
		if (playerChoices.Length == 0)
			throw new Exception("PlayerPromptIds not found in dialog");

        _DialogActions.DisplayPlayerPromptChoicesRequested.Invoke(playerChoices);
		string playerAnswer = _DialogActions.PlayerAnswerRequested.Invoke();

		List<PlayerPrompt> playerPromptChoices = dialog.Player_prompts.Where(x => playerPromptIds.Contains(x.Id)).ToList();

		return playerPromptChoices.Find(x => x.Text[0] == playerAnswer)
			?? throw new Exception("No PlayerPrompt found for player answer");
	}

	private static string[] GetPlayerChoices(string[] playerPromptIds, DialogContainer dialog) {
        PlayerPrompt[] playerPrompts = dialog.Player_prompts.Where(x => playerPromptIds.Contains(x.Id)).ToArray();
		return playerPrompts.Select(x => x.Text[0]).ToArray();
	}
}
