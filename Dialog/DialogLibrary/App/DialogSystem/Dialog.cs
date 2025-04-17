using DialogLibrary.App.DialogSystem.ActionManagement;
using DialogLibrary.App.DialogSystem.Datasets;
using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoContainers;
using DialogLibrary.App.DialogSystem.PromptChanceManagement;
using DialogLibrary.App.DialogSystem.Repositories;
using DialogLibrary.App.DialogSystem.TraitManagement;
using DialogLibrary.App.Helpers;

using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace YuiGameSystems.DialogSystem;
public class Dialog
{
    private readonly Random               _Random        = new();
    private readonly ActionHandler        _ActionHandler = new();
    private readonly Npc                  _DialogNpc;
    private readonly Npc                  _interNpc;
    private readonly DialogContainer      _Dialog;
    private readonly DialogActions        _DialogActions;
    private readonly PromptChanceManager  _PromptChanceManager;
    private readonly DatasetManager       _DatasetManager;
    private readonly PlayerChoiceHandler? _PlayerChoiceHandler;
    private readonly bool                 _IsPlayerConversation;

    private NpcPrompt _CurrentNpcPrompt;
    private Npc       _CurrentSpeaking;

    public Dialog(DatasetManager datasetManager, string dialogId, Npc targetNpc, Npc interlocutor, bool isPlayerConversation, DialogActions dialogActions) {
		_CurrentSpeaking = targetNpc;
		_DialogActions = dialogActions;
		_DatasetManager = datasetManager;
		_DatasetManager.Datasets.Dialogs.TryGetValue(dialogId, out DialogContainer? dialog);
		if (dialog == null) throw new Exception($"Dialog with id {dialogId} not found");
		_Dialog = dialog;
		_DialogNpc = targetNpc;

        _CurrentNpcPrompt = PromptRepo.GetPromptById(_Dialog.Npc_prompts, _Dialog.Initial_prompt_id)
			?? throw new Exception($"Dialog with id {dialogId} not found");

        _IsPlayerConversation = isPlayerConversation;
        _interNpc = interlocutor;
        TraitsRepo traitsRepo = new(_DatasetManager);

        TraitInformationManager traitInformationManager = new(_IsPlayerConversation, traitsRepo);
		TraitInformation traitInformation = traitInformationManager.GetTraitInformation(_DialogNpc, _interNpc);

        _PromptChanceManager = new PromptChanceManager(_DialogNpc, _interNpc, traitsRepo, _Dialog, traitInformation);

        if (_IsPlayerConversation) {
            _PlayerChoiceHandler = new PlayerChoiceHandler(_DialogActions);
        }

		if (!AcquaintanceRepo.DoesNpcHaveAcquaintance(_DialogNpc, _interNpc)) {
            AcquaintanceRepo.TryAddAsAcquaintance(_DialogNpc, _interNpc);
        }

		if (!AcquaintanceRepo.DoesNpcHaveAcquaintance(_interNpc, _DialogNpc)) {
			AcquaintanceRepo.TryAddAsAcquaintance(_interNpc, _DialogNpc);
		}

        _DialogActions.OnStartConversation?.Invoke();
        RunDialog();
    }

	private void OnDialog(List<string> toSay) {
        bool CurrentlySpeakingIsDialogNpc() => _CurrentSpeaking.Id == _DialogNpc.Id;
		bool CurrentlySpeakingIsInterlocutor() => _CurrentSpeaking.Id == _interNpc.Id;

        _DialogActions.DisplayPrompt?.Invoke(toSay[_Random.Next(0, toSay.Count)]);

		Acquaintance? Acquaintance = null;
        if (CurrentlySpeakingIsDialogNpc() && !_IsPlayerConversation) {
            Acquaintance = AcquaintanceRepo.TryGetAcquaintance(_DialogNpc, _interNpc);
        }

		if (CurrentlySpeakingIsInterlocutor()) {
			Acquaintance = AcquaintanceRepo.TryGetAcquaintance(_interNpc, _DialogNpc);
		}

		if (Acquaintance != null) { // meaning the dialogNpc made a remark, but a player does not alter attitude
			Acquaintance.Attitude += _CurrentNpcPrompt.Added_Attitude_Towards_target_interlocutor ?? 0;
			_ActionHandler.DoPossibleAction(Acquaintance, _CurrentNpcPrompt, _DatasetManager);
		}

		_CurrentSpeaking = (_CurrentSpeaking.Id == _interNpc.Id) ? _DialogNpc : _interNpc;
	}

    private void RunDialog() {
		OnDialog(_CurrentNpcPrompt.Text);

		// npcToNpc
		List<string> promptChanceIds = _CurrentNpcPrompt.Target_npc_prompt_chance_ids;

		if (_IsPlayerConversation) {
			string[] playerPromptIds = Guard.ListOrNullToArray(_CurrentNpcPrompt.Player_prompt_ids);
			if (playerPromptIds.Length == 0) return;
			if (_PlayerChoiceHandler == null) throw new Exception("PlayerChoiceHandler is null");
			PlayerPrompt playerPrompt = _PlayerChoiceHandler.MakeResponseChoice(_Dialog, playerPromptIds);

			OnDialog(playerPrompt.Text);
			promptChanceIds = playerPrompt.Target_npc_prompt_chance_ids;
		}

		if (_PromptChanceManager.TryGetPromptByPromptChances(promptChanceIds, _CurrentSpeaking, out NpcPrompt? npcPrompt)) {
			if (npcPrompt == null) throw new Exception("PromptChanceManager returned null");
			_CurrentNpcPrompt = npcPrompt;
			RunDialog();
			return;
		}

		_DialogActions.OnEndConversation?.Invoke();
	}
}
