using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace NewTerminal
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Lethal Company.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance;
        internal static ManualLogSource Log;
        private void Awake()
        {
            // Plugin startup logic
            Log = Logger;
            Instance = this;
            
            var harmony = new Harmony("newterminal.patch");
            harmony.PatchAll();
            
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Logger.LogInfo("Config will fully load once entering a lobby");
        }
    }
}
