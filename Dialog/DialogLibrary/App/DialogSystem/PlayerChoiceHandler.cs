using YuiGameSystems.DialogSystem.FileLoading.DataFiles;
using DialogLibrary.App.DialogSystem.Repositories;
using YuiGameSystems.DialogSystem.FileLoading.ValidatedDataContainers;

namespace YuiGameSystems.DialogSystem;
public class PlayerChoiceHandler(DialogActions _DialogActions) {

	public Action<PromptChance[]> DoNextDialog { get; set; }

	private DialogContainer _Dialog;
	private Npc _DialogNpc;
	private Npc _interNpc;

	public PlayerPrompt? MakeResponseChoice(DialogContainer dialog, Npc dialogNpc, Npc interNpc, string[] playerPromptIds) {
		_Dialog = dialog;
		_DialogNpc = dialogNpc;
		_interNpc = interNpc;

		if (_DialogActions.DisplayPlayerPromptChoicesRequested == null)
			throw new Exception("DisplayPlayerPromptChoicesRequested is null");
		if (_DialogActions.PlayerAnswerRequested == null)
			throw new Exception("PlayerAnswerRequested is null");

		string[]? playerChoices = GetPlayerChoices(playerPromptIds);
		if (playerChoices is null) return null;

		_DialogActions.DisplayPlayerPromptChoicesRequested.Invoke(playerChoices);
		string playerAnswer = _DialogActions.PlayerAnswerRequested.Invoke();

		var playerPromptChoices = _Dialog.Player_prompts.Where(x => playerPromptIds.Contains(x.Id)).ToList();

		return playerPromptChoices.Find(x => x.Text[0] == playerAnswer)
			?? throw new Exception("No PlayerPrompt found for player answer");
	}

	private string[]? GetPlayerChoices(string[] playerPromptIds) {
		var playerPrompts = _Dialog.Player_prompts.Where(x => playerPromptIds.Contains(x.Id)).ToArray();
		return playerPrompts.Select(x => x.Text[0]).ToArray();
	}
}
