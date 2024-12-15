using System.Text.RegularExpressions;
using KokoSharpTranspiler.Data;
using KokoSharpTranspiler.Helpers;
using Newtonsoft.Json;

namespace KokoSharpTranspiler;

internal class Transpiler
{
    internal Transpiler(string rootProjectLocation)
    {
        ConfigData.ROOT_PROJECT_LOCATION = rootProjectLocation;
        ConfigData.CONFIG_FILE_JSON_DATA = JsonConvert.DeserializeObject<ConfigFileJsonData>(
            File.ReadAllText(Path.GetFullPath(Path.Combine(ConfigData.ROOT_PROJECT_LOCATION, ConfigData.CONFIG_FILE)))
        ) ?? throw new Exception("Config file error!");
    }

    internal void Setup()
    {
        var codeFilesLocation = Path.Combine(ConfigData.ROOT_PROJECT_LOCATION, ConfigData.CONFIG_FILE_JSON_DATA.RootCodeFileLocation);
        Directory.GetFiles(codeFilesLocation, "*", SearchOption.AllDirectories).ToList().ForEach(
            codeFileIncludingLocation => ConfigData.CODE_FILES.Add(
                codeFileIncludingLocation.Replace(ConfigData.ROOT_PROJECT_LOCATION, "").TrimStart('\\').TrimStart('/'),
                File.ReadAllText(codeFileIncludingLocation)
            )
        );

        Rebuild();
    }

    private void Rebuild()
    {
        var buildDirectory = Path.Combine(ConfigData.ROOT_PROJECT_LOCATION, ConfigData.CONFIG_FILE_JSON_DATA.RootBuildFileLocation);
        var addedBuildFolder = Path.Combine(buildDirectory, ConfigData.CONFIG_FILE_JSON_DATA.BuildFolderName);

        if (Directory.Exists(addedBuildFolder))
            Directory.Delete(addedBuildFolder, true);
        ConfigData.BUILD_FOLDER_FROM_ROOT = CreateBuildFolderFromRootIfItDoesntExist();
    }

    internal void Transpile()
    {
        ConfigData.CODE_FILES.ToList().ForEach(codeFile =>
        {
            TerminalHelper.Log("Transpiling file", codeFile.Key, ConsoleColor.Cyan);
            TranspileCode(codeFile.Key, codeFile.Value);
            TerminalHelper.Log("Finished transpiling file", codeFile.Key, ConsoleColor.Cyan);
            Console.WriteLine();
        });
    }

    private void TranspileCode(FileNameAndLocation fileNameAndLocation, KokoScriptCodeData kokoScriptCodeData)
    {
        TerminalHelper.Log("File content character count", kokoScriptCodeData.Data.Length.ToString(), ConsoleColor.Cyan);

        string? csharpFile = TryCreateObjectType(fileNameAndLocation);
        if (csharpFile == null) return;

        var filledFile = ParseBodyOfFile(fileNameAndLocation, csharpFile);

        TerminalHelper.Log("(", "Happy file", ConsoleColor.Green);
    }

    private string ParseBodyOfFile(FileNameAndLocation fileNameAndLocation, string csharpFile)
    {
        var fileName = Path.GetFileName(fileNameAndLocation);
        var fileLocation = Path.GetDirectoryName(fileNameAndLocation);
        var fileBody = File.ReadAllText(Path.Combine(ConfigData.ROOT_PROJECT_LOCATION, fileNameAndLocation));

        const string allMatchesWhichAreWordsOrSpecialCharacters = @"\w+|[^\s\w]";
        List<string> tokensInLine = [];
        var lines = fileBody.Split("\n").ToList();

        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            var nextLine = i + 1 < lines.Count ? lines[i + 1] : "";

            foreach (Match match in Regex.Matches(line, allMatchesWhichAreWordsOrSpecialCharacters).Cast<Match>())
                tokensInLine.Add(match.Value);

            TryParseMethod(allMatchesWhichAreWordsOrSpecialCharacters, tokensInLine, ref line, nextLine);
            lines[i] = line;
            TryParseTryCatch(allMatchesWhichAreWordsOrSpecialCharacters, tokensInLine, nextLine);

            tokensInLine.Clear();
        }

        // recombine all lines add a \n
        var newFileBody = string.Join("\n", lines);
        File.WriteAllText(Path.Combine(ConfigData.BUILD_FOLDER_FROM_ROOT, fileNameAndLocation), newFileBody);

