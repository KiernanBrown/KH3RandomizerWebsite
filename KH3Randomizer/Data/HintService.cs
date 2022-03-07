using System;
using System.Collections.Generic;
using UE4DataTableInterpreter.Enums;
using System.Linq;
using UE4DataTableInterpreter.DataTables;
using KH3Randomizer.Models;

namespace KH3Randomizer.Data
{
    public class HintService
    {
        public List<string> ShuffleHints(string seed, Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, string hintType, List<string> importantChecks)
        {
            Dictionary<string, List<string>> hintDictionary = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> reportHints = new Dictionary<string, List<string>>();
            List<SecretReport> reports = new List<SecretReport>();

            if (hintType.Equals("Off"))
                return new List<string>();

            var levelUpTracker = new List<string>();

            List<string> hintCategories = new List<string>()
            {
                "in Olympus.", "in Twilight Town.", "in Toy Box.", "in Kingdom of Corona.",
                "in Monstropolis.", "in Arendelle.", "in The Caribbean.", "in San Fransokyo.",
                "in 100 Acre Wood.", "in The Dark World.", "in The Keyblade Graveyard.", "in The Final World.",
                "in Re:Mind (The Keyblade Graveyard).", "in Scala Ad Caelum.", "in Radiant Garden."
            };

            foreach (var searchCategory in randomizedOptions)
            {
                foreach (var searchSubCategory in searchCategory.Value)
                {
                    foreach (var (searchName, searchValue) in searchSubCategory.Value)
                    {
                        if (importantChecks.Contains(searchValue.ValueIdToDisplay()))
                        {
                            if (searchCategory.Key == DataTableEnum.LevelUp)
                            {
                                var contained = false;
                                foreach (var levelUpItem in levelUpTracker)
                                {
                                    if (levelUpItem.Contains($"{searchSubCategory.Key} ({searchName.ToLevelUpRoute()})"))
                                    {
                                        contained = true;

                                        break;
                                    }
                                }

                                var tempAltLevelUps = this.GetLevelUpAlternatives(randomizedOptions, searchSubCategory.Key, searchName, searchValue);

                                if (!levelUpTracker.Contains(tempAltLevelUps))
                                {
                                    levelUpTracker.Add(tempAltLevelUps);
                                }
                                else if (contained)
                                {
                                    continue;
                                }
                            }

                            var hintLocation = this.GetHintItemLocation(randomizedOptions, searchCategory.Key, searchSubCategory.Key, searchName, searchValue, hintType);
                            var hintText = string.Empty;
                            var hintName = CleanHintName(searchValue.ValueIdToDisplay());

                            if (hintType.Equals("Verbose"))
                                hintText = $"{hintName} is {hintLocation}";
                            else if (hintType.Equals("Vague"))
                                hintText = $"There is 1 check {hintLocation}";
                            else if (hintType.Equals("World"))
                                hintText = hintLocation;

                            if (hintDictionary.ContainsKey(hintText))
                                hintDictionary[hintText].Add(hintName);
                            else
                                hintDictionary.Add(hintText, new List<string> { hintName });

                        }
                    }
                }
            }

            var hash = seed.StringToSeed();
            var rng = new Random((int)hash);

            if (hintType.Equals("World"))
            {
                // Add empty hints if there are not enough to fill reports
                while (hintDictionary.Count < 13)
                {
                    hintDictionary.Add(GetUnusedHintCategory(hintCategories, hintDictionary, rng), new List<string>());
                }
            }

            // Instead of just shuffling, make sure reports don't hint themselves
            int totalHintCount = hintType.Equals("World") ? 13 * ((hintDictionary.Count() - 1) / 13 + 1) : 13 * ((hintDictionary.Sum(x => x.Value.Count) - 1) / 13 + 1);
            int hintsPerReport = totalHintCount / 13;
            int hintPass = 1;
            List<string> shuffledHints = new List<string>();

            bool allHintsPlaced = false;
            while (!allHintsPlaced)
            {
                allHintsPlaced = true;
                // Initialize the list of SecretReports
                reports.Clear();
                for (int i = 1; i < 14; i++)
                {
                    reports.Add(new SecretReport { Name = $"Secret Report {i}", Hints = new List<string>() });
                }

                foreach (var hint in hintDictionary)
                {
                    // Parse the hint for a report number
                    // If it has one, make sure that the hint for this report is outside the bounds this report hints
                    List<string> hintedReports = new List<string>();

                    foreach (var hintName in hint.Value)
                    {
                        if (hintName.Contains("Secret Report"))
                            hintedReports.Add(hintName);
                    }

                    bool isReportHint = hintedReports.Count > 0;
                    int hintTotal = hintType.Equals("World") ? 1 : hint.Value.Count;

                    for (int i = 0; i < hintTotal; i++)
                    {
                        bool validPlacement = false;

                        // For safety, allow a hint to be placed 20 times before attempting to place all hints again
                        // This should ensure we don't end up in an infinite loop trying to place a hint
                        // I didn't run into this issue at all while testing, so this might be unnecessary 
                        int attempts = 0;
                        while (!validPlacement && attempts < 20)
                        {
                            var usableReports = reports.Where(x => x.Hints.Count < hintPass);
                            if (usableReports.Count() == 0)
                            {
                                hintPass++;
                                usableReports = reports.Where(x => x.Hints.Count < hintPass);
                            }
                            int reportIndex = rng.Next(0, usableReports.Count());

                            // Only use this report if it's not being hinted
                            if (!hintedReports.Contains(usableReports.ElementAt(reportIndex).Name))
                            {
                                if (hintType.Equals("World"))
                                    usableReports.ElementAt(reportIndex).Hints.Add(GetWorldHint(hint.Key, hint.Value));
                                else
                                    usableReports.ElementAt(reportIndex).Hints.Add(hint.Key);
                                validPlacement = true;
                            }
                            attempts++;
                        }

                        if (!validPlacement)
                        {
                            allHintsPlaced = false;
                            break;
                        }
                    }

                    if (!allHintsPlaced)
                    {
                        break;
                    }
                }
            }

            // Reports that only hold a "Sora's Initial Setup" hint on World hints are pretty useless
            // Add another hint to these reports
            if (hintType.Equals("World"))
            {
                SecretReport initialSetupReport = null;
                foreach(var report in reports)
                {
                    foreach(var hint in report.Hints)
                    {
                        if (hint.Contains("Sora's Initial Setup"))
                        {
                            initialSetupReport = report;
                        }
                    }
                }

                if (initialSetupReport != null && initialSetupReport.Hints.Count == 1)
                {
                    // Add a 0 check hint to the inital setup hint if it's on its own
                    initialSetupReport.Hints.Add(GetWorldHint(GetUnusedHintCategory(hintCategories, hintDictionary, rng), new List<string>()));

                    if (initialSetupReport.Hints.Count > hintsPerReport)
                        hintsPerReport = initialSetupReport.Hints.Count;
                }
            }

            foreach (var sr in reports)
            {
                shuffledHints.AddRange(sr.GetHintList(hintsPerReport, rng));
            }

            return shuffledHints;
        }

        public byte[] GenerateHints(string seed, Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, string hintType, List<string> importantChecks)
        {
            var hints = new byte[0];
            var mobile = new Mobile();
            List<string> shuffledHints = ShuffleHints(seed, randomizedOptions, hintType, importantChecks);
            int hintsPerReport = shuffledHints.Count / 13;
            hints = mobile.Process(shuffledHints, hintsPerReport).ToArray();

            return hints;
        }

