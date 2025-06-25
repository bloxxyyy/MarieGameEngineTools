using System;

using Microsoft.Xna.Framework;

namespace DialogConfigurator.App.Ui.DataClasses;

internal readonly struct Click(Action onClick, Rectangle bounds) {
    internal readonly Action onClick = onClick;
    internal readonly Rectangle bounds = bounds;
}
