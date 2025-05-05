using System.IO;

using DialogConfigurator.App.Input;

using FontStashSharp;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using DialogConfigurator.App.RenderingHelper;
using DialogConfigurator.App.Screens.ScreenConfig;
using MonoGame.Extended;
using static DialogConfigurator.App.Helpers.GradientHelper;

namespace DialogConfigurator.App.ProjectSetup;
public class Setup : Game
{
    private readonly KeyboardInput _keyboardInput = new();
    private readonly MouseInput    _mouseInput    = new();

    private Texture2D _GradientBackground;

    public Setup()
    {
        RenderingObjects.Game = this;
        RenderingObjects.Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory     = "Content";
        IsMouseVisible            = true;
        Window.IsBorderless       = true;
        RenderingObjects.Graphics.PreferredBackBufferWidth = 1600;
        RenderingObjects.Graphics.PreferredBackBufferHeight = 960;

        RenderingObjects.Graphics.ApplyChanges();
        Window.Position = new Point(
            GraphicsDevice.DisplayMode.Width / 2 - RenderingObjects.Graphics.PreferredBackBufferWidth / 2,
            GraphicsDevice.DisplayMode.Height / 2 - RenderingObjects.Graphics.PreferredBackBufferHeight / 2);
    }

    protected override void LoadContent()
    {
        RenderingObjects.SpriteBatch = new SpriteBatch(RenderingObjects.Graphics.GraphicsDevice);
        RenderingObjects.CurrentFontSystem = new FontSystem();
        //RenderingObjects.CurrentFontSystem.AddFont(File.ReadAllBytes("../../../Content/AnonymousPro-Bold.ttf"));
        //RenderingObjects.CurrentFontSystem.AddFont(File.ReadAllBytes("../../../Content/AnonymousPro-Regular.ttf"));
        RenderingObjects.CurrentFontSystem.AddFont(File.ReadAllBytes("../../../Content/Ubuntu-B.ttf"));
        RenderingObjects.CurrentFontSystem.AddFont(File.ReadAllBytes("../../../Content/Ubuntu-M.ttf"));

        RenderingObjects.FontBig         = RenderingObjects.CurrentFontSystem.GetFont(64);
        RenderingObjects.WindowTitleFont = RenderingObjects.CurrentFontSystem.GetFont(21);
        RenderingObjects.KeyboardInput   = _keyboardInput;
        RenderingObjects.MouseInput      = _mouseInput;

        RenderingObjects.WhitePixel = new Texture2D(GraphicsDevice, 1, 1);
        RenderingObjects.WhitePixel.SetData(new[] { Color.White });

        _GradientBackground = CreateGradientTexture(
            GraphicsDevice,
            GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height,
            Color.CornflowerBlue,
            ColorHelper.FromHex("#140a47"),
            angleDegrees: 45f,
            midpoint: 0.3f
        );
    }

    protected override void Initialize()
    {
        base.Initialize();
        RenderingObjects.Window       = Window;
        RenderingObjects.ScreenWidth  = GraphicsDevice.PresentationParameters.BackBufferWidth;
        RenderingObjects.ScreenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
        ScreenConfiguration.OnReset();
    }

    protected override void Update(GameTime gameTime)
    {
        RenderingObjects.KeyboardInput.Update();
        RenderingObjects.MouseInput.Update();

        RenderingObjects.GameTime = gameTime;
        if (RenderingObjects.KeyboardInput.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            Exit();

        ScreenConfiguration.Update();
    }

    protected override void Draw(GameTime gameTime)
    {
        RenderingObjects.ScreenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
        RenderingObjects.ScreenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
        RenderingObjects.GameTime = gameTime;
        RenderingObjects.Graphics.GraphicsDevice.Clear(ColorHelper.FromHex("#000000"));

        RenderingObjects.SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        RenderingObjects.SpriteBatch.Draw(_GradientBackground, Vector2.Zero, Color.White);
        ScreenConfiguration.Draw();
        RenderingObjects.SpriteBatch.End();
    }
}

public static class Program
{
    private static void Main()
    {
        using Setup game = new();
        game.Run();
    }
}