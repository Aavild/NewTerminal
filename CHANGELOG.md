## v1.4.0
* tested for LethalCompany v45
  * Running the plugin even on older versions will produce the new commands which include Transmit (SendSignalTranslator), buy/info SignalTranslator and the "Other" command contains the following: "TRANSMIT \[message\]\\nTo transmit a message with the signal translator"
* Added TerminalMenus.Unused-cmd - if anyone can figure out how to call these commands let me know
* Added TerminalMenus.accessTerminalObjects-cmd in Other
* Fixed github link

## v1.3.0
* SomeCommand-cmd for all commands which allows renaming commands
* Added Creative Commons which permits anyone to copy everything in the mod and redistribute without including the license

## v1.2.0

* Made so that NewTerminals can't crash the Terminal (I think)
* Set the upper limit to 4794 characters

## v1.1.1

* Restructured project with following new structure:
  * src - code project
  * wiki - WIP guide on how to use the plugin beyond the README.md
  * .github/workflows - workflow for automatically creating releases on Github and ThunderStore
  * A single README.md
  * Stopped producing NewTerminal.deps.json

## v1.1.0

* Fixed some keywords being combined into a single keyword. (`NewTerminal.cfg`'s `[NewTerminal.Nouns]` specifically)
  * Example: `Buy ProFlashlight` and `Info ProFlashlight` or `Route 56-Vow` and Info `56-Vow`
* Split the config into multiple files: `NewTerminal-Other.cfg`, `NewTerminal-Special.cfg`, `NewTerminal-Verbs.cfg` and `NewTerminal.cfg` (unused for now)
  * Note: this deprecates the old values in the `NewTerminal.cfg`, although the values will not be immediately removed. If you want to preserve the values you can copy them over in their new associated files and you can safely delete the file

## v1.0.0
* initial release