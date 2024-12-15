namespace KokoSharpTranspiler.Data;

internal static class ConfigData
{
    public static string CONFIG_FILE = "config.json";
    public static Dictionary<FileNameAndLocation, KokoScriptCodeData> CODE_FILES = [];
    public static string ROOT_PROJECT_LOCATION = string.Empty;
    public static ConfigFileJsonData CONFIG_FILE_JSON_DATA = null!;
    public static string BUILD_FOLDER_FROM_ROOT = string.Empty;
    internal static bool ERROR = false;

    // templating
    public static string OBJECT_TEMPLATE = "../../../Templates/objectTemplate.txt";
    public static string TRY_TEMPLATE = "../../../Templates/tryTemplate.txt";
}
