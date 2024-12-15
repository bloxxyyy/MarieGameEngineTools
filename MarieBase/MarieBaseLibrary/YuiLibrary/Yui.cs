namespace MarieBaseLibrary.YuiLibrary;
public class Yui
{
    private readonly Dictionary<Label, DisplayableBase> _displayables = [];

    public void AddDisplayable<T>(Label label, Label? parent = null) where T : DisplayableBase, new()
    {
        DisplayableBase displayable = (DisplayableBase?)Activator.CreateInstance(typeof(T), label) ?? throw new InvalidOperationException();
        _displayables.Add(label, displayable);

        if (parent is null) return;

        _displayables.TryGetValue(parent, out DisplayableBase? parentDisplayable);
        if (parentDisplayable is null) throw new InvalidOperationException();

        try
        {
            var convertedObj = (IContainable)parentDisplayable;
            convertedObj.AddChild(label, displayable);
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The object cannot be cast to IContainable.", ex);
        }
    }
}

public class TextField(Label label) : DisplayableBase(label)
{
    
}

public class Grid(Label label) : DisplayableBase(label), IContainable
{
    private readonly Dictionary<Label, DisplayableBase> _displayables = [];
    public void AddChild(Label label, DisplayableBase childObject) => _displayables.Add(label, childObject);
}

public interface IContainable
{
    public void AddChild(Label label, DisplayableBase childObject);
}

public record Label (string Text)
{
    public static implicit operator Label(string Text) => new(Text);
}

public abstract class DisplayableBase(Label label)
{
    public Label Label { get; } = label;
}