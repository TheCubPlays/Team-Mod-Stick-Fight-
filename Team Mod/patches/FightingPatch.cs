/* 
What it takes to make a man unhittable in stick fight.
The hardest part was finding the correct methods, the game's code is confusing.
Well, this cancels the damage dealt & the force applied to the player when you hit them.
It works for melee too, however I did not modify pushing a person's body because that'd probably be a very weird feature in practice....
Anyway there's a minor flaw, these guns are problematic:
    - Thruster: I can't find a way to make the fire have no effect on you.
    - Grenade launcher: This isn't really an issue, it's meant to be this way. 
        In the normal game, you can hit yourself with grenades so why not your teammates too? 
        It'd be very weird otherwise.
        However, in reality I also just can't find a way to make this work because grenades damage you environmentally. 
        Like I'm not sure if there's a way to track who shot the grenade, it just kinda damages you when it explodes.
        So, I believe that unless I were to make the teammate immune to grenades in general I wouldn't be able to do this.
    - Glue Gun: I don't know how to disable this gun's effects, when I try to patch the "Glue" class the custom colors break. 
        I have no idea. 
        Though, probably not possible because like grenades they are pretty environment-based.
        You can get stuck on them when *interacting with them by yourself*, so this wouldn't work.
    - Ice Gun: I was actually able to cancel this gun's effects, however I don't know how to disable the particles so it looks like it does something when it doesn't.
*/

using HarmonyLib;
using UnityEngine;

namespace TMOD;

public class FightingPatch
{
    public static void Patches(Harmony harmonyInstance)
    {
        /* 
        Basically we use Harmony to patch methods from classes, and we modify their behavior. 
        I'll only explain how 1 works because.. that's all needed and I'm lazy. Though I will be detailed.

        So takeDamageWithParticleMethod is used to locate the method we want from the BodyPart class, obviously we need to specify which one that is.. (TakeDamageWithParticle)
        We also need the appropriate parameters that the original method takes. 
        I got all of this information by inspecting the game's original code (or at least close to it) using dnSpy.
        */
        var takeDamageWithParticleMethod = AccessTools.Method(typeof(BodyPart), "TakeDamageWithParticle", new[]
        {
            typeof(float),
            typeof(Vector3),
            typeof(Vector3),
            typeof(bool),
            typeof(Controller)
        }
        );
        /*
        We're creating a patch method, the method we'll use is TakeDamageWithParticleMethodPrefix
        */
        var takeDamageWithParticleMethodPrefix = new HarmonyMethod(typeof(FightingPatch).GetMethod(nameof(TakeDamageWithParticleMethodPrefix)));
        /*
        So, practically we say "every time the method located in takeDamageWithParticleMethod executes, execute takeDamageWithParticleMethodPrefix first".
        But, that's because we're creating a "prefix" patch method.
        "pre" means it'll execute before the patched method, "post" would mean it'll execute after. (For postfix)
        Ok that's all, good luck.
        */
        harmonyInstance.Patch(takeDamageWithParticleMethod, prefix: takeDamageWithParticleMethodPrefix);

        var takeDamageWithParticle2Method = AccessTools.Method(typeof(BodyPart), "TakeDamageWithParticle", new[]
        {
            typeof(float),
            typeof(Vector3),
            typeof(Vector3),
            typeof(Controller),
            typeof(DamageType)
        }
        );

        var takeDamageWithParticle2MethodPrefix = new HarmonyMethod(typeof(FightingPatch).GetMethod(nameof(TakeDamageWithParticle2MethodPrefix)));

        harmonyInstance.Patch(takeDamageWithParticle2Method, prefix: takeDamageWithParticle2MethodPrefix);

        var method = AccessTools.Method(typeof(ProjectileCollision), "AddForce");
        var prefix = new HarmonyMethod(typeof(FightingPatch).GetMethod(nameof(AddForcePrefix)));
        harmonyInstance.Patch(method, prefix: prefix);

        var iceSpecialMethod = AccessTools.Method(typeof(IceEffect), "IceSpecial", new[] {typeof(Rigidbody)});

        var iceSpecialPrefix = new HarmonyMethod(typeof(FightingPatch), nameof(IceSpecialPrefix));

        harmonyInstance.Patch(iceSpecialMethod, prefix: iceSpecialPrefix);

        var onCollisionEnterMethod = AccessTools.Method(typeof(PunchForce), "OnCollisionEnter", new[] {typeof(Collision)});

        var onCollisionEnterPrefix = new HarmonyMethod(typeof(FightingPatch), nameof(OnCollisionEnterPrefix));

        harmonyInstance.Patch(onCollisionEnterMethod, prefix: onCollisionEnterPrefix);

    }

