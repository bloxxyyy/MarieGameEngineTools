using Microsoft.Xna.Framework;

namespace DialogConfigurator.App.Helpers;
internal static class HexColorHelper
{
    internal static Color ColorFromHex(string hex)
    {
        if (hex.StartsWith('#'))
            hex = hex[1..];

        byte r = byte.Parse(hex[..2],            System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color(r, g, b);
    }
}
