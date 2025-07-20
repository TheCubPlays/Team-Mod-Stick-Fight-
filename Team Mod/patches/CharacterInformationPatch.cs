using HarmonyLib;
using UnityEngine;

namespace TMOD;

class CharacterInformationPatch
{
    public static void Patch(Harmony harmonyInstance)
    {
        // If you wanna know how patching works, check FightingPatch.cs because I don't wanna repeat.
        var startMethod = AccessTools.Method(typeof(CharacterInformation), "Start");
        var startMethodPostfix = new HarmonyMethod(typeof(CharacterInformationPatch).GetMethod(nameof(StartMethodPostfix)))
        {
            priority = Priority.Last
        };
        harmonyInstance.Patch(startMethod, postfix: startMethodPostfix);
    }

    // It's for when you join the main lobby, we just setup the color.
    public static void StartMethodPostfix(CharacterInformation __instance)
    {
        if (MatchmakingHandler.Instance.IsInsideLobby) return;

        var customTeamColor = ConfigHandler.GetEntry<Color>("TeamColor");
        var isCustomPlayerColor = customTeamColor != ConfigHandler.GetEntry<Color>("TeamColor", true);
        var color = isCustomPlayerColor ? customTeamColor : new Color(0f, 0f, 1f, 1f);
        if (!(Helper.customTeamColorToggle && Helper.customAllColorToggle))
        {
            if (!Plugin.isQolEnabled)
            {
                color = Helper.getRGBFromColor("yellow");
            }
            else
            {
                color = QOLConfigHandler.isCustomColor() ? QOLConfigHandler.GetCustomColor() : QOLConfigHandler.getColor(0);
            }
        }
        if (!Helper.useQolColorsToggle || !Plugin.isQolEnabled)
        {
            MultiplayerManagerPatches.ChangeAllCharacterColors(color, __instance.gameObject);
        }
        Plugin.InitModText();
    }
}
