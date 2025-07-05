using DialogConfigurator.App.Helpers;
using DialogConfigurator.App.RenderingHelper;
using DialogConfigurator.App.Ui.DataClasses;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;

namespace DialogConfigurator.App.Ui;

internal static class DrawHelper {

    internal static void DrawHelperBackground(float width, float height, string color, Position pos, Border? border = null, Padding? padding = null) {
        Texture2D pixel = new(RenderingObjects.SpriteBatch.GraphicsDevice, 1, 1);
        pixel.SetData(new Color[] { HexColorHelper.ColorFromHex(color) });
        Rectangle rect = new(pos.X, pos.Y, (int)width, (int)height);

        RenderingObjects.SpriteBatch.Draw(pixel, rect, Color.White);

        if (border != null) DrawBorder(pos, width, height, (Border)border);
    }

    private static void DrawBorder(Position pos, float width, float height, Border border) {
        if (border.Size > 0) {
            RenderingObjects.SpriteBatch.DrawLine( // Top
                new Vector2(pos.X, pos.Y),
                new Vector2(pos.X + width, pos.Y),
                HexColorHelper.ColorFromHex(border.ColorHex),
                border.Size
            );

            RenderingObjects.SpriteBatch.DrawLine( // Left
                new Vector2(pos.X, pos.Y),
                new Vector2(pos.X, pos.Y + height),
                HexColorHelper.ColorFromHex(border.ColorHex),
                border.Size
            );

            RenderingObjects.SpriteBatch.DrawLine( // Right
                new Vector2(pos.X + width, pos.Y),
                new Vector2(pos.X + width, pos.Y + height),
                HexColorHelper.ColorFromHex(border.ColorHex),
                border.Size
            );

            RenderingObjects.SpriteBatch.DrawLine( // Bottom
                new Vector2(pos.X, pos.Y + height),
                new Vector2(pos.X + width, pos.Y + height),
                HexColorHelper.ColorFromHex(border.ColorHex),
                border.Size
            );
        }
    }
}