        public Dictionary<string, List<string>> GenerateHintSpoilers(string seed, Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, string hintType, List<string> importantChecks)
        {
            List<string> shuffledHints = ShuffleHints(seed, randomizedOptions, hintType, importantChecks);
            int hintsPerReport = shuffledHints.Count / 13;
            Dictionary<string, List<string>> hintSpoilers = new Dictionary<string, List<string>>();
            for(int i = 0; i <= shuffledHints.Count / hintsPerReport; i++)
            {
                int report = i * hintsPerReport;
                List<string> reportHints = new List<string>();
                for (int j = 0; j < hintsPerReport; j++)
                {
                    if (shuffledHints.Count > report + j)
                        reportHints.Add(shuffledHints[report + j]);
                }

                if (reportHints.Count > 0)
                    hintSpoilers.Add(@$"Secret Report {i + 1}", reportHints);
            }

            return hintSpoilers;
        }

        /// <summary>
        /// Method for getting a hint string when usingthe World hintType
        /// </summary>
        /// <returns>String used for hints</returns>
        public string GetWorldHint(string hintKey, List<string> hintValue)
        {
            return hintValue.Count == 1 ? $"There is {hintValue.Count} check {hintKey}" : $"There are {hintValue.Count} checks {hintKey}";
        }

        /// <summary>
        /// Method for getting a hint category that has no checks on it
        /// This is used for getting 0 check hints with the World hintType
        /// </summary>
        /// <returns>String of a category that has no checks on it</returns>
        public string GetUnusedHintCategory(List<string> hintCategories, Dictionary<string, List<string>> hintDictionary, Random rng)
        {
            bool categoryUnused = false;
            int categoryIndex = 0;
            while (!categoryUnused)
            {
                categoryUnused = true;
                categoryIndex = rng.Next(0, hintCategories.Count);
                if (hintDictionary.ContainsKey(hintCategories[categoryIndex]))
                {
                    categoryUnused = false;
                    continue;
                }
            }

            return hintCategories[categoryIndex];
        }

        public string GetHintItemLocation(Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, DataTableEnum category, string subCategory, string name, string value, string hintType)
        {
            var hintLocation = string.Empty;

            switch (category)
            {
                case DataTableEnum.ChrInit:
                    hintLocation = "on Sora's Initial Setup.";

                    break;
                case DataTableEnum.FullcourseAbility:
                    if (hintType.Equals("Verbose"))
                        hintLocation = $"on a Fullcourse Meal {this.GetFullcourseDescription(subCategory)}.";
                    else if (hintType.Equals("Vague") || hintType.Equals("World"))
                        hintLocation = $"on a Fullcourse Meal.";

                    break;
                case DataTableEnum.LevelUp:
                    if (hintType.Equals("Verbose"))
                        hintLocation = $"on Sora's Level Up {this.GetLevelUpAlternatives(randomizedOptions, subCategory, name, value)}.";
                    else if (hintType.Equals("Vague") || hintType.Equals("World"))
                        hintLocation = $"on Sora's Level Ups.";

                    break;
                case DataTableEnum.LuckyMark:
                    if (hintType.Equals("Verbose"))
                        hintLocation = $"on Lucky Emblem Milestone {subCategory}.";
                    else if (hintType.Equals("Vague") || hintType.Equals("World"))
                        hintLocation = $"on Lucky Emblem Milestones.";
                    
                    break;
                case DataTableEnum.VBonus:
                    var vbonusLocation = this.GetLocation(category, subCategory);

                    if (hintType.Equals("Verbose"))
                        hintLocation = $"in {vbonusLocation.Item1} {vbonusLocation.Item2}.";
                    else if (hintType.Equals("Vague") || hintType.Equals("World"))
                        hintLocation = $"in {vbonusLocation.Item1}.";

                    break;
                case DataTableEnum.Event:
                    var eventLocation = this.GetLocation(category, subCategory);
                    string preposition = subCategory.Contains("KEYITEM_003") ? "on" : "in";

                    if (hintType.Equals("Verbose"))
                        hintLocation = $"{preposition} {eventLocation.Item1} {eventLocation.Item2}.";
                    else if (hintType.Equals("Vague") || hintType.Equals("World"))
                        hintLocation = $"{preposition} {eventLocation.Item1}.";

                    break;
                case DataTableEnum.EquipItem:
                    var equipmentValue = this.ConvertSubCategoryToValue(subCategory);
                    var equipmentLocation = this.GetLocationByLookUp(randomizedOptions, equipmentValue);

                    if (hintType.Equals("Verbose"))
                    {
                        if (equipmentLocation != null && (equipmentLocation.Item2.Contains("Small Chest") || equipmentLocation.Item2.Contains("Large Chest")))
                        {
                            hintLocation = $"in {equipmentLocation.Item1} found on {this.GetEquipment(subCategory)} in {equipmentLocation.Item2.GetChestLocation(equipmentLocation.Item1.KeyToDataTableEnum())}.";
                        }
                        else
                        {
                            hintLocation = $"in {equipmentLocation} found on {this.GetEquipment(subCategory)}.";
                        }
                    }
                    else if (hintType.Equals("Vague"))
                        if (equipmentLocation != null)
                            hintLocation = $"in {equipmentLocation.Item1} on an Equipment.";
                        else
                            hintLocation = $"on {this.GetEquipment(subCategory)}.";
                    else if (hintType.Equals("World"))
                        if (equipmentLocation != null)
                            hintLocation = equipmentLocation.Item1.Equals("Sora's Initial Setup") || equipmentLocation.Item1.Equals("Lucky Emblem Milestones") ? $"on {equipmentLocation.Item1}." : $"in {equipmentLocation.Item1}.";
                        else
                            hintLocation = $"on {this.GetEquipment(subCategory)}.";

                    break;
                case DataTableEnum.WeaponEnhance:
                    var keyblade = this.GetKeybladeUpgrade(subCategory);
                    var keybladeId = this.ConvertWeaponUpgradeToKeybladeId(subCategory);
                    var weaponLocation = this.GetLocationByLookUp(randomizedOptions, keybladeId);

                    if (weaponLocation == null)
                        weaponLocation = new Tuple<string, string>("Sora's Initial Setup", "");

                    var weaponDescription = weaponLocation.Item2.Length > 0 ? $" in {weaponLocation.Item2.GetChestLocation(weaponLocation.Item1.KeyToDataTableEnum())}" : "";
                    if (hintType.Equals("Verbose"))
                        hintLocation = $"in {weaponLocation.Item1}{weaponDescription} on {keyblade.Item1} on {keyblade.Item2}.";
                    else if (hintType.Equals("Vague"))
                        hintLocation = $"in {weaponLocation.Item1} on {keyblade.Item1}.";
                    else if (hintType.Equals("World"))
                        hintLocation = weaponLocation.Item1.Equals("Sora's Initial Setup") || weaponLocation.Item1.Equals("Lucky Emblem Milestones") ? $"on {weaponLocation.Item1}." : $"in {weaponLocation.Item1}.";

                    break;
                case DataTableEnum.SynthesisItem:
                    hintLocation = "in the Moogle Synthesis Shop.";
                    var synthItemSplit = subCategory.Split('_');
                    if (synthItemSplit.Length > 0)
                    {
                        int synthItemNum;
                        bool synthParsed = int.TryParse(synthItemSplit[1], out synthItemNum);
                        if (synthParsed && synthItemNum >= 61 && synthItemNum <= 80)
                        {
                            if (hintType.Equals("Verbose"))
                            {
                                hintLocation = $"in the Moogle Synthesis Shop ({synthItemNum.GetPhotoMission()}).";
                            }

                            else if (hintType.Equals("Vague"))
                                hintLocation = $"in the Moogle Synthesis Shop (Photo Mission).";
                            else if (hintType.Equals("World"))
                                hintLocation = $"in the Moogle Synthesis Shop.";
                        }
                    } 
                    break;

                case DataTableEnum.TreasureBT:
                    if (hintType.Equals("Verbose"))
                        hintLocation = $"in Scala Ad Caelum in {subCategory.KeyIdToDisplay().GetChestLocation(DataTableEnum.TreasureBT)}.";
                    else if (hintType.Equals("Vague") || hintType.Equals("World"))
                        hintLocation = $"in Scala Ad Caelum.";

                    break;
                case DataTableEnum.TreasureBX:
                    if (hintType.Equals("Verbose"))
                        hintLocation = $"in San Fransokyo in {subCategory.KeyIdToDisplay().GetChestLocation(DataTableEnum.TreasureBX)}.";
                    else if (hintType.Equals("Vague") || hintType.Equals("World"))
                        hintLocation = $"in San Fransokyo.";

                    break;
                case DataTableEnum.TreasureCA:
                    if (hintType.Equals("Verbose"))
                        hintLocation = $"in The Caribbean in {subCategory.KeyIdToDisplay().GetChestLocation(DataTableEnum.TreasureCA)}.";
                    else if (hintType.Equals("Vague") || hintType.Equals("World"))
                        hintLocation = $"in The Caribbean.";

                    break;
                case DataTableEnum.TreasureEW:
                    if (hintType.Equals("Verbose"))
                        hintLocation = $"in The Final World in {subCategory.KeyIdToDisplay().GetChestLocation(DataTableEnum.TreasureEW)}.";
                    else if (hintType.Equals("Vague") || hintType.Equals("World"))
                        hintLocation = $"in The Final World.";

                    break;
                case DataTableEnum.TreasureFZ:
                    if (hintType.Equals("Verbose"))
                        hintLocation = $"in Arendelle in {subCategory.KeyIdToDisplay().GetChestLocation(DataTableEnum.TreasureFZ)}.";
                    else if (hintType.Equals("Vague") || hintType.Equals("World"))
                        hintLocation = $"in Arendelle.";

                    break;
                case DataTableEnum.TreasureHE:
                    if (hintType.Equals("Verbose"))
                        hintLocation = $"in Olympus in {subCategory.KeyIdToDisplay().GetChestLocation(DataTableEnum.TreasureHE)}.";
                    else if (hintType.Equals("Vague") || hintType.Equals("World"))
                        hintLocation = $"in Olympus.";

                    break;
                case DataTableEnum.TreasureKG:
                    if (hintType.Equals("Verbose"))
                        hintLocation = $"in The Keyblade Graveyard in {subCategory.KeyIdToDisplay().GetChestLocation(DataTableEnum.TreasureKG)}.";
                    else if (hintType.Equals("Vague") || hintType.Equals("World"))
                        hintLocation = $"in The Keyblade Graveyard.";

                    break;
                case DataTableEnum.TreasureMI:
                    if (hintType.Equals("Verbose"))
                        hintLocation = $"in Monstropolis in {subCategory.KeyIdToDisplay().GetChestLocation(DataTableEnum.TreasureMI)}.";
                    else if (hintType.Equals("Vague") || hintType.Equals("World"))
                        hintLocation = $"in Monstropolis.";

                    break;
                case DataTableEnum.TreasureRA:
                    if (hintType.Equals("Verbose"))
                        hintLocation = $"in Kingdom of Corona in {subCategory.KeyIdToDisplay().GetChestLocation(DataTableEnum.TreasureRA)}.";
                    else if (hintType.Equals("Vague") || hintType.Equals("World"))
                        hintLocation = $"in Kingdom of Corona.";

                    break;
                case DataTableEnum.TreasureTS:
                    if (hintType.Equals("Verbose"))
                        hintLocation = $"in Toy Box in {subCategory.KeyIdToDisplay().GetChestLocation(DataTableEnum.TreasureTS)}.";
                    else if (hintType.Equals("Vague") || hintType.Equals("World"))
                        hintLocation = $"in Toy Box.";

                    break;
                case DataTableEnum.TreasureTT:
                    if (hintType.Equals("Verbose"))
                        hintLocation = $"in Twilight Town in {subCategory.KeyIdToDisplay().GetChestLocation(DataTableEnum.TreasureTT)}.";
                    else if (hintType.Equals("Vague") || hintType.Equals("World"))
                        hintLocation = $"in Twilight Town.";

                    break;
                default:
                    break;
            }

            return hintLocation;
        }


