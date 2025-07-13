// Used to modify the wincounters for merged team-scores. Cool stuff.

using HarmonyLib;
using TMPro;
using UnityEngine;
using System.Reflection;
using System;

namespace TMOD;

public static class WinTextPatch
{
    public static void Patches(Harmony harmonyInstance)
    {
        // If you wanna know how patching works, check FightingPatch.cs because I don't wanna repeat.
        var refreshMethod = AccessTools.Method(typeof(WinCounterUI), "RefreshWinTexts");
        var refreshPostfix = new HarmonyMethod(typeof(WinTextPatch).GetMethod(nameof(RefreshWinTextsPostfix)));
        harmonyInstance.Patch(refreshMethod, postfix: refreshPostfix);
    }
    public static void RefreshWinTextsPostfix(WinCounterUI __instance)
    {
        // Whether team-wins are enabled or not.
        if (Helper.teamWinsToggle)
        {
            // THE TRY-CATCH IS VERY CRUCIAL HERE! It will break the game if you don't include this, and I mean LITERALLY break it.
            try
            {
                // Basically we re-write the entire method. We'll need to grab mPlayerWinTexts from WinCounterUI.
                var field = typeof(WinCounterUI).GetField("mPlayerWinTexts", BindingFlags.NonPublic | BindingFlags.Instance);
                var winTexts = (TextMeshProUGUI[])field.GetValue(__instance);

                /*
                    wins - Used to store each player's wins individually.
                    wins_id - Used to store each player's ID corresponding to their wins.
                */
                string[] wins = new string[winTexts.Length];
                ushort[] wins_id = new ushort[winTexts.Length];

                for (int i = 0; i < winTexts.Length; i++)
                {
                    if (i >= ControllerHandler.Instance.players.Count || ControllerHandler.Instance.players[i] == null)
                        continue;

                    wins[i] = ControllerHandler.Instance.players[i].GetComponent<CharacterStats>().wins.ToString();
                    wins_id[i] = (ushort)ControllerHandler.Instance.players[i].playerID;
                }

                ushort local_id = GameManager.Instance.mMultiplayerManager.LocalPlayerIndex;
                int[] team_wins = [0, 0]; // Team 0 is our team, Team 1 is enemy team (Team 0 & 1 Index-wise).
                bool other_team_exists = false;

                for (int i = 0; i < wins.Length; i++)
                {
                    if (wins[i] == null)
                        continue;

                    var color = Helper.GetColorFromID(wins_id[i]);
                    // We sum each team members' wins, "wins_id[i] == local_id" is to include ourselves in the sum.
                    if (ChatCommands.Teammates.Contains(color.ToLower()) || wins_id[i] == local_id)
                    {
                        team_wins[0] += int.Parse(wins[i]);
                    }
                    else
                    {
                        other_team_exists = true;
                        team_wins[1] += int.Parse(wins[i]);
                    }
                }
                var customTeamColor = ConfigHandler.GetEntry<Color>("TeamColor");
                var isCustomTeamColor = customTeamColor != ConfigHandler.GetEntry<Color>("TeamColor", true);

                var team_color = isCustomTeamColor ? customTeamColor : new Color(0f, 0f, 1f, 1f);

                var customEnemyColor = ConfigHandler.GetEntry<Color>("EnemyColor");
                var isCustomEnemyColor = customEnemyColor != ConfigHandler.GetEntry<Color>("EnemyColor", true);

                var enemy_color = isCustomEnemyColor ? customEnemyColor : new Color(1f, 0f, 0f, 1f);
                // We disable all original win counters (Meaning they'll all dissapear, but don't worry we'll add ours)
                for (int i = 0; i < winTexts.Length; i++)
                {
                    winTexts[i].gameObject.SetActive(false);
                }
                // If local player (us) is Yellow or Red, show their wins on the left side (That's the side they spawn in), otherwise right side.
                // Left - WinText[0] | Right - WinText[1]
                if (Helper.GetColorFromID(local_id).ToLower().Equals("yellow") || Helper.GetColorFromID(local_id).ToLower().Equals("red"))
                {
                    winTexts[0].gameObject.SetActive(true);
                    winTexts[0].text = team_wins[0].ToString();
                    winTexts[0].color = team_color;
                    winTexts[0].ForceMeshUpdate();

                    if (other_team_exists && winTexts.Length > 1)
                    {
                        winTexts[1].gameObject.SetActive(true);
                        winTexts[1].text = team_wins[1].ToString();
                        winTexts[1].color = enemy_color;
                        winTexts[1].ForceMeshUpdate();
                    }
                }
                else
                {
                    winTexts[0].gameObject.SetActive(true);
                    winTexts[0].text = team_wins[1].ToString();
                    winTexts[0].color = enemy_color;
                    winTexts[0].ForceMeshUpdate();

                    if (other_team_exists && winTexts.Length > 1)
                    {
                        winTexts[1].gameObject.SetActive(true);
                        winTexts[1].text = team_wins[0].ToString();
                        winTexts[1].color = team_color;
                        winTexts[1].ForceMeshUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("WinText Error: " + ex);
            }
        }
        else
        {
            // If it's disabled, we still handle the event because perhaps the user modified the config while in-game. So changes must be applied.

            // THE TRY-CATCH IS VERY CRUCIAL HERE! It will break the game if you don't include this, and I mean LITERALLY break it.
            try
            {
                // Similar to the last one so, I won't be commenting this one.
                var field = typeof(WinCounterUI).GetField("mPlayerWinTexts", BindingFlags.NonPublic | BindingFlags.Instance);
                var winTexts = (TextMeshProUGUI[])field.GetValue(__instance);
                var customTeamColor = ConfigHandler.GetEntry<Color>("TeamColor");
                var isCustomTeamColor = customTeamColor != ConfigHandler.GetEntry<Color>("TeamColor", true);

                var team_color = isCustomTeamColor ? customTeamColor : new Color(0f, 0f, 1f, 1f);
                var customEnemyColor = ConfigHandler.GetEntry<Color>("EnemyColor");
                var isCustomEnemyColor = customEnemyColor != ConfigHandler.GetEntry<Color>("EnemyColor", true);

                var enemy_color = isCustomEnemyColor ? customEnemyColor : new Color(1f, 0f, 0f, 1f);
                for (int i = 0; i < winTexts.Length; i++)
                {
                    if (i >= ControllerHandler.Instance.players.Count || ControllerHandler.Instance.players[i] == null)
                        continue;
                    var player_ID = (ushort)ControllerHandler.Instance.players[i].playerID;
                    var color = Helper.getRGBFromColor(Helper.GetColorFromID((ushort)ControllerHandler.Instance.players[i].playerID));
                    if (player_ID != GameManager.Instance.mMultiplayerManager.LocalPlayerIndex)
                    {
                        color = team_color;
                        if (!(Helper.customTeamColorToggle && Helper.customAllColorToggle))
                        {
                            color = Helper.getRGBFromColor(Helper.GetColorFromID(player_ID));
                        }
                        if (!ChatCommands.Teammates.Contains(Helper.GetColorFromID(player_ID)))
                        {
                            if (Helper.customEnemyColorToggle && Helper.customAllColorToggle)
                            {
                                color = enemy_color;
                            }
                            else
                            {
                                color = Helper.getRGBFromColor(Helper.GetColorFromID(player_ID));
                            }
                        }
                    }
                    else
                    {
                        if (Helper.customTeamColorToggle && Helper.customAllColorToggle)
                        {
                            color = team_color;
                        }
                    }
                    winTexts[i].color = color;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("WinText Error: " + e);
            }
        }
    }
}