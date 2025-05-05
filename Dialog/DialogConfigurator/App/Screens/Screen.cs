using DialogConfigurator.App.Helpers;
using DialogConfigurator.App.RenderingHelper;
using DialogConfigurator.App.Screens.ScreenConfig;
using DialogConfigurator.App.Ui;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DialogConfigurator.App.Screens;

public class Screen : IScreen
{
    private Texture2D _WindowTitleColorTex;

    private Texture2D _WindowExitColorTex;
    private Texture2D _WindowExitColorClickedTex;
    private Texture2D _WindowExitColorDefaultTex;


    private readonly Vector2   _WindowTitlePosition = new(10, 2);
    private readonly string    _WindowTitle         = "Dialog Configurator Tool";
    private readonly int       _WindowBarHeight     = 25;

    private          Vector2   _DraggingMouseInWindowPos;
    private          bool      _IsDragging = false;
    private          Rectangle _WindowBar;
    private          Rectangle _exitButton;

    public virtual void OnReset()
    {
        _WindowTitleColorTex = new Texture2D(RenderingObjects.SpriteBatch.GraphicsDevice, 1, 1);
        _WindowTitleColorTex.SetData(new Color[] { HexColorHelper.ColorFromHex("#424242") });
        _WindowBar = new Rectangle(0, 0, RenderingObjects.ScreenWidth - _WindowBarHeight, _WindowBarHeight);

        _WindowExitColorDefaultTex = new Texture2D(RenderingObjects.SpriteBatch.GraphicsDevice, 1, 1);
        _WindowExitColorDefaultTex.SetData(new Color[] { HexColorHelper.ColorFromHex("#ff0000") });

        _WindowExitColorClickedTex = new Texture2D(RenderingObjects.SpriteBatch.GraphicsDevice, 1, 1);
        _WindowExitColorClickedTex.SetData(new Color[] { HexColorHelper.ColorFromHex("#800000") });

        _WindowExitColorTex = _WindowExitColorDefaultTex;

        _exitButton = new Rectangle(RenderingObjects.ScreenWidth - _WindowBarHeight, 0, _WindowBarHeight, _WindowBarHeight);
    }

    public virtual void Draw()
    {
        UI.Tag<Button>(button => {
            button.Position = new Position(400, 400);
            button.Text     = "This is my New teXt!";
            button.Border   = new Border("#FF0000", 1);
            button.Padding  = new Padding(20, 10);
        });

        UI.Tag<ParentableElement>();
            UI.Tag<Button>();
           // UI.Tag<Button>();

            UI.Tag<ParentableElement>();
           //     UI.Tag<Button>();
            UI.EndTag();

        UI.EndTag();

        UI.Draw();

        RenderingObjects.SpriteBatch.Draw(_WindowTitleColorTex, _WindowBar, Color.Black);
        RenderingObjects.WindowTitleFont.DrawText(RenderingObjects.SpriteBatch, _WindowTitle, _WindowTitlePosition, Color.Wheat);

        RenderingObjects.SpriteBatch.Draw(_WindowExitColorTex, _exitButton, Color.Red);
        RenderingObjects.WindowTitleFont.DrawText(RenderingObjects.SpriteBatch, "X", new Vector2(_exitButton.X + 5, _exitButton.Y + 2), Color.Wheat);
    }

    public virtual void Update()
    {
        Vector2 mouseDesktopPos = RenderingObjects.Window.ClientBounds.Location.ToVector2() + RenderingObjects.MouseInput.Position.ToVector2();

        Vector2 mousePosInWindow = RenderingObjects.MouseInput.Position.ToVector2();
        if (_WindowBar.Contains(mousePosInWindow) && RenderingObjects.MouseInput.IsLeftButtonPressed() && !_IsDragging)
        {
            _IsDragging = true;
            _DraggingMouseInWindowPos = RenderingObjects.MouseInput.Position.ToVector2();
        }

        if (_IsDragging)
        {
            RenderingObjects.Window.Position = (mouseDesktopPos - _DraggingMouseInWindowPos).ToPoint();
            _IsDragging = !RenderingObjects.MouseInput.IsLeftButtonReleased();
        }

        if (_exitButton.Contains(mousePosInWindow))
        {
            _WindowExitColorTex = _WindowExitColorClickedTex;

            if (RenderingObjects.MouseInput.IsLeftButtonPressed())
            {
                RenderingObjects.Graphics.Dispose();
                RenderingObjects.Game.Exit();
            }
        }
        else
        {
            _WindowExitColorTex = _WindowExitColorDefaultTex;
        }
    }
}