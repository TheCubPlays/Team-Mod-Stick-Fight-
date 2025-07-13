// Credits to Monky for the fundamentals to make a config file, I'm honestly too lazy to make my own
// I won't be commenting anything here since I consider everything self-explainatory.

using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace TMOD;

public static class ConfigHandler
{
    private static readonly Dictionary<string, ConfigEntryBase> EntriesDict = new(StringComparer.InvariantCultureIgnoreCase);
    
    // Config sections
    private const string MiscSect = "Misc. Options";
    private const string PlayerColorSect = "Player Color Options";
    
    // Properties that need to be updated whenever their respective config entry is modified
    public static bool IsCustomTeamColor { get; private set; }
    public static bool IsCustomEnemyColor { get; private set; }
    public static Color[] DefaultColors { get; } = new Color[4];


    public static void InitializeConfig(ConfigFile config)
    {
        var cmdPrefixEntry = config.Bind(MiscSect,
            "CommandPrefix",
            "/",
            "If this is the first character of your message, it'll register as a command. (Recommended: /, !, $, ., &, ?, ~)");
        var teamWinsToggleEntry = config.Bind(MiscSect,
            "teamWinsToggle",
            true,
            "When 'true' your team and the enemy team will each have their own win counters, calculated as the sum of all team members' individual wins. Set to false to disable.");
        var customTeamColorToggleEntry = config.Bind(PlayerColorSect,
            "useTeamColor",
            true,
            "When 'true' your team's members will share the same custom color (TeamColor). When 'false' your team members will have the default colors of the game. (Yellow/Red/Blue/Green)");
        var customEnemyColorToggleEntry = config.Bind(PlayerColorSect,
            "useEnemyColor",
            true,
            "When 'true' the enemy team's members will share the same custom color (EnemyColor). When 'false' the enemy team members will have the default colors of the game. (Yellow/Red/Blue/Green)");
        var customAllColorToggleEntry = config.Bind(PlayerColorSect,
            "useColors",
            true,
            "When 'false' all stick figures will have default colors (Yellow/Red/Blue/Green) regardless of the 2 options above. When 'true', it depends on the other 2 options. Note that this doesn't affect the win counter color, that one will still use the custom team colors from below.");
        var teamColorEntry = config.Bind(PlayerColorSect,
            "TeamColor",
            new Color(0, 0, 1),
            "The stickmen of your team will have this color (Use a HEX value)");
        var enemyColorEntry = config.Bind(PlayerColorSect,
            "EnemyColor",
            new Color(1, 0, 0),
            "The stickmen of the enemy team will have this color (Use a HEX value)");

        var teamColorEntryKey = teamColorEntry.Definition.Key;
        EntriesDict[teamColorEntryKey] = teamColorEntry;
        var enemyColorEntryKey = enemyColorEntry.Definition.Key;
        EntriesDict[enemyColorEntryKey] = enemyColorEntry;

        // All that "SettingChanged" stuff only apply to changes that were caused by code (commands). Manual configuration of the file won't trigger those.

        teamColorEntry.SettingChanged += (_, _) =>
        {
            // Similar stuff to what we did on the OnPlayerSpawnMethodPostfix in MultiplayerManagerPatches.cs, you can check that if you need help understanding.
            var customTeamColor = GetEntry<Color>("TeamColor");
            var isCustomTeamColor = customTeamColor != GetEntry<Color>("TeamColor", true);

            var team_color = isCustomTeamColor ? customTeamColor : new Color(0f, 0f, 1f, 1f);
            foreach (var player in UnityEngine.Object.FindObjectsOfType<NetworkPlayer>())
            {
                if (player.NetworkSpawnID != GameManager.Instance.mMultiplayerManager.LocalPlayerIndex)
                {
                    var otherCharacter = player.transform.root.gameObject;
                    Color otherColor = team_color;
                    if (ChatCommands.Teammates.Contains(Helper.GetColorFromID(player.NetworkSpawnID).ToLower()))
                    {
                        if (!(Helper.customTeamColorToggle && Helper.customAllColorToggle))
                        {
                            otherColor = Helper.getRGBFromColor(Helper.GetColorFromID(player.NetworkSpawnID));
                        }
                        MultiplayerManagerPatches.ChangeAllCharacterColors(otherColor, otherCharacter);
                    }
                }
                else
                {
                    var character = player.transform.root.gameObject;
                    Color color = team_color;
                    if (!(Helper.customTeamColorToggle && Helper.customAllColorToggle))
                    {
                        color = Helper.getRGBFromColor(Helper.GetColorFromID(player.NetworkSpawnID));
                    }
                    MultiplayerManagerPatches.ChangeAllCharacterColors(color, character);
                }
            }
            // We update win counters
            var winCounterUI = UnityEngine.Object.FindObjectOfType<WinCounterUI>();
            if (winCounterUI != null)
            {
                Helper.InitWinText(winCounterUI);
            } 
        };
        enemyColorEntry.SettingChanged += (_, _) =>
        {
            // Similar stuff to what we did on the OnPlayerSpawnMethodPostfix in MultiplayerManagerPatches.cs, you can check that if you need help understanding.
            var customEnemyColor = GetEntry<Color>("EnemyColor");
            var isCustomEnemyColor = customEnemyColor != GetEntry<Color>("EnemyColor", true);

            var enemy_color = isCustomEnemyColor ? customEnemyColor : new Color(1f, 0f, 0f, 1f);
            foreach (var player in UnityEngine.Object.FindObjectsOfType<NetworkPlayer>())
            {
                if (player.NetworkSpawnID != GameManager.Instance.mMultiplayerManager.LocalPlayerIndex)
                {
                    var otherCharacter = player.transform.root.gameObject;
                    Color otherColor = enemy_color;
                    if (!ChatCommands.Teammates.Contains(Helper.GetColorFromID(player.NetworkSpawnID).ToLower()))
                    {
                        if (!(Helper.customEnemyColorToggle && Helper.customAllColorToggle))
                        {
                            otherColor = Helper.getRGBFromColor(Helper.GetColorFromID(player.NetworkSpawnID));
                        }
                        MultiplayerManagerPatches.ChangeAllCharacterColors(otherColor, otherCharacter);
                    }
                }
            } 
            // We update win counters
            var winCounterUI = UnityEngine.Object.FindObjectOfType<WinCounterUI>();
            if (winCounterUI != null)
            {
                Helper.InitWinText(winCounterUI);
            }
        };
        var cmdPrefixEntryKey = cmdPrefixEntry.Definition.Key;
        EntriesDict[cmdPrefixEntryKey] = cmdPrefixEntry;

        var teamWinsToggleEntryKey = teamWinsToggleEntry.Definition.Key;
        EntriesDict[teamWinsToggleEntryKey] = teamWinsToggleEntry;
        var customTeamColorToggleEntryKey = customTeamColorToggleEntry.Definition.Key;
        EntriesDict[customTeamColorToggleEntryKey] = customTeamColorToggleEntry;
        var customEnemyColorToggleEntryKey = customEnemyColorToggleEntry.Definition.Key;
        EntriesDict[customEnemyColorToggleEntryKey] = customEnemyColorToggleEntry;
        var customAllColorToggleEntryKey = customAllColorToggleEntry.Definition.Key;
        EntriesDict[customAllColorToggleEntryKey] = customAllColorToggleEntry;

        cmdPrefixEntry.SettingChanged += (_, _) =>
        {
            Command.CmdPrefix = cmdPrefixEntry.Value.Length == 1
                ? cmdPrefixEntry.Value[0]
                : '/';

            var cmdNames = ChatCommands.CmdNames;

            for (var i = 0; i < cmdNames.Count; i++)
                cmdNames[i] = Command.CmdPrefix + cmdNames[i].Substring(1);
        };

        teamWinsToggleEntry.SettingChanged += (_, _) =>
        {
            Helper.teamWinsToggle = teamWinsToggleEntry.Value;
            var winCounterUI = UnityEngine.Object.FindObjectOfType<WinCounterUI>();
            if (winCounterUI != null)
            {
                Helper.InitWinText(winCounterUI);
            }
        };
        customTeamColorToggleEntry.SettingChanged += (_, _) =>
        {
            // Similar stuff to what we did on the OnPlayerSpawnMethodPostfix in MultiplayerManagerPatches.cs, you can check that if you need help understanding.
            Helper.customTeamColorToggle = customTeamColorToggleEntry.Value;
            var customTeamColor = GetEntry<Color>("TeamColor");
            var isCustomTeamColor = customTeamColor != GetEntry<Color>("TeamColor", true);

            var team_color = isCustomTeamColor ? customTeamColor : new Color(0f, 0f, 1f, 1f);
            foreach (var player in UnityEngine.Object.FindObjectsOfType<NetworkPlayer>())
            {
                if (player.NetworkSpawnID != GameManager.Instance.mMultiplayerManager.LocalPlayerIndex)
                {
                    var otherCharacter = player.transform.root.gameObject;
                    Color otherColor = team_color;
                    if (ChatCommands.Teammates.Contains(Helper.GetColorFromID(player.NetworkSpawnID).ToLower()))
                    {
                        if (!(Helper.customTeamColorToggle && Helper.customAllColorToggle))
                        {
                            otherColor = Helper.getRGBFromColor(Helper.GetColorFromID(player.NetworkSpawnID));
                        }
                        MultiplayerManagerPatches.ChangeAllCharacterColors(otherColor, otherCharacter);
                    }
                }
                else
                {
                    var character = player.transform.root.gameObject;
                    Color color = team_color;
                    if (!(Helper.customTeamColorToggle && Helper.customAllColorToggle))
                    {
                        color = Helper.getRGBFromColor(Helper.GetColorFromID(player.NetworkSpawnID));
                    }
                    MultiplayerManagerPatches.ChangeAllCharacterColors(color, character);
                }
            }
            // We update win counters
            var winCounterUI = UnityEngine.Object.FindObjectOfType<WinCounterUI>();
            if (winCounterUI != null)
            {
                Helper.InitWinText(winCounterUI);
            }    
        };
        customEnemyColorToggleEntry.SettingChanged += (_, _) =>
        {
            // Similar stuff to what we did on the OnPlayerSpawnMethodPostfix in MultiplayerManagerPatches.cs, you can check that if you need help understanding.
            Helper.customEnemyColorToggle = customEnemyColorToggleEntry.Value;
            var customEnemyColor = GetEntry<Color>("EnemyColor");
            var isCustomEnemyColor = customEnemyColor != GetEntry<Color>("EnemyColor", true);

            var enemy_color = isCustomEnemyColor ? customEnemyColor : new Color(1f, 0f, 0f, 1f);
            foreach (var player in UnityEngine.Object.FindObjectsOfType<NetworkPlayer>())
            {
                if (player.NetworkSpawnID != GameManager.Instance.mMultiplayerManager.LocalPlayerIndex)
                {
                    var otherCharacter = player.transform.root.gameObject;
                    Color otherColor = enemy_color;
                    if (!ChatCommands.Teammates.Contains(Helper.GetColorFromID(player.NetworkSpawnID).ToLower()))
                    {
                        if (!(Helper.customEnemyColorToggle && Helper.customAllColorToggle))
                        {
                            otherColor = Helper.getRGBFromColor(Helper.GetColorFromID(player.NetworkSpawnID));
                        }
                        MultiplayerManagerPatches.ChangeAllCharacterColors(otherColor, otherCharacter);
                    }
                } // We're not doing anything for ourselves (local player) since we can't be on the enemy-side so, this doesn't affect us.
            } 
            // We update win counters
            var winCounterUI = UnityEngine.Object.FindObjectOfType<WinCounterUI>();
            if (winCounterUI != null)
            {
                Helper.InitWinText(winCounterUI);
            }   
        };
        customAllColorToggleEntry.SettingChanged += (_, _) =>
        {
            // Similar stuff to what we did on the OnPlayerSpawnMethodPostfix in MultiplayerManagerPatches.cs, you can check that if you need help understanding.
            Helper.customAllColorToggle = customAllColorToggleEntry.Value;

            var customTeamColor = GetEntry<Color>("TeamColor");
            var isCustomTeamColor = customTeamColor != GetEntry<Color>("TeamColor", true);

            var team_color = isCustomTeamColor ? customTeamColor : new Color(0f, 0f, 1f, 1f);
            var customEnemyColor = GetEntry<Color>("EnemyColor");
            var isCustomEnemyColor = customEnemyColor != GetEntry<Color>("EnemyColor", true);

            var enemy_color = isCustomEnemyColor ? customEnemyColor : new Color(1f, 0f, 0f, 1f);
            foreach (var player in UnityEngine.Object.FindObjectsOfType<NetworkPlayer>())
            {
                if (player.NetworkSpawnID != GameManager.Instance.mMultiplayerManager.LocalPlayerIndex)
                {
                    var otherCharacter = player.transform.root.gameObject;
                    Color otherColor = team_color;
                    if (!ChatCommands.Teammates.Contains(Helper.GetColorFromID(player.NetworkSpawnID).ToLower()))
                    {
                        if (Helper.customEnemyColorToggle && Helper.customAllColorToggle)
                        {
                            otherColor = enemy_color;
                        }
                        else
                        {
                            otherColor = Helper.getRGBFromColor(Helper.GetColorFromID(player.NetworkSpawnID));
                        }
                    }
                    else
                    {
                        if (!(Helper.customTeamColorToggle && Helper.customAllColorToggle))
                        {
                            otherColor = Helper.getRGBFromColor(Helper.GetColorFromID(player.NetworkSpawnID));
                        }
                    }

                    MultiplayerManagerPatches.ChangeAllCharacterColors(otherColor, otherCharacter);
                }
                else
                {
                    var character = player.transform.root.gameObject;
                    Color color = team_color;
                    if (!(Helper.customTeamColorToggle && Helper.customAllColorToggle))
                    {
                        color = Helper.getRGBFromColor(Helper.GetColorFromID(player.NetworkSpawnID));
                    }
                    MultiplayerManagerPatches.ChangeAllCharacterColors(color, character);
                }
            }
            // We update win counters
            var winCounterUI = UnityEngine.Object.FindObjectOfType<WinCounterUI>();
            if (winCounterUI != null)
            {
                Helper.InitWinText(winCounterUI);
            }   
        };

        IsCustomTeamColor = teamColorEntry.Value != (Color)teamColorEntry.DefaultValue;
        IsCustomEnemyColor = enemyColorEntry.Value != (Color)enemyColorEntry.DefaultValue;
    }
    // Gets an entry from the config file (For example: CommandPrefix's entry is '/' by default)
    public static T GetEntry<T>(string entryKey, bool defaultValue = false) 
        => defaultValue ? (T)EntriesDict[entryKey].DefaultValue : (T)EntriesDict[entryKey].BoxedValue;
    // Modifies an entry of the config file
    public static void ModifyEntry(string entryKey, string value) 
        => EntriesDict[entryKey].SetSerializedValue(value);

    // Resets an entry of the config file to the default value
    public static void ResetEntry(string entryKey)
    {
        var configEntry = EntriesDict[entryKey];
        configEntry.BoxedValue = configEntry.DefaultValue;
    }
    // Resets all entries of the config file to their default values
    public static void ResetConfig()
    {
        ResetEntry("CommandPrefix");
        ResetEntry("teamWinsToggle");
        ResetEntry("useTeamColor");
        ResetEntry("useEnemyColor");
        ResetEntry("useColors");
        ResetEntry("TeamColor");
        ResetEntry("EnemyColor");
    }
}