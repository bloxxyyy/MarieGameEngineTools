using DialogConfigurator.App.Input;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YuiConfigurator.App.RenderingHelper;
public static class RenderingObjects
{
    public static SpriteBatch           SpriteBatch       { get; internal set; }
    public static GraphicsDeviceManager Graphics          { get; internal set; }
    public static GameTime              GameTime          { get; internal set; }
    public static FontSystem            CurrentFontSystem { get; internal set; }
    public static DynamicSpriteFont     WindowTitleFont   { get; internal set; }
    public static DynamicSpriteFont     FontBig           { get; internal set; }
    public static int                   ScreenWidth       { get; internal set; }
    public static int                   ScreenHeight      { get; internal set; }
    public static KeyboardInput         KeyboardInput     { get; internal set; }
    public static MouseInput            MouseInput        { get; internal set; }
    public static GameWindow            Window            { get; internal set; }
}
