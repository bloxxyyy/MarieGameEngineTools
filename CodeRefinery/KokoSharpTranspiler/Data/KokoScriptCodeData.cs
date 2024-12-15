namespace KokoSharpTranspiler.Data;

internal record KokoScriptCodeData(string Data)
{
    public static implicit operator KokoScriptCodeData(string name) => new(name);
    public static implicit operator string(KokoScriptCodeData fileData) => fileData.Data;
}
