/*
QOL Mod Compatibility

This is used to gather necessary information regarding the QOL Mod's config settings
*/

using BepInEx;
using System.IO;
using UnityEngine;

namespace TMOD;

public static class QOLConfigHandler
{
    // Gets the command prefix for the QOL Mod
    public static string GetPrefix()
    {
        string configPath = Path.Combine(Paths.ConfigPath, "monky.plugins.QOL.cfg");

        foreach (var line in File.ReadAllLines(configPath))
        {
            if (line.Trim().StartsWith("CommandPrefix"))
            {
                var split = line.Split('=');
                if (split.Length == 2)
                    return split[1].Trim();
            }
        }

        return "/";

    }
    // Gets QOL's default color for Yellow
    public static string GetYellow()
    {
        string configPath = Path.Combine(Paths.ConfigPath, "monky.plugins.QOL.cfg");

        foreach (var line in File.ReadAllLines(configPath))
        {
            if (line.Trim().StartsWith("DefaultPlayerColors"))
            {
                var split = line.Split('=');
                if (split.Length == 2)
                    return split[1].Split(' ')[1];
            }
        }

        return "D88C47";

    }
    // Gets QOL's default color for Blue
    public static string GetBlue()
    {
        string configPath = Path.Combine(Paths.ConfigPath, "monky.plugins.QOL.cfg");

        foreach (var line in File.ReadAllLines(configPath))
        {
            if (line.Trim().StartsWith("DefaultPlayerColors"))
            {
                var split = line.Split('=');
                if (split.Length == 2)
                    return split[1].Split(' ')[2];
            }
        }

        return "5573AD";

    }
    // Gets QOL's default color for Red
    public static string GetRed()
    {
        string configPath = Path.Combine(Paths.ConfigPath, "monky.plugins.QOL.cfg");

        foreach (var line in File.ReadAllLines(configPath))
        {
            if (line.Trim().StartsWith("DefaultPlayerColors"))
            {
                var split = line.Split('=');
                if (split.Length == 2)
                    return split[1].Split(' ')[3];
            }
        }

        return "D6554D";

    }
    // Gets QOL's default color for Green
    public static string GetGreen()
    {
        string configPath = Path.Combine(Paths.ConfigPath, "monky.plugins.QOL.cfg");

        foreach (var line in File.ReadAllLines(configPath))
        {
            if (line.Trim().StartsWith("DefaultPlayerColors"))
            {
                var split = line.Split('=');
                if (split.Length == 2)
                    return split[1].Split(' ')[4];
            }
        }

        return "578B49";

    }
    // Checks whether the QOL Mod has a custom color or not
    public static bool isCustomColor()
    {
        string default_value = "FFFFFFFF";
        string configPath = Path.Combine(Paths.ConfigPath, "monky.plugins.QOL.cfg");

        foreach (var line in File.ReadAllLines(configPath))
        {
            if (line.Trim().StartsWith("CustomColor"))
            {
                var split = line.Split('=');
                if (split.Length == 2)
                    return !(split[1] == default_value);
            }
            else if (line.Trim().StartsWith("# Default value:"))
            {
                var split = line.Split(':');
                if (split.Length == 2)
                    default_value = split[1];
            }
        }

        return false;

    }
    // Gets QOL's custom color for the local player
    public static Color GetCustomColor()
    {
        string configPath = Path.Combine(Paths.ConfigPath, "monky.plugins.QOL.cfg");
        Color color;

        if (File.Exists(configPath))
        {
            foreach (var line in File.ReadAllLines(configPath))
            {
                if (line.Trim().StartsWith("CustomColor"))
                {
                    var split = line.Split('=');
                    if (split.Length == 2)
                    {
                        string hex = split[1].Trim().Replace("#", "");
                        return ColorUtils.HexToColor(hex, out color);
                    }
                }
            }
        }

        return ColorUtils.HexToColor("FFFFFF", out color);
    }
    // Returns QOL's custom colour for a player using their Spawn ID
    public static Color getColor(ushort spawnID)
    {
        Color color;
        switch (spawnID)
        {
            case 0:
                ColorUtils.HexToColor(GetYellow(), out color);
                break;
            case 1:
                ColorUtils.HexToColor(GetBlue(), out color);
                break;
            case 2:
                ColorUtils.HexToColor(GetRed(), out color);
                break;
            case 3:
                ColorUtils.HexToColor(GetGreen(), out color);
                break;
            default:
                ColorUtils.HexToColor(GetYellow(), out color);
                break;
        }
        return color;
    }
}
