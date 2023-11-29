using BepInEx.Configuration;
using BepInEx.Logging;

namespace NewTerminal.Config;

public class ConfigManager
{
    private ConfigEntry<string> configGreeting;
    private ConfigEntry<bool> configDisplayGreeting;

    internal void LoadConfig()
    {
        configGreeting = Plugin.Instance.Config.Bind("General",      // The section under which the option is shown
            "GreetingText",  // The key of the configuration option in the configuration file
            "Hello, world!", // The default value
            "A greeting text to show when the game is launched"); // Description of the option to show in the config file
        
        configDisplayGreeting = Plugin.Instance.Config.Bind("General.Toggles", 
            "DisplayGreeting",
            true,
            "Whether or not to show the greeting text");
        // Test code
        Plugin.Log.LogInfo("Hello, world!");
    }
}