        #region Helpers

        public string GetDisplayToId(string display)
        {
            switch (display)
            {
                case "Proof of Promises":
                    return "KEY_ITEM15\u0000";
                case "Proof of Times Past":
                    return "KEY_ITEM16\u0000";
                case "Proof of Fantasy":
                    return "KEY_ITEM14\u0000";

                case "Pole Spin":
                    return "ETresAbilityKind::POLE_SPIN\u0000";
                case "Block":
                    return "ETresAbilityKind::REFLECT_GUARD\u0000";
                case "Dodge Roll":
                    return "ETresAbilityKind::DODGE\u0000";
                case "Air Slide":
                    return "ETresAbilityKind::AIRSLIDE\u0000";
                case "Doubleflight":
                    return "ETresAbilityKind::DOUBLEFLIGHT\u0000";
                case "Second Chance":
                    return "ETresAbilityKind::LAST_LEAVE\u0000";
                case "Withstand Combo":
                    return "ETresAbilityKind::COMBO_LEAVE\u0000";

                case "Dream Heartbinder":
                    return "KEY_ITEM06\u0000";
                case "Pixel Heartbinder":
                    return "KEY_ITEM07\u0000";
                case "Pride Heartbinder":
                    return "KEY_ITEM09\u0000";
                case "Ocean Heartbinder":
                    return "KEY_ITEM10\u0000";
                case "\'Ohana Heartbinder":
                    return "KEY_ITEM08\u0000";

                case "Fire":
                case "Fira":
                case "Firaga":
                    return "ETresVictoryBonusKind::MELEM_FIRE\u0000";
                case "Water":
                case "Watera":
                case "Waterga":
                    return "ETresVictoryBonusKind::MELEM_WATER\u0000";
                case "Cure":
                case "Cura":
                case "Curaga":
                    return "ETresVictoryBonusKind::MELEM_CURE\u0000";
                case "Blizzard":
                case "Blizzara":
                case "Blizzaga":
                    return "ETresVictoryBonusKind::MELEM_BLIZZARD\u0000";
                case "Thunder":
                case "Thundara":
                case "Thundaga":
                    return "ETresVictoryBonusKind::MELEM_THUNDER\u0000";
                case "Aero":
                case "Aerora":
                case "Aeroga":
                    return "ETresVictoryBonusKind::MELEM_AERO\u0000";

                case "Hero\'s Origin":
                    return "WEP_KEYBLADE_SO_01\u0000";
                case "Grand Chef":
                    return "WEP_KEYBLADE_SO_011\u0000";
                case "Ultima Weapon":
                    return "WEP_KEYBLADE_SO_015\u0000";
                case "Oblivion":
                    return "WEP_KEYBLADE_SO_014\u0000";
                case "Oathkeeper":
                    return "WEP_KEYBLADE_SO_013\u0000";

                case "Secret Report 1":
                    return "REPORT_ITEM02\u0000";
                case "Secret Report 2":
                    return "REPORT_ITEM03\u0000";
                case "Secret Report 3":
                    return "REPORT_ITEM04\u0000";
                case "Secret Report 4":
                    return "REPORT_ITEM05\u0000";
                case "Secret Report 5":
                    return "REPORT_ITEM06\u0000";
                case "Secret Report 6":
                    return "REPORT_ITEM07\u0000";
                case "Secret Report 7":
                    return "REPORT_ITEM08\u0000";
                case "Secret Report 8":
                    return "REPORT_ITEM09\u0000";
                case "Secret Report 9":
                    return "REPORT_ITEM10\u0000";
                case "Secret Report 10":
                    return "REPORT_ITEM11\u0000";
                case "Secret Report 11":
                    return "REPORT_ITEM12\u0000";
                case "Secret Report 12":
                    return "REPORT_ITEM13\u0000";
                case "Secret Report 13":
                    return "REPORT_ITEM14\u0000";

                default:
                    return "UNKNOWN";
            }
        }

