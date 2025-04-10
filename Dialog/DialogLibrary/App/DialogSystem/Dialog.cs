using DialogLibrary.App.DialogSystem;
using DialogLibrary.App.DialogSystem.Datasets;
using DialogLibrary.App.DialogSystem.Repositories;
using DialogLibrary.App.Helpers;
using YuiGameSystems.DialogSystem.FileLoading.DataFiles;
using YuiGameSystems.DialogSystem.FileLoading.ValidatedDataContainers;

namespace YuiGameSystems.DialogSystem;
public class Dialog
{
    private DatasetManager _DatasetManager { get; }

    private Npc                   _DialogNpc;
    private Npc                   _interNpc;
	private Npc					  _CurrentSpeaking;

	private bool                  _IsPlayerConversation;

	private readonly Random _Random = new();

	private ChoiceModifyingHelper _ChoiceModifyingHelper;
	private PlayerChoiceHandler   _PlayerChoiceHandler;
	private PromptChanceManager   _PromptChanceManager;
	private DialogActions         _DialogActions;
	private TraitsRepo		      _TraitsRepo;

	private ActionHandler	 _ActionHandler    = new();
	private AcquaintanceRepo _AcquaintanceRepo = new();

    private DialogContainer _Dialog;
    private NpcPrompt		_CurrentNpcPrompt;

	public Dialog(DatasetManager datasetManager, string dialogId, Npc targetNpc, DialogActions dialogActions) {
		_CurrentSpeaking = targetNpc;
		_DialogActions = dialogActions;
		_DatasetManager = datasetManager;
		_DatasetManager.Datasets.Dialogs.TryGetValue(dialogId, out DialogContainer? dialog);
		_Dialog = dialog;
		_DialogNpc = targetNpc;
		_CurrentNpcPrompt = _DatasetManager.GetPromptById(_Dialog, _Dialog.Initial_prompt_id)
			?? throw new Exception($"Dialog with id {dialogId} not found");
	}

	public void InitializeConversationWith(Npc interlocutor, bool isPlayerConversation) {
		_IsPlayerConversation = isPlayerConversation;
		_interNpc = interlocutor;
		_TraitsRepo = new TraitsRepo(_DatasetManager);
		_PromptChanceManager = new PromptChanceManager(_TraitsRepo, _Dialog);

		if (_IsPlayerConversation) {
			_PlayerChoiceHandler = new PlayerChoiceHandler(_DialogActions);
		}
	}

	public void StartConversation()
    {
		_DialogActions.OnStartConversation?.Invoke();

		_ChoiceModifyingHelper = new ChoiceModifyingHelper(_DialogNpc, _interNpc, _DatasetManager, _Dialog);
		_ChoiceModifyingHelper.Initialize(_IsPlayerConversation);

		_AcquaintanceRepo.AddAsAcquaintances(_DialogNpc, _interNpc);
        RunDialog();
    }

	private void OnDialog(List<string> toSay) {
		_DialogActions.DisplayPrompt?.Invoke(toSay[_Random.Next(0, toSay.Count)]);

		var Acquaintance = _AcquaintanceRepo.GetAcquaintanceOrNullIfTalkingToPlayer(
			_CurrentSpeaking.Id, _DialogNpc, _interNpc, _IsPlayerConversation
		);

		if (Acquaintance != null) { // meaning the dialogNpc made a remark, but a player does not alter attitude
			Acquaintance.Attitude += _CurrentNpcPrompt.Added_Attitude_Towards_target_interlocutor ?? 0;
			_ActionHandler.DoPossibleAction(Acquaintance, _CurrentNpcPrompt, _DatasetManager);
		}

		_CurrentSpeaking = (_CurrentSpeaking.Id == _interNpc.Id) ? _DialogNpc : _interNpc;
	}

	private void RunDialog() {
		OnDialog(_CurrentNpcPrompt.Text);

		// npcToNpc
		var allPromptChanceIds = _CurrentNpcPrompt.Target_npc_prompt_chance_ids;

		if (_IsPlayerConversation) {
			var playerPromptIds = Guard.ListOrNullToArray(_CurrentNpcPrompt.Player_prompt_ids);
			if (playerPromptIds.Length == 0) return;
			var playerPrompt = _PlayerChoiceHandler.MakeResponseChoice(
				_Dialog, _DialogNpc, _interNpc, playerPromptIds
			);

			OnDialog(playerPrompt.Text);
			allPromptChanceIds = playerPrompt.Target_npc_prompt_chance_ids;
		}

		if (_PromptChanceManager.HasPromptChances(allPromptChanceIds, _CurrentSpeaking, out PromptChance[] possiblePromptChances)) {
			_CurrentNpcPrompt = _ChoiceModifyingHelper.GetAConnectedPromptByChanceModifier(possiblePromptChances, _CurrentSpeaking.Id);
			RunDialog();
			return;
		}
		
		_DialogActions.OnEndConversation?.Invoke();
	}
}
