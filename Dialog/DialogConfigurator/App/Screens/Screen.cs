using System;
using System.Diagnostics;
using DialogConfigurator.App.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YuiConfigurator.App.RenderingHelper;
using YuiConfigurator.App.Screens.ScreenConfig;

namespace YuiConfigurator.App.Screens;

public class Screen : IScreen
{
    private Texture2D _pixel;

    private readonly Color     _WindowTitleColor    = Color.Wheat;
    private readonly Vector2   _WindowTitlePosition = new(10, 2);
    private readonly string    _WindowTitle         = "Dialog Configurator Tool";
    private readonly int       _WindowBarHeight     = 25;

    private          Vector2   _DraggingMouseInWindowPos;
    private          bool      _IsDragging = false;
    private          Rectangle _WindowBar;

    public virtual void OnReset()
    {
        _pixel = new Texture2D(RenderingObjects.SpriteBatch.GraphicsDevice, 1, 1);
        _pixel.SetData(new Color[] { ColorHelper.ColorFromHex("#424242") });
        _WindowBar = new Rectangle(0, 0, RenderingObjects.ScreenWidth, _WindowBarHeight);
    }

    public virtual void Draw()
    {
        RenderingObjects.SpriteBatch.Draw(_pixel, _WindowBar, _WindowTitleColor);
        RenderingObjects.WindowTitleFont.DrawText(RenderingObjects.SpriteBatch, _WindowTitle, _WindowTitlePosition, Color.Wheat);
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
    }
}