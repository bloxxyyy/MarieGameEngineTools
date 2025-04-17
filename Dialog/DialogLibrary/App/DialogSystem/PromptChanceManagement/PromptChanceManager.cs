using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoContainers;
using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoObjects;
using DialogLibrary.App.DialogSystem.Repositories;
using DialogLibrary.App.DialogSystem.TraitManagement;
using DialogLibrary.App.Helpers;

using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogLibrary.App.DialogSystem.PromptChanceManagement;
public class PromptChanceManager(
    Npc targetNpc,
    Npc interNpc,
    TraitsRepo traitsRepo,
    DialogContainer dialog,
    TraitInformation traitInformation
)
{
    private readonly TraitsRepo            _TraitsRepo = traitsRepo;
    private readonly DialogContainer       _Dialog = dialog;
    private readonly PromptChanceMod       _ChoiceModifyingHelper = new(targetNpc, interNpc, dialog, traitInformation);

    public bool TryGetPromptByPromptChances(List<string> prompChanceIds, Npc currentSpeakingNpc, out NpcPrompt? npcPrompt)
    {
        npcPrompt = null;
        PromptChance[] possiblePromptChances = GetPossiblePromptChances(prompChanceIds, currentSpeakingNpc);
        if (Guard.IsNullOrEmpty(possiblePromptChances)) return false;

        npcPrompt = _ChoiceModifyingHelper.GetAConnectedPromptByChanceModifier(possiblePromptChances, currentSpeakingNpc.Id);
        return true;
    }

    private PromptChance[] GetPossiblePromptChances(List<string> target_npc_prompt_chance_ids, Npc _CurrentSpeakingNpc)
    {
        string[] promptChanceIds = Guard.ListOrNullToArray(target_npc_prompt_chance_ids);
        PromptChance[] promptChances = GetPromptChancesByIds(promptChanceIds);
        return GetUseablePromptChances(promptChances, _CurrentSpeakingNpc);
    }

    private PromptChance[] GetUseablePromptChances(PromptChance[] npcPromptChances, Npc npc)
    {
        if (Guard.IsNullOrEmpty(npcPromptChances)) return [];

        string[] traitIds = Guard.ListOrNullToArray(npc.Trait_Ids);

        List<PromptChance> usablePrompts = [];
        foreach (PromptChance prompt in npcPromptChances)
        {
            bool isInclusiveToNpc = IsInclusiveToNpc(prompt.Exclusive_to_npc ?? "", npc.Id);
            bool isInclusiveForTrait = IsInclusiveForAnyOfTraitIds(prompt.Exclusive_to_trait ?? "", traitIds);

            if (isInclusiveToNpc && isInclusiveForTrait)
                usablePrompts.Add(prompt);
        }

        return [.. usablePrompts];
    }

    private PromptChance[] GetPromptChancesByIds(string[] promptChanceIds)
    {
        HashSet<string>    idSet           = [.. promptChanceIds];
        List<PromptChance> matchingPrompts = [];

        foreach (PromptChance prompt in _Dialog.Npc_prompt_chances)
        {
            if (idSet.Contains(prompt.Id))
                matchingPrompts.Add(prompt);
        }

        return [.. matchingPrompts];
    }

    private static bool IsInclusiveToNpc(string exclusiveToNpc, string npcId)
    {
        if (string.IsNullOrEmpty(exclusiveToNpc)) return true;
        return exclusiveToNpc == npcId;
    }

    private bool IsInclusiveForAnyOfTraitIds(string exclusiveToTrait, string[] traitIds)
    {
        if (string.IsNullOrEmpty(exclusiveToTrait)) return true;
        Trait[] traits = _TraitsRepo.GetTraitsByIds(traitIds);
        return traits.Any(t => t.Id == exclusiveToTrait);
    }
}
