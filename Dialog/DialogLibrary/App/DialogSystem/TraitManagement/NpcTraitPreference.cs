
using DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoObjects;
using DialogLibrary.App.DialogSystem.Repositories;
using DialogLibrary.App.Helpers;

using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogLibrary.App.DialogSystem.TraitManagement;
public class NpcTraitPreference(Npc npc, string[] likesTraits, string[] dislikesTraits) {
    public Npc      Character      { get; } = npc;
    public string[] MyTraits       { get; } = Guard.ListOrNullToArray(npc.Trait_Ids);
    public string[] LikesTraits    { get; } = likesTraits;
    public string[] DislikesTraits { get; } = dislikesTraits;

    public List<string> GetMyTraitsWhereIn(string[] traitsToLookFor) {
        List<string> foundTraits = [];
        foreach (string trait in traitsToLookFor) {
            if (MyTraits.Contains(trait)) {
                foundTraits.Add(trait);
            }
        }
        return foundTraits;
    }
}

public static class NpcTraitPreferenceFactory {
    public static NpcTraitPreference Create(Npc npc, TraitsRepo traitsRepo) {
        string[]                traitIds       = Guard.ListOrNullToArray(npc.Trait_Ids);
        Trait[]                 traits         = traitsRepo.GetTraitsByIds(traitIds);
        Dictionary<string, int> traitSentiment = [];

        foreach (Trait trait in traits) {
            UpdateTraitSentiment(traitSentiment, trait.Likes ?? [], 1);      // Likes increase sentiment
            UpdateTraitSentiment(traitSentiment, trait.Dislikes ?? [], -1);  // Dislikes decrease sentiment
        }

        // Determine final liked and disliked traits based on net sentiment
        HashSet<string> likedTraits    = [];
        HashSet<string> dislikedTraits = [];

        foreach (KeyValuePair<string, int> kvp in traitSentiment) {
            if (kvp.Value > 0) likedTraits.Add(kvp.Key);  // Trait is liked
            else if (kvp.Value < 0) dislikedTraits.Add(kvp.Key);  // Trait is disliked
            // If equal (0), don't add to either list
        }

        return new NpcTraitPreference(npc, likedTraits.ToArray(), dislikedTraits.ToArray());
    }

    private static void UpdateTraitSentiment(Dictionary<string, int> sentimentDictionary, IEnumerable<string> traits, int sentimentChange) {
        foreach (string trait in traits) {
            if (sentimentDictionary.ContainsKey(trait)) {
                sentimentDictionary[trait] += sentimentChange;
            }
            else {
                sentimentDictionary[trait] = sentimentChange;
            }
        }
    }
}
