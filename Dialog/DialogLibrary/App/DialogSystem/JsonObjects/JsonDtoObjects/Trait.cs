namespace YuiGameSystems.DialogSystem.FileLoading.DataFiles;

public record Trait(
    string Id,
    string Name,
    List<string> Likes,
    List<string> Dislikes
);
