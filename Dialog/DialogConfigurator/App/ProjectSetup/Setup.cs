using System.IO;
using DialogConfigurator.App.Input;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YuiConfigurator.App.RenderingHelper;
using YuiConfigurator.App.Screens.ScreenConfig;

namespace YuiConfigurator.App.Setup;
public class Setup : Game
{
    private readonly KeyboardInput _keyboardInput = new();
    private readonly MouseInput    _mouseInput    = new();

    public Setup()
    {
        RenderingObjects.Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory     = "Content";
        IsMouseVisible            = true;
        Window.IsBorderless       = true;
        RenderingObjects.Graphics.PreferredBackBufferWidth = 800;
        RenderingObjects.Graphics.PreferredBackBufferHeight = 480;

        RenderingObjects.Graphics.ApplyChanges();
        Window.Position = new Point(
            (GraphicsDevice.DisplayMode.Width / 2) - (RenderingObjects.Graphics.PreferredBackBufferWidth / 2),
            (GraphicsDevice.DisplayMode.Height / 2) - (RenderingObjects.Graphics.PreferredBackBufferHeight / 2));
    }

    protected override void LoadContent()
    {
        RenderingObjects.SpriteBatch = new SpriteBatch(RenderingObjects.Graphics.GraphicsDevice);
        RenderingObjects.CurrentFontSystem = new FontSystem();
        RenderingObjects.CurrentFontSystem.AddFont(File.ReadAllBytes("../../../Content/AnonymousPro-Bold.ttf"));
        RenderingObjects.CurrentFontSystem.AddFont(File.ReadAllBytes("../../../Content/AnonymousPro-Regular.ttf"));

        RenderingObjects.FontBig         = RenderingObjects.CurrentFontSystem.GetFont(30);
        RenderingObjects.WindowTitleFont = RenderingObjects.CurrentFontSystem.GetFont(21);
        RenderingObjects.KeyboardInput   = _keyboardInput;
        RenderingObjects.MouseInput      = _mouseInput;
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
        RenderingObjects.Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        RenderingObjects.SpriteBatch.Begin();
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