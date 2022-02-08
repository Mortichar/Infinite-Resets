using BepInEx;
using HarmonyLib;

namespace level_uncapper
{
    public class PluginInfo
    {
        public const string PLUGIN_GUID = "com.mortichar.infinite_resets";
        public const string PLUGIN_NAME = "InfiniteResets";
        public const string PLUGIN_VERSION = "1.0.0";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Star Valor.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(CharacterScreen), "ResetSkills")]
        [HarmonyPrefix]
        static void ResetSkills(ref CharacterScreen __instance, ref bool __runOriginal)
        {   
            PChar.Char.resetSkillsPoints++;
            __runOriginal = true;
        }
    }
}
