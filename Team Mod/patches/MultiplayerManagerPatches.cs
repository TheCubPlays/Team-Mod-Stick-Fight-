// Credits to Monky for half of this, I mean I basically only made everything inside OnPlayerSpawnedMethodPostfix

using System.Linq;
using HarmonyLib;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;


namespace TMOD;

class MultiplayerManagerPatches
{
    public static void Patches(Harmony harmonyInstance) // Multiplayer methods to patch with the harmony __instance
    {
        var onPlayerSpawnedMethod = AccessTools.Method(typeof(MultiplayerManager), "OnPlayerSpawned");

        var onPlayerSpawnedMethodPostfix = new HarmonyMethod(typeof(MultiplayerManagerPatches)
            .GetMethod(nameof(OnPlayerSpawnedMethodPostfix)));
        harmonyInstance.Patch(onPlayerSpawnedMethod, postfix: onPlayerSpawnedMethodPostfix);
    }

    public static void OnPlayerSpawnedMethodPostfix(MultiplayerManager __instance)
    {
        var customTeamColor = ConfigHandler.GetEntry<Color>("TeamColor");
        var isCustomTeamColor = customTeamColor != ConfigHandler.GetEntry<Color>("TeamColor", true);
            
        var team_color = isCustomTeamColor ? customTeamColor : new Color(0f, 0f, 1f, 1f);
        var customEnemyColor = ConfigHandler.GetEntry<Color>("EnemyColor");
        var isCustomEnemyColor = customEnemyColor != ConfigHandler.GetEntry<Color>("EnemyColor", true);
            
        var enemy_color = isCustomEnemyColor ? customEnemyColor : new Color(1f, 0f, 0f, 1f);

        foreach (var player in Object.FindObjectsOfType<NetworkPlayer>())
        {
            if (player.NetworkSpawnID != __instance.LocalPlayerIndex)
            {
                // Modifies other players
                var otherCharacter = player.transform.root.gameObject;
                // It starts by assuming the player is our teammate.
                Color otherColor = team_color;
                // If team colors are disabled, it gives otherColor the default color (Corresponding to the player's spawn ID).
                if (!(Helper.customTeamColorToggle && Helper.customAllColorToggle))
                {
                    otherColor = Helper.getRGBFromColor(Helper.GetColorFromID(player.NetworkSpawnID));
                }
                // If the player turns out not to be our teammate, it gives otherColor the enemy color.
                if (!ChatCommands.Teammates.Contains(Helper.GetColorFromID(player.NetworkSpawnID).ToLower()))
                {
                    // If enemy colors are disabled, it gives otherColor the default color.
                    if (Helper.customEnemyColorToggle && Helper.customAllColorToggle)
                    {
                        otherColor = enemy_color;
                    }
                    else
                    {
                        otherColor = Helper.getRGBFromColor(Helper.GetColorFromID(player.NetworkSpawnID));
                    }
                }

                ChangeAllCharacterColors(otherColor, otherCharacter);
            }
            else
            {
                // Modifies us
                var character = player.transform.root.gameObject;
                // If team colors are enabled, make the team color our character's default color.
                if (!(Helper.customTeamColorToggle && Helper.customAllColorToggle))
                {
                    team_color = Helper.getRGBFromColor(Helper.GetColorFromID(player.NetworkSpawnID));
                }
                ChangeAllCharacterColors(team_color, character);
            }
        }
    }

    public static void ChangeSpriteRendColor(Color colorWanted, GameObject character)
    {
        foreach (var spriteRenderer in character.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.color = colorWanted;
            spriteRenderer.GetComponentInParent<SetColorWhenDamaged>().startColor = colorWanted;
        }
    }

    public static void ChangeLineRendColor(Color colorWanted, GameObject character)
    {
        foreach (var t in character.GetComponentsInChildren<LineRenderer>())
            t.sharedMaterial.color = colorWanted;
    }

    public static void ChangeParticleColor(Color colorWanted, GameObject character)
    {
        var unchangedEffects = new[]
        {
            "punchPartilce",
            "JumpParticle",
            "landParticle (1)",
            "footParticle",
            "footParticle (1)"
        };

        foreach (var partSys in character.GetComponentsInChildren<ParticleSystem>())
        {
            if (unchangedEffects.Contains(partSys.name))
                continue;

            var main = partSys.main;
            main.startColor = colorWanted;
        }
    }

    public static void ChangeWinTextColor(Color colorWanted, int playerID)
    {
        var winTexts = Traverse.Create(Object.FindObjectOfType<WinCounterUI>()).Field("mPlayerWinTexts")
            .GetValue<TextMeshProUGUI[]>();

        winTexts[playerID].color = colorWanted;
    }

    public static void ChangeAllCharacterColors(Color colorWanted, GameObject character)
    {
        var playerID = 0;
        if (MatchmakingHandler.Instance.IsInsideLobby)
            playerID = character.GetComponent<NetworkPlayer>().NetworkSpawnID;

        ChangeLineRendColor(colorWanted, character);
        ChangeSpriteRendColor(colorWanted, character);
        ChangeParticleColor(colorWanted, character);
        ChangeWinTextColor(colorWanted, playerID);

        Traverse.Create(character.GetComponentInChildren<BlockAnimation>()).Field("startColor").SetValue(colorWanted);
        var playerNames = Traverse.Create(Object.FindObjectOfType<OnlinePlayerUI>())
            .Field("mPlayerTexts").GetValue<TextMeshProUGUI[]>();

        playerNames[playerID].color = colorWanted;
    }

}