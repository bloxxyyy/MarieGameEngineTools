namespace KokoSharpTranspiler.Data;

internal record FileNameAndLocation(string Name)
{
    public static implicit operator FileNameAndLocation(string name) => new(name);
    public static implicit operator string(FileNameAndLocation fileNameAndLocation) => fileNameAndLocation.Name;
}
