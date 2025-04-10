using DialogLibrary.App.DialogSystem.Repositories;
using DialogLibrary.App.Helpers;
using YuiGameSystems.DialogSystem;
using YuiGameSystems.DialogSystem.FileLoading.DataFiles;
using YuiGameSystems.DialogSystem.FileLoading.ValidatedDataContainers;

namespace DialogLibrary.App.DialogSystem;
public class PromptChanceManager(TraitsRepo traitsRepo, DialogContainer _Dialog) {

	private readonly TraitsRepo      _TraitsRepo = traitsRepo;
	private readonly DialogContainer _Dialog     = _Dialog;

	public bool HasPromptChances(List<string> prompChanceIds, Npc currentSpeakingNpc, out PromptChance[] possiblePromptChances) {
		possiblePromptChances = GetPossiblePromptChances(prompChanceIds, currentSpeakingNpc);
		return !Guard.IsNullOrEmpty(possiblePromptChances);
	}

	private PromptChance[] GetPossiblePromptChances(List<string> target_npc_prompt_chance_ids, Npc _CurrentSpeakingNpc) {
		string[] promptChanceIds = Guard.ListOrNullToArray(target_npc_prompt_chance_ids);
		PromptChance[] promptChances = GetPromptChancesByIds(promptChanceIds);
		return GetUseablePromptChances(promptChances, _CurrentSpeakingNpc);
	}

	private PromptChance[] GetUseablePromptChances(PromptChance[] npcPromptChances, Npc npc) {
		if (Guard.IsNullOrEmpty(npcPromptChances)) return [];

		string[] traitIds = Guard.ListOrNullToArray(npc.Trait_Ids);

		List<PromptChance> usablePrompts = [];
		foreach (var prompt in npcPromptChances) {
			bool isInclusiveToNpc       = IsInclusiveToNpc(           prompt.Exclusive_to_npc   ?? "", npc.Id);
			bool isInclusiveForTrait    = IsInclusiveForAnyOfTraitIds(prompt.Exclusive_to_trait ?? "", traitIds);

			if (isInclusiveToNpc && isInclusiveForTrait)
				usablePrompts.Add(prompt);
		}

		return [.. usablePrompts];
	}

	private PromptChance[] GetPromptChancesByIds(string[] promptChanceIds) {
		HashSet<string> idSet = new HashSet<string>(promptChanceIds);
		List<PromptChance> matchingPrompts = [];

		foreach (PromptChance prompt in _Dialog.Npc_prompt_chances) {
			if (idSet.Contains(prompt.Id))
				matchingPrompts.Add(prompt);
		}

		return matchingPrompts.ToArray();
	}

	private bool IsInclusiveToNpc(string exclusiveToNpc, string npcId) {
		if (string.IsNullOrEmpty(exclusiveToNpc)) return true;
		return exclusiveToNpc == npcId;
	}

	private bool IsInclusiveForAnyOfTraitIds(string exclusiveToTrait, string[] traitIds) {
		if (string.IsNullOrEmpty(exclusiveToTrait)) return true;
		var traits = _TraitsRepo.GetTraitsByIds(traitIds);
		return traits.Any(t => t.Id == exclusiveToTrait);
	}
}
