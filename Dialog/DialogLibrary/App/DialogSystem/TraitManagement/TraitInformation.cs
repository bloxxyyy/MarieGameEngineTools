namespace DialogLibrary.App.DialogSystem.TraitManagement;

using CharacterId = String;
using TraitsCharacterLikesFromSpeakingPartner = List<string>;
using TraitsCharacterDislikesFromSpeakingPartner = List<string>;

/// <summary>
/// For a specific dialog interaction both characters are added to the lists.
/// For each character they check the other for liked and disliked traits.
/// </summary>
public class TraitInformation {
    private readonly Dictionary<CharacterId, TraitsCharacterLikesFromSpeakingPartner> _LikedTraitsBy = [];
    private readonly Dictionary<CharacterId, TraitsCharacterDislikesFromSpeakingPartner> _DislikedTraitsBy = [];

    public List<string> GetTraitsILikeFromTarget(string from) => _LikedTraitsBy[from] ?? [];
    public List<string> GetTraitsIDislikeFromTarget(string from) => _DislikedTraitsBy[from] ?? [];

    public void AddTraitsILikeFromTarget(string to, List<string> traits) {
        if (_LikedTraitsBy.TryGetValue(to, out List<string>? value)) value.AddRange(traits);
        else _LikedTraitsBy.Add(to, traits);
    }

    public void AddTraitsIDislikeFromTarget(string to, List<string> traits) {
        if (_DislikedTraitsBy.TryGetValue(to, out List<string>? value)) value.AddRange(traits);
        else _DislikedTraitsBy.Add(to, traits);
    }
}
