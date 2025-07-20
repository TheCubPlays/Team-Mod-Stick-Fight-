/* Credits to Monky for InitModText, though the rest was also inspired by him.
Monky's files: Commands.cs, ChatManagerPatches.cs, Helper.cs (Mostly), PlayerUtils.cs
My files: ChatCommands.cs, CharacterInformationPatch.cs, FightingPatch.cs, OnLeavePatch.cs, WinTextPatch.cs, ConfigHandler.cs (Mostly), ColorUtils.cs, QOLConfigHandler.cs, ModLogger.cs
Monky's files that were modified a good bit by me: MultiplayerManagerPaches.cs, Plugin.cs

Note: I only grabbed what was essential from Monky's files (QOL Mod), so don't assume that's all he made in those. I also cut certain parts from some methods.

Monky: https://github.com/Mn0ky
*/

using System;
using System.Collections;
using BepInEx;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BepInEx.Bootstrap;


namespace TMOD;

[BepInPlugin(Guid, "TMOD", VersionNumber)]
[BepInProcess("StickFight.exe")]

public class Plugin : BaseUnityPlugin
{
    /*
    QOL Mod Compatibility

    Checking if the QOL Mod is being used
    */
    public static bool isQolEnabled = false;
    public static bool IsQOLModLoaded()
    {
        string qolModGUID = "monky.plugins.QOL";

        return Chainloader.PluginInfos.ContainsKey(qolModGUID);
    }
    private void Awake()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.2f); // We wait for 0.2 seconds before loading the mod, that's because we want all other mods to load first so that we can unpatch their methods when needed.
        if (IsQOLModLoaded())
        {
            isQolEnabled = true;
        }
        Logger.LogInfo("Plugin " + Guid + " is loaded! [v" + VersionNumber + "]");

        try
        {
            Harmony harmony = new("cub.TMOD");
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
        if (isQolEnabled)
        {
            modTextTMP.text = "<color=green>Cub's Team Mod</color> " + "<color=orange>v" + VersionNumber + " <color=white>&</color> " + "<color=#FFFFFF00>                                         .</color>";
        }
        else
        {
            modTextTMP.text = "<color=green>Cub's Team Mod</color> " + "<color=orange>v" + VersionNumber;
        }

        modTextTMP.fontSizeMax = 25;
        modTextTMP.fontSize = 25;
        modTextTMP.enableAutoSizing = true;
        modTextTMP.color = Color.red;
        modTextTMP.fontStyle = FontStyles.Bold;
        modTextTMP.alignment = TextAlignmentOptions.TopRight;
        modTextTMP.richText = true; 
    }
    public const string VersionNumber = "1.2.2"; // Version number
    public const string Guid = "cub.plugins.TMOD"; 

}