        public string GetDisplayToDefKeyblade(string display)
        {
            switch (display)
            {
                case "Hero\'s Origin":
                    return "ETresItemDefWeapon::WEP_KEYBLADE02\u0000";
                case "Grand Chef":
                    return "ETresItemDefWeapon::WEP_KEYBLADE11\u0000";
                case "Ultima Weapon":
                    return "ETresItemDefWeapon::WEP_KEYBLADE14\u0000";
                case "Oblivion":
                    return "ETresItemDefWeapon::WEP_KEYBLADE13\u0000";
                case "Oathkeeper":
                    return "ETresItemDefWeapon::WEP_KEYBLADE12\u0000";

                default:
                    return "UNKNOWN";
            }
        }

        public string CleanHintName(string hintName)
        {
            string newHintName = hintName;
            if (hintName.Contains("Magic: "))
            {
                newHintName = hintName.Split("Magic: ")[1] + " Element";
            }
            else if (hintName.Contains("Ability: "))
            {
                newHintName = hintName.Split("Ability: ")[1];
            }
            else if (hintName.Contains("Bonus: "))
            {
                newHintName = hintName.Split("Bonus: ")[1];
            }

            return newHintName;
        }

        public Tuple<string, string> GetLocation(DataTableEnum category, string subCategory)
        {
            var world = "";
            var description = "";

            switch (category)
            {
                case DataTableEnum.LevelUp:
                    world = "Sora's Level Up";
                    
                    break;
                case DataTableEnum.VBonus:
                    switch (subCategory)
                    {
                        #region Olympus
                        case "Vbonus_001":
                            world = "Olympus";
                            description = "(Cliff Ascent Heartless)";
                            break;
                        case "Vbonus_002":
                            world = "Olympus";
                            description = "(Thebes Agora Flame Cores)";
                            break;
                        case "Vbonus_005":
                            world = "Olympus";
                            description = "(Thebes Overlook Flame Cores)";
                            break;
                        case "Vbonus_006":
                            world = "Olympus";
                            description = "(Thebes Garden Flame Cores)";
                            break;
                        case "Vbonus_007":
                            world = "Olympus";
                            description = "(Thebes Alleyway Heartless)";
                            break;
                        case "Vbonus_008":
                            world = "Olympus";
                            description = "(Rock Troll Boss)";
                            break;
                        case "Vbonus_010":
                            world = "Olympus";
                            description = "(Rock Titan Boss)";
                            break;
                        case "Vbonus_011":
                            world = "Olympus";
                            description = "(Satyr Heartless)";
                            break;
                        case "Vbonus_013":
                            world = "Olympus";
                            description = "(Tornado Titan Boss)";
                            break;
                        #endregion Olympus

                        #region Twilight Town
                        case "Vbonus_014":
                            world = "Twilight Town";
                            description = "(Demon Tide Boss)";
                            break;
                        case "Vbonus_015":
                            world = "Twilight Town";
                            description = "(The Woods Powerwild Heartless)";
                            break;
                        case "Vbonus_016":
                            world = "Twilight Town";
                            description = "(The Old Mansion Nobodies & Heartless)";
                            break;
                        #endregion Twilight Town

                        #region Toy Box
                        case "Vbonus_017":
                            world = "Toy Box";
                            description = "(Andy's Room Heartless)";
                            break;
                        case "Vbonus_018":
                            world = "Toy Box";
                            description = "(Galaxy Toys Main Floor: 1F Gigas)";
                            break;
                        case "Vbonus_019":
                            world = "Toy Box";
                            description = "(Galaxy Toys Action Figures Supreme Smashers)";
                            break;
                        case "Vbonus_020":
                            world = "Toy Box";
                            description = "(Angelic Amber Boss)";
                            break;
                        case "Vbonus_021":
                            world = "Toy Box";
                            description = "(UFO Mini-Boss)";
                            break;
                        case "Vbonus_022":
                            world = "Toy Box";
                            description = "(Verum Rex: Beat of Lead)";
                            break;
                        case "Vbonus_023":
                            world = "Toy Box";
                            description = "(King of Toys Boss)";
                            break;
                        #endregion Toy Box

                        #region Kingdom of Corona
                        case "Vbonus_024":
                            world = "Kingdom of Corona";
                            description = "(Hills Heartless 1)";
                            break;
                        case "Vbonus_025":
                            world = "Kingdom of Corona";
                            description = "(Hills Heartless 2)";
                            break;
                        case "Vbonus_026":
                            world = "Kingdom of Corona";
                            description = "(Hills Reapers)";
                            break;
                        case "Vbonus_027":
                            world = "Kingdom of Corona";
                            description = "(Chaos Carriage Mini-Boss)";
                            break;
                        case "Vbonus_028":
                            world = "Kingdom of Corona";
                            description = "(The Kingdom Nobodies)";
                            break;
                        case "Vbonus_029":
                            world = "Kingdom of Corona";
                            description = "(Tower Heartless)";
                            break;
                        case "Vbonus_030":
                            world = "Kingdom of Corona";
                            description = "(Grim Guardianess Boss)";
                            break;
                        #endregion Kingdom of Corona

                        #region Monstropolis
                        case "Vbonus_032":
                            world = "Monstropolis";
                            description = "(Lobby & Offices Unversed)";
                            break;
                        case "Vbonus_033":
                            world = "Monstropolis";
                            description = "(Laugh Floor Unversed)";
                            break;
                        case "Vbonus_034":
                            world = "Monstropolis";
                            description = "(Upper Level Heartless)";
                            break;
                        case "Vbonus_035":
                            world = "Monstropolis";
                            description = "(Second Floor Unversed & Heartless)";
                            break;
                        case "Vbonus_036":
                            world = "Monstropolis";
                            description = "(Second Floor Unversed)";
                            break;
                        case "Vbonus_037":
                            world = "Monstropolis";
                            description = "(Accessway Unversed & Heartless)";
                            break;
                        case "Vbonus_038":
                            world = "Monstropolis";
                            description = "(Tank Yard Heartless)";
                            break;
                        case "Vbonus_039":
                            world = "Monstropolis";
                            description = "(Tank Yard Unversed)";
                            break;
                        case "Vbonus_040":
                            world = "Monstropolis";
                            description = "(Lump of Horror Boss)";
                            break;
                        #endregion Monstropolis

                        #region Arendelle
                        case "Vbonus_041":
                            world = "Arendelle";
                            description = "(Rock Troll & Winterhorns Mini-Boss)";
                            break;
                        case "Vbonus_042":
                            world = "Arendelle";
                            description = "(1st Ninja Nobodies)";
                            break;
                        case "Vbonus_043":
                            world = "Arendelle";
                            description = "(2nd Ninja Nobodies)";
                            break;
                        case "Vbonus_044":
                            world = "Arendelle";
                            description = "(3rd Ninja Nobodies)";
                            break;
                        case "Vbonus_045":
                            world = "Arendelle";
                            description = "(4th Ninja Nobodies)";
                            break;
                        case "Vbonus_047":
                            world = "Arendelle";
                            description = "(Marshmallow Boss)";
                            break;
                        case "Vbonus_048":
                            world = "Arendelle";
                            description = "(Frost Serpent Heartless)";
                            break;
                        case "Vbonus_049":
                            world = "Arendelle";
                            description = "(Valley of Ice Heartless)";
                            break;
                        case "Vbonus_050":
                            world = "Arendelle";
                            description = "(Skoll Boss)";
                            break;
                        #endregion Arendelle

                        #region San Fransokyo
                        case "Vbonus_051":
                            world = "San Fransokyo";
                            description = "(Metal Troll Mini-Boss)";
                            break;
                        case "Vbonus_052":
                            world = "San Fransokyo";
                            description = "(Meeting Hiro in the Garage)";
                            break;
                        case "Vbonus_053":
                            world = "San Fransokyo";
                            description = "(Satyr Heartless Roof)";
                            break;
                        case "Vbonus_054":
                            world = "San Fransokyo";
                            description = "(Catastrochorus Boss)";
                            break;
                        case "Vbonus_055":
                            world = "San Fransokyo";
                            description = "(Big Hero 6 Rescue Mission)";
                            break;
                        case "Vbonus_056":
                            world = "San Fransokyo";
                            description = "(Darkube Boss)";
                            break;
                        case "Vbonus_057":
                            world = "San Fransokyo";
                            description = "(Dark Baymax Boss)";
                            break;
                        #endregion San Fransokyo

                        #region The Caribbean
                        case "Vbonus_058":
                            world = "The Caribbean";
                            description = "(Black Pearl Davy Jones Locker)";
                            break;
                        case "Vbonus_059":
                            world = "The Caribbean";
                            description = "(1st Ship Battle)";
                            break;
                        case "Vbonus_060":
                            world = "The Caribbean";
                            description = "(Raging Vulture Boss)";
                            break;
                        case "Vbonus_061":
                            world = "The Caribbean";
                            description = "(Lightning Angler Boss)";
                            break;
                        case "Vbonus_062":
                            world = "The Caribbean";
                            description = "(Luxord Ship Race)";
                            break;
                        case "Vbonus_063":
                            world = "The Caribbean";
                            description = "(2nd Ship Battle)";
                            break;
                        case "Vbonus_064":
                            world = "The Caribbean";
                            description = "(3rd Ship Battle)";
                            break;
                        case "Vbonus_065":
                            world = "The Caribbean";
                            description = "(Kraken Boss)";
                            break;
                        case "Vbonus_066":
                            world = "The Caribbean";
                            description = "(Davy Jones Boss)";
                            break;
                        #endregion The Caribbean

                        case "Vbonus_067":
                            world = "The Dark World";
                            description = "(Anti-Aqua Boss)";
                            break;

                        #region The Keyblade Graveyard
                        case "Vbonus_068":
                            world = "The Keyblade Graveyard";
                            description = "(Lich Boss)";
                            break;
                        case "Vbonus_069":
                            world = "The Keyblade Graveyard";
                            description = "(10,000 Enemy Fight)";
                            break;
                        case "Vbonus_070":
                            world = "The Keyblade Graveyard";
                            description = "(Demon Tide Boss)";
                            break;
                        case "Vbonus_071":
                            world = "The Keyblade Graveyard";
                            description = "(Xigbar & Dark Riku Boss)";
                            break;
                        case "Vbonus_072":
                            world = "The Keyblade Graveyard";
                            description = "(Luxord, Larxene & Marluxia Boss)";
                            break;
                        case "Vbonus_073":
                            world = "The Keyblade Graveyard";
                            description = "(Terra-Xehanort & Vanitas Boss)";
                            break;
                        case "Vbonus_074":
                            world = "The Keyblade Graveyard";
                            description = "(Saix Boss)";
                            break;
                        case "Vbonus_075":
                            world = "The Keyblade Graveyard";
                            description = "(Young Xehanort, Ansem & Xemnas Boss)";
                            break;
                        case "Vbonus_076":
                            world = "The Keyblade Graveyard";
                            description = "(10,000 Enemy Fight)";
                            break;
                        #endregion The Keyblade Graveyard

                        #region The Final World
                        case "Vbonus_082":
                            world = "The Final World";
                            description = "(Darkside Tutorial Boss)";
                            break;
                        case "Vbonus_083":
                            world = "The Final World";
                            description = "(Collect 222 Soras)";
                            break;
                        case "Vbonus_084":
                            world = "The Final World";
                            description = "(Collect 333 Soras)";
                            break;
                        #endregion The Final World

                        #region Mini-Games
                        case "VBonus_Minigame001":
                            world = "Toy Box";
                            description = "(A-rank Verum Rex: Beat of Lead)";
                            break;
                        case "VBonus_Minigame002":
                            world = "Kingdom of Corona";
                            description = "(A-rank Festival Dance)";
                            break;
                        case "VBonus_Minigame003":
                            world = "Arendelle";
                            description = "(A-rank Frozen Slider)";
                            break;
                        case "VBonus_Minigame004":
                            world = "Arendelle";
                            description = "(All Treasures Frozen Slider)";
                            break;
                        case "VBonus_Minigame005":
                            world = "San Fransokyo";
                            description = "(A-rank Flash Tracer 1 (Fred))";
                            break;
                        case "VBonus_Minigame006":
                            world = "San Fransokyo";
                            description = "(A-rank Flash Tracer 2 (Go Go))";
                            break;
                        #endregion Mini-Games

                        #region Flans
                        case "VBonus_Minigame007":
                            world = "Olympus";
                            description = "(A-rank Cherry Flan)";
                            break;
                        case "VBonus_Minigame008":
                            world = "Toy Box";
                            description = "(A-rank Strawberry Flan)";
                            break;
                        case "VBonus_Minigame009":
                            world = "Kingdom of Corona";
                            description = "(A-rank Orange Flan)";
                            break;
                        case "VBonus_Minigame010":
                            world = "Monstropolis";
                            description = "(A-rank Banana Flan)";
                            break;
                        case "VBonus_Minigame011":
                            world = "Arendelle";
                            description = "(A-rank Grape Flan)";
                            break;
                        case "VBonus_Minigame012":
                            world = "The Caribbean";
                            description = "(A-rank Watermelon Flan)";
                            break;
                        case "VBonus_Minigame013":
                            world = "San Fransokyo";
                            description = "(A-rank Melon Flan)";
                            break;
                        #endregion Flans

                        #region Re:Mind
                        case "VBonus_DLC_001":
                            world = "Re:Mind (The Keyblade Graveyard)";
                            description = "(Dark Inferno Boss)";
                            break;
                        case "VBonus_DLC_002":
                            world = "Re:Mind (The Keyblade Graveyard)";
                            description = "(Anti-Aqua Boss)";
                            break;
                        case "VBonus_DLC_003":
                            world = "Re:Mind (The Keyblade Graveyard)";
                            description = "(Terra-Xehanort Boss)";
                            break;
                        case "VBonus_DLC_004":
                            world = "Re:Mind (The Keyblade Graveyard)";
                            description = "(Xigbar & Dark Riku Boss)";
                            break;
                        case "VBonus_DLC_005":
                            world = "Re:Mind (The Keyblade Graveyard)";
                            description = "(Luxord, Larxene & Marluxia Boss)";
                            break;
                        case "VBonus_DLC_006":
                            world = "Re:Mind (The Keyblade Graveyard)";
                            description = "(Terra-Xehanort & Vanitas Boss)";
                            break;
                        case "VBonus_DLC_007":
                            world = "Re:Mind (The Keyblade Graveyard)";
                            description = "(Saix Boss)";
                            break;
                        case "VBonus_DLC_008":
                            world = "Re:Mind (The Keyblade Graveyard)";
                            description = "(Young Xehanort, Ansem & Xemnas Boss)";
                            break;

                        case "VBonus_DLC_010":
                            world = "Scala Ad Caelum";
                            description = "(Darkside Boss)";
                            break;

                        case "VBonus_DLC_015":
                            world = "Scala Ad Caelum";
                            description = "(Armored Xehanort Boss)";
                            break;
                        #endregion Re:Mind

                        default:
                            world = "UNKNOWN";
                            break;
                    }

                    break;

                case DataTableEnum.Event:
                    switch (subCategory)
                    {
                        #region Events
                        case "EVENT_001":
                            world = "Olympus";
                            description = "(Forge Goofy's Shield)";
                            break;

                        case "EVENT_002":
                        case "EVENT_003":
                        case "EVENT_004":
                        case "EVENT_005":
                        case "EVENT_006":
                            world = "Olympus";
                            description = "(Golden Herc Statue)";
                            break;

                        case "EVENT_007":
                            world = "Olympus";
                            description = "(Return Herc Statues)";
                            break;

                        case "EVENT_008":
                            world = "Monstropolis";
                            description = "(Defeat the Unversed before the Power Plant)";
                            break;

                        case "EVENT_009":
                            world = "San Fransokyo";
                            description = "(Rescue the Trapped Big Hero 6 Members)";
                            break;
                        #endregion Events

                        #region Keyblades
                        case "TresUIMobilePortalDataAsset":
                            world = "Classic Kingdom";
                            description = "(Complete All Classic Kingdom Games)";
                            break;

                        case "EVENT_KEYBLADE_001":
                            world = "Olympus";
                            description = "(Complete Olympus)";
                            break;
                        case "EVENT_KEYBLADE_002":
                        case "EVENT_CKGAME_001":
                            world = "Twilight Town";
                            description = "(Complete Twilight Town)";
                            break;
                        case "EVENT_KEYBLADE_003":
                            world = "Toy Box";
                            description = "(Complete Toy Box)";
                            break;
                        case "EVENT_KEYBLADE_004":
                            world = "Kingdom of Corona";
                            description = "(Complete Kingdom of Corona)";
                            break;
                        case "EVENT_KEYBLADE_005":
                            world = "Monstropolis";
                            description = "(Complete Monstropolis)";
                            break;
                        case "EVENT_KEYBLADE_006":
                            world = "100 Acre Wood";
                            description = "(Complete 100 Acre Wood)";
                            break;
                        case "EVENT_KEYBLADE_007":
                            world = "Arendelle";
                            description = "(Complete Arendelle)";
                            break;
                        case "EVENT_KEYBLADE_008":
                            world = "The Caribbean";
                            description = "(Complete The Caribbean)";
                            break;
                        case "EVENT_KEYBLADE_009":
                            world = "San Fransokyo";
                            description = "(Complete San Fransokyo)";
                            break;
                        case "EVENT_KEYBLADE_010":
                            world = "Twilight Town";
                            description = "(Complete All Recipes)";
                            break;
                        case "EVENT_KEYBLADE_011":
                            world = "The Keyblade Graveyard";
                            description = "(Defeat Demon Tide)";
                            break;
                        #endregion Keyblades

                        #region Heartbinders
                        case "EVENT_HEARTBINDER_001":
                            world = "Twilight Town";
                            description = "(After Yen Sid's Tower)";
                            break;
                        case "EVENT_HEARTBINDER_002":
                            world = "Toy Box";
                            description = "(After Verum Rex: Beat of Lead)";
                            break;
                        case "EVENT_HEARTBINDER_003":
                            world = "Monstropolis";
                            description = "(After Tank Yard Heartless)";
                            break;
                        case "EVENT_HEARTBINDER_004":
                            world = "San Fransokyo";
                            description = "(After Flash Tracer)";
                            break;
                        #endregion Heartbinders

                        #region Battle Gates
                        case "EVENT_REPORT_001a":
                        case "EVENT_REPORT_001b":
                        case "EVENT_REPORT_002a":
                        case "EVENT_REPORT_002b":
                            world = "Olympus";
                            description = "(Complete Battle Portal)";
                            break;
                        case "EVENT_REPORT_003a":
                        case "EVENT_REPORT_003b":
                            world = "Twilight Town";
                            description = "(Complete Battle Portal)";
                            break;
                        case "EVENT_REPORT_004a":
                        case "EVENT_REPORT_004b":
                        case "EVENT_REPORT_005a":
                        case "EVENT_REPORT_005b":
                            world = "Toy Box";
                            description = "(Complete Battle Portal)";
                            break;
                        case "EVENT_REPORT_006a":
                        case "EVENT_REPORT_006b":
                        case "EVENT_REPORT_007a":
                        case "EVENT_REPORT_007b":
                            world = "Kingdom of Corona";
                            description = "(Complete Battle Portal)";
                            break;
                        case "EVENT_REPORT_008a":
                        case "EVENT_REPORT_008b":
                            world = "Monstropolis";
                            description = "(Complete Battle Portal)";
                            break;
                        case "EVENT_REPORT_009a":
                        case "EVENT_REPORT_009b":
                            world = "Arendelle";
                            description = "(Complete Battle Portal)";
                            break;
                        case "EVENT_REPORT_010a":
                        case "EVENT_REPORT_010b":
                            world = "The Caribbean";
                            description = "(Complete Battle Portal)";
                            break;
                        case "EVENT_REPORT_011a":
                        case "EVENT_REPORT_011b":
                        case "EVENT_REPORT_012a":
                        case "EVENT_REPORT_012b":
                            world = "San Fransokyo";
                            description = "(Complete Battle Portal)";
                            break;
                        case "EVENT_REPORT_013a":
                        case "EVENT_REPORT_013b":
                            world = "The Keyblade Graveyard";
                            description = "(Complete Battle Portal)";
                            break;
                        case "EVENT_REPORT_014":
                            world = "The Keyblade Graveyard";
                            description = "(Defeat Dark Inferno)";
                            break;
                        #endregion Battle Gates

                        #region Key Items
                        case "EVENT_KEYITEM_001":
                            world = "Twilight Town";
                            description = "(After Yen Sid's Tower)";
                            break;
                        case "EVENT_KEYITEM_002":
                            world = "San Fransokyo";
                            description = "(Hiro's Garage First Visit)";
                            break;
                        case "EVENT_KEYITEM_003":
                            world = "Lucky Emblem Milestones";
                            description = "(Find All Lucky Emblems)";
                            break;
                        case "EVENT_KEYITEM_005":
                        case "EVENT_YOZORA_001":
                            world = "Unreality";
                            description = "(Defeat Yozora)";
                            break;
                        #endregion Key Items

                        #region Data-Battles
                        case "EVENT_DATAB_001":
                            world = "Radiant Garden";
                            description = "(Defeat Data Master Xehanort)";
                            break;
                        case "EVENT_DATAB_002":
                            world = "Radiant Garden";
                            description = "(Defeat Data Ansem)";
                            break;
                        case "EVENT_DATAB_003":
                            world = "Radiant Garden";
                            description = "(Defeat Data Xemnas)";
                            break;
                        case "EVENT_DATAB_004":
                            world = "Radiant Garden";
                            description = "(Defeat Data Xigbar)";
                            break;
                        case "EVENT_DATAB_005":
                            world = "Radiant Garden";
                            description = "(Defeat Data Luxord)";
                            break;
                        case "EVENT_DATAB_006":
                            world = "Radiant Garden";
                            description = "(Defeat Data Larxene)";
                            break;
                        case "EVENT_DATAB_007":
                            world = "Radiant Garden";
                            description = "(Defeat Data Marluxia)";
                            break;
                        case "EVENT_DATAB_008":
                            world = "Radiant Garden";
                            description = "(Defeat Data Saix)";
                            break;
                        case "EVENT_DATAB_009":
                            world = "Radiant Garden";
                            description = "(Defeat Data Terra-Xehanort)";
                            break;
                        case "EVENT_DATAB_010":
                            world = "Radiant Garden";
                            description = "(Defeat Data Dark Riku)";
                            break;
                        case "EVENT_DATAB_011":
                            world = "Radiant Garden";
                            description = "(Defeat Data Vanitas)";
                            break;
                        case "EVENT_DATAB_012":
                            world = "Radiant Garden";
                            description = "(Defeat Data Young Xehanort)";
                            break;
                        case "EVENT_DATAB_013":
                            world = "Radiant Garden";
                            description = "(Defeat Data Xion)";
                            break;
                        #endregion Data-Battles

                        default:
                            world = "UNKNOWN";
                            break;
                    }
                    
                    break;

                case DataTableEnum.TreasureBT:
                    world = "Scala Ad Caelum";
                    description = this.GetTreasure(subCategory);
                    break;
                case DataTableEnum.TreasureBX:
                    world = "San Fransokyo";
                    description = this.GetTreasure(subCategory);
                    break;
                case DataTableEnum.TreasureCA:
                    world = "The Caribbean";
                    description = this.GetTreasure(subCategory);
                    break;
                case DataTableEnum.TreasureEW:
                    world = "The Final World";
                    description = this.GetTreasure(subCategory);
                    break;
                case DataTableEnum.TreasureFZ:
                    world = "Arendelle";
                    description = this.GetTreasure(subCategory);
                    break;
                case DataTableEnum.TreasureHE:
                    world = "Olympus";
                    description = this.GetTreasure(subCategory);
                    break;
                case DataTableEnum.TreasureKG:
                    world = "The Keyblade Graveyard";
                    description = this.GetTreasure(subCategory);
                    break;
                case DataTableEnum.TreasureMI:
                    world = "Monstropolis";
                    description = this.GetTreasure(subCategory);
                    break;
                case DataTableEnum.TreasureRA:
                    world = "Kingdom of Corona";
                    description = this.GetTreasure(subCategory);
                    break;
                case DataTableEnum.TreasureTS:
                    world = "Toy Box";
                    description = this.GetTreasure(subCategory);
                    break;
                case DataTableEnum.TreasureTT:
                    world = "Twilight Town";
                    description = this.GetTreasure(subCategory);
                    break;
                case DataTableEnum.SynthesisItem:
                    world = "the Moogle Synthesis Shop";
                    break;
                case DataTableEnum.LuckyMark:
                    world = "Lucky Emblem Milestones";
                    break;
            }

            return new Tuple<string, string>(world, description);
        }

