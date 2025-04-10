using YuiConfigurator.App.RenderingHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YuiConfigurator.App.Screens;

public class DialogPicker : Screen
{
    private Texture2D _pixel;

    public override void OnReset()
    {
        base.OnReset();
        _pixel = new Texture2D(RenderingObjects.SpriteBatch.GraphicsDevice, 1, 1);
        _pixel.SetData(new Color[] { Color.White });
    }

    public override void Draw()
    {
        base.Draw();
        RenderingObjects.SpriteBatch.Draw(_pixel, new Rectangle(100, 100, 100, 100), Color.Red);
        RenderingObjects.FontBig.DrawText(RenderingObjects.SpriteBatch, "Skibidi", new System.Numerics.Vector2(400, 100), Color.Black);
    }
}
