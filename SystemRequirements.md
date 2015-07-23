# System Requirements #

Below, all the programs and utilities required for running the trained Evolutionary (E)MAPF-based AI are listed:
<ul>
<li>  A full version of StarCraft Brood War updated to patch 1.16.1.</li>
<li> Chaoslauncher Version 0.5.2.1 for Starcraft 1.16.1 or newer4.</li>
<li> BWAPI version 3.4 beta. Follow the first 4 steps in the instructions of the read me file in the zip file.</li>
<li> Microsoft .Net 3.5 Framework SP1 and/or Mono 2.8 or later.</li>
<li> Microsoft Windows 7<br>
<ul>
<li>Older versions of Windows might work with the AI, but have not been tested.) </li>
<li> </li>
<li> BWAPI uses DLL injection and is therefore unlikely to work in Wine, a virtual windows environment or Mac. </li>
</ul>
</li>
<li> Microsoft Visual Studio 2010 is only needed if you want to see or change code in the solution (Not needed for running the AI). If you want to be able to compile the bwapi-native module inside the solution, Microsoft Visual Studio 2008 with SP 1 is also needed. </li>
</ul>


# Instructions #
<p>
To start with copy the release folder to your hard drive. To start<br>
the AI: run the Mono- or the Windows commando prompt (CMD) as administrator,<br>
then locate and run the StarcraftBot.exe (admin mode is very important, or<br>
Chaoslauncher will not be able to see the client AI). Be sure that the xml le "`!Star-<br>
CraftUnitAgent.xml"' is located in the same folder, as the .exe le. Then run the<br>
Chaoslauncher as administrator. Be sure that the BWAPI Injector (RELEASE),<br>
Chaosplugin and optionally W-Mode (if StarCraft is to run in window mode) is<br>
added to the list of plugins inside of Chaoslauncher. Then just click start, and the<br>
game should start. Choose a single or multi player game, and start choosing one of<br>
the test StarCraft maps used in the thesis and set the type of game to "use map<br>
settings", otherwise both teams start with a command center, instead of only units.<br>
</p>
<p>
For easier menu handling, the bwapi.ini le from the bwapi-data folder can be set to automatically start the game, using a predened map.<br>
</p>