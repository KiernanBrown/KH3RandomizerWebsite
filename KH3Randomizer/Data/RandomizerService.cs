using System;
using System.Collections.Generic;
using UE4DataTableInterpreter.Enums;
using KH3Randomizer.Models;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.IO.Compression;

namespace KH3Randomizer.Data
{
    public class RandomizerService
    {
        private Random rng;
        private List<string> blockedChecks = new List<string>();
        private List<string> keyAbilities = new List<string>()
        {
            "Ability: Dodge Roll", "Ability: Air Slide", "Ability: Block", "Ability: Pole Spin", "Ability: Glide",
            "Ability: Second Chance", "Ability: Withstand Combo"
        };

        public Dictionary<string, Dictionary<string, bool>> GetAvailableOptions(Dictionary<string, bool> availablePools, ref Dictionary<string, Dictionary<string, bool>> availableOptions, 
                                                                                ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, bool backTo = false)
        {
            if (backTo)
                return availableOptions;
            
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot\DefaultKH3.json"));
            var defaultOptions = JsonSerializer.Deserialize<Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>>>(streamReader.ReadToEnd());

            availableOptions = new();
            randomizedOptions = new();

            foreach (var pool in availablePools)
            {
                if (!pool.Value)
                    continue;

                switch (pool.Key)
                {
                    case "Treasures":
                        availableOptions.Add("Treasures", new Dictionary<string, bool>
                        {
                            { "Olympus", true }, { "Twilight Town", true }, { "Kingdom of Corona", true },
                            { "Toy Box", true }, { "Arendelle", true }, { "Monstropolis", true },
                            { "The Caribbean", true }, { "San Fransokyo", true }, { "Keyblade Graveyard", true },
                            { "The Final World", true }, { "Scala Ad Caelum", true }
                        });
                        
                        randomizedOptions.Add(DataTableEnum.TreasureBT, defaultOptions[DataTableEnum.TreasureBT]);
                        randomizedOptions.Add(DataTableEnum.TreasureBX, defaultOptions[DataTableEnum.TreasureBX]);
                        randomizedOptions.Add(DataTableEnum.TreasureCA, defaultOptions[DataTableEnum.TreasureCA]);
                        randomizedOptions.Add(DataTableEnum.TreasureEW, defaultOptions[DataTableEnum.TreasureEW]);
                        randomizedOptions.Add(DataTableEnum.TreasureFZ, defaultOptions[DataTableEnum.TreasureFZ]);
                        randomizedOptions.Add(DataTableEnum.TreasureHE, defaultOptions[DataTableEnum.TreasureHE]);
                        randomizedOptions.Add(DataTableEnum.TreasureKG, defaultOptions[DataTableEnum.TreasureKG]);
                        randomizedOptions.Add(DataTableEnum.TreasureMI, defaultOptions[DataTableEnum.TreasureMI]);
                        randomizedOptions.Add(DataTableEnum.TreasureRA, defaultOptions[DataTableEnum.TreasureRA]);
                        randomizedOptions.Add(DataTableEnum.TreasureTS, defaultOptions[DataTableEnum.TreasureTS]);
                        randomizedOptions.Add(DataTableEnum.TreasureTT, defaultOptions[DataTableEnum.TreasureTT]);

                        break;
                    case "Equippables":
                        availableOptions.Add("Equippables", new Dictionary<string, bool>
                        {
                            { "Weapons", true }, { "Accessories", true }, { "Armor", true }
                        });

                        randomizedOptions.Add(DataTableEnum.EquipItem, defaultOptions[DataTableEnum.EquipItem]);

                        break;
                    case "Starting Stats":
                        availableOptions.Add("Starting Stats", new Dictionary<string, bool>
                        {
                            { "Weapons", true }, { "Abilities", true }, { "Critical Abilities", true }
                        });

                        randomizedOptions.Add(DataTableEnum.ChrInit, defaultOptions[DataTableEnum.ChrInit]);

                        break;
                    case "Lucky Emblems":
                        availableOptions.Add("Lucky Emblems", new Dictionary<string, bool>
                        {
                            { "Lucky Emblems", true }
                        });

                        randomizedOptions.Add(DataTableEnum.LuckyMark, defaultOptions[DataTableEnum.LuckyMark]);

                        break;
                    case "Bonuses":
                        availableOptions.Add("Bonuses", new Dictionary<string, bool>
                        {
                            { "VBonus", true }, { "Flantastic Seven", true }, { "Minigames", true }
                        });
                        
                        randomizedOptions.Add(DataTableEnum.VBonus, defaultOptions[DataTableEnum.VBonus]);

                        break;
                    case "Fullcourse Abilities":
                        availableOptions.Add("Fullcourse Abilities", new Dictionary<string, bool>
                        {
                            { "Abilities", true }
                        });

                        randomizedOptions.Add(DataTableEnum.FullcourseAbility, defaultOptions[DataTableEnum.FullcourseAbility]);

                        break;
                    case "Level Ups":
                        availableOptions.Add("Level Ups", new Dictionary<string, bool>
                        {
                            { "Levels", true }
                        });

                        randomizedOptions.Add(DataTableEnum.LevelUp, defaultOptions[DataTableEnum.LevelUp]);

                        break;
                    case "Weapon Upgrades":
                        availableOptions.Add("Weapon Upgrades", new Dictionary<string, bool>
                        {
                            { "Kingdom Key", true }, { "Hero's Origin", true }, { "Shooting Star", true }, { "Favorite Deputy", true },
                            { "Ever After", true }, { "Happy Gear", true }, { "Crystal Snow", true }, { "Hunny Spout", true },
                            { "Wheel of Fate", true }, { "Nano Gear", true }, { "Starlight", true }, { "Grand Chef", true },
                            { "Classic Tone", true }, { "Ultima Weapon", true }, { "Elemental Encoder", true }, { "Oblivion", true },
                            { "Oathkeeper", true }
                        });

                        randomizedOptions.Add(DataTableEnum.WeaponEnhance, defaultOptions[DataTableEnum.WeaponEnhance]);

                        break;
                    case "Events":
                        availableOptions.Add("Events", new Dictionary<string, bool>
                        {
                            { "Events", true }, { "Keyblades", true }, { "Heartbinders", true }, { "Reports", true }, 
                            { "Classic Kingdom", true }, { "Key Items", true }, { "Data Battles", true }, { "Yozora", true }
                        });

                        randomizedOptions.Add(DataTableEnum.Event, defaultOptions[DataTableEnum.Event]);

                        break;
                    case "Synthesis Items":
                        availableOptions.Add("Synthesis Items", new Dictionary<string, bool>
                        {
                            { "Synthesis Items", true }
                        });

                        randomizedOptions.Add(DataTableEnum.SynthesisItem, defaultOptions[DataTableEnum.SynthesisItem]);

                        break;
                    default:
                        break;
                }
            }

            return availableOptions;
        }

        public void UpdateRandomizedItem(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions,
                                         DataTableEnum dataTableEnum, string category, string subCategory, string itemToChange,
                                         DataTableEnum swapDataTableEnum, string swapCategory, string swapSubCategory, string swapItemToChange)
        {
            if (dataTableEnum == DataTableEnum.ChrInit && subCategory == "Weapon")
            {
                var alteredSwapItemToChange = this.ConvertKeybladeWeaponToDefenseWeaponEnum(swapItemToChange);
                var alteredItemToChange = this.ConvertDefenseWeaponEnumToKeybladeWeapon(itemToChange);

                randomizedOptions[dataTableEnum][category][subCategory] = alteredSwapItemToChange;
                randomizedOptions[swapDataTableEnum][swapCategory][swapSubCategory] = alteredItemToChange;
            }
            else if (swapDataTableEnum == DataTableEnum.ChrInit && swapSubCategory == "Weapon")
            {
                var alteredSwapItemToChange = this.ConvertDefenseWeaponEnumToKeybladeWeapon(swapItemToChange);
                var alteredItemToChange = this.ConvertKeybladeWeaponToDefenseWeaponEnum(itemToChange);

                randomizedOptions[dataTableEnum][category][subCategory] = alteredSwapItemToChange;
                randomizedOptions[swapDataTableEnum][swapCategory][swapSubCategory] = alteredItemToChange;
            }
            else
            {
                randomizedOptions[dataTableEnum][category][subCategory] = swapItemToChange;
                randomizedOptions[swapDataTableEnum][swapCategory][swapSubCategory] = itemToChange;
            }
        }

        public Option UpdateRandomizedItemWithDefault(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions,
                                                      DataTableEnum dataTableEnum, string category, string subCategory, string itemToChange)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot\DefaultKH3.json"));
            
            var defaultOptions = JsonSerializer.Deserialize<Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>>>(streamReader.ReadToEnd());
            var swapItemToFind = (string)defaultOptions[dataTableEnum][category][subCategory].Clone();

            var option = new Option();

            var isFound = false;
            foreach (var randomOption in randomizedOptions)
            {
                foreach (var randomSubOption in randomOption.Value)
                {
                    foreach (var (name, value) in randomSubOption.Value)
                    {
                        if (value == swapItemToFind)
                        {
                            option = new Option { Category = randomOption.Key, SubCategory = randomSubOption.Key, Name = name, Value = value };

                            randomizedOptions[dataTableEnum][category][subCategory] = value;
                            randomizedOptions[randomOption.Key][randomSubOption.Key][name] = itemToChange;

                            isFound = true;
                        }
                    }

                    if (isFound)
                        break;
                }

                if (isFound)
                    break;
            }

            return option;
        }

        public Option UpdateRandomizedItemWithNone(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions,
                                                   ref Dictionary<string, Dictionary<string, bool>> availableOptions,
                                                   DataTableEnum dataTableEnum, string category, string subCategory, string itemToChange)
        {
            var hash = "ChangeOption".StringToSeed();

            var option = new Option();

            while (true) // Is there a way we can use a var instead of true?
            {
                var swapDataTable = randomizedOptions.ElementAt(rng.Next(0, randomizedOptions.Count()));
                var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));

