using YuiGameSystems.DialogSystem.FileLoading.DataFiles;
using DialogLibrary.App.Helpers;
using DialogLibrary.App.DialogSystem.Repositories;
using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoObjects;
using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoContainers;

namespace DialogLibrary.App.DialogSystem.PromptChanceManagement;
public class PromptChanceMod {
    private readonly TraitInformation _TraitInformation = new();
    private readonly Npc              _DialogNpc;
    private readonly Npc              _InterNpc;
    private readonly DialogContainer  _DialogContainer;
    private readonly TraitsRepo       _TraitsRepo;
    private readonly bool             _IsPlayerConversation;

    public PromptChanceMod(
        Npc             targetNpc,
        Npc             interlocutorNpc,
        bool            isPlayerConversation,
        DialogContainer dialogContainer,
        TraitsRepo      traitsRepo
    ) {
        _DialogNpc            = targetNpc;
        _InterNpc             = interlocutorNpc;
        _DialogContainer      = dialogContainer;
        _TraitsRepo           = traitsRepo;
        _IsPlayerConversation = isPlayerConversation;

        if (!_IsPlayerConversation) SetupDialogNpcTraits(_TraitsRepo);
        SetupInterTraits(_TraitsRepo);
    }

    /// <summary>
    /// Gets a connected NpcPrompt by calculating the chance of each NpcPromptChance.
    /// </summary>
    public NpcPrompt GetAConnectedPromptByChanceModifier(PromptChance[] npcChoices, string currentSpeakingNpcId) {
        // found a -1 so just return that one
        PromptChance? selectedChoice = Array.Find(npcChoices, choice => choice?.Base_chance_percentage == -1);
        if (selectedChoice is not null) {
            string npcPromptId = selectedChoice.Npc_prompt_id;
            return _DialogContainer.Npc_prompts.Find(x => x.Id == npcPromptId) ?? throw new Exception("NpcPrompt not found");
        }

        Npc npcFor = currentSpeakingNpcId == _DialogNpc.Id ? _DialogNpc : _InterNpc;
        Npc npcTo = currentSpeakingNpcId == _DialogNpc.Id ? _InterNpc : _DialogNpc;

        npcChoices = FilterNpcPromptTypesByAcquaintanceStatus(npcChoices, npcFor, npcTo);

        List<Modifier> npcPromptChanceModifiers = SetupChanceModifiers(npcChoices, npcFor, npcTo, _TraitInformation);
        Modifier npcPromptChanceModifier = GetRandomChanceModifier(npcPromptChanceModifiers);

        string id = npcPromptChanceModifier.NpcPromptId;
        return _DialogContainer.Npc_prompts.Find(x => x.Id == id) ?? throw new Exception("NpcPrompt not found");
    }

    private void SetupDialogNpcTraits(TraitsRepo repo) {
        Trait[] forNpcTraits = repo.GetTraitsByIds(Guard.ListOrNullToArray(_DialogNpc.Trait_Ids));
        Trait[] useNpcTraits = repo.GetTraitsByIds(Guard.ListOrNullToArray(_InterNpc.Trait_Ids));
        _TraitInformation.AddTraitsToLiked(_DialogNpc.Name, FindLikedTraits(useNpcTraits, forNpcTraits));
        _TraitInformation.AddTraitsToDisliked(_DialogNpc.Name, FindDislikedTraits(useNpcTraits, forNpcTraits));
    }

    private void SetupInterTraits(TraitsRepo repo) {
        Trait[] forNpcTraits = repo.GetTraitsByIds(Guard.ListOrNullToArray(_InterNpc.Trait_Ids));
        Trait[] useNpcTraits = repo.GetTraitsByIds(Guard.ListOrNullToArray(_DialogNpc.Trait_Ids));
        _TraitInformation.AddTraitsToLiked(_InterNpc.Name, FindLikedTraits(useNpcTraits, forNpcTraits));
        _TraitInformation.AddTraitsToDisliked(_InterNpc.Name, FindDislikedTraits(useNpcTraits, forNpcTraits));
    }

