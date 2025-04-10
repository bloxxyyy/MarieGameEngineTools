using ChampFleur.App.Data;
using ChampFleur.App.GameWindows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChampFleur.App;

public class OnStart : Game
{
    private IGameWindow gameWindow = new InGame();

    public OnStart()
    {
        WindowData.GRAPHICS = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        WindowData.SPRITEBATCH = new SpriteBatch(WindowData.GRAPHICSDEVICE);
        gameWindow.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        WindowData.GAMETIME = gameTime;
        gameWindow.Update();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        gameWindow.Draw();
    }

    private static void Main(string[] args)
    {
        using var OnStart = new OnStart();
        OnStart.Run();
    }
}
