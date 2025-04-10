using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ChampFleur.App.Data;
public static class WindowData
{
    public static GraphicsDeviceManager GRAPHICS;
    public static SpriteBatch SPRITEBATCH;
    public static GameTime GAMETIME;
    public static GraphicsDevice GRAPHICSDEVICE => GRAPHICS.GraphicsDevice;


}