    private static List<Modifier> SetupChanceModifiers(PromptChance[] npcChoices, Npc npcFor, Npc npcTo, TraitInformation traitInfo) {
        List<Modifier> npcPromptChanceModifiers = [];
        int startModifier = 0;

        foreach (PromptChance choice in npcChoices)
        {
            PromptType promptType = choice.Prompt_type == null ? PromptType.Neutral :
                (PromptType)Enum.Parse(typeof(PromptType), choice.Prompt_type);

            int modifier = choice.Base_chance_percentage;
            modifier = ModifyForTraitOpinion(promptType, modifier, npcTo.Name, traitInfo);
            modifier = ModifyForAttitude(promptType, modifier, npcFor, npcTo);

            int endModifier = startModifier + modifier;
            Modifier chance = new(choice.Npc_prompt_id, startModifier, endModifier);
            startModifier = endModifier + 1;
            npcPromptChanceModifiers.Add(chance);
        }

        return npcPromptChanceModifiers;
    }

    private static List<string> FindLikedTraits(Trait[] traitsToCheck, Trait[] prompterTraits) {
        List<string> likedTrait = prompterTraits.SelectMany(trait => trait.Likes ?? Enumerable.Empty<string>()).ToList();
        Dictionary<string, int> likedTraitsNameWithCount = GetTraitWithCountOfTimesFoundInList(likedTrait);
        return GetTraitsByCount(traitsToCheck, likedTraitsNameWithCount);
    }

    private static List<string> FindDislikedTraits(Trait[] traitsToCheck, Trait[] prompterTraits) {
        List<string> dislikedTrait = prompterTraits.SelectMany(trait => trait.Dislikes ?? Enumerable.Empty<string>()).ToList();
        Dictionary<string, int> dislikeTraitsNameWithCount = GetTraitWithCountOfTimesFoundInList(dislikedTrait);
        return GetTraitsByCount(traitsToCheck, dislikeTraitsNameWithCount);
    }

    private static Modifier GetRandomChanceModifier(List<Modifier> npcPromptChanceModifiers) {
        float totalChance = npcPromptChanceModifiers.Last().EndModifier;
        int randomChance = new Random().Next(0, (int)totalChance);
        return npcPromptChanceModifiers.First(x => x.StartModifier <= randomChance && x.EndModifier >= randomChance);
    }

