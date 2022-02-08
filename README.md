# KH3 Randomizer Website
This is an Electron.NET project that is used to generate seeds for the KH3 Randomizer! This project is a fork of WaterKH's original [KH3 Randomizer Website](https://github.com/WaterKH/KH3RandomizerWebsite) and is meant to be used with WaterKH and CriticPerfect's [KH3 Randomizer + GoA Mod Pack.](https://github.com/Water-and-Critic/KH3-Rando-GoA)

Some major changes that this fork makes compared to the original site can be seen as follows:
* Added additional extras that can be chosen on the Pools page. These will be expanded upon in the future and they aim to provide more options for people to play the randomizer in a way they enjoy most.
* By default, this project will replace all checks that are on Reports and Level Ups. This means that any bonuses that would normally be placed on either battlegates or level up rewards will instead be put elsewhere in the pool.
  * Battlegates are inaccessible until you beat the game, so any checks that would be placed there are currently inaccessible. If you want to have hints, having Secret Reports actually be in the pool is necessary, so this change allows for them to be obtainable without potentially locking checks behind inaccessible battlegates.
  * Level Ups also seem to be working inconsistently at this point in time and have a chance to not give you items/magic/abilities for a seed, so with this setup, Level Ups will only give you their normal stat bonuses instead of potentially locking you out of important checks.

## Table of Contents
* [Setup](#setup)
* [Seed Generation Instructions](#instructions)
* [Bug Reports](#bugs)

## Setup <a name="setup"></a>
- Download the newest version of this project from [the releases page here.](https://github.com/KiernanBrown/KH3RandomizerWebsite/releases)
- Extract the KH3Randomizer folder from the zip that you downloaded.
- Inside the KH3Randomizer is a KH3Randomizer.exe file. This is the file you'll want to run.

If you don't already have the KH3 Randomizer + GoA Mod Pack set up, you can find more information on how to set that up either on the Guide page of this project or by following the steps below:

**Make sure to back up your KH3 save files before following these steps, as your save files will be corrupted and deleted upon launching the game with the mod for the first time!!!**

* Download the newest version of the KH3 Randomizer + GoA Mod Pack from [the releases page here.](https://github.com/Water-and-Critic/KH3-Rando-GoA/releases) You'll need to download the pakchunk9-GardenOfAssemblageRandomizer_99_P.pak file but nothing else.
* Navigate to your Epic Games Kingdom Hearts 3 installation folder. Once there, you'll want to follow this path: \KINGDOM HEARTS III\Content\Paks which will lead you to your Paks folder. Inside this folder, create a new folder and call it ~mods
* Place the pakchunk9-GardenOfAssemblageRandomizer_99_P.pak file that you downloaded before into this newly created ~mods folder.

You will also need to make sure that you have the most recent version of the [UnrealPak Tool](https://github.com/allcoolthingsatoneplace/UnrealPakTool/releases/) downloaded. This tool is used to turn the seed you create from this site into a pak file that can be used with KH3.

## Seed Generation Instructions <a name="instructions"></a>
1. Choose whatever pools and extras that you want to have active on your seed and continue to the Options page.
2. Enter a seed name on the bar at the top. This is required to actually randomize your seed so make sure that you do this!
3. Continue to the Hints page and choose which hints settings that you want. These hints will display as the text on your Secret Reports in game.
    * None: No hints will be given on reports.
    * Vague: Reports will give you a vague description of where a check is.
      * If Proof of Times Past was in Twilight Town Small Chest 1, a report could tell you "There is 1 check in Twilight Town"
    * Verbose: Reports will give you a detailed description of exactly where a check is.
      * If Proof of Times Past was in Twilight Town Small Chest 1, a report could tell you "Proof of Times Past is in Twilight Town in Small Chest 1"
4. Click the Generate Seed button on the bottom right of the Review Page to download your seed's zip file.
5. Extract the zip file that you just downloaded. 
    * If you're using WinRAR then you can right click on the zip file and and choose the option that says Extract To pakchunk99-randomizer-SeedName and it will create a new folder and extract the needed files to it.
    * If you're not using WinRAR, extract both the KINGDOM HEARTS III folder and the SpoilerLog.json file into a new folder. The name of this folder doesn't matter but name it something that you'll remember.
6. Drag the folder that you just extracted/created onto the UnrealPak-With-Compression.bat file that. This will create a pak file for your seed in the same location that your extracted folder was in.
7. Place your seed's pak file that you just made into your ~mods folder in your Kingdom Hearts 3 installation folder.
8. You're all set to go!

## Bug Reports <a name="bugs"></a>
If you experience any issues with this project, feel free to let me know by sending me a message on Discord (SwiftShadow#5004). Please note that this side of the project is only focused on the website end and seed generation, so if you have a general bug report that relates to the Randomizer + GoA mod itself, it would be best to go through [this bug report form.](https://forms.gle/bN8YFXhxRUwWSWbp7) If your bug report does pertain to a seed that you were trying, it would be extremely helpful if you can include the pak file for your seed and the version number of the website that you used (you can find this on the bottom left hand corner of any page). Thank you so much for taking the time to test this out and I appreciate any feedback you can provide! 