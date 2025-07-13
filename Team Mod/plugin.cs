/* Credits to Monky for InitModText, though the rest was also inspired by him.
Monky's files: Commands.cs, ChatManagerPatches.cs, Helper.cs (Mostly), HSBColor.cs, PlayerUtils.cs
My files: ChatCommands.cs, CharacterInformationPatch.cs, FightingPatch.cs, OnLeavePatch.cs, WinTextPatch.cs, ConfigHandler.cs (Mostly), ModLogger.cs
Monky's files that were modified a good bit by me: MultiplayerManagerPaches.cs, Plugin.cs

Note: I only grabbed what was essential from Monky's files (QOL Mod), so don't assume that's all he made in those. I also cut certain parts from some methods.

Monky: https://github.com/Mn0ky
*/

using System;
using BepInEx;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMOD;

[BepInPlugin(Guid, "TMOD", VersionNumber)]
[BepInProcess("StickFight.exe")]

public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        // Runs when the plugin starts
        Logger.LogInfo("Plugin " + Guid + " is loaded! [v" + VersionNumber + "]");
        try
        {
            Harmony harmony = new("cub.TMOD"); // Creates harmony instance with identifier
            Logger.LogInfo("Applying ChatManager patch...");
            ChatManagerPatches.Patches(harmony);
            Logger.LogInfo("Applying Fighting patch...");
            FightingPatch.Patches(harmony);
            Logger.LogInfo("Applying OnLeave patch...");
            OnLeavePatch.Patches(harmony);
            Logger.LogInfo("Applying CharacterInformation patch...");
            CharacterInformationPatch.Patch(harmony);
            Logger.LogInfo("Applying MultiplayerManager patch...");
            MultiplayerManagerPatches.Patches(harmony);
            Logger.LogInfo("Applying WinText patch...");
            WinTextPatch.Patches(harmony);
        }
        catch (Exception ex)
        {
            Logger.LogError("Exception on applying patches: " + ex.InnerException);
        }
        InitModText();
        try
        {
            Logger.LogInfo("Loading configuration options from config file...");
            ConfigHandler.InitializeConfig(Config);
        }
        catch (Exception ex)
        {
            Logger.LogError("Exception on loading configuration: " + ex.StackTrace + ex.Message + ex.Source +
                            ex.InnerException);
        }

    }
    public static void InitModText()
    {
        var modText = new GameObject("ModText");
        var canvas = modText.AddComponent<Canvas>();
        var canvasScaler = modText.AddComponent<CanvasScaler>();
        var modTextTMP = modText.AddComponent<TextMeshProUGUI>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);

        modTextTMP.text = "<color=green>Cub's Team Mod</color> " + "<color=orange>v" + VersionNumber;

        modTextTMP.fontSizeMax = 25;
        modTextTMP.fontSize = 25;
        modTextTMP.enableAutoSizing = true;
        modTextTMP.color = Color.red;
        modTextTMP.fontStyle = FontStyles.Bold;
        modTextTMP.alignment = TextAlignmentOptions.TopRight;
        modTextTMP.richText = true;
    }
    public const string VersionNumber = "1.1.1"; // Version number
    public const string Guid = "cub.plugins.TMOD"; 
}
