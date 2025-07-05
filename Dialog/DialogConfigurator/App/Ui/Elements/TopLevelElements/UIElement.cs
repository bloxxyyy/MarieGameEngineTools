using System;

using DialogConfigurator.App.Ui.DataClasses;

using Microsoft.Xna.Framework;

namespace DialogConfigurator.App.Ui.Elements.TopLevelElements;

internal abstract class UIElement(string id = "") {
    internal string             Id            { get; } = GetOrGenerateId(id);
    internal Position           Position      { get; set; } = new(0, 0);
    internal ParentableElement  ParentElement { get; set; }
    
    internal abstract void      Draw();
    internal abstract Rectangle GetBounds();

    private static string GetOrGenerateId(string id) {
        return id != "" ? id : typeof(UIElement).Name + Guid.NewGuid().ToString("N");
    }
}
