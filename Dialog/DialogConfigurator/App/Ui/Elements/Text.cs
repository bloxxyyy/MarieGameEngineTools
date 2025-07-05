using DialogConfigurator.App.Helpers;
using DialogConfigurator.App.RenderingHelper;
using DialogConfigurator.App.Ui.DataClasses;
using DialogConfigurator.App.Ui.Elements.TopLevelElements;

using FontStashSharp;

using Microsoft.Xna.Framework;

namespace DialogConfigurator.App.Ui.Elements;

internal class Text(string id = "") : UIElement(id) {

    internal string Data         { get; set; } = "Text";
    internal string FontHexColor { get; set; } = "#FFFFFF";

    private readonly DynamicSpriteFont font = RenderingObjects.FontDefault;

    internal override Rectangle GetBounds() {
        return new Rectangle(
           Position.X,
           Position.Y,
           (int)font.MeasureString(Data).X,
           font.LineHeight
       );
    }

    internal override void Draw() {
        RenderingObjects.SpriteBatch.DrawString(
            RenderingObjects.FontDefault,
            Data,
            new Vector2(Position.X, Position.Y),
            HexColorHelper.ColorFromHex(FontHexColor)
        );
    }
}