    // Pretty sure most of these parameters are not needed but I'm too lazy to remove them.

    // In all of those, __instance is essential to access the method's properties.

    // This method ONLY covers melee attacks, damage-wise. (Idk if melee weapons are included or not, I'd assume not).
    public static bool TakeDamageWithParticleMethodPrefix(BodyPart __instance, ref float damage, Vector3 position, Vector3 direction, bool physicalDamage, Controller damager)
    {
        // We get the victim
        Controller victim = __instance.GetComponentInParent<Controller>();
        if (victim != null)
        {
            // We get the victim's color
            ushort victimID = (ushort)victim.playerID;
            string victimColor = Helper.GetColorFromID(victimID);

            // If the victim is our teammate, don't hit. Preferably.
            if (ChatCommands.Teammates.Contains(victimColor.ToLower()))
            {
                return false;
            }
        }
        return true; // Otherwise, hit.
    }
    // This method ONLY covers weapon attacks, damage-wise.
    public static bool TakeDamageWithParticle2MethodPrefix(BodyPart __instance, ref float damage, Vector3 position, Vector3 direction, Controller damager, DamageType type)
    {
        // We get the victim
        Controller victim = __instance.GetComponentInParent<Controller>();
        if (victim != null)
        {
            // We get the victim's color
            ushort victimID = (ushort)victim.playerID;
            string victimColor = Helper.GetColorFromID(victimID);

            // If the victim is our teammate, don't hit. Preferably.
            if (ChatCommands.Teammates.Contains(victimColor.ToLower()))
            {
                return false;
            }
        }
        return true; // Otherwise, hit.
    }
    // Deals with knockback for weapons.
    public static bool AddForcePrefix(Rigidbody rig, float resistance)
    {
        // We get the victim & their color
        Controller victim = rig.GetComponentInParent<Controller>();
        if (victim == null) return true;

        string color = Helper.GetColorFromID((ushort)victim.playerID).ToLower();

        // If the victim is our teammate, don't knock.
        if (ChatCommands.Teammates.Contains(color))
        {
            return false;
        }
        return true; // Otherwise, hit.
    }
    // Deals with ice gun effects, though does not disable particles.
    public static bool IceSpecialPrefix(Rigidbody rig)
    {
        // We get the victim
        Controller victim = rig.GetComponentInParent<Controller>();
        if (victim == null) return true;

        string color = Helper.GetColorFromID((ushort)victim.playerID).ToLower();
        // If the victim is our teammate, don't freeze.
        if (ChatCommands.Teammates.Contains(color))
        {
            return false;
        }

        return true; // Otherwise, hit.
    }

    // Deals with knockback for melee (Melee-Weapons too)
    public static bool OnCollisionEnterPrefix(object __instance, Collision collision)
    {
        // Making sure it's a *player* collision so that we don't break the game
        if (!collision.rigidbody) return true;

        // We get the victim
        Controller victim = collision.transform.root.GetComponent<Controller>();
        if (victim == null) return true;

        string color = Helper.GetColorFromID((ushort)victim.playerID).ToLower();
        // If the victim is our teammate, don't knock.
        if (ChatCommands.Teammates.Contains(color))
        {
            return false;
        }

        return true; // Otherwise, hit.
    }
}