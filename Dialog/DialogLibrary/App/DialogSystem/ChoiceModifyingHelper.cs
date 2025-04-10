using YuiGameSystems.DialogSystem.FileLoading.DataFiles;
using DialogLibrary.App.DialogSystem.Datasets;
using YuiGameSystems.DialogSystem.FileLoading.ValidatedDataContainers;

namespace YuiGameSystems.DialogSystem;
public class ChoiceModifyingHelper(
    Npc targetNpc,
    Npc interlocutorNpc,
    DatasetManager datasetManager,
    DialogContainer dialogContainer
) {

    public record NpcPromptChanceModifier(
        PromptChance Chance,
        int StartModifier,
        int EndModifier
    );

    private readonly Dictionary<string, List<string>> _TraitsILike    = [];
    private readonly Dictionary<string, List<string>> _TraitsIDislike = [];

    private bool _IsPlayerConversation;
    private readonly Npc _DialogNpc = targetNpc;
    private readonly Npc? _InterNpc = interlocutorNpc;
    private readonly Random _Random = new();

    private readonly DatasetManager _DatasetManager = datasetManager;
    private readonly DialogContainer _DialogContainer = dialogContainer;

	public void Initialize(bool isPlayerConversation)
    {
		_IsPlayerConversation = isPlayerConversation;
		if (!_IsPlayerConversation) SetupTargetNpcTraits();
		SetupPlayerTraits();
    }

    public void SetupTargetNpcTraits()
    {
		List<Trait> npcTraits = _DatasetManager.Datasets.Traits.Values.Where(x => _DialogNpc.Trait_Ids.Contains(x.Id)).ToList();
		List<Trait> playerTraits = _DatasetManager.Datasets.Traits.Values.Where(x => _InterNpc.Trait_Ids.Contains(x.Id)).ToList();

        FindTraits(playerTraits, npcTraits, out var traitsILike, out var traitsIDislike);
        _TraitsILike.Add(_DialogNpc.Name, traitsILike);
        _TraitsIDislike.Add(_DialogNpc.Name, traitsIDislike);
    }

    public void SetupPlayerTraits()
    {
		List<Trait> npcTraits = _DatasetManager.Datasets.Traits.Values.Where(x => _DialogNpc.Trait_Ids.Contains(x.Id)).ToList();
		List<Trait> playerTraits = _DatasetManager.Datasets.Traits.Values.Where(x => _InterNpc.Trait_Ids.Contains(x.Id)).ToList();

        FindTraits(npcTraits, playerTraits, out var foundTraitsILike, out var foundTraitsIDislike);
        _TraitsILike.Add(_InterNpc.Name, foundTraitsILike);
        _TraitsIDislike.Add(_InterNpc.Name, foundTraitsIDislike);
    }

    private static void FindTraits(List<Trait> traitsToCheck, List<Trait> prompterTraits, out List<string> likes, out List<string> dislikes)
    {
        List<string> likedTrait = prompterTraits.SelectMany(trait => trait.Likes ?? Enumerable.Empty<string>()).ToList();
        List<string> dislikedTrait = prompterTraits.SelectMany(trait => trait.Dislikes ?? Enumerable.Empty<string>()).ToList();
        likes = FindTraitsWithDuplicates(traitsToCheck, likedTrait);
        dislikes = FindTraitsWithDuplicates(traitsToCheck, dislikedTrait);
    }

    /// <summary>
    /// Gets a connected NpcPrompt by calculating the chance of each NpcPromptChance.
    /// </summary>
    public virtual NpcPrompt GetAConnectedPromptByChanceModifier(PromptChance[] npcChoices, string currentSpeakingNpcId)
    {
        // found a -1 so just return that one
        var selectedChoice = Array.Find(npcChoices, choice => choice?.Base_chance_percentage == -1);
        if (selectedChoice is not null) {

            var npcPromptId = selectedChoice.Npc_prompt_id;
			return _DialogContainer.Npc_prompts.Find(x => x.Id == npcPromptId) ?? throw new Exception("NpcPrompt not found");
        }

		Npc npcFor = currentSpeakingNpcId == _DialogNpc.Id ? _DialogNpc : _InterNpc;
		Npc npcTo  = currentSpeakingNpcId == _DialogNpc.Id ? _InterNpc  : _DialogNpc;

        npcChoices = FilterNpcPromptTypesByAcquaintanceStatus(npcChoices, npcFor, npcTo);

        List<NpcPromptChanceModifier> npcPromptChanceModifiers = SetupChanceModifiers(npcChoices, npcFor, npcTo);
        NpcPromptChanceModifier       npcPromptChanceModifier  = GetRandomChanceModifier(npcPromptChanceModifiers);

        var id = npcPromptChanceModifier.Chance.Npc_prompt_id;
		return _DialogContainer.Npc_prompts.Find(x => x.Id == id) ?? throw new Exception("NpcPrompt not found");
    }

    private NpcPromptChanceModifier GetRandomChanceModifier(List<NpcPromptChanceModifier> npcPromptChanceModifiers)
    {
        float  totalChance  = npcPromptChanceModifiers.Last().EndModifier;
        int    randomChance = _Random.Next(0, (int)totalChance);
        return npcPromptChanceModifiers.First(x => x.StartModifier <= randomChance && x.EndModifier >= randomChance);
    }

    public List<NpcPromptChanceModifier> SetupChanceModifiers(PromptChance[] npcChoices, Npc npcFor, Npc npcTo)
    {
        List<NpcPromptChanceModifier> npcPromptChanceModifiers = [];
        int startModifier = 0;
        float startChance = npcChoices.Length / 100;

        foreach (PromptChance choice in npcChoices)
        {
            var promptType = (choice.Prompt_type == null) ? PromptType.Neutral :
                (PromptType)Enum.Parse(typeof(PromptType), choice.Prompt_type);

            int modifier = (int)(startChance + choice.Base_chance_percentage);
            modifier = ModifyForTraitOpinion(promptType, modifier, npcTo);
            modifier = ModifyForAttitude    (promptType, modifier, npcFor, npcTo);

            var endModifier = startModifier + modifier;
            NpcPromptChanceModifier chance = new(choice, startModifier, endModifier);
            startModifier = endModifier + 1;
            npcPromptChanceModifiers.Add(chance);
        }

        return npcPromptChanceModifiers;
    }

    private static PromptChance[] FilterNpcPromptTypesByAcquaintanceStatus(PromptChance[] npcChoices, Npc forNpc, Npc toNpc)
    {
        int attitude = forNpc.Acquaintances.Find(x => x.Id == toNpc.Id)?.Attitude ?? 0;

        PromptChance[] choices = npcChoices
            .Where(x => IsNeutralOrFitsAttitude(x, attitude) && HasRequiredAttitudeForChoice(x, attitude))
            .ToArray();

        if (choices.Length == 0) throw new Exception("No choices where attitude is low enough!");
        return choices;
    }

    private static bool HasRequiredAttitudeForChoice(PromptChance npcPromptChance, int attitude)
    {
        int exl = npcPromptChance.Exclusive_to_attitude;
        return (exl == 0) || (exl > 0 && exl < attitude) || (exl < 0 && exl > attitude);
    }

    private static bool IsNeutralOrFitsAttitude(PromptChance npcPromptChance, int attitude)
    {
        const int modifier = -5;
		PromptType type = (npcPromptChance.Prompt_type == null) ? PromptType.Neutral :
		(PromptType)Enum.Parse(typeof(PromptType), npcPromptChance.Prompt_type);

		return (type == PromptType.Friendly && attitude > modifier)
            || (type == PromptType.Hostile && attitude < -modifier)
            || (type == PromptType.Neutral);
    }

    private static int ModifyForAttitude(PromptType promptType, int modifier, Npc npcFor, Npc npcTo)
    {
        if (promptType == PromptType.Neutral) return modifier;

        int attitude = npcFor.Acquaintances.Find(x => x.Id == npcTo.Id)?.Attitude ?? 0;

        var OneFourthOfAttiudeAsBonus = Math.Abs(attitude / 4);
        if (promptType == PromptType.Friendly && attitude > 0)
        {
            modifier += OneFourthOfAttiudeAsBonus;
            return modifier;
        }

        if (promptType == PromptType.Hostile && attitude < 0)
        {
            modifier += OneFourthOfAttiudeAsBonus;
            return modifier;
        }

        return modifier;
    }

    /// <summary>
    /// Get, Filter and return the modifier with the trait bonus.
    /// </summary>
    public virtual int ModifyForTraitOpinion(PromptType promptType, int modifier, Npc npcTo)
    {
        if (promptType == PromptType.Neutral) return modifier;

        List<string> foundTraitsILike    = _TraitsILike[npcTo.Name]    ?? [];
        List<string> foundTraitsIDislike = _TraitsIDislike[npcTo.Name] ?? [];

        FilterAndReturnDistinct(ref foundTraitsILike, ref foundTraitsIDislike);
        return GetModifierWithTraitBonus(promptType, 15, modifier, foundTraitsILike, foundTraitsIDislike);
    }

    public static int GetModifierWithTraitBonus(PromptType type, int addedValue, int modifier, List<string> traitsILike, List<string> traitsIDislike)
    {
        if (type == PromptType.Neutral) return modifier;

        int likesLengthWithoutDislikes = traitsILike.Count          - traitsIDislike.Count;
        int likeModifierToAdd          = likesLengthWithoutDislikes * addedValue;

        int dislikesLengthWithoutLikes = traitsIDislike.Count       - traitsILike.Count;
        int dislikeModifierToAdd       = dislikesLengthWithoutLikes * addedValue;

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
    public static void FilterAndReturnDistinct(ref List<string> traitsILikeWithDuplicates, ref List<string> traitsIDislikeWithDuplicates)
    {
        foreach (string trait in traitsILikeWithDuplicates.Intersect(traitsIDislikeWithDuplicates).ToList())
        {
            int likeCount    = traitsILikeWithDuplicates.Count(x => x == trait);
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
        traitsILikeWithDuplicates    = traitsILikeWithDuplicates   .Distinct().ToList();
        traitsIDislikeWithDuplicates = traitsIDislikeWithDuplicates.Distinct().ToList();
    }

    public static List<string> FindTraitsWithDuplicates(List<Trait> traitsToCheck, List<string> filterList)
    {
        return traitsToCheck
            .Where(x => filterList.Contains(x.Name))
            .SelectMany(x => Enumerable.Repeat(x.Name, filterList.Count(y => y == x.Name)))
            .ToList();
    }
}
