using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoObjects;
using DialogLibrary.App.DialogSystem.Repositories;
using DialogLibrary.App.Helpers;

using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogLibrary.App.DialogSystem.TraitManagement;
public class TraitInformationManager(bool isPlayerConversation, TraitsRepo traitsRepo) {
    private readonly TraitsRepo _TraitsRepo           = traitsRepo;
    private readonly bool       _IsPlayerConversation = isPlayerConversation;

    public TraitInformation GetTraitInformation(Npc dialogNpc, Npc interNpc) {
        TraitInformation traitInformation = new();


        SetupTraits(_TraitsRepo, dialogNpc, interNpc, traitInformation);
        if (_IsPlayerConversation) return traitInformation; // player doesn't care about dialogNpc's traits

        SetupTraits(_TraitsRepo, interNpc, dialogNpc, traitInformation);
        return traitInformation;
    }

    private static void SetupTraits(TraitsRepo repo, Npc me, Npc target, TraitInformation traitInformation) {
        Trait[] myTraits     = repo.GetTraitsByIds(Guard.ListOrNullToArray(me.Trait_Ids));
        Trait[] targetTraits = repo.GetTraitsByIds(Guard.ListOrNullToArray(target.Trait_Ids));
        traitInformation.AddTraitsILikeFromTarget(me.Name, FindLikedTargetTraits(targetTraits, myTraits));
        traitInformation.AddTraitsIDislikeFromTarget(me.Name, FindDislikedTargetTraits(targetTraits, myTraits));
    }

    private static List<string> FindLikedTargetTraits(Trait[] traitsToCheck, Trait[] prompterTraits) {
        List<string> likedTrait = prompterTraits.SelectMany(trait => trait.Likes ?? Enumerable.Empty<string>()).ToList();
        Dictionary<string, int> likedTraitsNameWithCount = GetTraitWithCountOfTimesFoundInList(likedTrait);
        return GetTraitsByCount(traitsToCheck, likedTraitsNameWithCount);
    }

    private static List<string> FindDislikedTargetTraits(Trait[] traitsToCheck, Trait[] prompterTraits) {
        List<string> dislikedTrait = prompterTraits.SelectMany(trait => trait.Dislikes ?? Enumerable.Empty<string>()).ToList();
        Dictionary<string, int> dislikeTraitsNameWithCount = GetTraitWithCountOfTimesFoundInList(dislikedTrait);
        return GetTraitsByCount(traitsToCheck, dislikeTraitsNameWithCount);
    }

    private static List<string> GetTraitsByCount(Trait[] traits, Dictionary<string, int> traitsNameWithCount) {
        List<string> result = [];
        foreach (Trait trait in traits) {
            if (traitsNameWithCount.TryGetValue(trait.Name, out int count)) {
                for (int i = 0; i < count; i++)
                    result.Add(trait.Name);
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
