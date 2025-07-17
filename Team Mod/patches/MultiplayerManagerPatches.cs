// Credits to Monky for the colour functions below. Since V1.2.1, OnPlayerSpawnedMethodPrefix's functionality was moved to Helper.cs. Now the method runs with a 1 second delay so that it overrides QOL Mod's behavior if needed.

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
            .GetMethod(nameof(OnPlayerSpawnedMethodPostfix)))
        {
            priority = Priority.Last
        };
        harmonyInstance.Patch(onPlayerSpawnedMethod, postfix: onPlayerSpawnedMethodPostfix);

    }
    public static void OnPlayerSpawnedMethodPostfix(MultiplayerManager __instance)
    {
        Helper.Instance.StartCoroutine(Helper.DelayedColorUpdate(__instance));
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
