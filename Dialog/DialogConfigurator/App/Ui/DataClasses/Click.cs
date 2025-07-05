using System;

using DialogConfigurator.App.Ui.Elements.TopLevelElements;

using Microsoft.Xna.Framework;

namespace DialogConfigurator.App.Ui.DataClasses;

internal readonly struct Click(Action onClick, Rectangle bounds, UIElement obj) {
    internal readonly Action onClick = onClick;
    internal readonly Rectangle bounds = bounds;
    internal readonly UIElement obj = obj;
}
