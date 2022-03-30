# KH3 Randomizer Seed Generator
This is an Electron.NET project that is used to generate seeds for the KH3 Randomizer! This project is a fork of WaterKH's original [KH3 Randomizer Website](https://github.com/WaterKH/KH3RandomizerWebsite) and is meant to be used with WaterKH and CriticPerfect's [KH3 Randomizer + GoA Mod Pack.](https://github.com/Water-and-Critic/KH3-Rando-GoA) This fork heavily relies on the existing infrastructure that was set up by Water and the changes that are being made are more focused on the seed generation aspect of things. The goal of this fork is to provide more options during seed generation that people can experiment with, and these changes should make their way to the main branch of the project at some point in the future.

## Table of Contents
* [Changes](#changes)
* [Setup](#setup)
* [Seed Generation Instructions](#instructions)
* [Planned Changes](#plans)
* [Bug Reports](#bugs)

## Changes <a name="changes"></a>
All of the major changes that have been made on this fork can be seen below:
### <b>Extras</b>
Added the following extras that can be chosen on the Pools page. These will be expanded upon in the future and they aim to provide more options for people to play the randomizer in a way they enjoy most.
<details><summary>All Extras</summary>

<ul>
  <li>Balanced Bonuses: Evenly distributes checks across bonuses.</li>
  <li>Pole Spin Start: Ensures you start with Pole Spin as a starting ability.</li>
  <li>Synthesizable Proofs: Allows you to toggle whether Proofs can appear in the Moogle Synthesis menu.</li>
  <li>Allow Ultima: Either puts Ultima Weapon into the pool or removes it from the pool regardless of if Synthesis Items are enabled.</li>
  <li>Allow Key Abilities on Fullcourse/Equippables/Weapon Upgrades: Allows/disallows key abilities from being on abilities gained from fullcourse meals/equippables/weapon upgrades. These extras work with a Key Abilities list that you can set on the Pools page.</li>
</ul>
</details>

### <b>Sub Pools/Replacements</b>
Added a Sub Pools section to the Pools and Spoiler Log pages where users can choose whether certain options in a pool should be Replaced or kept Randomized.
<details><summary>More Details</summary>

<ul>
  <li>The following options are set to Replace by default:</li>
  <ul>
    <li>Battlegates are inaccessible until you beat the game, so any checks that would be placed there are currently inaccessible. If you want to have hints, having Secret Reports actually be in the pool is necessary, so this change allows for them to be obtainable without potentially locking checks behind inaccessible battlegates.</li>
    <li>Level Ups also seem to be working inconsistently at this point in time and have a chance to not give you items/magic/abilities for a seed, so with this setup, Level Ups will only give you their normal stat bonuses instead of potentially locking you out of important checks.</li>
    <li>Defeating Yozora ends up taking away your Proof of Fantasy, so replacing his rewards stops him from being a necessary fight for the time being.</li>
    <li>Completing 100 Acre Wood, All Classic Kingdom Minigames, or All Bistro Recipes. These are somewhat lengthy sections of fluff and minigames.</li>
  </ul>
  <li>More options will be added in the future. This first batch is to test and make sure that this way of doing replacing isn't causing any major problems elsewhere.</li>
</ul>
</details>

### <b>Hints</b>
Overhauled the Hints page to provide more options.
<details><summary>More Details</summary>

<ul>
  <li>Added a search bar that can be used to build a list of Important Checks which will then be hinted.</li>
  <li>Added a "World" Hint Type where a report will give the number of important checks for a world/category. This is similar in structure to JSmartee's hints for the 2FM rando.</li>
  <li>Hints for checks that are on Moogle Photo Missions are a bit more descriptive now when using the Vague or Verbose hint types.</li>
  <li>Reports should no longer hint themselves.</li>
  <li>Changed references to magic in verbose hints to be in the form of [base spell] element instead of base spell/ra/ga.</li>
  <ul>
    <li>For example: The three fire checks will now all be referred to as Fire Element instead of Fire, Fira, and Firaga.</li>
  </ul>
  <li>If you load a spoiler log on the Spoiler Log page, you will now get a collapsable that shows what hints are present on that seed. This was mostly for testing purposes and might be removed in the future.</li>
</ul>
</details>

### <b>Presets</b>
Added the ability to use presets for your seed's settings which can speed up seed generation and make things less tedious.
<details><summary>More Details</summary>

<ul>
  <li>Added a Presets button on the Pools page which opens up a Presets menu. Users can use this menu to select a preset selection of settings to be used for seed generation.</li>
  <ul>
    <li>Your preset files are located in the resources/bin/Settings folder of the KH3Randomizer folder that you download from here. This release comes with 3 files that contain my preferred settings. If you want to share your preset settings with others, feel free to take the files from this folder and share them that way. Likewise, you can add preset settings files to this folder and they'll be accessible from the seed generator. You will also want to keep this location in mind for bringing your settings presets over to new versions of this fork when they are released.</li>
  </ul>
  <li>Added an Upload Spoiler Log option on the Presets menu that allows you to take a SpoilerLog.json file and save the settings from that as a settings file.</li>
  <li>Added options to rename/delete preset settings from the Preset menu.</li>
  <li>Added a Save Preset button on the Review page when you are not using a preset. This will allow you to save your current settings as a preset that you can use later.</li>
  <li>If you create a seed without using a preset and don't save your settings as a preset, a Previous Seed preset will be automatically generated. This file will get deleted the next time you generate a seed, but you can rename it from the Presets menu to stop this from happening if you do want to keep those settings.</li>
</ul>
</details>

### <b>Other Changes</b>
<details><summary>More Details</summary>
  <ul>
    <li>Added a Key Abilities section to the Pools page where users can create a list of Key Abilities. This list works with the Allow Key Abilities on Fullcourse/Equippables/Weapon Upgrades extras and the Key Abilities quick toggle on the Hints page.</li>
    <li>Added a Duplicate Abilities section to the Pools page where users can create a list of additional copies of abilities that are added into the randomization pool. This is currently capped at a total of 20 abilities.</li>
    <li>Added a button on the Options page that will generate a random seed name for you.</li>
    <li>Added a warning for users trying to continue from the Options page to the Hints page without entering a seed name.</li>
    <li>Tooltips and hints are a bit more descriptive (thanks to Sonicshadowsilver2).</li>
    <li>Chests now have tooltips and their hints are more descriptive (thanks to Clara_The_Classy).</li>
    <ul>
      <li>Verbose hints will now tell you the gummiphone chest number and the area the chest is found in instead of the chest numbers the game uses for reference.</li>
    </ul>
    <li>VBonuses from fights are now grouped by their world and can be toggled independently.</li>
    <li>Photo Missions are now grouped as a sub category in Synthesis Items and can be toggled or replaced.</li>
    <li>Changed where Pole Spin can be placed.</li>
    <ul>
      <li>Pole Spin can only be found in Disney Worlds (no Dark World, Keyblade Graveyard, Re:Mind Keyblade Graveyard, Scala Ad Caelum, Final World).</li>
    </ul>
</details>

### <b>Bug Fixes</b>
* Fixed an issue where seed randomization could hang when trying to place Pole Spin in a valid spot.
* Fixed some Bonus descriptions, missing synthesized armor names, and accessory mistranslations.
* Fixed an issue where seeds could be missing a proof.

## Setup <a name="setup"></a>
[![KH3 Rando Tutorial - Everything you need to know to start playing](https://res.cloudinary.com/marcomontalbano/image/upload/v1648596247/video_to_markdown/images/youtube--0wmPApYfbtM-c05b58ac6eb4c4700831b2b3070cd403.jpg)](https://youtu.be/0wmPApYfbtM "KH3 Rando Tutorial - Everything you need to know to start playing")
[https://youtu.be/0wmPApYfbtM](https://youtu.be/0wmPApYfbtM)
 
  
 Follow the video tutorial above, or use this text tutorial to get setup with everything that you need!
- Download the newest version of this project from [the releases page here.](https://github.com/KiernanBrown/KH3RandomizerWebsite/releases)
- Extract the KH3Randomizer folder from the zip that you downloaded.
- Inside the KH3Randomizer is a KH3Randomizer.exe file. This is the file you'll want to run.

If you don't already have the KH3 Randomizer + GoA Mod Pack set up, you can find more information on how to set that up either on the Guide page of this project or by following the steps below:

**Make sure to back up your KH3 save files before following these steps, as your save files will be corrupted and deleted upon launching the game with the mod for the first time!!!**

* Download the newest version of the KH3 Randomizer + GoA Mod Pack from [the releases page here.](https://github.com/Water-and-Critic/KH3-Rando-GoA/releases) You'll need to download the pakchunk9-GardenOfAssemblageRandomizer_99_P.pak file but nothing else.
* Navigate to your Epic Games Kingdom Hearts 3 installation folder. Once there, you'll want to follow this path: \KINGDOM HEARTS III\Content\Paks which will lead you to your Paks folder. Inside this folder, create a new folder and call it ~mods
* Place the pakchunk9-GardenOfAssemblageRandomizer_99_P.pak file that you downloaded before into this newly created ~mods folder.

You will also need to make sure that you have the most recent version of [unrealpak](http://modderbase.com/showthread.php?tid=834) downloaded. This tool is used to turn the seed you create from this site into a pak file that can be used with KH3.

## Seed Generation Instructions <a name="instructions"></a>
1. Choose whatever pools and extras that you want to have active on your seed and continue to the Options page.
2. Enter a seed name on the bar at the top. This is required to actually randomize your seed so make sure that you do this!
3. Continue to the Hints page and choose which hints settings that you want. These hints will display as the text on your Secret Reports in game. You can select your own list of important checks using the search bar on the hints page. The Hint Selection Types are detailed below:
    * None: No hints will be given on reports.
    * Vague: Reports will give you a vague description of where a check is.
      * If Proof of Times Past was in Toy Box Large Chest 1, a report could tell you "There is 1 check in Toy Box"
    * Verbose: Reports will give you a detailed description of exactly where a check is.
      * If Proof of Times Past was in Toy Box Large Chest 1, a report could tell you "Proof of Times Past is in Toy Box in Chest 1 (Large Chest, Andy's House)"
    * World: Reports will hint the number of checks in a given world/category.
          * If Proof of Times Past was in Toy Box Large Chest 1 and there were 2 other checks in Toy Box, a report could tell you "There are 3 checks in Toy Box."
4. Click the Generate Seed button on the bottom right of the Review Page to download your seed's zip file.
5. Extract the zip file that you just downloaded. 
    * If you're using WinRAR then you can right click on the zip file and and choose the option that says Extract To pakchunk99-randomizer-SeedName and it will create a new folder and extract the needed files to it.
    * If you're not using WinRAR, extract both the KINGDOM HEARTS III folder and the SpoilerLog.json file into a new folder. The name of this folder doesn't matter but name it something that you'll remember.
6. Drag the folder that you just extracted/created onto the UnrealPak-With-Compression.bat file that. This will create a pak file for your seed in the same location that your extracted folder was in.
7. Place your seed's pak file that you just made into your ~mods folder in your Kingdom Hearts 3 installation folder.
8. You're all set to go!

## Planned Changes <a name="plans"></a>
I have a [public trello that you can find here](https://trello.com/b/jGIE27bG) which details any changes/features that are currently planned. If you have any other ideas that you think would be good additions, feel free to let me know by sending me a message on Discord (SwiftShadow#5004)!

## Bug Reports <a name="bugs"></a>
Any currently known bugs with the seed generator will be kept both on [the issues page of this GitHub](https://github.com/KiernanBrown/KH3RandomizerWebsite/issues) and [the bugs section of my trello](https://trello.com/b/jGIE27bG). If you experience any other issues with this project, feel free to let me know by sending me a message on Discord (SwiftShadow#5004) and I'll add them to these lists. If your bug report does pertain to a seed that you were trying, it would be extremely helpful if you can include the pak file for your seed and the version number of the website that you used (you can find this on the bottom left hand corner of any page). Please note that this side of the project is only focused on the website end and seed generation, so if you have a general bug report that relates to the Randomizer + GoA mod itself, it would be best to go through [this bug report form](https://forms.gle/bN8YFXhxRUwWSWbp7) so that WaterKH and CriticPerfect are made aware of it as well! Thank you so much for taking the time to test this out and I appreciate any feedback you can provide! 
