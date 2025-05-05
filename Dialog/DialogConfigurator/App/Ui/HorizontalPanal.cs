using System;
using System.Collections.Generic;

using DialogConfigurator.App.Helpers;
using DialogConfigurator.App.RenderingHelper;

using FontStashSharp;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;

namespace DialogConfigurator.App.Ui;

internal sealed class UI {

    internal ParentableElement BaseElement { get; set; }
    internal UIElement CurrentElement { get; set; }

    private UI() {
        BaseElement = new ParentableElement();
        CurrentElement = BaseElement;
    }

    private static UI _Instance;
    public  static UI Instance => _Instance ??= new UI();

    public static void EndTag() {
        Instance.CurrentElement = _Instance.CurrentElement.ParentElement;
    }

    public static void Tag<T>(Action<T> init = null) where T : UIElement, new() {
        T element = new();
        init?.Invoke(element);

        if (Instance.CurrentElement is not ParentableElement) EndTag();

        if (Instance.CurrentElement is ParentableElement parentableElement) {
            element.ParentElement = Instance.CurrentElement;
            parentableElement.Enqueue(element);
        }

        Instance.CurrentElement = element;
    }

    public static void Draw() {
        Instance.BaseElement.Draw();
        Instance.CurrentElement = Instance.BaseElement;
    }
}

internal struct Position(int x, int y) {
    internal int X = x;
    internal int Y = y;
}

internal struct Padding(int leftRight, int topBottom) {
    internal int LeftRight = leftRight;
    internal int TopBottom = topBottom;
}

internal struct Border(string colorHex, int size) {
    internal string ColorHex = colorHex;
    internal int    Size     = size;
}

internal class UIElement() {
    internal Position  Position      { get; set; } = new Position(0, 0);
    internal UIElement ParentElement { get; set; }

    internal virtual void Draw() {}
}

internal class ParentableElement() : UIElement() {
    private readonly Queue<UIElement> _Elements = new();

    internal void Enqueue(UIElement element) => _Elements.Enqueue(element);

    internal override void Draw() {
        while (_Elements.Count > 0) {
            _Elements.Dequeue().Draw();
        }
    }
}

internal class Button : UIElement {
    public string  Text               { get; set; } = "Button";
    public string  FontHexColor       { get; set; } = "#FFFFFF";
    public string  BackgroundHexColor { get; set; } = "#000000";
    public Border? Border             { get; set; } = null;
    public Padding Padding            { get; set; } = new Padding(0, 0);

    internal override void Draw() {
        DynamicSpriteFont font = RenderingObjects.FontBig;
        Vector2 size = font.MeasureString(Text);

        DrawHelper.DrawHelperBackground(size.X, font.LineHeight, BackgroundHexColor, Position, Border, Padding);

        if (Padding.LeftRight > 0) Position = new Position(Position.X + (Padding.LeftRight / 2), Position.Y);
        if (Padding.TopBottom > 0) Position = new Position(Position.X, Position.Y + (Padding.TopBottom / 2));

         RenderingObjects.SpriteBatch.DrawString(font, Text, new Vector2(Position.X, Position.Y), HexColorHelper.ColorFromHex(FontHexColor));
    }
}

internal static class DrawHelper {
    internal static void DrawHelperBackground(float width, float height, string color, Position pos, Border? border = null, Padding? padding = null) {
        if (padding != null) {
            width += ((Padding)padding).LeftRight;
            height += ((Padding)padding).TopBottom;
        }

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