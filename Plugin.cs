using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using NewTerminal.Config;
using UnityEngine;

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
            Log = Logger;
            Instance = this;
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            
            Logger.LogInfo("This is information");
            Logger.LogWarning("This is a warning");
            Logger.LogError("This is an error");

            ConfigManager config = new ConfigManager();
            config.LoadConfig();
        }
    }
}
