using System;
using System.Collections.Generic;
using System.Linq;

using DialogConfigurator.App.Helpers;
using DialogConfigurator.App.Input;
using DialogConfigurator.App.RenderingHelper;
using DialogConfigurator.App.Ui.DataClasses;

using FontStashSharp;

using Microsoft.Xna.Framework;

using static System.Collections.Specialized.BitVector32;

namespace DialogConfigurator.App.Ui;


internal sealed class UI {
    internal ParentableElement BaseElement { get; set; }
    internal UIElement CurrentElement { get; set; }
    internal static Dictionary<string, Click> HitTestRegistry { get; } = [];
    internal static string LastClickedId { get; set; } = string.Empty;

    private UI() {
        BaseElement = new BaseWindowElement(typeof(UI).Name + Guid.NewGuid().ToString("N"));
        CurrentElement = BaseElement;
    }

    private static UI _Instance;
    internal static UI Instance => _Instance ??= new UI();

    internal static void EndTag() {
        if (Instance.CurrentElement != Instance.BaseElement) {
            Instance.CurrentElement = Instance.CurrentElement.ParentElement;
        }
    }

    internal static void Tag<T>(string id = "", Action<T> init = null) where T : UIElement {

        if (id?.Length == 0) id = typeof(T).Name + Guid.NewGuid().ToString("N");

        T element = ActivatorFactory.CreateWithId<T>(id);
        init?.Invoke(element);

        if (Instance.CurrentElement is not ParentableElement) EndTag();

        element.ParentElement = (ParentableElement)Instance.CurrentElement;
        element.ParentElement.Enqueue(element);

        Instance.CurrentElement = element;
    }

    internal static void Draw() {
        HitTestRegistry.Clear();
        Instance.BaseElement.Draw();
        Instance.CurrentElement = Instance.BaseElement;
    }

    internal static void ProcessInteractions() {
        MouseInput mouse = RenderingObjects.MouseInput;
        bool isLeftMouseButtonPressed = mouse.IsLeftButtonReleased();
        Vector2 mousePosition = mouse.Position.ToVector2();

        if (isLeftMouseButtonPressed) {

            KeyValuePair<string, Click>? clicked = HitTestRegistry.LastOrDefault(entry => entry.Value.bounds.Contains(mousePosition));
            LastClickedId = string.Empty; // Reset LastClickedId to empty string before checking for a click

            if (clicked == null) return;

            KeyValuePair<string, Click> kvp = (KeyValuePair<string, Click>)clicked;
            LastClickedId = kvp.Key;
            kvp.Value.onClick?.Invoke();
        }
    }
}

internal abstract class UIElement(string id = "") {
    internal string             Id            { get; } = id != "" ? id : typeof(UIElement).Name + Guid.NewGuid().ToString("N");
    internal Position           Position      { get; set; } = new(0, 0);
    internal ParentableElement  ParentElement { get; set; }
    
    internal abstract void      Draw();
    internal abstract Rectangle GetBounds();
}

internal abstract class ParentableElement(string id = "") : UIElement(id) {
    private readonly Queue<UIElement> _Elements = new();

    internal void Enqueue(UIElement element) => _Elements.Enqueue(element);

    internal override void Draw() {
        while (_Elements.Count > 0) {
            _Elements.Dequeue().Draw();
        }
    }
}

internal class InputField(string id = "") : UIElement(id), IInput {

    internal Text TextElement { get; set; } = new();
    internal string BackgroundHexColor { get; set; } = "#000000";
    internal Border? Border { get; set; } = null;
    internal Padding Padding { get; set; } = new Padding(0, 0);
    public Func<string> Consume { get; set; }

    internal override Rectangle GetBounds() {

        Rectangle textBounds = TextElement.GetBounds();

        return new Rectangle(
            Position.X,
            Position.Y,
            textBounds.Width + (Padding.LeftRight * 2),
            textBounds.Height + (Padding.TopBottom * 2)
        );
    }

    internal override void Draw() {

        Rectangle bounds = GetBounds();
        UI.HitTestRegistry.Add(Id, new Click(() => { }, bounds));

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

internal class Text(string id = "") : UIElement(id) {

    internal string Data         { get; set; } = "Text";
    internal string FontHexColor { get; set; } = "#FFFFFF";

    private readonly DynamicSpriteFont font = RenderingObjects.FontBig;

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
            RenderingObjects.FontBig,
            Data,
            new Vector2(Position.X, Position.Y),
            HexColorHelper.ColorFromHex(FontHexColor)
        );
    }
}

internal class Button(string id = "") : UIElement(id) {

    internal Text        TextElement        { get; set; } = new();
    internal string      BackgroundHexColor { get; set; } = "#000000";
    internal Border?     Border             { get; set; } = null;
    internal Padding     Padding            { get; set; } = new Padding(0, 0);
    internal Action?     OnClick            { get; set; } = null;

    internal override Rectangle GetBounds() {

        Rectangle textBounds = TextElement.GetBounds();

        return new Rectangle(
            Position.X,
            Position.Y,
            textBounds.Width + (Padding.LeftRight * 2),
            textBounds.Height + (Padding.TopBottom * 2)
        );
    }

    internal override void Draw() {
        Rectangle bounds = GetBounds();

        if (OnClick != null) {
            UI.HitTestRegistry.Add(Id, new Click(OnClick, bounds));
        }

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
