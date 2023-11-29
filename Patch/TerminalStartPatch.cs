using System;
using HarmonyLib;

namespace NewTerminal.Patch;
/// <summary>
/// Prefix is run before the Start method on Terminal
/// </summary>
[HarmonyPatch(typeof(Terminal), "Start")]
public class TerminalStartPatch
{
    static bool Prefix(Terminal __instance)
    {
        foreach (TerminalNode node in __instance.terminalNodes.specialNodes)
            ConfigTerminalMenu("TerminalMenus.Special", node.name, ref node.displayText);

        // __instance.terminalNodes.allKeywords are super weird
        foreach (TerminalKeyword keyword in __instance.terminalNodes.allKeywords)
        {
            if (keyword.isVerb)
            {
                foreach (CompatibleNoun noun in keyword.compatibleNouns)
                    ConfigTerminalMenu("TerminalMenus.Nouns", noun.noun.name, ref noun.result.displayText);
                
                continue;
            }
            if (keyword.accessTerminalObjects || keyword.defaultVerb != null)
                continue;
            if (keyword.specialKeywordResult == null)
            {
                Plugin.Log.LogWarning($"Skipping {keyword.name}");
                // Plugin.Log.LogInfo(ObjectDumper.Dump(keyword));
                continue;
            }
            
            ConfigTerminalMenu("TerminalMenus.Other", keyword.name, ref keyword.specialKeywordResult.displayText);
            
        }
        
        // __instance.terminalNodes.terminalNodes is empty

        return true;
    }

    /// <summary>
    /// Creates or uses a config entry for a given terminal element.
    /// </summary>
    /// <param name="section"></param> The section under which the option is shown
    /// <param name="name"></param> The key of the configuration option in the configuration file
    /// <param name="displayText"></param> The text to be configured
    private static void ConfigTerminalMenu(string section, string name, ref string displayText)
    {
        try
        {
            var configEntry = Plugin.Instance.Config.Bind(
                section,
                name,
                displayText, 
                "");
            if (configEntry.Value.Length > 240)
                throw new OverflowException(
                    "Terminal Message can't be longer than 240 letters");
            displayText = configEntry.Value;
        }
        catch (Exception e) //IOException, Overflow, displayText null
        {
            Plugin.Log.LogError($"Error loading {name} in {section}");
            Plugin.Log.LogError(e);
        }
    }
}