        public string GetFullcourseDescription(string subCategory)
        {
            switch (subCategory)
            {
                case "item00":
                    return "(Base Pool)";
                case "item01":
                    return "(1 Star Pool)";
                case "item02":
                case "item03":
                case "item04":
                case "item05":
                    return "(2 Star Pool)";
                case "item06":
                case "item07":
                case "item08":
                case "item09":
                case "item10":
                    return "(3 Star Pool)";
                case "item11":
                case "item12":
                    return "(4 Star Pool)";
                case "item13":
                case "item14":
                    return "(5 Star Pool)";

                default:
                    return "UNKNOWN";
            }
        }

        public string GetTreasure(string subCategory)
        {
            var treasureSplit = subCategory.Split('_');

            if (treasureSplit.Length == 3 && (treasureSplit[1] == "SBOX" || treasureSplit[1] == "LBOX"))
                return treasureSplit[1] == "LBOX" ? $"Large Chest {int.Parse(treasureSplit[2])}" : $"Small Chest {int.Parse(treasureSplit[2])}";

            return "UNKNOWN";
        }

        public string GetEquipment(string subCategory)
        {
            switch (subCategory)
            {
                #region Keyblades
                case "I03001":
                    return "Kingdom Key";
                case "I03002":
                    return "Hero's Origin";
                case "I03003":
                    return "Shooting Star";
                case "I03004":
                    return "Favorite Deputy";
                case "I03005":
                    return "Ever After";
                case "I03006":
                    return "Happy Gear";
                case "I03007":
                    return "Crystal Snow";
                case "I03008":
                    return "Hunny Spout";
                case "I03009":
                    return "Nano Gear";
                case "I03010":
                    return "Wheel of Fate";
                case "I03011":
                    return "Grand Chef";
                case "I03012":
                    return "Classic Tone";
                case "I03013":
                    return "Oathkeeper";
                case "I03014":
                    return "Oblivion";
                case "I03015":
                    return "Ultima Weapon";
                case "I03018":
                    return "Starlight";
                case "I03019":
                    return "Elemental Encoder";
                #endregion Keyblades

                #region Armor
                case "I04001":
                    return "Hero's Belt";
                case "I04002":
                    return "Hero's Glove";
                case "I04045":
                    return "Firefighter Rosette";
                case "I04046":
                    return "Umbrella Rosette";
                case "I04047":
                    return "Mask Rosette";
                case "I04048":
                    return "Snowman Rosette";
                case "I04049":
                    return "Insulator Rosette";
                #endregion Armor

                #region Accessories
                case "I05001":
                    return "Laughter Pin";
                case "I05002":
                    return "Forest Clasp";
                case "I05013":
                    return "Buster Ring";
                case "I05014":
                    return "Brave Ring";
                case "I05015":
                    return "Phantom Ring";
                case "I05016":
                    return "Orichalcum Ring";

                case "I05017":
                    return "Magic Ring";
                case "I05018":
                    return "Rune Ring";
                case "I05019":
                    return "Force Ring";
                case "I05020":
                    return "Sorcerer's Ring";
                case "I05021":
                    return "Wisdom Ring";

                case "I05022":
                    return "Bronze Necklace";
                case "I05023":
                    return "Silver Necklace";
                case "I05024":
                    return "Master's Necklace";

                case "I05025":
                    return "Bronze Amulet";
                case "I05026":
                    return "Silver Amulet";
                case "I05027":
                    return "Gold Amulet";

                case "I05028":
                    return "Junior Medal";
                case "I05029":
                    return "Star Medal";
                case "I05030":
                    return "Master Medal";

                case "I05031":
                    return "Mickey's Brooch";

                case "I05032":
                    return "Soldier's Earring";
                case "I05033":
                    return "Fencer's Earring";
                case "I05034":
                    return "Mage's Earring";
                case "I05035":
                    return "Slayer's Earring";

                case "I05036":
                    return "Moon Amulet";
                case "I05037":
                    return "Star Charm";
                case "I05038":
                    return "Cosmic Arts";

                case "I05039":
                    return "Crystal Regalia";

                case "I05040":
                    return "Water Cufflink";
                case "I05041":
                    return "Thunder Cufflink";
                case "I05042":
                    return "Fire Cufflink";
                case "I05043":
                    return "Aero Cufflink";
                case "I05044":
                    return "Blizzard Cufflink";
                case "I05045":
                    return "Celestriad";
                case "I05046":
                    return "Yin-Yang Cufflink";

                case "I05047":
                    return "Gourmand's Ring";
                case "I05048":
                    return "Draw Ring";
                case "I05049":
                    return "Lucky Ring";

                case "I05050":
                    return "Flanniversary Badge";

                case "I05051":
                case "I05052":
                case "I05053":
                case "I05054":
                case "I05055":
                case "I05056":
                case "I05057":
                case "I05058":
                case "I05059":
                case "I05060":
                case "I05061":
                case "I05062":
                    return "Bronze Necklace";

                case "I05063":
                case "I05064":
                case "I05065":
                case "I05066":
                case "I05067":
                case "I05068":
                case "I05069":
                case "I05070":
                case "I05071":
                case "I05072":
                case "I05073":
                    return "Silver Necklace";

                case "I05074":
                case "I05075":
                case "I05076":
                case "I05077":
                case "I05078":
                case "I05079":
                case "I05080":
                    return "Master's Necklace";

                case "I05081":
                case "I05082":
                    return "Star Medal";

                case "I05083":
                    return "Junior Medal";

                case "I05084":
                    return "Star Medal";

                case "I05085":
                case "I05086":
                case "I05087":
                case "I05088":
                case "I05089":
                case "I05090":
                case "I05091":
                case "I05092":
                    return "Junior Medal";

                case "I05093":
                    return "Master Medal";

                case "I05094":
                case "I05095":
                    return "Star Medal";

                case "I05096":
                    return "Master Medal";

                case "I05097":
                    return "Star Medal";

                case "I05098":
                    return "Master Medal";

                case "I05099":
                case "I05100":
                case "I05101":
                case "I05102":
                case "I05103":
                    return "Star Medal";

                case "I05104":
                case "I05105":
                case "I05106":
                case "I05107":
                case "I05108":
                case "I05109":
                case "I05110":
                    return "Master Medal";

                case "I05111":
                    return "Breakthrough";
                case "I05112":
                    return "Crystal Regalia+";
                #endregion Accessories

                default:
                    return "UNKNOWN";
            }
        }

