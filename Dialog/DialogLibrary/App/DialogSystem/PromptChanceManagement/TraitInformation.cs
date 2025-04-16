namespace DialogLibrary.App.DialogSystem.PromptChanceManagement;

/// <summary>
/// For a specific dialog interaction both characters are added to the lists.
/// For each character they check the other for liked and disliked traits.
/// </summary>
public class TraitInformation {
    private readonly Dictionary<string, List<string>> _LikedTraitsBy = [];
    private readonly Dictionary<string, List<string>> _DislikedTraitsBy = [];

    public List<string> GetTraitsILike(string from) => _LikedTraitsBy[from] ?? [];
    public List<string> GetTraitsIDislike(string from) => _DislikedTraitsBy[from] ?? [];

    public void AddTraitsToLiked(string to, List<string> traits) {
        if (_LikedTraitsBy.TryGetValue(to, out List<string>? value)) value.AddRange(traits);
        else _LikedTraitsBy.Add(to, traits);
    }

    public void AddTraitsToDisliked(string to, List<string> traits) {
        if (_DislikedTraitsBy.TryGetValue(to, out List<string>? value)) value.AddRange(traits);
        else _DislikedTraitsBy.Add(to, traits);
    }
}
