using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace InfinityWarhorn;

[HarmonyPatch]
public class Patch
{
    // This is a patch to remove the condition that the warhorn can only be used every 5 waves.
    // https://github.com/Elin-Modding-Resources/Elin-Decompiled/blob/aecb7a5b7f2828cb102023dfdb5aa02fe7ac94f3/Elin/TraitCoreDefense.cs#L18
    [HarmonyTranspiler, HarmonyPatch(typeof(TraitCoreDefense), "TrySetAct")]
    public static IEnumerable<CodeInstruction> TraitCoreDefense_TrySetAct_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);

        // idc.i4.5   -> pop
        // rem        -> ldc.i4.1
        // brfalse.s
        //
        // `pop` is necessary to keep the stack clean
        for (int i = 0; i < codes.Count - 2; i++)
        {
            if (
                codes[i].opcode == OpCodes.Ldc_I4_5 &&
                codes[i + 1].opcode == OpCodes.Rem &&
                codes[i + 2].opcode == OpCodes.Brfalse
            )
            {
                codes[i] = new CodeInstruction(OpCodes.Pop);
                codes[i + 1] = new CodeInstruction(OpCodes.Ldc_I4_1);
                InfinityWarhorn.Log("IR patched");
                break;
            }
        }

        return codes;
    }

    // This is a patch to spawn a boss even if the player skips the wave with the horn.
    //
    // In the original code, the boss is spawned on turn 10 every 5 waves.
    // But if the player skips the wave with the horn, the boss will not spawn. Because the turn 10 is skipped.
    // So, spawn a boss manually if the player skips the wave.
    [HarmonyPrefix, HarmonyPatch(typeof(ZoneEventDefenseGame), "Horn_Next")]
    public static void ZoneEventDefenseGame_Horn_Next_Postfix(ZoneEventDefenseGame __instance)
    {
        var ev = __instance;
        if (ev.wave % 5 == 0 && ev.turns < 10) {
			Rand.SetSeed(ev.wave + ev.quest.uid);
			ev.SpawnBoss(((ev.wave >= 10) ? (ev.wave * 2) : 0) > EClass.rnd(100));
			Rand.SetSeed();
        }
    }
}