    private static PromptChance[] FilterNpcPromptTypesByAcquaintanceStatus(PromptChance[] npcChoices, Npc forNpc, Npc toNpc)
    {
        Acquaintance[] acquaintances = Guard.ListOrNullToArray(forNpc.Acquaintances);
        int attitudeIHaveToward = Array.Find(acquaintances, acquaintance => acquaintance.Id == toNpc.Id)?.Attitude ?? 0;

        PromptChance[] choices = npcChoices
            .Where(x => IsNeutralOrFitsAttitude(x, attitudeIHaveToward) && HasRequiredAttitudeForChoice(x, attitudeIHaveToward))
            .ToArray();

        if (choices.Length == 0) throw new Exception("No choices where attitude is low enough!");
        return choices;
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

    private static int ModifyForAttitude(PromptType promptType, int modifier, Npc npcFor, Npc npcTo)
    {
        if (promptType == PromptType.Neutral) return modifier;

        Acquaintance[] acquaintances = Guard.ListOrNullToArray(npcFor.Acquaintances);
        int attitude = Array.Find(acquaintances, acquaintance => acquaintance.Id == npcTo.Id)?.Attitude ?? 0;

        int OneFourthOfAttitudeAsBonus = Math.Abs(attitude / 4);
        if (promptType == PromptType.Friendly && attitude > 0)
        {
            return modifier += OneFourthOfAttitudeAsBonus;
        }

        if (promptType == PromptType.Hostile && attitude < 0)
        {
            return modifier += OneFourthOfAttitudeAsBonus;
        }

        return modifier;
    }

    /// <summary>
    /// Get, Filter and return the modifier with the trait bonus.
    /// </summary>
    private static int ModifyForTraitOpinion(PromptType promptType, int modifier, string npcToName, TraitInformation traitInformation)
    {
        if (promptType == PromptType.Neutral) return modifier;

        List<string> foundTraitsILike = traitInformation.GetTraitsILike(npcToName);
        List<string> foundTraitsIDislike = traitInformation.GetTraitsIDislike(npcToName);

        FilterAndReturnDistinct(ref foundTraitsILike, ref foundTraitsIDislike);
        return GetModifierWithTraitBonus(promptType, 15, modifier, foundTraitsILike, foundTraitsIDislike);
    }

    private static int GetModifierWithTraitBonus(PromptType type, int addedValue, int modifier, List<string> traitsILike, List<string> traitsIDislike)
    {
        if (type == PromptType.Neutral) return modifier;

        int likesLengthWithoutDislikes = traitsILike.Count - traitsIDislike.Count;
        int likeModifierToAdd = likesLengthWithoutDislikes * addedValue;

        int dislikesLengthWithoutLikes = traitsIDislike.Count - traitsILike.Count;
        int dislikeModifierToAdd = dislikesLengthWithoutLikes * addedValue;

        if (type == PromptType.Friendly)
        {
            if (likesLengthWithoutDislikes > 0) modifier += likeModifierToAdd;
            if (dislikesLengthWithoutLikes > 0) modifier -= dislikeModifierToAdd;
        }

        if (type == PromptType.Hostile)
        {
            if (likesLengthWithoutDislikes > 0) modifier -= likeModifierToAdd;
            if (dislikesLengthWithoutLikes > 0) modifier += dislikeModifierToAdd;
        }

        return modifier;
    }

    /// <summary>
    /// Filter the trait to only be in 1 list depending on the count of the specific trait.
    /// After we got the trait removed from the other list. Get the traits distinct.
    /// </summary>
    private static void FilterAndReturnDistinct(ref List<string> traitsILikeWithDuplicates, ref List<string> traitsIDislikeWithDuplicates)
    {
        foreach (string trait in traitsILikeWithDuplicates.Intersect(traitsIDislikeWithDuplicates).ToList())
        {
            int likeCount = traitsILikeWithDuplicates.Count(x => x == trait);
            int dislikeCount = traitsIDislikeWithDuplicates.Count(x => x == trait);

            if (likeCount == dislikeCount)
            {
                traitsILikeWithDuplicates.RemoveAll(x => x == trait);
                traitsIDislikeWithDuplicates.RemoveAll(x => x == trait);
                continue;
            }

            if (likeCount > dislikeCount) traitsIDislikeWithDuplicates.RemoveAll(x => x == trait);
            else traitsILikeWithDuplicates.RemoveAll(x => x == trait);
        }

        // We have now removed all traits that are in both like and dislike.
        // So now we can count the traits distinct and see if we have more likes or dislikes.
        traitsILikeWithDuplicates = traitsILikeWithDuplicates.Distinct().ToList();
        traitsIDislikeWithDuplicates = traitsIDislikeWithDuplicates.Distinct().ToList();
    }

    private static List<string> GetTraitsByCount(Trait[] traits, Dictionary<string, int> traitsNameWithCount) {
        List<string> result = [];
        foreach (Trait trait in traits) {
            if (traitsNameWithCount.TryGetValue(trait.Name, out int count)) {
                for (int i = 0; i < count; i++) result.Add(trait.Name);
            }
        }

        return result;
    }

    private static Dictionary<string, int> GetTraitWithCountOfTimesFoundInList(List<string> other) {
        Dictionary<string, int> traitsNameWithCount = [];
        foreach (string name in other) {
            traitsNameWithCount[name] = traitsNameWithCount.TryGetValue(name, out int count) ? count + 1 : 1;
        }

        return traitsNameWithCount;
    }
}
