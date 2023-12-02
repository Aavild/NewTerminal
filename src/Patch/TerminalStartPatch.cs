using System;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace NewTerminal.Patch;
/// <summary>
/// Prefix is run before the Start method on Terminal
/// </summary>
[HarmonyPatch(typeof(Terminal), "Start")]
public class TerminalStartPatch
{
    static bool Prefix(Terminal __instance)
    {
        var specialConfigFile = CreateNewConfig("NewTerminal-Special");
        var verbsConfigFile = CreateNewConfig("NewTerminal-Verbs");
        var otherConfigFile = CreateNewConfig("NewTerminal-Other");
        
        // __instance.terminalNodes.specialNodes
        foreach (TerminalNode node in __instance.terminalNodes.specialNodes)
            ConfigTerminalMenu(specialConfigFile, "TerminalMenus.Special", node.name, ref node.displayText);

        // __instance.terminalNodes.allKeywords
        foreach (TerminalKeyword keyword in __instance.terminalNodes.allKeywords)
        {
            if (keyword.isVerb)
            {
                foreach (CompatibleNoun noun in keyword.compatibleNouns)
                    ConfigTerminalMenu(verbsConfigFile, $"TerminalMenus.{keyword.name}", $"{noun.noun.name}", ref noun.result.displayText);
                
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
            
            ConfigTerminalMenu(otherConfigFile, "TerminalMenus.Other", keyword.name, ref keyword.specialKeywordResult.displayText);
            
        }
        
        // __instance.terminalNodes.terminalNodes is empty

        return true;
    }

    
    private static readonly string DefaultPath = Plugin.Instance.Config.ConfigFilePath;
    private static ConfigFile CreateNewConfig(string fileName)
    {
        return
            new ConfigFile(
                DefaultPath.Replace("NewTerminal", fileName, StringComparison.OrdinalIgnoreCase), false,
                Plugin.InstanceDefinition);
    }

    /// <summary>
    /// Creates or uses a config entry for a given terminal element.
    /// </summary>
    /// <param name="file"></param> the ConfigFile to save to
    /// <param name="section"></param> The section under which the option is shown
    /// <param name="name"></param> The key of the configuration option in the configuration file
    /// <param name="displayText"></param> The text to be configured
    private static void ConfigTerminalMenu(ConfigFile file, string section, string name, ref string displayText)
    {
        try
        {
            var configEntry = file.Bind(
                section,
                name,
                displayText, 
                "");
            if (configEntry.Value.Length > 150000)
            {
                Plugin.Log.LogError($"{name} has a length beyond 150.000 and will be skipped");
                return;
            }
            displayText = configEntry.Value;
        }
        catch (Exception e) //IOException, displayText null
        {
            Plugin.Log.LogError($"Error loading {name} in {section} in {file.ConfigFilePath}");
            Plugin.Log.LogError(e);
        }
    }
}