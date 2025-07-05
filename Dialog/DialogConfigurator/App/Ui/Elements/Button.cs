using System;

using DialogConfigurator.App.Ui.DataClasses;
using DialogConfigurator.App.Ui.Elements.TopLevelElements;

using Microsoft.Xna.Framework;

namespace DialogConfigurator.App.Ui.Elements;

internal class Button(string id) : UIElement(id) {

    internal Text        TextElement        { get; set; } = new();
    internal string      BackgroundHexColor { get; set; } = "#000000";
    internal Border?     Border             { get; set; } = null;
    internal Padding     Padding            { get; set; } = new Padding(0, 0);
    internal Action      OnClick            { get; set; } = null;

    internal override Rectangle GetBounds() {

        Rectangle textBounds = TextElement.GetBounds();

        return new Rectangle(
            Position.X,
            Position.Y,
            textBounds.Width + Padding.LeftRight * 2,
            textBounds.Height + Padding.TopBottom * 2
        );
    }

    internal override void Draw() {
        Rectangle bounds = GetBounds();

        if (OnClick != null) UI.HitTestRegistry.Add(Id, new Click(OnClick, bounds, this));

        DrawHelper.DrawHelperBackground(
            bounds.Width,
            bounds.Height,
            BackgroundHexColor,
            Position,
            Border,
            Padding
        );

        float textX = Position.X + Padding.LeftRight;
        float textY = Position.Y + Padding.TopBottom;
        TextElement.Position = new Position((int)textX, (int)textY);
        TextElement.Draw();
    }
}
