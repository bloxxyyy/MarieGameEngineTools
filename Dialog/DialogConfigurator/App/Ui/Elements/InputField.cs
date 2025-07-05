using System;
using System.Collections.Generic;

using DialogConfigurator.App.Ui.DataClasses;
using DialogConfigurator.App.Ui.Elements.TopLevelElements;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using KeyboardInput = DialogConfigurator.App.Input.KeyboardInput;

namespace DialogConfigurator.App.Ui.Elements;

internal class InputField(string id) : UIElement(id) {

    private static readonly Dictionary<string, string> _textStates = new();

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
        // update the text to the updated text state after change.
        TextElement.Data = _textStates.TryGetValue(Id, out string text) ? text : TextElement.Data;

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

    public void CaptureInput(Keys[] pressedKeys, KeyboardInput keyboardInput) {
        if (!_textStates.TryGetValue(Id, out string text))
            text = TextElement.Data;

        foreach (Keys key in pressedKeys) {
            if (!keyboardInput.IsKeyPressed(key))
                continue;

            if (key >= Keys.A && key <= Keys.Z) {
                char c = (char)('a' + (key - Keys.A));
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) ||
                    Keyboard.GetState().IsKeyDown(Keys.RightShift)) {
                    c = char.ToUpper(c);
                }
                text += c;
            }
            else if (key == Keys.Space) {
                text += " ";
            }
            else if (key == Keys.Back && text.Length > 0) {
                text = text[..^1];
            }
        }

        _textStates[Id] = text;
        TextElement.Data = text;
    }
}
