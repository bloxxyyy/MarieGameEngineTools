using System;
using System.Collections.Generic;
using System.Linq;

using DialogConfigurator.App.Input;
using DialogConfigurator.App.RenderingHelper;
using DialogConfigurator.App.Ui.DataClasses;
using DialogConfigurator.App.Ui.Elements;
using DialogConfigurator.App.Ui.Elements.TopLevelElements;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using KeyboardInput = DialogConfigurator.App.Input.KeyboardInput;

namespace DialogConfigurator.App.Ui;

internal sealed class UI {

    // Single ton instance
    private  static UI _Instance;
    internal static UI Instance => _Instance ??= new UI();

    internal ParentableElement BaseElement    { get; set; }
    internal UIElement         CurrentElement { get; set; }

    internal static Dictionary<string, Click> HitTestRegistry { get; }      = [];
    internal static string                    LastClickedId   { get; set; } = string.Empty;

    private UI() {
        BaseElement = new BaseWindowElement(typeof(UI).Name + Guid.NewGuid().ToString("N"));
        CurrentElement = BaseElement;
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

    internal static void EndTag() {
        if (Instance.CurrentElement != Instance.BaseElement) {
            Instance.CurrentElement = Instance.CurrentElement.ParentElement;
        }
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

            KeyValuePair<string, Click>? clicked = HitTestRegistry
                .LastOrDefault(entry => entry.Value.bounds.Contains(mousePosition));

            LastClickedId = string.Empty; // Reset LastClickedId to empty string before checking for a click

            if (clicked == null) return;

            KeyValuePair<string, Click> kvp = (KeyValuePair<string, Click>)clicked;
            LastClickedId = kvp.Key;
            kvp.Value.onClick?.Invoke();
        }

        if (LastClickedId != null && LastClickedId != string.Empty) {
            KeyboardInput keyboardInput = RenderingObjects.KeyboardInput;
            if (HitTestRegistry.TryGetValue(LastClickedId, out Click click) && click.obj is InputField inputField) {

                inputField.CaptureInput(
                    Keyboard.GetState().GetPressedKeys(),
                    RenderingObjects.KeyboardInput
                );
            }
        }
    }
}
