using BepInEx;
using HarmonyLib;

namespace InfinityWarhorn;

internal static class ModInfo
{
    internal const string Guid = "me.pocke.infinity-warhorn";
    internal const string Name = "Infinity War Horn";
    internal const string Version = "1.0.0";
}

[BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
internal class InfinityWarhorn : BaseUnityPlugin
{
    internal static InfinityWarhorn Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
        new Harmony("InfinityWarhorn").PatchAll();
    }

    public static void Log(object message)
    {
        Instance.Logger.LogInfo(message);
    }
}
