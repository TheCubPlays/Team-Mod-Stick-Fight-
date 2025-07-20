using UnityEngine;

namespace TMOD;

// Gets a string hex code and turns it into a unityengine color
public static class ColorUtils
{
    public static Color HexToColor(string hex, out Color color)
    {
        if (!hex.StartsWith("#"))
            hex = "#" + hex;

        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
}
