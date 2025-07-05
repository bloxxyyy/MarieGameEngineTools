using YuiGameSystems.DialogSystem.FileLoading.DataFiles;
using DialogLibrary.App.Helpers;
using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoObjects;
using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoContainers;
using DialogLibrary.App.DialogSystem.TraitManagement;

namespace DialogLibrary.App.DialogSystem.PromptChanceManagement;
public class PromptChanceMod(
    DialogContainer    dialogContainer,
    NpcTraitPreference dialogNpcTraitPreference,
    NpcTraitPreference interNpcTraitPreference
) {
    private readonly DialogContainer    _DialogContainer          = dialogContainer;
    private readonly NpcTraitPreference _DialogNpcTraitPreference = dialogNpcTraitPreference;
    private readonly NpcTraitPreference _InterNpcTraitInformation = interNpcTraitPreference;

    public NpcPrompt GetAConnectedPromptByChanceModifier(PromptChance[] npcChoices, Npc currentSpeakingNpc) {
        Npc conversationPartner = GetConversationPartner(currentSpeakingNpc);

        NpcPrompt? requiredPrompt = TryGetRequiredPrompt(npcChoices);
        if (requiredPrompt != null)
            return requiredPrompt;

        // First check if the prompt is allowed to be said based on how much the speaker likes the convo partner.
        npcChoices = FilterNpcPromptTypesByAcquaintanceStatus(npcChoices, currentSpeakingNpc, conversationPartner);

        int traitAttitude = GetAttitudeTowardsConvoPartnerBasedOnTraits(currentSpeakingNpc);
        int acquaintanceAttitude = GetAttitudeTowardsConvoPartner(currentSpeakingNpc, conversationPartner);

        List<Modifier> promptsWithPickChanceBasedOnFeelings = [];

        int startModifier = 0;
        foreach (PromptChance choice in npcChoices) {
            PromptType promptType = GetPromptType(choice);
            int modifierToAdd = choice.Base_chance_percentage;

            modifierToAdd += UpdateModifierBasedOnFeelings(promptType, traitAttitude, acquaintanceAttitude);

            int endModifier = startModifier + modifierToAdd;
            Modifier chance = new(choice.Npc_prompt_id, startModifier, endModifier);
            startModifier = endModifier + 1;

            promptsWithPickChanceBasedOnFeelings.Add(chance);
        }

        Modifier chosenPromptChance = GetRandomChanceModifier(promptsWithPickChanceBasedOnFeelings);
        string id = chosenPromptChance.NpcPromptId;
        return _DialogContainer.Npc_prompts.Find(x => x.Id == id) ?? throw new Exception("NpcPrompt not found");
    }

    private Npc GetConversationPartner(Npc currentSpeakingNpc) {
        return currentSpeakingNpc == _DialogNpcTraitPreference.Character
            ? _InterNpcTraitInformation.Character
            : _DialogNpcTraitPreference.Character;
    }

    private static int UpdateModifierBasedOnFeelings(PromptType promptType, int traitFeelings, int acquaintanceFeelings) {
        const int toAddForTraits = 15;
        int currentModifier = 0;
        acquaintanceFeelings = Math.Clamp(acquaintanceFeelings, -100, 100);

        switch (promptType) {
            case PromptType.Neutral:
                return 0;

            case PromptType.Friendly:
                if (traitFeelings > 0)        currentModifier += toAddForTraits * traitFeelings;
                if (traitFeelings < 0)        currentModifier -= toAddForTraits * Math.Abs(traitFeelings);
                if (acquaintanceFeelings > 0) currentModifier += acquaintanceFeelings;
                if (acquaintanceFeelings < 0) currentModifier -= Math.Abs(acquaintanceFeelings);
                break;

            case PromptType.Hostile:
                if (traitFeelings < 0)        currentModifier += toAddForTraits * Math.Abs(traitFeelings);
                if (traitFeelings > 0)        currentModifier -= toAddForTraits * traitFeelings;
                if (acquaintanceFeelings < 0) currentModifier += Math.Abs(acquaintanceFeelings);
                if (acquaintanceFeelings > 0) currentModifier -= acquaintanceFeelings;
                break;
        }
        return currentModifier;
    }

    private static int GetAttitudeTowardsConvoPartner(Npc currentSpeakingNpc, Npc convoPartner) {
        Acquaintance[] acquaintances = Guard.ListOrNullToArray(currentSpeakingNpc.Acquaintances);
        return Array.Find(acquaintances, acquaintance => acquaintance.Id == convoPartner.Id)?.Attitude ?? 0;
    }

    private static PromptType GetPromptType(PromptChance choice) {
        return choice.Prompt_type == null ? PromptType.Neutral :
            (PromptType)Enum.Parse(typeof(PromptType), choice.Prompt_type);
    }

    private int GetAttitudeTowardsConvoPartnerBasedOnTraits(Npc currentSpeakingNpc) {
        NpcTraitPreference currentSpeakingNpcTraitData;
        NpcTraitPreference conversationPartnerTraitData;

        if (currentSpeakingNpc.Id == _DialogNpcTraitPreference.Character.Id) {
            currentSpeakingNpcTraitData  = _DialogNpcTraitPreference;
            conversationPartnerTraitData = _InterNpcTraitInformation;
        }
        else {
            currentSpeakingNpcTraitData  = _InterNpcTraitInformation;
            conversationPartnerTraitData = _DialogNpcTraitPreference;
        }

        int iLikeConvoPartnerAmount    = conversationPartnerTraitData.GetMyTraitsWhereIn(currentSpeakingNpcTraitData.LikesTraits).Count;
        int iDislikeConvoPartnerAmount = conversationPartnerTraitData.GetMyTraitsWhereIn(currentSpeakingNpcTraitData.DislikesTraits).Count;

        return iLikeConvoPartnerAmount - iDislikeConvoPartnerAmount;
    }

    private NpcPrompt? TryGetRequiredPrompt(PromptChance[] npcChoices) {
        // found a -1 so just return that one
        PromptChance? selectedChoice = Array.Find(npcChoices, choice => choice?.Base_chance_percentage == -1);
        if (selectedChoice is not null) {
            string npcPromptId = selectedChoice.Npc_prompt_id;
            return _DialogContainer.Npc_prompts.Find(x => x.Id == npcPromptId) ?? throw new Exception("NpcPrompt not found");
        }

        return null;
    }

    private static Modifier GetRandomChanceModifier(List<Modifier> npcPromptChanceModifiers) {
        float totalChance = npcPromptChanceModifiers.Last().EndModifier;
        int randomChance = new Random().Next(0, (int)totalChance);
        return npcPromptChanceModifiers.First(x => x.StartModifier <= randomChance && x.EndModifier >= randomChance);
    }

    private PromptChance[] FilterNpcPromptTypesByAcquaintanceStatus(PromptChance[] npcChoices, Npc currentSpeakingNpc, Npc convoPartner)
    {
        int attitude = GetAttitudeTowardsConvoPartner(currentSpeakingNpc, convoPartner);

        return npcChoices
            .Where(x => IsNeutralOrFitsAttitude(x, attitude) && HasRequiredAttitudeForChoice(x, attitude))
            .ToArray();
    }

    private static bool HasRequiredAttitudeForChoice(PromptChance npcPromptChance, int currentAttitude)
    {
        int neededAttitude = npcPromptChance.Exclusive_to_attitude;
        bool forPossitiveAndHigherThanNeeded = neededAttitude > 0 && neededAttitude < currentAttitude;
        bool forNegativeAndLowerThanNeeded   = neededAttitude < 0 && neededAttitude > currentAttitude;
        return neededAttitude == 0 || forPossitiveAndHigherThanNeeded || forNegativeAndLowerThanNeeded;
    }

    private static bool IsNeutralOrFitsAttitude(PromptChance npcPromptChance, int attitude)
    {
        const int modifier = -5;
        PromptType type = npcPromptChance.Prompt_type == null
            ? PromptType.Neutral
            : (PromptType)Enum.Parse(typeof(PromptType), npcPromptChance.Prompt_type);

        return (type == PromptType.Friendly && attitude > modifier)
            || (type == PromptType.Hostile && attitude < -modifier)
            || type == PromptType.Neutral;
    }
}