        return csharpFile; // todo update?
    }

    private void TryParseTryCatch(string allMatches, List<string> tokensInLine, string nextLine)
    {
        List<string> tokensOfNextLine = [];
        foreach (Match match in Regex.Matches(nextLine, allMatches).Cast<Match>())
            tokensOfNextLine.Add(match.Value);

        var isTryCatch = IsTryCatch(tokensInLine, tokensOfNextLine);

        if (isTryCatch)
        {
            TerminalHelper.Log("Found try catch", "Parsing", ConsoleColor.Yellow);

            var objectTemplate = File.ReadAllText(ConfigData.TRY_TEMPLATE);
            //objectTemplate = objectTemplate.Replace("{{OBJECTTYPE}}", objectName);
        }
    }

    private bool IsTryCatch(List<string> tokensInLine, List<string> tokensOfNextLine)
    {
        return tokensInLine.Contains("try") && tokensOfNextLine.Contains("catch");
    }

    private void TryParseMethod(string allMatches, List<string> tokensInLine, ref string currentLine, string nextLine)
    {
        var isMethod = IsMethod(allMatches, tokensInLine, nextLine);

        if (isMethod)
        {
            TerminalHelper.Log("Found method", "Parsing", ConsoleColor.Yellow);

            var returnValue = tokensInLine.Count == 4 ? tokensInLine[3] : tokensInLine[2];
            var parameter   = tokensInLine.Count == 4 ? tokensInLine[1] : null;
            var methodName  = tokensInLine[0];
            var paramObj    = parameter?.First().ToString().ToUpper() + parameter?[1..];

            currentLine = (tokensInLine.LastOrDefault() == "{")
                ? "public " + returnValue + " " + methodName + "(" + paramObj + " " + parameter + ") {"
                : "public " + returnValue + " " + methodName + "(" + paramObj + " " + parameter + ")";
        }
    }

    private bool IsMethod(string allMatchesWhichAreWordsOrSpecialCharacters, List<string> tokensInLine, string nextLine)
    {
        bool hasLeftCurlyBracket = tokensInLine.LastOrDefault() == "{";
        if (!hasLeftCurlyBracket) hasLeftCurlyBracket = Regex.Match(nextLine, allMatchesWhichAreWordsOrSpecialCharacters).Value == "{";
        return tokensInLine.Count > 1 && (tokensInLine[1] == ":" || tokensInLine[2] == ":") && hasLeftCurlyBracket;
    }

    private string? TryCreateObjectType(string fileNameAndLocation)
    {
        var fileName     = Path.GetFileName(fileNameAndLocation);
        var fileLocation = Path.GetDirectoryName(fileNameAndLocation);

        EObjectType probablyTypeOff = fileName[0] switch
        {
            'a' => EObjectType.AbstractClass,
            'i' => EObjectType.Interface,
            'e' => EObjectType.Enum,
            's' => EObjectType.Struct,
            'r' => EObjectType.Record,
            _ => EObjectType.Class
        };

        if (probablyTypeOff == EObjectType.Class && !char.IsUpper(fileName[0]))
        {
            ConfigData.ERROR = true;
            TerminalHelper.Error("Object name should be a capital letter { exp: (Testable) } in", fileName);
            return null;
        }

        if (probablyTypeOff != EObjectType.Class && !char.IsUpper(fileName[1]))
        {
            ConfigData.ERROR = true;
            TerminalHelper.Error("Object name after object type definition should be a capital letter { exp: (iTestable) } in", fileName);
            return null;
        }

        var objectTemplate = File.ReadAllText(ConfigData.OBJECT_TEMPLATE);
        var objectName = fileName[0].ToString().ToUpper() + fileName[1..]; // capitalizing first letter
        objectTemplate = objectTemplate.Replace("{{OBJECTNAME}}", objectName);
        objectTemplate = objectTemplate.Replace("{{NAMESPACE}}", fileLocation.Replace("\\", ".").Replace("/", ".").Trim('.'));
        objectTemplate = objectTemplate.Replace("{{OBJECTTYPE}}", probablyTypeOff.ToString().ToLower());

        var newFileNameWithLocation = Path.Combine(fileLocation, objectName + ".cs");
        var addFileAtLocation = Path.Combine(ConfigData.BUILD_FOLDER_FROM_ROOT, newFileNameWithLocation);
        if (!Directory.Exists(Path.GetDirectoryName(addFileAtLocation))) Directory.CreateDirectory(Path.GetDirectoryName(addFileAtLocation));
        File.Create(addFileAtLocation).Close();
        File.WriteAllText(addFileAtLocation, objectTemplate);

        TerminalHelper.Log("Created object", objectName, ConsoleColor.Green);
        return addFileAtLocation;
    }

    private string CreateBuildFolderFromRootIfItDoesntExist()
    {
        var buildFileWithLocation = Path.Combine(ConfigData.CONFIG_FILE_JSON_DATA.RootBuildFileLocation, ConfigData.CONFIG_FILE_JSON_DATA.BuildFolderName);
        var buildFolderLocationFromRoot = Path.Combine(ConfigData.ROOT_PROJECT_LOCATION, buildFileWithLocation);
        if (!Directory.Exists(buildFolderLocationFromRoot)) Directory.CreateDirectory(buildFolderLocationFromRoot);
        return buildFolderLocationFromRoot;
    }

    private enum EObjectType
    {
        Class,
        AbstractClass,
        Interface,
        Enum,
        Struct,
        Record
    }
}
