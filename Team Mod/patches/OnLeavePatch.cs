// Used to update the player's team once a teammate leaves. I think everything here is self-explainatory.

using HarmonyLib;
using Steamworks;
using System.Collections.Generic;

namespace TMOD;

public class OnLeavePatch 
{
    public static void Patches(Harmony harmonyInstance) 
    {
        // If you wanna know how patching works, check FightingPatch.cs because I don't wanna repeat.
        var onPlayerLeftMethod = AccessTools.Method(typeof(MultiplayerManager),"OnPlayerLeft",new[] {typeof(CSteamID)});

        var onPlayerLeftPostfix = new HarmonyMethod(typeof(OnLeavePatch),nameof(OnPlayerLeftPostfix));

        harmonyInstance.Patch(onPlayerLeftMethod, postfix: onPlayerLeftPostfix);
    }

    public static void OnPlayerLeftPostfix(CSteamID SteamID, MultiplayerManager __instance)
    {
        var toRemove = new List<string>();

        // We can't modify a list while iterating it so we make a copy for iteration.
        foreach (string teammate in ChatCommands.Teammates)
        {
            // If the teammate is no longer here, we remove them.
            if (!PlayerUtils.IsPlayerInLobby(Helper.GetIDFromColor(teammate.ToLower())))
            {
                toRemove.Add(teammate);
            }
        }

        foreach (string teammate in toRemove)
        {
            ChatCommands.Teammates.Remove(teammate.ToLower());
        }
        // We update the win counters
        var winCounterUI = UnityEngine.Object.FindObjectOfType<WinCounterUI>();
        if (winCounterUI != null)
        {
            Helper.InitWinText(winCounterUI);
        }
    }
}