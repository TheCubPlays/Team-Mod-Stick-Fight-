// Credits to Monky for the fundamentials to make a command.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TMOD;

public static class ChatCommands
{
    private static readonly List<string> ValidColors = new List<string>
    {
        "red",
        "yellow",
        "blue",
        "green"
    };

    public static List<string> Teammates = new List<string> { };
    private static readonly List<Command> Cmds = new()
    {
        new Command("team", Team, 1, true),
        new Command("reset", Reset, 1, true),
        new Command("prefix", Prefix, 0, true),
        new Command("teamwinstoggle", teamWinsToggle, 0, true),
        new Command("useteamcolor", useTeamColor, 0, true),
        new Command("useenemycolor", useEnemyColor, 0, true),
        new Command("usecolors", useColors, 0, true),
        new Command("teamcolor", TeamColor, 0, true),
        new Command("enemycolor", EnemyColor, 0, true),
        new Command("scouter", Scouter, 0, true)
    };
    // Used to assign members to a team, remove them and also list them.
    // This is the only one I'll put comments for since I think everything else is either the same or self-explainatory.
    private static void Team(string[] args, Command cmd)
    {
        switch (args[0].ToLower())
        {
            case "reset":
                if (args.Length != 1)
                {
                    Helper.SendModOutput("Invalid Parameters!", Command.LogType.Error, false);
                    break;
                }
                var toRemove = new List<string>();
                bool removed = false;
                // We can't modify a list while iterating it so, we make this copy instead for iteration.
                foreach (string teammate in Teammates)
                {
                    toRemove.Add(teammate);
                }
                foreach (string teammate in toRemove)
                {
                    Teammates.Remove(teammate.ToLower());
                    // The local ID always represents OUR ID (In-game spawn ID)
                    var local_ID = GameManager.Instance.mMultiplayerManager.LocalPlayerIndex;  
                    var player = Helper.GetNetworkPlayer(Helper.GetIDFromColor(teammate.ToLower()));
                    var character = player.transform.root.gameObject; // The targeted player's character
                    // If no custom colour was set up, the default one will be used.
                    var customEnemyColor = ConfigHandler.GetEntry<Color>("EnemyColor");
                    var isCustomEnemyColor = customEnemyColor != ConfigHandler.GetEntry<Color>("EnemyColor", true);

                    var enemy_color = isCustomEnemyColor ? customEnemyColor : new Color(1f, 0f, 0f, 1f);
                    if (!(Helper.customEnemyColorToggle && Helper.customAllColorToggle))
                    {
                        enemy_color = Helper.getRGBFromColor(teammate.ToLower());
                    }
                    MultiplayerManagerPatches.ChangeAllCharacterColors(enemy_color, character);
                    // When doing anything like this, we always update the win counter.
                    var winCounterUI = Object.FindObjectOfType<WinCounterUI>();
                    // We create an instance of WinCounterUI, it's how we gain access to the class's methods.
                    if (winCounterUI != null)
                    {
                        Helper.InitWinText(winCounterUI);
                    }
                    removed = true;
                }
                if (removed)
                {
                    Helper.SendModOutput("Successfully reset your team!", Command.LogType.Success, false);
                }
                else
                {
                    Helper.SendModOutput("You don't have any teammates! Use '/team add [PlayerColor]' to add one!", Command.LogType.Warning, false);
                }
                break;
            case "add":
                if (args.Length != 2)
                {
                    Helper.SendModOutput("Invalid Parameters!", Command.LogType.Error, false);
                    break;
                }
                if (ValidColors.Contains(args[1].ToLower()))
                {
                    if (!Teammates.Contains(args[1].ToLower()))
                    {
                        var local_ID = GameManager.Instance.mMultiplayerManager.LocalPlayerIndex;
                        if (!(Helper.GetColorFromID(local_ID).ToLower() == args[1].ToLower()))
                        {
                            if (PlayerUtils.IsPlayerInLobby(Helper.GetIDFromColor(args[1].ToLower())))
                            {
                                Teammates.Add(args[1].ToLower());
                                Helper.SendModOutput($"Successfully added {args[1].ToLower()} to your team!", Command.LogType.Success, false);
                                var player = Helper.GetNetworkPlayer(Helper.GetIDFromColor(args[1].ToLower()));
                                var character = player.transform.root.gameObject;
                                var customTeamColor = ConfigHandler.GetEntry<Color>("TeamColor");
                                var isCustomTeamColor = customTeamColor != ConfigHandler.GetEntry<Color>("TeamColor", true);

                                var team_color = isCustomTeamColor ? customTeamColor : new Color(0f, 0f, 1f, 1f);
                                if (!(Helper.customTeamColorToggle && Helper.customAllColorToggle))
                                {
                                    team_color = Helper.getRGBFromColor(args[1].ToLower());
                                }
                                MultiplayerManagerPatches.ChangeAllCharacterColors(team_color, character);
                                var winCounterUI = Object.FindObjectOfType<WinCounterUI>();
                                if (winCounterUI != null)
                                {
                                    Helper.InitWinText(winCounterUI);
                                }
                            }
                            else
                            {
                                Helper.SendModOutput($"You can't add a player who's not in-game to your team!", Command.LogType.Error, false);
                            }
                        }
                        else
                        {
                            Helper.SendModOutput("You can't add yourself to your team!", Command.LogType.Error, false);
                        }
                    }
                    else
                    {
                        Helper.SendModOutput("This player is already in your team!", Command.LogType.Error, false);
                    }
                }
                else
                {
                    Helper.SendModOutput("Invalid colour, please choose between Yellow/Blue/Green/Red.", Command.LogType.Error, false);
                }
                break;
            case "remove":
                if (args.Length != 2)
                {
                    Helper.SendModOutput("Invalid Parameters!", Command.LogType.Error, false);
                    break;
                }
                if (ValidColors.Contains(args[1].ToLower()))
                {
                    if (Teammates.Contains(args[1].ToLower()))
                    {
                        var local_ID = GameManager.Instance.mMultiplayerManager.LocalPlayerIndex;
                        if (!(Helper.GetColorFromID(local_ID).ToLower() == args[1].ToLower()))
                        {
                            Teammates.Remove(args[1].ToLower());
                            Helper.SendModOutput($"Successfully removed {args[1].ToLower()} from your team!", Command.LogType.Success, false);
                            var player = Helper.GetNetworkPlayer(Helper.GetIDFromColor(args[1].ToLower()));
                            var character = player.transform.root.gameObject;
                            var customEnemyColor = ConfigHandler.GetEntry<Color>("EnemyColor");
                            var isCustomEnemyColor = customEnemyColor != ConfigHandler.GetEntry<Color>("EnemyColor", true);

                            var enemy_color = isCustomEnemyColor ? customEnemyColor : new Color(1f, 0f, 0f, 1f);
                            if (!(Helper.customEnemyColorToggle && Helper.customAllColorToggle))
                            {
                                enemy_color = Helper.getRGBFromColor(args[1].ToLower());
                            }
                            MultiplayerManagerPatches.ChangeAllCharacterColors(enemy_color, character);
                            var winCounterUI = Object.FindObjectOfType<WinCounterUI>();
                            if (winCounterUI != null)
                            {
                                Helper.InitWinText(winCounterUI);
                            }
                        }
                        else
                        {
                            Helper.SendModOutput("You can't remove yourself from your team!", Command.LogType.Error, false);
                        }
                    }
                    else
                    {
                        Helper.SendModOutput("This player is not in your team!", Command.LogType.Error, false);
                    }
                }
                else
                {
                    Helper.SendModOutput("Invalid colour, please choose between Yellow/Blue/Green/Red.", Command.LogType.Error, false);
                }
                break;
            case "list":
                if (Teammates.Count == 0)
                {
                    Helper.SendModOutput("You don't have any teammates! Use '/team add [PlayerColor]' to add one!", Command.LogType.Warning, false);
                }
                else
                {
                    string message = "Your teammates: " + string.Join(", ", Teammates.ToArray());
                    Helper.SendModOutput(message, Command.LogType.Success, false);
                }
                break;
            default:
                Helper.SendModOutput("Invalid Parameters!", Command.LogType.Error, false);
                break;
        }
    }
    // Reseting configuration settings back to their default values.
    private static void Reset(string[] args, Command cmd)
    {
        if (args.Length > 1)
        {
            Helper.SendModOutput("Invalid Parameters!", Command.LogType.Error, false);
            return;
        }
        switch (args[0].ToLower())
        {
            case "config":
                ConfigHandler.ResetConfig();
                Helper.SendModOutput("Successfully reset config settings to default values!", Command.LogType.Success, false);
                break;
            case "prefix":
                ConfigHandler.ResetEntry("CommandPrefix");
                Helper.SendModOutput("Successfully reset CommandPrefix setting to default value!", Command.LogType.Success, false);
                break;
            case "teamwinstoggle":
                ConfigHandler.ResetEntry("teamWinsToggle");
                Helper.SendModOutput("Successfully reset teamWinsToggle setting to default value!", Command.LogType.Success, false);
                break;
            case "useteamcolor":
                ConfigHandler.ResetEntry("useTeamColor");
                Helper.SendModOutput("Successfully reset useTeamColor setting to default value!", Command.LogType.Success, false);
                break;
            case "useenemycolor":
                ConfigHandler.ResetEntry("useEnemyColor");
                Helper.SendModOutput("Successfully reset useEnemyColor setting to default value!", Command.LogType.Success, false);
                break;
            case "usecolors":
                ConfigHandler.ResetEntry("useColors");
                Helper.SendModOutput("Successfully reset useColors setting to default value!", Command.LogType.Success, false);
                break;
            case "teamcolor":
                ConfigHandler.ResetEntry("TeamColor");
                Helper.SendModOutput("Successfully reset TeamColor setting to default value!", Command.LogType.Success, false);
                break;
            case "enemycolor":
                ConfigHandler.ResetEntry("EnemyColor");
                Helper.SendModOutput("Successfully reset EnemyColor setting to default value!", Command.LogType.Success, false);
                break;
            default:
                Helper.SendModOutput("Invalid Parameters.", Command.LogType.Error, false);
                break;
        }
    }
    // The prefix for the Mod's commands
    private static void Prefix(string[] args, Command cmd)
    {
        if (args.Length == 0)
        {
            var CommandPrefix = ConfigHandler.GetEntry<string>("CommandPrefix");
            Helper.SendModOutput($"Command Prefix: '{CommandPrefix}'", Command.LogType.Info, false);
            return;
        }
        else if (args.Length > 1)
        {
            Helper.SendModOutput("Invalid Parameters!", Command.LogType.Error, false);
        }
        else
        {
            if (char.TryParse(args[0], out _))
            {
                ConfigHandler.ModifyEntry("CommandPrefix", args[0]);
                Helper.SendModOutput($"Command Prefix set to: {args[0]}", Command.LogType.Success, false);
            }
            else
            {
                Helper.SendModOutput("Value must be a character!", Command.LogType.Error, false);
            }
        }
    }
    // Whether win counters among team members will be merged to one or not
    private static void teamWinsToggle(string[] args, Command cmd)
    {
        if (args.Length == 0)
        {
            var teamsWinsToggle = ConfigHandler.GetEntry<bool>("teamWinsToggle");
            Helper.SendModOutput($"teamsWinsToggle: '{teamsWinsToggle}'", Command.LogType.Info, false);
            return;
        }
        else if (args.Length > 1)
        {
            Helper.SendModOutput("Invalid Parameters!", Command.LogType.Error, false);
        }
        else
        {
            if (bool.TryParse(args[0].ToLower(), out _))
            {
                ConfigHandler.ModifyEntry("teamWinsToggle", args[0].ToLower());
                Helper.SendModOutput($"teamWinsToggle set to: {args[0].ToLower()}", Command.LogType.Success, false);
            }
            else
            {
                Helper.SendModOutput("Value must be 'true' or 'false'!", Command.LogType.Error, false);
            }
        }
    }
    // Whether custom colors shall be used or not for our team (For stickmen only, not team win counters).
    private static void useTeamColor(string[] args, Command cmd)
    {
        if (args.Length == 0)
        {
            var useTeamColor = ConfigHandler.GetEntry<bool>("useTeamColor");
            Helper.SendModOutput($"useTeamColor: '{useTeamColor}'", Command.LogType.Info, false);
            return;
        }
        else if (args.Length > 1)
        {
            Helper.SendModOutput("Invalid Parameters!", Command.LogType.Error, false);
        }
        else
        {
            if (bool.TryParse(args[0].ToLower(), out _))
            {
                ConfigHandler.ModifyEntry("useTeamColor", args[0].ToLower());
                Helper.SendModOutput($"useTeamColor set to: {args[0].ToLower()}", Command.LogType.Success, false);
            }
            else
            {
                Helper.SendModOutput("Value must be 'true' or 'false'!", Command.LogType.Error, false);
            }
        }
    }
    // Whether custom colors shall be used or not for the enemy team (For stickmen only, not team win counters).
    private static void useEnemyColor(string[] args, Command cmd)
    {
        if (args.Length == 0)
        {
            var useEnemyColor = ConfigHandler.GetEntry<bool>("useEnemyColor");
            Helper.SendModOutput($"useEnemyColor: '{useEnemyColor}'", Command.LogType.Info, false);
            return;
        }
        else if (args.Length > 1)
        {
            Helper.SendModOutput("Invalid Parameters!", Command.LogType.Error, false);
        }
        else
        {
            if (bool.TryParse(args[0].ToLower(), out _))
            {
                ConfigHandler.ModifyEntry("useEnemyColor", args[0].ToLower());
                Helper.SendModOutput($"useEnemyColor set to: {args[0].ToLower()}", Command.LogType.Success, false);
            }
            else
            {
                Helper.SendModOutput("Value must be 'true' or 'false'!", Command.LogType.Error, false);
            }
        }
    }
    // Whether custom colors shall be used or not for every team (For stickmen only, not team win counters).
    private static void useColors(string[] args, Command cmd)
    {
        if (args.Length == 0)
        {
            var useColors = ConfigHandler.GetEntry<bool>("useColors");
            Helper.SendModOutput($"useColors: '{useColors}'", Command.LogType.Info, false);
            return;
        }
        else if (args.Length > 1)
        {
            Helper.SendModOutput("Invalid Parameters!", Command.LogType.Error, false);
        }
        else
        {
            if (bool.TryParse(args[0].ToLower(), out _))
            {
                ConfigHandler.ModifyEntry("useColors", args[0].ToLower());
                Helper.SendModOutput($"useColors set to: {args[0].ToLower()}", Command.LogType.Success, false);
            }
            else
            {
                Helper.SendModOutput("Value must be 'true' or 'false'!", Command.LogType.Error, false);
            }
        }
    }
    // The color the stickmen of the enemy team will have. (Unless stick fight decides to be funny)
    private static void TeamColor(string[] args, Command cmd)
    {
        if (args.Length == 0)
        {
            var TeamColor = ConfigHandler.GetEntry<Color>("TeamColor");
            Helper.SendModOutput($"TeamColor: '{ColorUtility.ToHtmlStringRGBA(TeamColor)}'", Command.LogType.Info, false);
            return;
        }
        else if (args.Length > 1)
        {
            Helper.SendModOutput("Invalid Parameters!", Command.LogType.Error, false);
        }
        else
        {
            if (args[0].StartsWith("#") && (args[0].Length == 7 || args[0].Length == 9)) // #RRGGBB or #RRGGBBAA
            {
                if (ColorUtility.TryParseHtmlString(args[0], out _))
                {
                    ConfigHandler.ModifyEntry("TeamColor", args[0]);
                    Helper.SendModOutput($"TeamColor set to: {args[0]}", Command.LogType.Success, false);
                }
                else
                {
                    Helper.SendModOutput("Invalid hex color! Format: #RRGGBB or #RRGGBBAA", Command.LogType.Error, false);
                }
            }
            else
            {
                Helper.SendModOutput("Color must be in hex format (e.g., #FF0000 or #FF0000FF)", Command.LogType.Error, false);
            }
        }
    }
    // The color the stickmen of the enemy team will have. (Hopefully)
    private static void EnemyColor(string[] args, Command cmd)
    {
        if (args.Length == 0)
        {
            var EnemyColor = ConfigHandler.GetEntry<Color>("EnemyColor");
            Helper.SendModOutput($"EnemyColor: '{ColorUtility.ToHtmlStringRGBA(EnemyColor)}'", Command.LogType.Info, false);
            return;
        }
        else if (args.Length > 1)
        {
            Helper.SendModOutput("Invalid Parameters!", Command.LogType.Error, false);
        }
        else
        {
            if (args[0].StartsWith("#") && (args[0].Length == 7 || args[0].Length == 9)) // Checks for valid HEX format (#RRGGBB or #RRGGBBAA)
            {
                if (ColorUtility.TryParseHtmlString(args[0], out _))
                {
                    ConfigHandler.ModifyEntry("EnemyColor", args[0]);
                    Helper.SendModOutput($"EnemyColor set to: {args[0]}", Command.LogType.Success, false);

                    var winUI = Object.FindObjectOfType<WinCounterUI>();
                    if (winUI != null) Traverse.Create(winUI).Method("RefreshWinTexts").GetValue();
                }
                else
                {
                    Helper.SendModOutput("Invalid hex color! Format: #RRGGBB or #RRGGBBAA", Command.LogType.Error, false);
                }
            }
            else
            {
                Helper.SendModOutput("Invalid hex color! Format: #RRGGBB or #RRGGBBAA", Command.LogType.Error, false);
            }
        }
    }
    // Checks online players, mainly used to track players for '/team add' in case you don't know who's who.
    private static void Scouter(string[] args, Command cmd)
    {
        if (args.Length > 0)
        {
            Helper.SendModOutput("Invalid Parameters!", Command.LogType.Error, false);
        }
        var playerList = new StringBuilder();

        foreach (var player in Object.FindObjectsOfType<NetworkPlayer>())
        {
            playerList.AppendLine($"{Helper.GetColorFromID(player.NetworkSpawnID)}: {Helper.GetPlayerName(Helper.GetSteamID(player.NetworkSpawnID))}<#0000FF>");
        }

        Helper.SendModOutput(playerList.ToString(), Command.LogType.Info, false);
    }
    public static readonly Dictionary<string, Command> CmdDict = Cmds.ToDictionary(cmd => cmd.Name.Substring(1),
        cmd => cmd,
        StringComparer.InvariantCultureIgnoreCase);
    public static readonly List<string> CmdNames = Cmds.Select(cmd => cmd.Name).ToList();
}
