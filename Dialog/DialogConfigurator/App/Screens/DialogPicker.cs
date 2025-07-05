using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DialogConfigurator.App.RenderingHelper;
using DialogConfigurator.App.Ui;
using DialogConfigurator.App.Ui.DataClasses;
using DialogConfigurator.App.Ui.Elements;

namespace DialogConfigurator.App.Screens;

public class DialogPicker : Screen
{
    private Texture2D _pixel;

    public override void OnReset()
    {
        base.OnReset();
        _pixel = new Texture2D(RenderingObjects.SpriteBatch.GraphicsDevice, 1, 1);
        _pixel.SetData(new Color[] { Color.White });
    }

    public int xHeight = 20;

    public override void Draw()
    {
        base.Draw();

        // on key press ] xHeight++;
        if (RenderingObjects.KeyboardInput.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.OemCloseBrackets)) {
            xHeight++;
        }

        UI.Tag<InputField>("MyTestInputId", inputField => {
            inputField.Position = new Position(100, 200);
            inputField.Border = new Border("#FF0000", 1);
            inputField.Padding = new Padding(10, 10);
            inputField.OnClick = () => { };
            inputField.TextElement = new Text() {
                Data = "Hello World2!",
                FontHexColor = "#FFFFFF",
                Position = new Position(0, 0)
            };
        });

        UI.Tag<Text>("MyTestId", text => {
            text.Data = "Hello World!";
            text.Position = new Position(100, 100);
        });

        UI.Tag<Button>("MyTestId2", button => {
            button.Position = new Position(400, 400);
            button.Border = new Border("#FF0000", 1);
            button.Padding = new Padding(10, xHeight);
            button.OnClick = () => xHeight++;
            button.TextElement = new Text() {
                Data = "MyButton",
                FontHexColor = "#FFFFFF",
                Position = new Position(0, 0)
            };
        });

        UI.Draw();
    }

    public override void Update() {
        base.Update();
        UI.ProcessInteractions();
    }
}