        public string ConvertWeaponUpgradeToKeybladeId(string subCategory)
        {
            var keyblade = string.Empty;

            var subCategorySplit = subCategory.Split('_'); 
            var keybladeSwitch = (int.Parse(subCategorySplit[1]) / 10);

            switch (keybladeSwitch)
            {
                case 0:
                    keyblade = "WEP_KEYBLADE_SO_00\u0000";
                    break;
                case 1:
                    keyblade = "WEP_KEYBLADE_SO_02\u0000";
                    break;
                case 2:
                    keyblade = "WEP_KEYBLADE_SO_01\u0000";
                    break;
                case 3:
                    keyblade = "WEP_KEYBLADE_SO_03\u0000";
                    break;
                case 4:
                    keyblade = "WEP_KEYBLADE_SO_04\u0000";
                    break;
                case 5:
                    keyblade = "WEP_KEYBLADE_SO_09\u0000";
                    break;
                case 6:
                    keyblade = "WEP_KEYBLADE_SO_06\u0000";
                    break;
                case 7:
                    keyblade = "WEP_KEYBLADE_SO_07\u0000";
                    break;
                case 8:
                    keyblade = "WEP_KEYBLADE_SO_08\u0000";
                    break;
                case 9:
                    keyblade = "WEP_KEYBLADE_SO_05\u0000";
                    break;
                case 10:
                    keyblade = "WEP_KEYBLADE_SO_011\u0000";
                    break;
                case 11:
                    keyblade = "WEP_KEYBLADE_SO_012\u0000";
                    break;
                case 12:
                    keyblade = "WEP_KEYBLADE_SO_015\u0000";
                    break;
                case 15:
                    keyblade = "WEP_KEYBLADE_SO_019\u0000";
                    break;
                case 16:
                    keyblade = "WEP_KEYBLADE_SO_018\u0000";
                    break;
                case 17:
                    keyblade = "WEP_KEYBLADE_SO_013\u0000";
                    break;
                case 18:
                    keyblade = "WEP_KEYBLADE_SO_014\u0000";
                    break;
                default:
                    keyblade = "UNKNOWN";
                    break;
            }

            return keyblade;
        }

