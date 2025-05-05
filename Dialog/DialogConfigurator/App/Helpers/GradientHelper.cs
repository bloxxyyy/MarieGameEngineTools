using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DialogConfigurator.App.Helpers;

internal static class GradientHelper {
    public static Texture2D CreateGradientTexture(
         GraphicsDevice graphicsDevice,
         int width,
         int height,
         Color startColor,
         Color endColor,
         float angleDegrees = 90f,
         float midpoint = 0.5f
        ) {

        Texture2D texture = new(graphicsDevice, width, height);
        Color[] colorData = new Color[width * height];

        // Convert angle to radians and get direction vector
        float angleRad = MathHelper.ToRadians(angleDegrees);
        Vector2 direction = new((float)Math.Cos(angleRad), (float)Math.Sin(angleRad));

        // Normalize and scale to fit texture
        Vector2 center = new(width / 2f, height / 2f);
        float maxLength = Vector2.Distance(Vector2.Zero, new Vector2(width, height));

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                Vector2 point = new Vector2(x, y) - center;

                // Project point onto gradient direction
                float projection = Vector2.Dot(point, direction) / maxLength;

                // Normalize to 0–1 with midpoint bias
                float t = MathHelper.Clamp(projection + midpoint, 0f, 1f);

                colorData[(y * width) + x] = Color.Lerp(startColor, endColor, t);
            }
        }

        texture.SetData(colorData);
        return texture;
    }
}