                if (!availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapCategory.Key.CategoryToKey(swapDataTable.Key)])
                    continue;

                // Make sure this swap is valid before swapping
                if (!IsSwapValid(itemToChange, swapDataTable, swapCategory))
                {
                    continue;
                }

                if (swapCategory.Value.Where(x => x.Value.Contains("NONE")).Count() > 0)
                {
                    var swapData = swapCategory.Value.Where(x => x.Value.Contains("NONE")).ElementAt(rng.Next(0, swapCategory.Value.Where(x => x.Value.Contains("NONE")).Count()));

                    randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = itemToChange;
                    randomizedOptions[dataTableEnum][category][subCategory] = swapData.Value;

                    option = new Option { Category = swapDataTable.Key, SubCategory = swapCategory.Key, Name = swapData.Key, Value = swapData.Value };

                    break;
                }
            }


            return option;
        }


        /// <summary>
        /// Copy of the above method that takes an additional paramater replacements
        /// This is a Dictionary of DataTableEnums or subcategories that should not have anything placed on them
        /// </summary>
        /// <returns>Option of the items that have been swapped</returns>
        public Option UpdateRandomizedItemWithNone(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions,
                                                   ref Dictionary<string, Dictionary<string, bool>> availableOptions,
                                                   DataTableEnum dataTableEnum, string category, string subCategory, string itemToChange, Dictionary<DataTableEnum, Dictionary<string, bool>> replacements)
        {
            var option = new Option();

            while (true) // Is there a way we can use a var instead of true?
            {
                var swapDataTable = randomizedOptions.ElementAt(rng.Next(0, randomizedOptions.Count()));
                var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));

                if (!availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapCategory.Key.CategoryToKey(swapDataTable.Key)])
                {
                    continue;
                }

                if (replacements.ContainsKey(swapDataTable.Key) && replacements[swapDataTable.Key].Count == 0)
                {
                    continue;
                }

                if ((replacements.ContainsKey(swapDataTable.Key) && replacements[swapDataTable.Key].Count > 0 && replacements[swapDataTable.Key].GetValueOrDefault(swapCategory.Key.CategoryToKey(swapDataTable.Key))))
                {
                    continue;
                }

                // Make sure this swap is valid before swapping
                if (!IsSwapValid(itemToChange, swapDataTable, swapCategory))
                {
                    continue;
                }

                if (swapCategory.Value.Where(x => x.Value.Contains("NONE")).Count() > 0)
                {
                    var swapData = swapCategory.Value.Where(x => x.Value.Contains("NONE")).ElementAt(rng.Next(0, swapCategory.Value.Where(x => x.Value.Contains("NONE")).Count()));

                    if (swapDataTable.Key == DataTableEnum.ChrInit && swapData.Key.CategoryToKey(swapDataTable.Key) == "Critical Abilities")
                    {
                        continue;
                    }

                    if (blockedChecks.Contains(swapCategory.Key))
                    {
                        continue;
                    }

                    randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = itemToChange;
                    randomizedOptions[dataTableEnum][category][subCategory] = swapData.Value;
                    
                    option = new Option { Category = swapDataTable.Key, SubCategory = swapCategory.Key, Name = swapData.Key, Value = swapData.Value };

                    break;
                }
            }

            return option;
        }

        public void RandomizeItems(string seed, Dictionary<string, Extra> availableExtras, ref Dictionary<string, Dictionary<string, bool>> availableOptions,
                                   ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, Dictionary<DataTableEnum, Dictionary<string, bool>> replacements)
        {
            var hash = seed.StringToSeed();
            rng = new Random((int)hash);

            // Use randomizedItems
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot\DefaultKH3.json"));
            // Category > Id > Item > Value
            var defaultOptions = JsonSerializer.Deserialize<Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>>>(streamReader.ReadToEnd());

            List<string> proofs = new List<string>() { "Proof of Promises", "Proof of Times Past", "Proof of Fantasy" };
            List<string> importantChecks = new List<string>();
            if (availableOptions.ContainsKey(DataTableEnum.Event.DataTableEnumToKey()))
            {
                importantChecks.AddRange(proofs);
            }
            if (availableOptions.ContainsKey(DataTableEnum.ChrInit.DataTableEnumToKey())) 
            {
                // && availableOptions[DataTableEnum.ChrInit.DataTableEnumToKey()][]
                importantChecks.Add("Ability: Pole Spin");
            }

            List<string> foundChecks = new List<string>();

            while (foundChecks.Count != importantChecks.Count)
            {
                foundChecks = new List<string>();
                blockedChecks = new List<string>();

                randomizedOptions = new();
                var swapList = new List<string>();

                if (string.IsNullOrEmpty(seed))
                {
                    randomizedOptions = defaultOptions;
                    return;
                }

                // Get all related items
                foreach (var option in availableOptions)
                {
                    var firstPass = false;

                    foreach (var subOption in option.Value)
                    {
                        if (!subOption.Value || firstPass)
                            continue;

                        var dataTableEnum = this.GetDataTableEnumFromSelection(subOption.Key);

                        if (dataTableEnum == DataTableEnum.None)
                            dataTableEnum = this.ConvertDisplayStringToEnum(option.Key);

                        if (randomizedOptions.ContainsKey(dataTableEnum))
                            continue;

                        firstPass = (dataTableEnum == DataTableEnum.ChrInit || dataTableEnum == DataTableEnum.EquipItem ||
                                     dataTableEnum == DataTableEnum.VBonus || dataTableEnum == DataTableEnum.WeaponEnhance ||
                                     dataTableEnum == DataTableEnum.Event);

                        foreach (var (id, values) in defaultOptions[dataTableEnum])
                        {
                            if (id == "m_PlayerSora") { }

                            else if (id.Contains("GIVESORA") || !availableOptions[dataTableEnum.DataTableEnumToKey()][id.CategoryToKey(dataTableEnum)])
                                continue;

                            foreach (var value in values)
                            {
                                if (value.Key == "TypeB" || value.Key == "TypeC")// ||  value.Value.Contains("NONE"))
                                    continue;

                                if (id == "m_PlayerSora" && !availableOptions[dataTableEnum.DataTableEnumToKey()][value.Key.CategoryToKey(dataTableEnum)])
                                    continue;

                                // Remove Ultima if it's not enabled
                                if (value.Value.ValueIdToDisplay().Contains("Ultima Weapon") && !availableExtras["Allow Ultima"].Enabled)
                                {
                                    continue;
                                }

                                swapList.Add(value.Value);
                            }
                        }

                        var copy = defaultOptions[dataTableEnum].Where(x => !x.Key.Contains("GIVESORA")).ToDictionary(x => x.Key, y => new Dictionary<string, string>(y.Value));

                        randomizedOptions.Add(dataTableEnum, copy);
                    }
                }

                // Add Ultima if it's enabled and Synthesis Items aren't
                if (availableExtras["Allow Ultima"].Enabled && !availableOptions.ContainsKey("Synthesis Items") || (availableExtras["Allow Ultima"].Enabled && availableOptions.ContainsKey("Synthesis Items") && !availableOptions["Synthesis Items"]["Synthesis Items"]))
                {
                    swapList.Add("WEP_KEYBLADE_SO_015\u0000");
                }

                // Shuffle these around with our rng created from the seed
                swapList.Shuffle(rng);
                var queue = new Queue<string>(swapList);
                var tempQueue = new Queue<string>();

                // Put them back
                foreach (var option in availableOptions)
                {
                    var firstPass = false;
                    //if (!this.randomizedOptions.ContainsKey(option.Key))
                    //    continue;

                    foreach (var subOption in option.Value)
                    {
                        if (!subOption.Value || firstPass)
                            continue;

                        var dataTableEnum = this.GetDataTableEnumFromSelection(subOption.Key);

                        if (dataTableEnum == DataTableEnum.None)
                            dataTableEnum = this.ConvertDisplayStringToEnum(option.Key);

                        firstPass = (dataTableEnum == DataTableEnum.ChrInit || dataTableEnum == DataTableEnum.EquipItem ||
                                     dataTableEnum == DataTableEnum.VBonus || dataTableEnum == DataTableEnum.WeaponEnhance ||
                                     dataTableEnum == DataTableEnum.Event);

                        foreach (var (id, values) in defaultOptions[dataTableEnum])
                        {
                            if (id == "m_PlayerSora") { }

                            else if (id.Contains("GIVESORA") || !availableOptions[dataTableEnum.DataTableEnumToKey()][id.CategoryToKey(dataTableEnum)])
                                continue;

                            foreach (var value in values)
                            {
                                if (value.Key == "TypeB" || value.Key == "TypeC")// || value.Value.Contains("NONE"))
                                    continue;


                                if (id == "m_PlayerSora" && !availableOptions[dataTableEnum.DataTableEnumToKey()][value.Key.CategoryToKey(dataTableEnum)])
                                    continue;

                                if (value.Key.Contains("Ability"))
                                {
                                    var foundAvailableValue = false;

                                    while (!foundAvailableValue && queue.Count > 0)
                                    {
                                        var peekValue = queue.Peek();

                                        if (peekValue.Contains("ETresAbilityKind::"))
                                            foundAvailableValue = true;
                                        else
                                            tempQueue.Enqueue(queue.Dequeue());
                                    }

                                    if (queue.Count > 0)
                                    {
                                        randomizedOptions[dataTableEnum][id][value.Key] = queue.Dequeue();
                                    }
                                    else
                                    {
                                        // Empty TempQueue by enqueueing back into the queue with random lookups for abilities to swap the TempQueue with
                                        while (tempQueue.Count > 0)
                                        {
                                            // Find abilities by randomly selecting a randomizedOption to swap with
                                            var randCategory = randomizedOptions.ElementAt(rng.Next(0, randomizedOptions.Count));

                                            if (randCategory.Key != DataTableEnum.ChrInit && randCategory.Key != DataTableEnum.EquipItem && randCategory.Key != DataTableEnum.FullcourseAbility && randCategory.Key != DataTableEnum.WeaponEnhance)
                                            {
                                                var randData = randCategory.Value.ElementAt(rng.Next(0, randCategory.Value.Count));

                                                if (randData.Value.Count > 0)
                                                {
                                                    var randValue = randData.Value.ElementAt(rng.Next(0, randData.Value.Count));

                                                    if (!availableOptions[randCategory.Key.DataTableEnumToKey()][randData.Key.CategoryToKey(randCategory.Key)])
                                                        continue;

                                                    if (id == "m_PlayerSora" && !availableOptions[randCategory.Key.DataTableEnumToKey()][randValue.Key.CategoryToKey(dataTableEnum)])
                                                        continue;

                                                    if (randValue.Value.Contains("ETresAbilityKind::") && !(randValue.Key == "TypeB" || randValue.Key == "TypeC"))
                                                    {
                                                        queue.Enqueue(randomizedOptions[randCategory.Key][randData.Key][randValue.Key]);

                                                        randomizedOptions[randCategory.Key][randData.Key][randValue.Key] = tempQueue.Dequeue();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (tempQueue.Count > 0)
                                {
                                    randomizedOptions[dataTableEnum][id][value.Key] = tempQueue.Dequeue();
                                }
                                else
                                {
                                    if (queue.Count > 0)
                                    {
                                        randomizedOptions[dataTableEnum][id][value.Key] = queue.Dequeue();
                                    }
                                    else
                                    {
                                        // Empty TempQueue by enqueueing back into the queue with random lookups for abilities to swap the TempQueue with
                                        while (tempQueue.Count > 0)
                                        {
                                            // Find abilities by randomly selecting a randomizedOption to swap with
                                            var randCategory = randomizedOptions.ElementAt(rng.Next(0, randomizedOptions.Count));

                                            if (randCategory.Key != DataTableEnum.ChrInit && randCategory.Key != DataTableEnum.EquipItem && randCategory.Key != DataTableEnum.FullcourseAbility && randCategory.Key != DataTableEnum.WeaponEnhance)
                                            {
                                                var randData = randCategory.Value.ElementAt(rng.Next(0, randCategory.Value.Count));

                                                if (randData.Value.Count > 0)
                                                {
                                                    var randValue = randData.Value.ElementAt(rng.Next(0, randData.Value.Count));

                                                    if (!availableOptions[randCategory.Key.DataTableEnumToKey()][randData.Key.CategoryToKey(randCategory.Key)])
                                                        continue;

                                                    if (id == "m_PlayerSora" && !availableOptions[randCategory.Key.DataTableEnumToKey()][randValue.Key.CategoryToKey(dataTableEnum)])
                                                        continue;

                                                    if (randValue.Value.Contains("ETresAbilityKind::") && !(randValue.Key == "TypeB" || randValue.Key == "TypeC"))
                                                    {
                                                        queue.Enqueue(randomizedOptions[randCategory.Key][randData.Key][randValue.Key]);

                                                        randomizedOptions[randCategory.Key][randData.Key][randValue.Key] = tempQueue.Dequeue();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Account for VBonuses that need to be swapped for Proofs
                //if (availableOptions.ContainsKey("Bonuses") && availableOptions["Bonuses"]["VBonus"])
                //{
                //    var vbonusesToReplace = new string[2] { "Vbonus_077", "Vbonus_078" };

                //    foreach (var vbonus in vbonusesToReplace)
                //    {
                //        for (int i = 0; i < randomizedOptions[DataTableEnum.VBonus][vbonus].Count; ++i)
                //        {
                //            var bonus = randomizedOptions[DataTableEnum.VBonus][vbonus].ElementAt(i);
                //            if (bonus.Value == "KEY_ITEM12" || bonus.Value == "KEY_ITEM13" || bonus.Value == "KEY_ITEM14")
                //            {
                //                var swapDataTable = randomizedOptions.ElementAt(rng.Next(0, randomizedOptions.Count));
                //                var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));
                //                var swapData = swapCategory.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key][swapCategory.Key].Count));

                //                // Unsuccessful because we found another proof (somehow)
                //                if (swapData.Value == "KEY_ITEM12" || swapData.Value == "KEY_ITEM13" || swapData.Value == "KEY_ITEM14")
                //                {
                //                    --i;
                //                }
                //                else
                //                {
                //                    randomizedOptions[DataTableEnum.VBonus][vbonus][bonus.Key] = swapData.Value;
                //                    randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = bonus.Value;
                //                }
                //            }
                //        }
                //    }
                //}

                // Account for Keyblade on ChrInit
                if (availableOptions.ContainsKey("Starting Stats") && availableOptions["Starting Stats"]["Weapons"])
                {
                    var swapWeapon = "";
                    var swapOther = randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"]["Weapon"];

                    foreach (var (dataTableEnum, categories) in randomizedOptions)
                    {
                        foreach (var (categoryKey, options) in categories)
                        {
                            if (!availableOptions[dataTableEnum.DataTableEnumToKey()][categoryKey.CategoryToKey(dataTableEnum)])
                                continue;

                            var temp = options.FirstOrDefault(x => x.Value.Contains("WEP_KEYBLADE") && x.Value != ("WEP_KEYBLADE_SO_018"));

                            if (!string.IsNullOrEmpty(temp.Value))
                            {
                                swapWeapon = temp.Value;
                                randomizedOptions[dataTableEnum][categoryKey][temp.Key] = swapOther;
                                break;
                            }

                        }

                        if (!string.IsNullOrEmpty(swapWeapon))
                            break;
                    }

                    var altWeapon = this.ConvertKeybladeWeaponToDefenseWeaponEnum(swapWeapon);
                    randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"]["Weapon"] = altWeapon;
                }

                // Account for Luckymark Data
                if (availableOptions.ContainsKey("Lucky Emblems") && availableOptions["Lucky Emblems"]["Lucky Emblems"])
                {
                    foreach (var luckyMark in randomizedOptions[DataTableEnum.LuckyMark])
                    {
                        foreach (var subLuckyMark in luckyMark.Value)
                        {
                            if (!subLuckyMark.Value.Contains("::"))
                                continue;

                            while (true) // Is there a way we can use a var instead of true?
                            {
                                var tempOptions = randomizedOptions.Where(x => x.Key == DataTableEnum.EquipItem || x.Key == DataTableEnum.LevelUp || x.Key == DataTableEnum.VBonus);

                                var swapDataTable = tempOptions.ElementAt(rng.Next(0, tempOptions.Count()));
                                var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));

                                if (swapCategory.Key == "EVENT_KEYBLADE_012" || swapCategory.Key == "EVENT_KEYBLADE_013" || !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapCategory.Key.CategoryToKey(swapDataTable.Key)])
                                    continue;

                                if (swapCategory.Value.Where(x => !x.Value.Contains("::")).Count() > 0)
                                {
                                    var swapData = swapCategory.Value.Where(x => !x.Value.Contains("::")).ElementAt(rng.Next(0, swapCategory.Value.Where(x => !x.Value.Contains("::")).Count()));


                                    if (swapCategory.Key == "m_PlayerSora" && !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapData.Key.CategoryToKey(swapDataTable.Key)])
                                        continue;

                                    randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = subLuckyMark.Value;
                                    randomizedOptions[DataTableEnum.LuckyMark][luckyMark.Key][subLuckyMark.Key] = swapData.Value;

                                    break;
                                }
                            }
                        }
                    }
                }

                // Account for Synth Data (It will always needs items)
                if (availableOptions.ContainsKey("Synthesis Items") && availableOptions["Synthesis Items"]["Synthesis Items"])
                {
                    foreach (var synthesisItem in randomizedOptions[DataTableEnum.SynthesisItem])
                    {
                        foreach (var subSynthesisItem in synthesisItem.Value)
                        {
                            if (!subSynthesisItem.Value.Contains("::"))
                                continue;

                            while (true) // Is there a way we can use a var instead of true?
                            {
                                var tempOptions = randomizedOptions.Where(x => x.Key != DataTableEnum.SynthesisItem && x.Key != DataTableEnum.FullcourseAbility && x.Key != DataTableEnum.ChrInit && x.Key != DataTableEnum.LuckyMark);

                                var swapDataTable = tempOptions.ElementAt(rng.Next(0, tempOptions.Count()));
                                var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));

                                if (swapCategory.Key == "EVENT_KEYBLADE_012" || swapCategory.Key == "EVENT_KEYBLADE_013" || !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapCategory.Key.CategoryToKey(swapDataTable.Key)])
                                    continue;

                                if (swapCategory.Value.Where(x => !x.Value.Contains("::")).Count() > 0)
                                {
                                    var swapData = swapCategory.Value.Where(x => !x.Value.Contains("::")).ElementAt(rng.Next(0, swapCategory.Value.Where(x => !x.Value.Contains("::")).Count()));

                                    if (swapCategory.Key == "m_PlayerSora" && !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapData.Key.CategoryToKey(swapDataTable.Key)])
                                        continue;
                                    else if ((swapDataTable.Key == DataTableEnum.TreasureBT || swapDataTable.Key == DataTableEnum.TreasureBX || swapDataTable.Key == DataTableEnum.TreasureCA || swapDataTable.Key == DataTableEnum.TreasureEW ||
                                             swapDataTable.Key == DataTableEnum.TreasureFZ || swapDataTable.Key == DataTableEnum.TreasureHE || swapDataTable.Key == DataTableEnum.TreasureKG || swapDataTable.Key == DataTableEnum.TreasureMI ||
                                             swapDataTable.Key == DataTableEnum.TreasureRA || swapDataTable.Key == DataTableEnum.TreasureTS || swapDataTable.Key == DataTableEnum.TreasureTT) && subSynthesisItem.Value.ToLower().Contains("none"))
                                        continue;

                                    // Block proofs from appearing on synth if that extra isn't enabled
                                    if (proofs.Contains(swapData.Value.ValueIdToDisplay()) && !availableExtras["Synthesizable Proofs"].Enabled)
                                    {
                                        continue;
                                    }

                                    randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = subSynthesisItem.Value;
                                    randomizedOptions[DataTableEnum.SynthesisItem][synthesisItem.Key][subSynthesisItem.Key] = swapData.Value;

                                    break;
                                }
                            }
                        }
                    }
                }

                // Account for key abilities
                Dictionary<DataTableEnum, Dictionary<string, bool>> dataTablesToCheck = new Dictionary<DataTableEnum, Dictionary<string, bool>>();

                foreach (var extra in availableExtras)
                {
                    if (extra.Key.Contains("Key Abilities") && availableOptions.ContainsKey(extra.Value.RequiredPool) && !extra.Value.Enabled)
                    {
                        dataTablesToCheck.Add(extra.Value.RequiredPool.KeyToDataTableEnum(), new Dictionary<string, bool> { });
                    }
                }

                if (dataTablesToCheck.Count > 0)
                {
                    Dictionary<DataTableEnum, Dictionary<string, bool>> blockedDataTables = new Dictionary<DataTableEnum, Dictionary<string, bool>>(replacements);
                    blockedDataTables.Add(DataTableEnum.ChrInit, new Dictionary<string, bool> { { "Critical Abilities", true } });
                    foreach (var dt in dataTablesToCheck)
                    {
                        if (!blockedDataTables.ContainsKey(dt.Key))
                        {
                            blockedDataTables.Add(dt.Key, dt.Value);
                        }
                    }

                    foreach (var category in randomizedOptions)
                    {
                        foreach (var subCategory in category.Value)
                        {
                            foreach (var option in subCategory.Value)
                            {
                                if (keyAbilities.Contains(option.Value.ValueIdToDisplay()) && dataTablesToCheck.ContainsKey(category.Key)) 
                                {
                                    UpdateRandomizedItemWithNone(ref randomizedOptions, ref availableOptions, category.Key, subCategory.Key, option.Key, option.Value, blockedDataTables);
                                }
                            }
                        }
                    }
                }

                // Account for Treasures that have None in them
                if (availableOptions.ContainsKey("Treasures"))
                {
                    this.RemoveNoneFromTreasure(DataTableEnum.TreasureBT, ref randomizedOptions, availableOptions);
                    this.RemoveNoneFromTreasure(DataTableEnum.TreasureBX, ref randomizedOptions, availableOptions);
                    this.RemoveNoneFromTreasure(DataTableEnum.TreasureCA, ref randomizedOptions, availableOptions);
                    this.RemoveNoneFromTreasure(DataTableEnum.TreasureEW, ref randomizedOptions, availableOptions);
                    this.RemoveNoneFromTreasure(DataTableEnum.TreasureFZ, ref randomizedOptions, availableOptions);
                    this.RemoveNoneFromTreasure(DataTableEnum.TreasureHE, ref randomizedOptions, availableOptions);
                    this.RemoveNoneFromTreasure(DataTableEnum.TreasureKG, ref randomizedOptions, availableOptions);
                    this.RemoveNoneFromTreasure(DataTableEnum.TreasureMI, ref randomizedOptions, availableOptions);
                    this.RemoveNoneFromTreasure(DataTableEnum.TreasureRA, ref randomizedOptions, availableOptions);
                    this.RemoveNoneFromTreasure(DataTableEnum.TreasureTS, ref randomizedOptions, availableOptions);
                    this.RemoveNoneFromTreasure(DataTableEnum.TreasureTT, ref randomizedOptions, availableOptions);
                }

                // Account for Events that have None in them
                if (availableOptions.ContainsKey("Events"))
                {
                    this.RemoveNoneFromEvents(DataTableEnum.Event, ref randomizedOptions, availableOptions);
                }

                // Account for Pole Spin not being locked behind Frozen

                // List of places it can't be
                var disallowedPlaces = new List<string> { //"FZ_", // Disallow it in Frozen's treasures
                                                      //"21", // Disallow after level 20
                                                      //"20", // Disallow after lucky emblem milestone 15
                                                      //"IW_", // Disallow on weapon enhances
                                                      //"I0", // Disallow on equip items
                                                      "EVENT_007", // Disallow on Herc's Gold Statue Return
                                                      "TresUIMobilePortalDataAsset", // Disallow on all CK game reward
                                                      "EVENT_KEYBLADE_007", // Disallow on Keyblade event for Frozen
                                                      "EVENT_REPORT_009a", "EVENT_REPORT_009b", // Disallow on Reports in Arendelle
                                                      "Vbonus_041", "Vbonus_042", "Vbonus_043", "Vbonus_044", "Vbonus_045",
                                                      "Vbonus_046", "Vbonus_047", "Vbonus_048", "Vbonus_049", "Vbonus_050",
                                                      "VBonus_Minigame003", "VBonus_Minigame004", "VBonus_Minigame011" // Disallow on VBonuses related to Frozen
                                                    };
                foreach (var category in randomizedOptions)
                {
                    foreach (var subCategory in category.Value)
                    {
                        foreach (var option in subCategory.Value)
                        {
                            if (option.Value.Contains("POLE_SPIN"))
                            {
                                if (availableExtras["Pole Spin Start"].Enabled)
                                {
                                    // Pole Spin is already a starting ability
                                    if (category.Key == DataTableEnum.ChrInit && subCategory.Key.CategoryToKey(category.Key) == "Abilities")
                                    {
                                        break;
                                    }

                                    var chrInitValues = randomizedOptions[DataTableEnum.ChrInit].Values.ElementAt(0);
                                    var startingAbilities = chrInitValues.Where((x) => x.Key.CategoryToKey(DataTableEnum.ChrInit) == "Abilities");
                                    var emptyAbilities = startingAbilities.Where((x) => x.Value.Contains("NONE"));

                                    // Find a starting ability that is none and replace it
                                    // If all starting abilities are filled, replace one randomly
                                    var swapData = startingAbilities.ElementAt(rng.Next(0, startingAbilities.Count()));
                                    if (emptyAbilities.Count() > 0)
                                    {
                                        swapData = emptyAbilities.First();
                                    }

                                    randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"][swapData.Key] = option.Value;
                                    randomizedOptions[category.Key][subCategory.Key][option.Key] = swapData.Value;
                                }
                                else
                                {
                                    var swapLogic = this.IsPoleSpinDisallowed(category.Key, subCategory.Key);

                                    while (swapLogic) // Is there a way we can use a var instead of true?
                                    {
                                        var tempOptions = randomizedOptions.Where(x => x.Key != DataTableEnum.EquipItem && x.Key != DataTableEnum.WeaponEnhance && x.Key != DataTableEnum.TreasureFZ);

                                        var swapDataTable = tempOptions.ElementAt(rng.Next(0, tempOptions.Count()));
                                        var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));

                                        if (swapCategory.Key == "EVENT_KEYBLADE_012" || swapCategory.Key == "EVENT_KEYBLADE_013" || !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapCategory.Key.CategoryToKey(swapDataTable.Key)])
                                            continue;

                                        if (!this.IsPoleSpinDisallowed(swapDataTable.Key, swapCategory.Key) && swapCategory.Value.Where(x => x.Value.Contains("ETresAbilityKind::")).Count() > 0)
                                        {
                                            var swapData = swapCategory.Value.Where(x => x.Value.Contains("ETresAbilityKind::")).ElementAt(rng.Next(0, swapCategory.Value.Where(x => x.Value.Contains("ETresAbilityKind::")).Count()));

                                            if (swapCategory.Key == "m_PlayerSora" && !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapData.Key.CategoryToKey(swapDataTable.Key)])
                                                continue;

                                            // Stop Pole Spin from being a crit ability
                                            if (swapDataTable.Key == DataTableEnum.ChrInit && swapData.Key.CategoryToKey(swapDataTable.Key).Contains("Critical"))
                                                continue;

                                            // Stop Pole Spin from being on levels past 20
                                            if (swapDataTable.Key == DataTableEnum.LevelUp && (int.Parse(swapCategory.Key) > 20 || swapData.Key != "TypeA"))
                                                continue;

                                            randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = option.Value;
                                            randomizedOptions[category.Key][subCategory.Key][option.Key] = swapData.Value;

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Account for Levelup Data
                if (availableOptions.ContainsKey("Level Ups") && availableOptions["Level Ups"]["Levels"])
                {
                    foreach (var level in defaultOptions[DataTableEnum.LevelUp])
                    {
                        var typeA = level.Value["TypeA"];
                        var randValue = randomizedOptions[DataTableEnum.LevelUp][level.Key]["TypeA"];

                        // Only place checks on TypeA if replacing level ups
                        if (replacements.ContainsKey(DataTableEnum.LevelUp) && replacements[DataTableEnum.LevelUp].GetValueOrDefault("Levels"))
                        {
                            randomizedOptions[DataTableEnum.LevelUp][level.Key]["TypeB"] = "ETresAbilityKind::NONE\u0000";
                            randomizedOptions[DataTableEnum.LevelUp][level.Key]["TypeC"] = "ETresAbilityKind::NONE\u0000";
                        }
                        else
                        {
                            if (typeA.Contains("NONE"))
                            {
                                randomizedOptions[DataTableEnum.LevelUp][level.Key]["TypeB"] = randValue;
                                randomizedOptions[DataTableEnum.LevelUp][level.Key]["TypeC"] = randValue;

                                continue;
                            }

                            var levelTypeB = "";
                            var levelTypeC = "";

                            foreach (var subLevel in defaultOptions[DataTableEnum.LevelUp])
                            {
                                if (!string.IsNullOrEmpty(levelTypeB) && !string.IsNullOrEmpty(levelTypeC))
                                    break;

                                if (typeA == subLevel.Value["TypeB"])
                                    levelTypeB = subLevel.Key;

                                if (typeA == subLevel.Value["TypeC"])
                                    levelTypeC = subLevel.Key;
                            }

                            if (!string.IsNullOrEmpty(levelTypeB))
                                randomizedOptions[DataTableEnum.LevelUp][levelTypeB]["TypeB"] = randValue;

                            if (!string.IsNullOrEmpty(levelTypeC))
                                randomizedOptions[DataTableEnum.LevelUp][levelTypeC]["TypeC"] = randValue;
                        }
                    }

                    // Replace Level Ups
                    if (replacements.ContainsKey(DataTableEnum.LevelUp) && replacements[DataTableEnum.LevelUp].GetValueOrDefault("Levels"))
                    {
                        this.ReplaceChecks(DataTableEnum.LevelUp, ref randomizedOptions, availableOptions, replacements);
                    }
                }

                // Replace checks
                foreach (var replacePool in replacements)
                {
                    if (availableOptions.ContainsKey(replacePool.Key.DataTableEnumToKey()))
                    {
                        if (replacements[replacePool.Key].Count == 0 && replacePool.Key != DataTableEnum.LevelUp)
                        {
                            this.ReplaceChecks(replacePool.Key, ref randomizedOptions, availableOptions, replacements);
                        }
                        else if (replacePool.Key == DataTableEnum.WeaponEnhance && replacements[replacePool.Key]["Weapon Upgrades"])
                        {
                            // Replace each check in WeaponEnhance
                            // WeaponEnhance has subcategories for each keyblade, but we want to replace them all for now
                            foreach (var check in randomizedOptions[DataTableEnum.WeaponEnhance])
                            {
                                if (availableOptions["Weapon Upgrades"][check.Key.CategoryToKey(DataTableEnum.WeaponEnhance)])
                                {
                                    this.ReplaceCheck(ref randomizedOptions, availableOptions, replacements, DataTableEnum.WeaponEnhance, check.Key);
                                }
                            }
                        }
                        else if (replacePool.Value.Count > 0 && replacePool.Key != DataTableEnum.LevelUp)
                        {
                            foreach (var subPool in replacePool.Value)
                            {
                                if (replacements[replacePool.Key].Count > 0 && replacements[replacePool.Key][subPool.Key])
                                {
                                    this.ReplaceChecks(replacePool.Key, subPool.Key, ref randomizedOptions, availableOptions, replacements);
                                    if (subPool.Key == "Yozora")
                                    {
                                        ReplaceCheck(ref randomizedOptions, availableOptions, replacements, DataTableEnum.Event, "EVENT_KEYITEM_005");
                                    }
                                    if (subPool.Key == "Complete All Classic Kingdom Minigames" && !replacements[DataTableEnum.Event]["Keyblades"])
                                    {
                                        ReplaceCheck(ref randomizedOptions, availableOptions, replacements, DataTableEnum.Event, "TresUIMobilePortalDataAsset");
                                    }
                                    if (subPool.Key == "Complete All Bistro Recipes" && !replacements[DataTableEnum.Event]["Keyblades"])
                                    {
                                        ReplaceCheck(ref randomizedOptions, availableOptions, replacements, DataTableEnum.Event, "EVENT_KEYBLADE_010");
                                    }
                                }
                            }
                        }
                    } 
                }
                
                // Replace Arendelle Small Chest 13 and Synthesis Item 80 (IS_79)
                // The chest appears to be bugged and doesn't give any rewards?
                // The synth item is the photo mission for Demon Tower, which isn't possible without battlegates 
                // Remove these when the chest and battlegates are functioning properly (chest is being reenabled for more testing)
                //ReplaceCheck(ref randomizedOptions, availableOptions, replacements, DataTableEnum.TreasureFZ, "FZ_SBOX_013");
                ReplaceCheck(ref randomizedOptions, availableOptions, replacements, DataTableEnum.SynthesisItem, "IS_79");

                // Replace beat the game on crit and the two moogle proof checks
                // These need to be randomized in for proofs and oblivion/oathkeeper, but they are not accessible
                // Oblivion/Oathkeeper were being checked in multiple places before, but it might be easier to just pull them out here
                ReplaceCheck(ref randomizedOptions, availableOptions, replacements, DataTableEnum.Event, "EVENT_KEYITEM_004");
                ReplaceCheck(ref randomizedOptions, availableOptions, replacements, DataTableEnum.Event, "EVENT_KEYBLADE_012");
                ReplaceCheck(ref randomizedOptions, availableOptions, replacements, DataTableEnum.Event, "EVENT_KEYBLADE_013");

                // Clean VBonuses
                // This will reassign VBonuses such that each VBonus will have at least one check on it
                if (availableExtras["Balanced Bonuses"].Enabled)
                {
                    this.CleanVBonuses(DataTableEnum.VBonus, ref randomizedOptions, availableOptions, replacements);
                }

                // Add back the default values that were not included
                foreach (var option in availableOptions)
                {
                    foreach (var subOption in option.Value.Where(x => !x.Value))
                    {
                        var dataTableEnum = this.GetDataTableEnumFromSelection(subOption.Key);

                        if (dataTableEnum == DataTableEnum.None)
                        {
                            dataTableEnum = this.ConvertDisplayStringToEnum(option.Key);
                        }

                        if (!randomizedOptions.ContainsKey(dataTableEnum))
                            randomizedOptions.Add(dataTableEnum, defaultOptions[dataTableEnum]);
                    }
                }

                // Do one more pass through to make sure that important checks are in the pool
                foreach (var option in randomizedOptions)
                {
                    foreach (var reward in option.Value)
                    {
                        if (reward.Value.Count > 0)
                        {
                            foreach (var check in reward.Value)
                            {
                                // Add this check to foundChecks if it's important
                                // Only add the check once if it's on level ups (it'll be there 3 times, one per path)
                                if (importantChecks.Contains(check.Value.ValueIdToDisplay()) && (check.Key.CategoryToKey(option.Key) != "Levels" || !foundChecks.Contains(check.Value.ValueIdToDisplay())))
                                {
                                    foundChecks.Add(check.Value.ValueIdToDisplay());
                                }
                            }
                        }
                    }
                }
            }
        }

        public byte[] GenerateRandomizerSeed(string currentSeed, Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions,
                                             Dictionary<string, bool> availablePools, Dictionary<string, Extra> availableExtras, Dictionary<string, Dictionary<string, bool>> availableOptions, Dictionary<DataTableEnum, Dictionary<string, bool>> replacements, List<Tuple<Option, Option>> modifications, byte[] hints)
        {
            var dataTableManager = new UE4DataTableInterpreter.DataTableManager();
            var dataTables = dataTableManager.RandomizeDataTables(randomizedOptions);

            var zipArchive = this.CreateZipArchive(dataTables, currentSeed, availablePools, availableExtras, availableOptions, replacements, modifications, hints);

            return zipArchive;

            //var zipPath = @$".\Seeds\pakchunk99-randomizer-{currentSeed}\pakchunk99-randomizer-{currentSeed}.zip";
            //ZipFile.ExtractToDirectory(zipPath, @$".\Seeds\pakchunk99-randomizer-{currentSeed}\");

            //if (File.Exists(zipPath))
            //    File.Delete(zipPath);

            //if (File.Exists(@$".\Seeds\pakchunk99-randomizer-{currentSeed}.pak"))
            //    File.Delete(@$".\Seeds\pakchunk99-randomizer-{currentSeed}.pak");

            //var pakPath = Path.Combine(Environment.CurrentDirectory, @$"wwwroot\pak\Seeds\pakchunk99-randomizer-{currentSeed}");
            //Directory.Move(@$".\Seeds\pakchunk99-randomizer-{currentSeed}", pakPath);

            //pakPath.ExecuteCommand();

            //Directory.Move($"{pakPath}.pak", @$".\Seeds\pakchunk99-randomizer-{currentSeed}.pak");

            //return new List<byte[]> { this.GetFile(@$".\Seeds\pakchunk99-randomizer-{currentSeed}.pak"), this.GetFile(@$"{pakPath}\SpoilerLog.json") };
        }

        public byte[] CreateZipArchive(Dictionary<string, List<byte>> dataTables, string randomSeed, Dictionary<string, bool> availablePools, Dictionary<string, Extra> availableExtras, Dictionary<string, Dictionary<string, bool>> availableOptions, Dictionary<DataTableEnum, Dictionary<string, bool>> replacements, List<Tuple<Option, Option>> modifications, byte[] hints)
        {
            var zipPath = @$".\Seeds\pakchunk99-randomizer-{randomSeed}\pakchunk99-randomizer-{randomSeed}.zip";

            if (Directory.Exists(@$".\Seeds\pakchunk99-randomizer-{randomSeed}"))
                Directory.Delete(@$".\Seeds\pakchunk99-randomizer-{randomSeed}", true);

            // Create the ZIP Archive
            Directory.CreateDirectory(@$".\Seeds\pakchunk99-randomizer-{randomSeed}");

            if (File.Exists(zipPath))
                File.Delete(zipPath);

            using (var zipFile = new FileStream(zipPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using var archive = new ZipArchive(zipFile, ZipArchiveMode.Update);

                // Create the SpoilerLog file
                var spoilerEntry = archive.CreateEntry("SpoilerLog.json");
                using var spoilerWriter = new StreamWriter(spoilerEntry.Open());

                var jsonTupleList = new List<Tuple<Tuple<int, string, string, string>, Tuple<int, string, string, string>>>();
                foreach(var (initial, swap) in modifications)
                {
                    jsonTupleList.Add(new (new Tuple<int, string, string, string>((int)initial.Category, initial.SubCategory, initial.Name, initial.Value),
                                           new Tuple<int, string, string, string>((int)swap.Category, swap.SubCategory, swap.Name, swap.Value)));
                }

                var spoilerLogFile = new SpoilerLogFile
                {
                    SeedName = randomSeed,
                    AvailablePools = availablePools,
                    AvailableExtras = availableExtras,
                    AvailableOptions = availableOptions,
                    Replacements = replacements,
                    Modifications = jsonTupleList
                };

                var jsonSpoiler = JsonSerializer.Serialize(spoilerLogFile);

                spoilerWriter.WriteLine(jsonSpoiler);


                // Create Hints
                var hintEntry = archive.CreateEntry(@"KINGDOM HEARTS III\Content\Localization\Game\en\kh3_mobile.locres");
                using var hintStream = new MemoryStream(hints);
                using var hintEntryStream = hintEntry.Open();
                
                hintStream.CopyTo(hintEntryStream);


                // Create the entry from the file path/ name, open the data in a memory stream and copy it to the entry
                foreach (var (filePathAndName, data) in dataTables)
                {
                    var dataTableEntry = archive.CreateEntry(filePathAndName);
                    using var memoryStream = new MemoryStream(data.ToArray());
                    using var stream = dataTableEntry.Open();

                    memoryStream.CopyTo(stream);
                }
            }

            return this.GetFile(zipPath);
        }

        public byte[] GetFile(string path)
        {
            using var reader = new FileStream(path, FileMode.Open);
            using var fileDataStream = new MemoryStream();
            reader.CopyTo(fileDataStream);

            return fileDataStream.ToArray();
        }

        public void DeleteRandomizerSeed(string currentSeed)
        {
            Directory.Delete($@".\Seeds\pakchunk99-randomizer-{currentSeed}", true);
            //Directory.Delete($@".\wwwroot\pak\Seeds\pakchunk99-randomizer-{currentSeed}", true);
            //File.Delete($@".\Seeds\pakchunk99-randomizer-{currentSeed}.pak");
        }


        #region Helper Methods

        public bool IsPoleSpinDisallowed(DataTableEnum category, string subCategory)
        {
            bool swapLogic;

            if (category != DataTableEnum.TreasureFZ && category != DataTableEnum.EquipItem && category != DataTableEnum.WeaponEnhance)
            {
                if ((category == DataTableEnum.LevelUp || category == DataTableEnum.LuckyMark) && int.Parse(subCategory) <= 20)
                {
                    swapLogic = true;
                }
                else if (category == DataTableEnum.VBonus && (subCategory != "Vbonus_041" || subCategory != "Vbonus_042" || subCategory != "Vbonus_043" ||
                    subCategory != "Vbonus_044" || subCategory != "Vbonus_045" || subCategory != "Vbonus_046" || subCategory != "Vbonus_047" ||
                    subCategory != "Vbonus_048" || subCategory != "Vbonus_049" || subCategory != "Vbonus_050" ||
                    subCategory != "VBonus_Minigame003" || subCategory != "VBonus_Minigame004" || subCategory != "VBonus_Minigame005"))
                {
                    swapLogic = true;
                }
                else if (category == DataTableEnum.Event && (subCategory != "EVENT_007" || subCategory != "TresUIMobilePortalDataAsset" ||
                    subCategory != "EVENT_KEYBLADE_007" || subCategory != "EVENT_KEYBLADE_010" || subCategory != "EVENT_REPORT_009a" || subCategory != "EVENT_REPORT_009b"))
                {
                    swapLogic = true;
                }
                else
                {
                    swapLogic = false;
                }
            }
            else
            {
                swapLogic = true;
            }

            return swapLogic;
        }

        public bool IsOptionAvailable(Dictionary<string, Dictionary<string, bool>> availableOptions, DataTableEnum category, string subCategory)
        {
            return false;
        }

        public List<Treasure> GetAvailableTreasures(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var treasures = new List<Treasure>();
            var dataTableEnum = this.GetDataTableEnumFromSelection(currentSelection);

            if (randomizedOptions.ContainsKey(dataTableEnum))
            {
                foreach (var treasure in randomizedOptions[dataTableEnum])
                {
                    var treasureId = treasure.Key;
                    var treasureName = treasure.Value["Treasure"];

                    treasures.Add(new Treasure { Id = treasureId, Reward = treasureName });
                }
            }

            return treasures;
        }

        public List<Equippable> GetAvailableEquippables(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var equippables = new List<Equippable>();

            if (randomizedOptions.ContainsKey(DataTableEnum.EquipItem))
            {
                var equippableSubsets = new Dictionary<string, Dictionary<string, string>>();

                if (currentSelection.Equals("Weapons"))
                    equippableSubsets = randomizedOptions[DataTableEnum.EquipItem].Where(x => x.Key.Contains("I03")).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Armor"))
                    equippableSubsets = randomizedOptions[DataTableEnum.EquipItem].Where(x => x.Key.Contains("I04")).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Accessories"))
                    equippableSubsets = randomizedOptions[DataTableEnum.EquipItem].Where(x => x.Key.Contains("I05")).ToDictionary(x => x.Key, y => y.Value);

                foreach (var subset in equippableSubsets)
                {
                    if (subset.Value.Count > 0)
                        equippables.Add(new Equippable { EquipItem = subset.Key, Ability1 = subset.Value.GetValueOrDefault("Ability0", ""), 
                                                         Ability2 = subset.Value.GetValueOrDefault("Ability1", ""), Ability3 = subset.Value.GetValueOrDefault("Ability2", "") });
                }
            }

            return equippables;
        }

        public List<ChrInit> GetAvailableChrInits(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var chrInits = new List<ChrInit>();

            if (randomizedOptions.ContainsKey(DataTableEnum.ChrInit))
            {
                foreach (var chrInit in randomizedOptions[DataTableEnum.ChrInit])
                {
                    var chrInitSubsets = new Dictionary<string, string>();

                    if (currentSelection.Equals("Weapons"))
                        chrInitSubsets = chrInit.Value.Where(x => x.Key == "Weapon").ToDictionary(x => x.Key, y => y.Value);   
                    else if (currentSelection.Equals("Abilities"))
                        chrInitSubsets = chrInit.Value.Where(x => !x.Key.Contains("Crit") && x.Key != "Weapon").ToDictionary(x => x.Key, y => y.Value);
                    else if (currentSelection.Equals("Critical Abilities"))
                        chrInitSubsets = chrInit.Value.Where(x => x.Key.Contains("Crit")).ToDictionary(x => x.Key, y => y.Value);


                    foreach (var subset in chrInitSubsets)
                        chrInits.Add(new ChrInit { Player = chrInit.Key, Name = subset.Key, Value = subset.Value });
                }
            }

            return chrInits;
        }

        public List<LuckyEmblem> GetAvailableLuckyEmblems(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var luckyEmblems = new List<LuckyEmblem>();

            if (randomizedOptions.ContainsKey(DataTableEnum.LuckyMark))
            {
                foreach (var luckyEmblem in randomizedOptions[DataTableEnum.LuckyMark])
                {
                    var luckyEmblemSubsets = new Dictionary<string, string>();

                    if (currentSelection.Equals("Lucky Emblems"))
                        luckyEmblemSubsets = luckyEmblem.Value.Where(x => x.Key == "Lucky Emblems").ToDictionary(x => x.Key, y => y.Value);

                    foreach (var subset in luckyEmblem.Value)
                        luckyEmblems.Add(new LuckyEmblem { Milestone = luckyEmblem.Key, Reward = subset.Value });
                }
            }

            return luckyEmblems;
        }

        public List<Bonus> GetAvailableBonuses(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var bonuses = new List<Bonus>();

            if (randomizedOptions.ContainsKey(DataTableEnum.VBonus))
            {
                foreach (var bonus in randomizedOptions[DataTableEnum.VBonus])
                {
                    if (bonus.Key.Contains("GIVESORA"))
                        continue;

                    var bonusSubsets = new Dictionary<string, string>();

                    if (currentSelection.Equals("VBonus"))
                    {
                        if (!bonus.Key.Contains("Minigame"))
                            bonusSubsets = bonus.Value.ToDictionary(x => x.Key, y => y.Value);
                    }
                    else if (currentSelection.Equals("Flantastic Seven"))
                    {
                        if (bonus.Key.Contains("Minigame") && int.Parse(bonus.Key[^3..]) >= 7)
                            bonusSubsets = bonus.Value.ToDictionary(x => x.Key, y => y.Value);
                    }
                    else if (currentSelection.Equals("Minigames"))
                    {
                        if (bonus.Key.Contains("Minigame") && int.Parse(bonus.Key[^3..]) < 7)
                            bonusSubsets = bonus.Value.ToDictionary(x => x.Key, y => y.Value);
                    }


                    if (bonusSubsets.Count > 0)
                        bonuses.Add(new Bonus { Name = bonus.Key, Bonus1 = bonusSubsets.GetValueOrDefault("Sora_Bonus1", ""), Ability1 = bonusSubsets.GetValueOrDefault("Sora_Ability1", ""),
                                                Bonus2 = bonusSubsets.GetValueOrDefault("Sora_Bonus2", ""), Ability2 = bonusSubsets.GetValueOrDefault("Sora_Ability2", "") });
                }
            }

            return bonuses;
        }

        public List<Fullcourse> GetAvailableFullcourses(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var fullcourses = new List<Fullcourse>();

            if (randomizedOptions.ContainsKey(DataTableEnum.FullcourseAbility))
            {
                foreach (var fullcourse in randomizedOptions[DataTableEnum.FullcourseAbility])
                {
                    var fullcourseSubsets = new Dictionary<string, string>();

                    if (currentSelection.Equals("Abilities"))
                        fullcourseSubsets = fullcourse.Value.Where(x => x.Key == "Abilities").ToDictionary(x => x.Key, y => y.Value);

                    foreach (var subset in fullcourse.Value)
                        fullcourses.Add(new Fullcourse { Id = fullcourse.Key, Ability = subset.Value });
                }
            }

            return fullcourses;
        }

        public List<LevelUp> GetAvailableLevelUps(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var levelUps = new List<LevelUp>();

            if (randomizedOptions.ContainsKey(DataTableEnum.LevelUp))
            {
                foreach (var levelUp in randomizedOptions[DataTableEnum.LevelUp])
                {
                    var levelUpSubsets = new Dictionary<string, string>();

                    if (currentSelection.Equals("Levels"))
                        levelUpSubsets = levelUp.Value.ToDictionary(x => x.Key, y => y.Value);

                    if (levelUpSubsets.Count > 0)
                        levelUps.Add(new LevelUp { Milestone = levelUp.Key, TypeAReward = levelUpSubsets.GetValueOrDefault("TypeA", ""),
                                                   TypeBReward = levelUpSubsets.GetValueOrDefault("TypeB", ""), TypeCReward = levelUpSubsets.GetValueOrDefault("TypeC", "") });
                }
            }

            return levelUps;
        }

        public List<WeaponUpgrade> GetAvailableWeaponUpgrades(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var weaponUpgrades = new List<WeaponUpgrade>();

            if (randomizedOptions.ContainsKey(DataTableEnum.WeaponEnhance))
            {
                var weaponUpgradeSubsets = new Dictionary<string, Dictionary<string, string>>();

                if (currentSelection.Equals("Kingdom Key"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) < 10).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Shooting Star"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 10 && int.Parse(x.Key.Split('_')[1]) < 20).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Hero's Origin"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 20 && int.Parse(x.Key.Split('_')[1]) < 30).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Favorite Deputy"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 30 && int.Parse(x.Key.Split('_')[1]) < 40).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Ever After"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 40 && int.Parse(x.Key.Split('_')[1]) < 50).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Wheel of Fate"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 50 && int.Parse(x.Key.Split('_')[1]) < 60).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Crystal Snow"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 60 && int.Parse(x.Key.Split('_')[1]) < 70).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Hunny Spout"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 70 && int.Parse(x.Key.Split('_')[1]) < 80).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Nano Gear"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 80 && int.Parse(x.Key.Split('_')[1]) < 90).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Happy Gear"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 90 && int.Parse(x.Key.Split('_')[1]) < 100).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Classic Tone"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 100 && int.Parse(x.Key.Split('_')[1]) < 110).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Grand Chef"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 110 && int.Parse(x.Key.Split('_')[1]) < 120).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Ultima Weapon"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 120 && int.Parse(x.Key.Split('_')[1]) < 130).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Elemental Encoder"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 150 && int.Parse(x.Key.Split('_')[1]) < 160).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Starlight"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 160 && int.Parse(x.Key.Split('_')[1]) < 170).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Oathkeeper"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 170 && int.Parse(x.Key.Split('_')[1]) < 180).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Oblivion"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 180 && int.Parse(x.Key.Split('_')[1]) < 190).ToDictionary(x => x.Key, y => y.Value);

                foreach (var weaponUpgrade in weaponUpgradeSubsets)
                {
                    foreach (var subSubset in weaponUpgrade.Value)
                        weaponUpgrades.Add(new WeaponUpgrade { Id = weaponUpgrade.Key, Name = subSubset.Value });
                }
            }

            return weaponUpgrades;
        }

        public List<Event> GetAvailableEvents(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var events = new List<Event>();

            if (randomizedOptions.ContainsKey(DataTableEnum.Event))
            {
                var eventSubsets = new Dictionary<string, Dictionary<string, string>>();
                
                if (currentSelection.Equals("Events"))
                    eventSubsets = randomizedOptions[DataTableEnum.Event]
                        .Where(x => !x.Key.Contains("TresUIMobilePortalDataAsset") && !x.Key.Contains("KEYBLADE") && !x.Key.Contains("HEARTBINDER") && !x.Key.Contains("REPORT") && !x.Key.Contains("CKGAME") && !x.Key.Contains("KEYITEM") && !x.Key.Contains("DATAB") && !x.Key.Contains("YOZORA"))
                        .ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Keyblades"))
                    eventSubsets = randomizedOptions[DataTableEnum.Event].Where(x => x.Key.Contains("KEYBLADE") || x.Key.Equals("TresUIMobilePortalDataAsset")).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Heartbinders"))
                    eventSubsets = randomizedOptions[DataTableEnum.Event].Where(x => x.Key.Contains("HEARTBINDER")).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Reports"))
                    eventSubsets = randomizedOptions[DataTableEnum.Event].Where(x => x.Key.Contains("REPORT")).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Classic Kingdom"))
                    eventSubsets = randomizedOptions[DataTableEnum.Event].Where(x => x.Key.Contains("CKGAME")).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Key Items"))
                    eventSubsets = randomizedOptions[DataTableEnum.Event].Where(x => x.Key.Contains("KEYITEM")).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Data Battles"))
                    eventSubsets = randomizedOptions[DataTableEnum.Event].Where(x => x.Key.Contains("DATAB")).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Yozora"))
                    eventSubsets = randomizedOptions[DataTableEnum.Event].Where(x => x.Key.Contains("YOZORA")).ToDictionary(x => x.Key, y => y.Value);

                foreach (var tempEvent in eventSubsets)
                {
                    foreach (var subSubset in tempEvent.Value)
                        events.Add(new Event { Id = tempEvent.Key, Category = subSubset.Key, Reward = subSubset.Value });
                }
            }

            return events;
        }

        public List<SynthesisItem> GetAvailableSynthesisItems(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var synthesisItems = new List<SynthesisItem>();

            if (randomizedOptions.ContainsKey(DataTableEnum.SynthesisItem))
            {
                var synthesisItemSubsets = new Dictionary<string, Dictionary<string, string>>();

                if (currentSelection.Equals("Synthesis Items"))
                    synthesisItemSubsets = randomizedOptions[DataTableEnum.SynthesisItem].ToDictionary(x => x.Key, y => y.Value);

                foreach (var tempSynthesisItem in synthesisItemSubsets)
                {
                    foreach (var subSubset in tempSynthesisItem.Value)
                        synthesisItems.Add(new SynthesisItem { Id = tempSynthesisItem.Key, Category = subSubset.Key, Reward = subSubset.Value });
                }
            }

            return synthesisItems;
        }

        private DataTableEnum GetDataTableEnumFromSelection(string selection)
        {
            switch(selection)
            {
                case "Olympus":
                    return DataTableEnum.TreasureHE;
                case "Twilight Town":
                    return DataTableEnum.TreasureTT;
                case "Kingdom of Corona":
                    return DataTableEnum.TreasureRA;
                case "Toy Box":
                    return DataTableEnum.TreasureTS;
                case "Arendelle":
                    return DataTableEnum.TreasureFZ;
                case "Monstropolis":
                    return DataTableEnum.TreasureMI;
                case "The Caribbean":
                    return DataTableEnum.TreasureCA;
                case "San Fransokyo":
                    return DataTableEnum.TreasureBX;
                case "Keyblade Graveyard":
                    return DataTableEnum.TreasureKG;
                case "The Final World":
                    return DataTableEnum.TreasureEW;
                case "Scala Ad Caelum":
                    return DataTableEnum.TreasureBT;
                default:
                    return DataTableEnum.None;
            }
        }

        private DataTableEnum ConvertDisplayStringToEnum(string displayString)
        {
            switch (displayString)
            {
                case "Starting Stats":
                    return DataTableEnum.ChrInit;
                case "Equippables":
                    return DataTableEnum.EquipItem;
                case "Events":
                    return DataTableEnum.Event;
                case "Fullcourse Abilities":
                    return DataTableEnum.FullcourseAbility;
                case "Level Ups":
                    return DataTableEnum.LevelUp;
                case "Lucky Emblems":
                    return DataTableEnum.LuckyMark;
                case "Bonuses":
                    return DataTableEnum.VBonus;
                case "Weapon Upgrades":
                    return DataTableEnum.WeaponEnhance;
                case "Synthesis Items":
                    return DataTableEnum.SynthesisItem;
                default:
                    return DataTableEnum.None;
            }
        }

        private string ConvertKeybladeWeaponToDefenseWeaponEnum(string keyblade)
        {
            var value = keyblade.ValueIdToDisplay();

            switch (value)
            {
                case "Kingdom Key":
                    return "ETresItemDefWeapon::WEP_KEYBLADE00\u0000";
                case "Hero's Origin":
                    return "ETresItemDefWeapon::WEP_KEYBLADE02\u0000";
                case "Shooting Star":
                    return "ETresItemDefWeapon::WEP_KEYBLADE01\u0000";
                case "Favorite Deputy":
                    return "ETresItemDefWeapon::WEP_KEYBLADE03\u0000";
                case "Ever After":
                    return "ETresItemDefWeapon::WEP_KEYBLADE04\u0000";
                case "Happy Gear":
                    return "ETresItemDefWeapon::WEP_KEYBLADE09\u0000";
                case "Crystal Snow":
                    return "ETresItemDefWeapon::WEP_KEYBLADE06\u0000";
                case "Hunny Spout":
                    return "ETresItemDefWeapon::WEP_KEYBLADE07\u0000";
                case "Nano Gear":
                    return "ETresItemDefWeapon::WEP_KEYBLADE08\u0000";
                case "Wheel of Fate":
                    return "ETresItemDefWeapon::WEP_KEYBLADE05\u0000";
                case "Grand Chef":
                    return "ETresItemDefWeapon::WEP_KEYBLADE11\u0000";
                case "Classic Tone":
                    return "ETresItemDefWeapon::WEP_KEYBLADE10\u0000";
                case "Oathkeeper":
                    return "ETresItemDefWeapon::WEP_KEYBLADE12\u0000";
                case "Oblivion":
                    return "ETresItemDefWeapon::WEP_KEYBLADE13\u0000";
                case "Ultima Weapon":
                    return "ETresItemDefWeapon::WEP_KEYBLADE14\u0000";
                case "Starlight":                                       // Shouldn't be hit
                    return "ETresItemDefWeapon::WEP_KEYBLADE17\u0000";
                case "Elemental Encoder":                               // SHouldn't be hit
                    return "ETresItemDefWeapon::WEP_KEYBLADE18\u0000";
                default:
                    return "ETresItemDefWeapon::WEP_KEYBLADE00\u0000";
            }
        }

        private string ConvertDefenseWeaponEnumToKeybladeWeapon(string keyblade)
        {
            var value = keyblade.ValueIdToDisplay();

            switch (value)
            {
                case "Kingdom Key":
                    return "WEP_KEYBLADE_SO_00\u0000";
                case "Hero's Origin":
                    return "WEP_KEYBLADE_SO_01\u0000";
                case "Shooting Star":
                    return "WEP_KEYBLADE_SO_02\u0000";
                case "Favorite Deputy":
                    return "WEP_KEYBLADE_SO_03\u0000";
                case "Ever After":
                    return "WEP_KEYBLADE_SO_04\u0000";
                case "Happy Gear":
                    return "WEP_KEYBLADE_SO_05\u0000";
                case "Crystal Snow":
                    return "WEP_KEYBLADE_SO_06\u0000";
                case "Hunny Spout":
                    return "WEP_KEYBLADE_SO_07\u0000";
                case "Nano Gear":
                    return "WEP_KEYBLADE_SO_08\u0000";
                case "Wheel of Fate":
                    return "WEP_KEYBLADE_SO_09\u0000";
                case "Grand Chef":
                    return "WEP_KEYBLADE_SO_011\u0000";
                case "Classic Tone":
                    return "WEP_KEYBLADE_SO_012\u0000";
                case "Oathkeeper":
                    return "WEP_KEYBLADE_SO_013\u0000";
                case "Oblivion":
                    return "WEP_KEYBLADE_SO_014\u0000";
                case "Ultima Weapon":
                    return "WEP_KEYBLADE_SO_015\u0000";
                case "Starlight":                                       // Shouldn't be hit
                    return "WEP_KEYBLADE_SO_018\u0000";
                case "Elemental Encoder":                               // SHouldn't be hit
                    return "WEP_KEYBLADE_SO_019\u0000";
                default:
                    return "WEP_KEYBLADE_SO_00\u0000";
            }
        }

        #endregion

        public void RemoveNoneFromTreasure(DataTableEnum dataTableEnum, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, Dictionary<string, Dictionary<string, bool>> availableOptions)
        {
            if (!randomizedOptions.ContainsKey(dataTableEnum))
                return;

            foreach (var treasure in randomizedOptions[dataTableEnum])
            {
                var treasureId = treasure.Key;
                var treasureName = treasure.Value["Treasure"];

                if (treasureName.Contains("NONE"))
                {
                    while (true) // Is there a way we can use a var instead of true?
                    {
                        var tempOptions = randomizedOptions.Where(x => x.Key == DataTableEnum.EquipItem || x.Key == DataTableEnum.VBonus);

                        var swapDataTable = tempOptions.ElementAt(rng.Next(0, tempOptions.Count()));
                        var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));
                        
                        if (swapCategory.Key == "EVENT_KEYBLADE_012" || swapCategory.Key == "EVENT_KEYBLADE_013" || !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapCategory.Key.CategoryToKey(swapDataTable.Key)])
                            continue;

                        // Make sure this swap is valid before swapping
                        if (!IsSwapValid(treasureName, swapDataTable, swapCategory))
                        {
                            continue;
                        }

                        if (swapCategory.Value.Where(x => !x.Value.Contains("NONE")).Count() > 0)
                        {
                            var swapData = swapCategory.Value.Where(x => !x.Value.Contains("NONE")).ElementAt(rng.Next(0, swapCategory.Value.Where(x => !x.Value.Contains("NONE")).Count()));

                            if (swapCategory.Key == "m_PlayerSora" && !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapData.Key.CategoryToKey(swapDataTable.Key)])
                                continue;

                            randomizedOptions[dataTableEnum][treasureId]["Treasure"] = swapData.Value;
                            randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = treasureName;

                            break;
                        }
                    }
                }
            }
        }

        public void RemoveNoneFromEvents(DataTableEnum dataTableEnum, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, Dictionary<string, Dictionary<string, bool>> availableOptions)
        {
            if (!randomizedOptions.ContainsKey(dataTableEnum))
                return;

            foreach (var tempEvent in randomizedOptions[dataTableEnum])
            {
                var tempEventId = tempEvent.Key;

                var key = "RandomizedItem";
                if (tempEvent.Value.ContainsKey("Reward"))
                    key = "Reward";

                var tempEventName = tempEvent.Value[key];

                if (tempEventId == "EVENT_KEYBLADE_012" || tempEventId == "EVENT_KEYBLADE_013")
                {
                    if (!tempEventName.Contains("NONE"))
                    {
                        while (true) // Is there a way we can use a var instead of true?
                        {
                            var tempOptions = randomizedOptions.Where(x => x.Key == DataTableEnum.EquipItem || x.Key == DataTableEnum.VBonus);

                            var swapDataTable = tempOptions.ElementAt(rng.Next(0, tempOptions.Count()));
                            var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));

                            if (!availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapCategory.Key.CategoryToKey(swapDataTable.Key)])
                                continue;

                            if (swapCategory.Value.Where(x => x.Value.Contains("NONE")).Count() > 0)
                            {
                                var swapData = swapCategory.Value.Where(x => x.Value.Contains("NONE")).ElementAt(rng.Next(0, swapCategory.Value.Where(x => x.Value.Contains("NONE")).Count()));

                                if (swapCategory.Key == "m_PlayerSora" && !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapData.Key.CategoryToKey(swapDataTable.Key)])
                                    continue;

                                // Make sure this swap is valid before swapping
                                if (!IsSwapValid(tempEventName, swapDataTable, swapCategory))
                                {
                                    continue;
                                }

                                randomizedOptions[dataTableEnum][tempEventId][key] = swapData.Value;
                                randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = tempEventName;

                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (tempEventName.Contains("NONE"))
                    {
                        while (true) // Is there a way we can use a var instead of true?
                        {
                            var tempOptions = randomizedOptions.Where(x => x.Key == DataTableEnum.EquipItem || x.Key == DataTableEnum.VBonus);

                            var swapDataTable = tempOptions.ElementAt(rng.Next(0, tempOptions.Count()));
                            var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));

                            if (swapCategory.Key == "EVENT_KEYBLADE_012" || swapCategory.Key == "EVENT_KEYBLADE_013" || !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapCategory.Key.CategoryToKey(swapDataTable.Key)])
                                continue;

                            if (swapCategory.Value.Where(x => !x.Value.Contains("NONE")).Count() > 0)
                            {
                                var swapData = swapCategory.Value.Where(x => !x.Value.Contains("NONE")).ElementAt(rng.Next(0, swapCategory.Value.Where(x => !x.Value.Contains("NONE")).Count()));

                                if (swapCategory.Key == "m_PlayerSora" && !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapData.Key.CategoryToKey(swapDataTable.Key)])
                                    continue;

                                randomizedOptions[dataTableEnum][tempEventId][key] = swapData.Value;
                                randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = tempEventName;

                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method that cleans up VBonuses. As of now, this takes all of the checks that are
        /// put onto VBonuses, removes them, and then reassigns them evenly throughout the
        /// VBonuses. This ensures that you don't encounter empty VBonuses and that these
        /// checks are spread evenly (at least as much so as the amount of checks allows).
        /// </summary>
        public void CleanVBonuses(DataTableEnum dataTableEnum, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, Dictionary<string, Dictionary<string, bool>> availableOptions, Dictionary<DataTableEnum, Dictionary<string, bool>> replacements)
        {
            if (!randomizedOptions.ContainsKey(dataTableEnum))
                return;

            // First, empty all VBonuses to get their options in a list
            var storedOptions = EmptyAvailableVBonuses(dataTableEnum, ref randomizedOptions, availableOptions, replacements);

            // Create a Dictionary of the VBonuses that are being used
            var availableVBonuses = GetAvailableVBonuses(ref randomizedOptions, availableOptions, replacements);

            // Copy the Dictionary we made previously
            var pendingVBonuses = new Dictionary<string, Dictionary<string, string>>(availableVBonuses);

            // Using the copy, spread elements from the list of removed options evenly between VBonuses
            // Once a VBonus has been given an option, it is removed from the pendingVBonuses
            // Once pendingVBonuses is empty, it copies the usableVBonuses again and continues
            // This process goes on until each of the storedOptions has been placed on a VBonus
            while (storedOptions.Count > 0)
            {
                // Get our currentOption (the first element of the list) and the VBonus it will be added to (determined by seed)
                var currentOption = storedOptions.First();
                var currentVBonus = pendingVBonuses.ElementAt(rng.Next(0, pendingVBonuses.Count()));

                // Get the slot of the VBonus to place the option into
                // Bonus 1 > Ability 1 > Bonus 2 > Ability 2
                var currentBonusOption = new KeyValuePair<string, string>();

                if (currentOption.Contains("Ability") && currentVBonus.Value.ContainsKey("Sora_Ability1") && currentVBonus.Value["Sora_Ability1"].Contains("NONE"))
                {
                    currentBonusOption = new KeyValuePair<string, string>("Sora_Ability1", currentVBonus.Value["Sora_Ability1"]);
                }

                else if (currentOption.Contains("Ability") && currentVBonus.Value.ContainsKey("Sora_Ability2") && currentVBonus.Value["Sora_Ability2"].Contains("NONE"))
                {
                    currentBonusOption = new KeyValuePair<string, string>("Sora_Ability2", currentVBonus.Value["Sora_Ability2"]);
                }

                else
                {
                    foreach (var bonusOption in currentVBonus.Value)
                    {
                        if (bonusOption.Value.Contains("NONE"))
                        {
                            currentBonusOption = bonusOption;
                            break;
                        }
                    }
                }

                // Add the currentOption to the VBonus
                randomizedOptions[dataTableEnum][currentVBonus.Key][currentBonusOption.Key] = currentOption;

                // Remove this VBonus from our pending Dictionary and remove the option that has been used
                pendingVBonuses.Remove(currentVBonus.Key);
                storedOptions.RemoveAt(0);

                // Every VBonus has gotten an option. Copy the usableVBonuses again and continue
                if (pendingVBonuses.Count <= 0)
                {
                    pendingVBonuses = new Dictionary<string, Dictionary<string, string>>(availableVBonuses);
                }
            }
        }

        /// <summary>
        /// Method that removes all checks from VBonuses to make them all empty. 
        /// This also returns a list of all of the checks that were removed from VBonuses, 
        /// which can be used later for reassignment.
        /// </summary>
        /// <returns>List of strings for all of the checks that were removed from VBonuses</returns>
        public List<string> EmptyAvailableVBonuses(DataTableEnum dataTableEnum, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, Dictionary<string, Dictionary<string, bool>> availableOptions, Dictionary<DataTableEnum, Dictionary<string, bool>> replacements)
        {
            var removedChecks = new List<string>();
            var availableVBonuses = GetAvailableVBonuses(ref randomizedOptions, availableOptions, replacements);

            foreach (var bonus in availableVBonuses)
            {
                var bonusId = bonus.Key;
                var bonusValues = bonus.Value;

                foreach (var check in bonusValues)
                {
                    if (!check.Value.Contains("NONE"))
                    {
                        // Save the removed bonus to be reassigned later
                        string removedCheck = check.Value;
                        removedChecks.Add(removedCheck);

                        // Change this check to be none
                        if (check.Key.Contains("Ability"))
                        {
                            randomizedOptions[dataTableEnum][bonus.Key][check.Key] = "ETresAbilityKind::NONE\u0000";
                        }
                        else if (check.Key.Contains("Bonus"))
                        {
                            randomizedOptions[dataTableEnum][bonus.Key][check.Key] = "ETresVictoryBonusKind::NONE\u0000";
                        }
                    }
                }
            }

            return removedChecks;
        }

        /// <summary>
        /// Method that gets the available VBonuses based on what availableOptions we have.
        /// This might not be necessary with other methods provided (GetAvailableBonuses), but it works for now.
        /// Reevaluate this later and see if this can be done in a better way.
        /// </summary>
        /// <returns>Dictionary of all the VBonuses for the given availableOptions</returns>
        public Dictionary<string, Dictionary<string, string>> GetAvailableVBonuses(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, Dictionary<string, Dictionary<string, bool>> availableOptions, Dictionary<DataTableEnum, Dictionary<string, bool>> replacements)
        {
            List<string> minigameBonusKeys = new List<string>() { "VBonus_Minigame001", "VBonus_Minigame002", "VBonus_Minigame003", "VBonus_Minigame004", "VBonus_Minigame005", "VBonus_Minigame006" };
            List<string> flanBonusKeys = new List<string>() { "VBonus_Minigame007", "VBonus_Minigame008", "VBonus_Minigame009", "VBonus_Minigame010", "VBonus_Minigame011", "VBonus_Minigame012", "VBonus_Minigame013" };


            Dictionary<string, Dictionary<string, string>> availableVBonuses = new Dictionary<string, Dictionary<string, string>>();
            foreach (var bonus in randomizedOptions[DataTableEnum.VBonus])
            {
                if (replacements[DataTableEnum.VBonus].ContainsKey(bonus.Key.CategoryToKey(DataTableEnum.VBonus)) && replacements[DataTableEnum.VBonus][bonus.Key.CategoryToKey(DataTableEnum.VBonus)])
                {
                    continue;
                }

                if ((availableOptions["Bonuses"]["Minigames"] && minigameBonusKeys.Contains(bonus.Key)) ||
                    (availableOptions["Bonuses"]["Flantastic Seven"] && flanBonusKeys.Contains(bonus.Key)) ||
                    (availableOptions["Bonuses"]["VBonus"] && !minigameBonusKeys.Contains(bonus.Key) && !flanBonusKeys.Contains(bonus.Key) && bonus.Value.Count > 0)
                    )
                {
                    availableVBonuses.Add(bonus.Key, bonus.Value);
                }
            }

            return availableVBonuses;
        }

        /// <summary>
        /// Method that updates all checks for a DataTableEnum with none.
        /// </summary>
        public void ReplaceChecks(DataTableEnum dataTableEnum, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, Dictionary<string, Dictionary<string, bool>> availableOptions, Dictionary<DataTableEnum, Dictionary<string, bool>> replacements)
        {
            if (!randomizedOptions.ContainsKey(dataTableEnum))
                return;

            foreach (var check in randomizedOptions[dataTableEnum])
            {
                ReplaceCheck(ref randomizedOptions, availableOptions, dataTableEnum, check, replacements);
            }
        }

        /// <summary>
        /// Method that updates all checks for a DataTableEnum's subCategory with none.
        /// </summary>
        public void ReplaceChecks(DataTableEnum dataTableEnum, string subCategory, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, Dictionary<string, Dictionary<string, bool>> availableOptions, Dictionary<DataTableEnum, Dictionary<string, bool>> replacements)
        {
            if (!randomizedOptions.ContainsKey(dataTableEnum))
                return;

            foreach (var check in randomizedOptions[dataTableEnum])
            {
                if (check.Key.CategoryToKey(dataTableEnum) == subCategory)
                {
                    ReplaceCheck(ref randomizedOptions, availableOptions, dataTableEnum, check, replacements);
                }
            }
        }

        /// <summary>
        /// Method that updates a check with none.
        /// </summary>
        public void ReplaceCheck(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, Dictionary<string, Dictionary<string, bool>> availableOptions, DataTableEnum dataTableEnum, KeyValuePair<string, Dictionary<string, string>> check, Dictionary<DataTableEnum, Dictionary<string, bool>> replacements)
        {
            if (!availableOptions.ContainsKey(dataTableEnum.DataTableEnumToKey()))
            {
                return;
            }
            foreach (var bonus in check.Value)
            {
                if (!bonus.Value.Contains("NONE"))
                {
                    UpdateRandomizedItemWithNone(ref randomizedOptions, ref availableOptions, dataTableEnum, check.Key, bonus.Key, bonus.Value, replacements);
                }
            }
        }

        /// <summary>
        /// Method that uses the above method to replace a check given a DataTableEnum and a string to look for
        /// </summary>
        public void ReplaceCheck(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, Dictionary<string, Dictionary<string, bool>> availableOptions, Dictionary<DataTableEnum, Dictionary<string, bool>> replacements, DataTableEnum dataTableEnum, string key)
        {
            if (!availableOptions.ContainsKey(dataTableEnum.DataTableEnumToKey()))
            {
                return;
            }
            var checkToReplace = randomizedOptions[dataTableEnum].FirstOrDefault(check => check.Key.Contains(key));
            if (checkToReplace.Key != null)
            {
                ReplaceCheck(ref randomizedOptions, availableOptions, dataTableEnum, checkToReplace, replacements);
                blockedChecks.Add(checkToReplace.Key);
            }
        }

        /// <summary>
        /// Method that checks if a swap is valid
        /// This should only allow abilities on Starting Stats (Abilities & Critical Abilities), Equippables, Fullcourse Abilities, or Weapon Upgrades
        /// This should also only allow items on Synthesis Items or Lucky Emblems
        /// This check is potentially not as robust as it needs to be and it could be addressed later if that's the case
        /// </summary>
        /// <returns>bool of if this swap is valid</returns>
        public bool IsSwapValid(string itemToChange, KeyValuePair<DataTableEnum, Dictionary<string, Dictionary<string, string>>> swapDataTable, KeyValuePair<string, Dictionary<string, string>> swapCategory)
        {
            // Only allow abilities on Starting Stats (Abilities & Critical Abilities), Equippables, Fullcourse Abilities, or Weapon Upgrades
            if (!itemToChange.Contains("ETresAbilityKind::") && ((swapDataTable.Key == DataTableEnum.ChrInit && swapCategory.Key.CategoryToKey(swapDataTable.Key) != "Weapon") || swapDataTable.Key == DataTableEnum.EquipItem || swapDataTable.Key == DataTableEnum.FullcourseAbility || swapDataTable.Key == DataTableEnum.WeaponEnhance))
            {
                return false;
            }

            // Only allow items on Synthesis Items or Lucky Emblems
            else if (itemToChange.Contains("ETresAbilityKind::") && (swapDataTable.Key == DataTableEnum.SynthesisItem || swapDataTable.Key == DataTableEnum.LuckyMark))
            {
                return false;
            }

            return true;
        }

    }
}