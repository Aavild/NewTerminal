using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using HarmonyLib;

namespace NewTerminal.Patch;
/// <summary>
/// Prefix is run before the Start method on Terminal
/// </summary>
[HarmonyPatch(typeof(Terminal), "Start")]
public class TerminalStartPatch
{
    static void Postfix(Terminal __instance)
    {
        var specialConfigFile = CreateNewConfig("NewTerminal-Special");
        var verbsConfigFile = CreateNewConfig("NewTerminal-Verbs");
        var otherConfigFile = CreateNewConfig("NewTerminal-Other");
        var unusedConfigFile = CreateNewConfig("NewTerminal-Unused");
        
        // __instance.terminalNodes.specialNodes
        foreach (TerminalNode node in __instance.terminalNodes.specialNodes)
        {
            ConfigTerminalMenuText(specialConfigFile, "TerminalMenus.Special", node.name, ref node.displayText);
        }

        // __instance.terminalNodes.allKeywords
        foreach (TerminalKeyword keyword in __instance.terminalNodes.allKeywords)
        {
            if (keyword.isVerb)
            {
                ConfigTerminalMenuCmd(verbsConfigFile, $"TerminalMenus.{keyword.name}-cmd", keyword, 1);
                foreach (CompatibleNoun noun in keyword.compatibleNouns)
                {
                    ConfigTerminalMenuText(verbsConfigFile, $"TerminalMenus.{keyword.name}", $"{noun.noun.name}", ref noun.result.displayText);
                    // Command for verb + noun is added if keyword.defaultVerb != null
                }
                
                continue;
            }

            if (keyword.defaultVerb != null)
            {
                ConfigTerminalMenuCmd(verbsConfigFile, $"TerminalMenus.{keyword.defaultVerb.name}-cmd", keyword, 2);
                continue;
            }
            if (keyword.accessTerminalObjects) {
                ConfigTerminalMenuCmd(otherConfigFile, "TerminalMenus.accessTerminalObjects-cmd", keyword, 2);
                continue;
            }
            if (keyword.specialKeywordResult == null)
            {
                Plugin.Log.LogWarning($"Unused: {keyword.name}");
                ConfigTerminalMenuCmd(unusedConfigFile, "TerminalMenus.Unused-cmd", keyword, 2);
                continue;
            }
            
            ConfigTerminalMenuText(otherConfigFile, "TerminalMenus.Other", keyword.name, ref keyword.specialKeywordResult.displayText);
            ConfigTerminalMenuCmd(otherConfigFile, "TerminalMenus.Other", keyword, 2);
        }
        
        // __instance.terminalNodes.terminalNodes is empty

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
    private static void ConfigTerminalMenuText(ConfigFile file, string section, string name, ref string displayText)
    {
        try
        {
            var configEntry = file.Bind(
                section,
                name,
                displayText, 
                "").Value;
            // Check https://github.com/Aavild/NewTerminal/issues/1#issuecomment-1841450262
            int sum = configEntry.Split('\n').Sum(s => (s.Length - 1) / 51 * 51 + 51); // using of newline is equivalent of rounding the characters for that line up to 51.
            if (sum > 4794)
            {
                Plugin.Log.LogError($"{name} has a length beyond 4.794 and will be skipped");
                return;
            }
            displayText = configEntry;
        }
        catch (Exception e) //IOException, displayText null
        {
            Plugin.Log.LogError($"Error loading {name} in {section} in {file.ConfigFilePath}");
            Plugin.Log.LogError(e);
        }
    }

    private static readonly List<string> _commands = new();

    /// <summary>
    /// Creates or uses a config entry for a terminal command name
    /// </summary>
    /// <param name="file"></param>
    /// <param name="section"></param>
    /// <param name="node"></param>
    /// <param name="minimalLetters"></param>
    private static void ConfigTerminalMenuCmd(ConfigFile file, string section, TerminalKeyword node, int minimalLetters)
    {
        try
        {
            var configEntry = file.Bind(
                section,
                node.name + "-cmd",
                node.word, 
                "").Value.ToLower();
            // Having a command being longer than 51 letters is ridiculous 
            if (configEntry.Length > 51)
            {
                Plugin.Log.LogError($"{node.name} command name has a length beyond 51 in length");
                if (_commands.Contains(node.word))
                    Plugin.Log.LogError($"{node.word} is also used in some other command. This will give unexpected behaviour");
                _commands.Add(node.word);
                return;
            }

            if (configEntry.Length < minimalLetters)
            {
                Plugin.Log.LogError($"{node.name} command name is too short. Needs {minimalLetters} letters");
                if (_commands.Contains(node.word))
                    Plugin.Log.LogError($"{node.word} is also used in some other command. This will give unexpected behaviour");
                _commands.Add(node.word);
                return;
            }

            if (_commands.Contains(configEntry))
            {
                Plugin.Log.LogError($"{configEntry} command name already exists. using default");
                _commands.Add(node.word);
                return;
            }
            node.word = configEntry;
        }
        catch (Exception e) //IOException, displayText null
        {
            Plugin.Log.LogError($"Error loading {node.name} in {section} in {file.ConfigFilePath}");
            Plugin.Log.LogError(e);
        }
    }
}