using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogLibrary.App.Helpers;
public static class Guard
{
    public static void NotNullOrEmpty(string? value)
    {
        if (string.IsNullOrEmpty(value)) throw new ArgumentException("Value cannot be null or empty.", nameof(value));
    }

	public static bool IsNullOrEmpty<T>(IEnumerable<T>? value) {
		return value?.Any() != true;
	}

	public static T[] ListOrNullToArray<T>(List<T>? value) {
        if (value == null || value.Count == 0) {
			return [];
		}

		return [.. value];
	}

    public static List<T> ListOrNullToList<T>(List<T>? items) => items ?? [];
}
