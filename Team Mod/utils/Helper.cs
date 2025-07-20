// Credits to Monky for the Get functions (except the RGB colour one below), the mod output methods and part of InitValues.

using System;
using System.Collections;
using HarmonyLib;
using Steamworks;
using TMPro;
using UnityEngine;
namespace TMOD;

public class Helper : MonoBehaviour
{
    // Returns Steam ID of specified colour
    public static CSteamID GetSteamID(ushort targetID) => ClientData[targetID].ClientID;

    // Returns spawn ID from player colour
    public static ushort GetIDFromColor(string targetSpawnColor) => targetSpawnColor.ToLower() switch
    {
        "yellow" or "y" => 0,
        "blue" or "b" => 1,
        "red" or "r" => 2,
        "green" or "g" => 3,
        _ => ushort.MaxValue
    };

    // Returns player colour from spawn ID
    public static string GetColorFromID(ushort x) => x switch { 1 => "Blue", 2 => "Red", 3 => "Green", _ => "Yellow" };

    // Returns player based on spawn ID
    public static NetworkPlayer GetNetworkPlayer(ushort targetID) => ClientData[targetID].PlayerObject.GetComponent<NetworkPlayer>();

    // Returns the name of the player using their Steam ID
    public static string GetPlayerName(CSteamID passedClientID) => SteamFriends.GetFriendPersonaName(passedClientID);
    private static Helper _instance;
    public static Helper Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("TMOD_Helper");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<Helper>();
            }
            return _instance;
        }
    }
    public static IEnumerator DelayedColorUpdate(MultiplayerManager __instance)
    {
        yield return new WaitForSeconds(1f); // Delayed execution so that it overrides QOL Mod's functionality if needed
        var customTeamColor = ConfigHandler.GetEntry<Color>("TeamColor");
        var isCustomTeamColor = customTeamColor != ConfigHandler.GetEntry<Color>("TeamColor", true);

        var team_color = isCustomTeamColor ? customTeamColor : new Color(0f, 0f, 1f, 1f);
        var customEnemyColor = ConfigHandler.GetEntry<Color>("EnemyColor");
        var isCustomEnemyColor = customEnemyColor != ConfigHandler.GetEntry<Color>("EnemyColor", true);

        var enemy_color = isCustomEnemyColor ? customEnemyColor : new Color(1f, 0f, 0f, 1f);

        foreach (var player in FindObjectsOfType<NetworkPlayer>())
        {
            if (player.NetworkSpawnID != __instance.LocalPlayerIndex)
            {
                // Modifies other players
                var otherCharacter = player.transform.root.gameObject;
                // It starts by assuming the player is our teammate.
                Color otherColor = team_color;
                // If team colors are disabled, it gives otherColor the default color (Corresponding to the player's spawn ID).
                if (!(customTeamColorToggle && customAllColorToggle))
                {
                    if (!Plugin.isQolEnabled)
                    {
                        otherColor = getRGBFromColor(GetColorFromID(player.NetworkSpawnID));
                    }
                    else
                    {
                        otherColor = QOLConfigHandler.getColor(player.NetworkSpawnID);
                    }
                }
                // If the player turns out not to be our teammate, it gives otherColor the enemy color.
                if (!ChatCommands.Teammates.Contains(GetColorFromID(player.NetworkSpawnID).ToLower()))
                {
                    // If enemy colors are disabled, it gives otherColor the default color.
                    if (customEnemyColorToggle && customAllColorToggle)
                    {
                        otherColor = enemy_color;
                    }
                    else
                    {
                        if (!Plugin.isQolEnabled)
                        {
                            otherColor = getRGBFromColor(GetColorFromID(player.NetworkSpawnID));
                        }
                        else
                        {
                            otherColor = QOLConfigHandler.getColor(player.NetworkSpawnID);
                        }
                    }
                }
                if (!useQolColorsToggle || !Plugin.isQolEnabled)
                {
                    MultiplayerManagerPatches.ChangeAllCharacterColors(otherColor, otherCharacter);
                }
            }
            else
            {
                // Modifies us
                var character = player.transform.root.gameObject;
                // If team colors are enabled, make the team color our character's default color.
                if (!(customTeamColorToggle && customAllColorToggle))
                {
                    if (!Plugin.isQolEnabled)
                    {
                        team_color = getRGBFromColor(GetColorFromID(player.NetworkSpawnID));
                    }
                    else
                    {
                        team_color = QOLConfigHandler.isCustomColor() ? QOLConfigHandler.GetCustomColor() : QOLConfigHandler.getColor(player.NetworkSpawnID);
                    }
                }
                if (!useQolColorsToggle || !Plugin.isQolEnabled)
                {
                    MultiplayerManagerPatches.ChangeAllCharacterColors(team_color, character);
                }
            }
        }
    }
    // This sets up and runs stuff when a player joins a lobby
    public static void InitValues(ChatManager __instance, ushort playerID)
    {
        if (playerID != GameManager.Instance.mMultiplayerManager.LocalPlayerIndex) return;

        ClientData = GameManager.Instance.mMultiplayerManager.ConnectedClients;
        var localID = GameManager.Instance.mMultiplayerManager.LocalPlayerIndex;
        localNetworkPlayer = ClientData[localID].PlayerObject.GetComponent<NetworkPlayer>();
        LocalChat = ClientData[localID].PlayerObject.GetComponentInChildren<ChatManager>();
        Debug.Log("Assigned the localNetworkPlayer!: " + localNetworkPlayer.NetworkSpawnID);
        TMPText = Traverse.Create(__instance).Field("text").GetValue<TextMeshPro>();
        TMPText.richText = true;
        Traverse.Create(__instance).Field("chatField").GetValue<TMP_InputField>().caretWidth = 3;
        ChatCommands.Teammates.Clear();
        Plugin.InitModText();
        var winCounterUI = UnityEngine.Object.FindObjectOfType<WinCounterUI>();
        if (winCounterUI != null)
        {
            InitWinText(winCounterUI);
        }
    }
    // Initialises win counters
    public static void InitWinText(WinCounterUI __instance)
    {
        if (__instance == null) return;
        try
        {
            Traverse.Create(__instance).Method("RefreshWinTexts").GetValue();
        }
        catch (Exception e)
        {
            Debug.LogError($"Refresh failed: {e}");
        }

    }
    // Public mod output
    public static void SendPublicOutput(string msg) => localNetworkPlayer.OnTalked(msg);

    // Private mod output
    public static void SendModOutput(string msg, Command.LogType logType, bool isPublic = true)
    {
        if (isPublic)
        {
            SendPublicOutput(msg);
            return;
        }

        var msgColor = logType switch
        {
            Command.LogType.Warning => "<#A020F0>",

            Command.LogType.Success => "<#008000>",

            Command.LogType.Info => "<#0000FF>",

            Command.LogType.Error => "<#FF0000>",
            _ => ""
        };

        LocalChat.Talk(msgColor + msg);
    }
    // Returns the RGB of the game's default character colors
    public static Color getRGBFromColor(string color)
    {
        switch (color.ToLower())
        {
            case "yellow":
                return new Color(0.847f, 0.549f, 0.278f, 1f);
            case "red":
                return new Color(0.839f, 0.333f, 0.302f, 1f);
            case "blue":
                return new Color(0.333f, 0.451f, 0.678f, 1f);
            case "green":
                return new Color(0.341f, 0.545f, 0.286f, 1f);
            default:
                return new Color(1f, 1f, 1f, 1f);
        }
    }

    public static ConnectedClientData[] ClientData;
    public static ChatManager LocalChat;
    public static NetworkPlayer localNetworkPlayer;
    public static TextMeshPro TMPText;

    public static bool teamWinsToggle = ConfigHandler.GetEntry<bool>("teamWinsToggle");
    public static bool customTeamColorToggle = ConfigHandler.GetEntry<bool>("useTeamColor");
    public static bool customEnemyColorToggle = ConfigHandler.GetEntry<bool>("useEnemyColor");
    public static bool customAllColorToggle = ConfigHandler.GetEntry<bool>("useColors");
    public static bool useQolColorsToggle = ConfigHandler.GetEntry<bool>("useQolColors");
}