        public string ConvertSubCategoryToValue(string subCategory)
        {
            if (subCategory.Contains("I04"))
                return $"PRT_ITEM{subCategory[^2..]}\u0000";
            else if (subCategory.Contains("I05"))
                return $"ACC_ITEM{subCategory[^2..]}\u0000";

            switch (subCategory)
            {
                #region Keyblades
                case "I03001":
                    return "WEP_KEYBLADE_SO_00\u0000";
                case "I03002":
                    return "WEP_KEYBLADE_SO_01\u0000";
                case "I03003":
                    return "WEP_KEYBLADE_SO_02\u0000";
                case "I03004":
                    return "WEP_KEYBLADE_SO_03\u0000";
                case "I03005":
                    return "WEP_KEYBLADE_SO_04\u0000";
                case "I03006":
                    return "WEP_KEYBLADE_SO_05\u0000";
                case "I03007":
                    return "WEP_KEYBLADE_SO_06\u0000";
                case "I03008":
                    return "WEP_KEYBLADE_SO_07\u0000";
                case "I03009":
                    return "WEP_KEYBLADE_SO_08\u0000";
                case "I03010":
                    return "WEP_KEYBLADE_SO_09\u0000";
                case "I03011":
                    return "WEP_KEYBLADE_SO_011\u0000";
                case "I03012":
                    return "WEP_KEYBLADE_SO_012\u0000";
                case "I03013":
                    return "WEP_KEYBLADE_SO_013\u0000";
                case "I03014":
                    return "WEP_KEYBLADE_SO_014\u0000";
                case "I03015":
                    return "WEP_KEYBLADE_SO_015\u0000";
                case "I03018":
                    return "WEP_KEYBLADE_SO_018\u0000";
                case "I03019":
                    return "WEP_KEYBLADE_SO_019\u0000";
                #endregion Keyblades


                default:
                    return "UNKNOWN";
            }
        }

