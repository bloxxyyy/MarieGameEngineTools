namespace DialogLibrary.App.DialogSystem.JsonObjects.JsonDtoObjects;

public record Trait(
    string Id,
    string Name,
    List<string> Likes,
    List<string> Dislikes
);
