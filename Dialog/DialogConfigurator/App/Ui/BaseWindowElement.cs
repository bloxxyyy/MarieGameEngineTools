using Microsoft.Xna.Framework;

namespace DialogConfigurator.App.Ui;

/// <summary>
/// Dont use, this is just as a base class for the UI.
/// </summary>
internal class BaseWindowElement(string id) : ParentableElement(id) {
    internal override Rectangle GetBounds() => new(0,0,0,0);
}