        public Tuple<string, string> GetKeybladeUpgrade(string subCategory)
        {
            var subCategorySplit = subCategory.Split('_');

            var keyblade = "";

            var level = $"Level {(int.Parse(subCategorySplit[1]) % 10) + 1}"; ;
            var keybladeSwitch = (int.Parse(subCategorySplit[1]) / 10);

            switch (keybladeSwitch)
            {
                case 0:
                    keyblade = "Kingdom Key";
                    break;
                case 1:
                    keyblade = "Shooting Star";
                    break;
                case 2:
                    keyblade = "Hero's Origin";
                    break;
                case 3:
                    keyblade = "Favorite Deputy";
                    break;
                case 4:
                    keyblade = "Ever After";
                    break;
                case 5:
                    keyblade = "Wheel of Fate";
                    break;
                case 6:
                    keyblade = "Crystal Snow";
                    break;
                case 7:
                    keyblade = "Hunny Spout";
                    break;
                case 8:
                    keyblade = "Nano Gear";
                    break;
                case 9:
                    keyblade = "Happy Gear";
                    break;
                case 10:
                    keyblade = "Grand Chef";
                    break;
                case 11:
                    keyblade = "Classic Tone";
                    break;
                case 12:
                    keyblade = "Ultima Weapon";
                    break;
                case 15:
                    keyblade = "Elemental Encoder";
                    break;
                case 16:
                    keyblade = "Starlight";
                    break;
                case 17:
                    keyblade = "Oathkeeper";
                    break;
                case 18:
                    keyblade = "Oblivion";
                    break;
                default:
                    keyblade = "UNKNOWN";
                    break;
            }

            return new Tuple<string, string>(keyblade, level);
        }

        public string GetLevelUpAlternatives(Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, string subCategory, string name, string value)
        {
            var levelUps = randomizedOptions[DataTableEnum.LevelUp];

            var levelUpTexts = new List<string>();

            foreach (var levelUp in levelUps)
            {
                if (levelUp.Value["TypeA"].Equals(value))
                    levelUpTexts.Add($"{levelUp.Key} (Warrior)");

                if (levelUp.Value["TypeB"].Equals(value))
                    levelUpTexts.Add($"{levelUp.Key} (Mystic)");
                
                if (levelUp.Value["TypeC"].Equals(value))
                    levelUpTexts.Add($"{levelUp.Key} (Guardian)");

                if (levelUpTexts.Count == 3)
                    break;
            }

            if (levelUpTexts.Count == 3)
                return $"[{levelUpTexts[0]} - {levelUpTexts[1]} - {levelUpTexts[2]}]";

            return "UNKNOWN";
        }

        public Tuple<string, string> GetLocationByLookUp(Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, string value)
        {
            Tuple<string, string> location = null;

            foreach (var searchCategory in randomizedOptions)
            {
                foreach (var searchSubCategory in searchCategory.Value)
                {
                    foreach (var (searchName, searchValue) in searchSubCategory.Value)
                    {
                        if (searchValue.Equals(value))
                        {
                            location = this.GetLocation(searchCategory.Key, searchSubCategory.Key);

                            break;
                        }
                    }

                    if (location != null)
                        break;
                }

                if (location != null)
                    break;
            }

            return location;
        }

        #endregion Helpers
    }
}