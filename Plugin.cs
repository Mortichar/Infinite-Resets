using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace InfiniteResets
{
    public class PluginInfo
    {
        public const string PLUGIN_GUID = "com.mortichar.infinite_resets";
        public const string PLUGIN_NAME = "InfiniteResets";
        public const string PLUGIN_VERSION = "0.2.0";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Star Valor.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private static ConfigEntry<bool> doResetsCost;
        private static ConfigEntry<float> resetCostPerPoint;
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Harmony.CreateAndPatchAll(typeof(Plugin));

            doResetsCost = Config.Bind("General",
                "Do Resets Cost",
                true,
                "Do resets cost credits? If true, they cost credits relative to the number of spent skill points.");

            resetCostPerPoint = Config.Bind("General",
                "Reset Cost Per Spent Point",
                1000.0f,
                "If resets aren't free, this is the price that must be paid per spent skill point to reset skills. Note that the game's free resets are used first.");
        }

        [HarmonyPatch(typeof(CharacterScreen), "ResetSkills")]
        [HarmonyPrefix]
        public static void ResetSkills()
        {
            // if the player doesn't have any available skill points
            if (PChar.Char.resetSkillsPoints == 0)
            {
                if (doResetsCost.Value)
                {

                    // get the player's cargo (and thus credits) component
                    var cargo_system = GameObject.FindGameObjectWithTag("Player").GetComponent<CargoSystem>();
                    var points_spent = 0;
                    foreach (var points_in_skill in PChar.Char.SK)
                    {
                        points_spent += points_in_skill;
                    }
                    var total_cost = points_spent * resetCostPerPoint.Value;
                    if (cargo_system.credits >= total_cost)
                    {
                        cargo_system.credits -= total_cost;
                        PChar.Char.resetSkillsPoints++;
                    }
                }
                else
                {
                    PChar.Char.resetSkillsPoints++;
                }
            }
        }
    }
}
