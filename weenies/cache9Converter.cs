using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Melt
{
    //public static class BadWords
    //{
    //    public static List<string> BadWordsList;
    //    public static void ReadFile()
    //    {
    //        StreamReader reader = new StreamReader(new FileStream("./TabooTable.json", FileMode.Open, FileAccess.Read));
    //        dynamic jsonData = JsonConvert.DeserializeObject(reader.ReadToEnd());
    //        BadWordsList = jsonData.words.ToObject<List<string>>();
    //    }
    //}

public class cCache9Converter
    {
        ConcurrentDictionary<int, cWeenie> weenies;
        cClothingTableManager clothingTableManager = null;
        Dictionary<int, int> creatureReplacementSpellMap;

        byte[] buffer = new byte[4096];

        public cWeenie getWeenie(int wcid)
        {
            cWeenie weenie;
            if(weenies.TryGetValue(wcid, out weenie))
                return weenie;
            return null;
        }


        public string compileEnumValue(int value, Type enumType)
        {
            if (!Enum.IsDefined(enumType, value))
            {
                Console.WriteLine("Unknown {0} value: {1}", enumType.Name, value);
                return $"{value} (Unknown)";
            }
            else
                return $"{value} ({Enum.ToObject(enumType, value)})";
        }

        public cCache9Converter()
        {
        }

        public void writeRawFromExtendedJson(string path)
        {
            loadExtendedWeenies(path);

            Console.WriteLine("Writing \"0009.raw\"");
            Stopwatch timer = new Stopwatch();
            timer.Start();
            StreamWriter outputFile = new StreamWriter(new FileStream("./intermediate/0009.raw", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open ./intermediate/0009.raw");
                return;
            }

            Utils.writeInt32(weenies.Count, outputFile);
            int weenieCount = 0;
            foreach (KeyValuePair<int, cWeenie> entry in weenies)
            {
                entry.Value.writeRaw(outputFile);
                outputFile.Flush();
                weenieCount++;
            }

            outputFile.Close();

            timer.Stop();
            Console.WriteLine("{0} weenies written in {1} seconds.", weenieCount, timer.ElapsedMilliseconds / 1000f);
        }

        void writeRaw()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Console.WriteLine("Writing \"0009.raw\"");
            StreamWriter outputFile = new StreamWriter(new FileStream("./intermediate/0009.raw", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open ./intermediate/0009.raw");
                return;
            }

            int weenieCount = 0;
            Utils.writeInt32(weenies.Count, outputFile);

            //do we need entries to be sorted?
            SortedDictionary<int, cWeenie> sortedWeenies = new SortedDictionary<int, cWeenie>(weenies);

            foreach (KeyValuePair<int, cWeenie> entry in sortedWeenies)
            {
                entry.Value.writeRaw(outputFile);
                outputFile.Flush();
                weenieCount++;
            }

            outputFile.Close();
            timer.Stop();
            Console.WriteLine("{0} weenies written in {1} seconds.", weenieCount, timer.ElapsedMilliseconds / 1000f);
        }

        public void writeExtendedJson(string filename, string outputPath, bool singleFile, eWeenieTypes specificType = eWeenieTypes.Undef)
        {
            loadWeeniesRaw(filename);
            writeJson(outputPath, singleFile, specificType, true);
        }

        void writeRaw(string filename)
        {
            loadWeeniesRaw(filename);
            writeRaw();
        }

        public void writeJson(string filename, string outputPath, bool singleFile, eWeenieTypes specificType = eWeenieTypes.Undef)
        {
            loadWeeniesRaw(filename);
            writeJson(outputPath, singleFile, specificType);
        }

        void loadExtendedWeenies(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Unable to open {0}", path);
                return;
            }

            string[] fileEntries = Directory.GetFiles(path,"*.json", SearchOption.AllDirectories);

            Console.WriteLine("Reading weenies from extended json files...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            weenies = new ConcurrentDictionary<int, cWeenie>();
            foreach (string fileName in fileEntries)
            {
                StreamReader reader = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read));
                if (reader == null)
                {
                    Console.WriteLine("Unable to open {0}", fileName);
                    continue;
                }
                string jsonData = reader.ReadToEnd();

                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.Auto;
                settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                //settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

                cWeenie weenie = JsonConvert.DeserializeObject<cWeenie>(jsonData, settings);
                weenies.TryAdd(weenie.wcid, weenie);
            }
            timer.Stop();
            Console.WriteLine("{0} weenies read in {1} seconds.", weenies.Count, timer.ElapsedMilliseconds / 1000f);
        }

        public void loadWeeniesRaw(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return;
            }

            Console.WriteLine("Reading weenies from 0009.raw...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            int totalWeenies = Utils.ReadInt32(buffer, inputFile);
            weenies = new ConcurrentDictionary<int, cWeenie>();

            int weenieCount;
            for (weenieCount = 0; weenieCount < totalWeenies; weenieCount++)
            //for (weenieCount = 0; weenieCount < 1000; weenieCount++)
            {
                cWeenie weenie = new cWeenie(buffer, inputFile);
                weenies.TryAdd(weenie.wcid, weenie);
            }

            inputFile.Close();
            timer.Stop();
            Console.WriteLine("{0} weenies read in {1} seconds.", weenieCount, timer.ElapsedMilliseconds / 1000f);
        }

        void generateMutableArmorList()
        {
            Dictionary<string, cWeenie> mutableEntries = new Dictionary<string, cWeenie>();
            List<string> repeatEntries = new List<string>();

            foreach (KeyValuePair<int, cWeenie> entry in weenies)
            {
                cWeenie weenie = entry.Value;


                if (weenie.getStat(eIntStat.TsysMutationData) != 0 && weenie.weenieType == eWeenieTypes.Clothing && weenie.getStat(eIntStat.ItemType) == (int)eItemType.Type_Armor)
                {
                    if (weenie.weenieName.Contains("Olthoi") ||
                        weenie.weenieName.Contains("Antius") ||
                        weenie.weenieName.Contains("Template"))
                        continue;

                    if (!mutableEntries.ContainsKey(weenie.getStat(eStringStat.Name)))
                        mutableEntries.Add(weenie.getStat(eStringStat.Name), weenie);
                    else
                    {
                        if(!WeenieClassNames.getWeenieClassName(weenie.wcid).Contains("new"))
                            repeatEntries.Add($"{weenie.getStat(eStringStat.Name)} - {weenie.wcid}");
                        else //replace what we have for the "new" version
                            mutableEntries[weenie.getStat(eStringStat.Name)] = weenie;
                    }
                }
            }

            List<sItemLootProfileEntry> allPieces = new List<sItemLootProfileEntry>();
            List<sLootProfileCategory> armorSets = new List<sLootProfileCategory>();
            foreach (KeyValuePair<string, cWeenie> entry in mutableEntries)
            {
                string name = entry.Key;

                sItemLootProfileEntry newArmor = new sItemLootProfileEntry();
                newArmor.name = name;
                newArmor.wcid = entry.Value.wcid;

                if (((eInventoryLocations)entry.Value.getStat(eIntStat.ValidLocations)).HasFlag(eInventoryLocations.Upper_Leg_Armor_Loc | eInventoryLocations.Lower_Leg_Armor_Loc))
                //if (((eInventoryLocations)entry.Value.getStat(eIntStat.ValidLocations)).HasFlag(eInventoryLocations.Chest_Armor_Loc))
                {
                    sLootProfileCategory newArmorSet = new sLootProfileCategory();
                    //newArmorSet.name = newArmor.name.Replace(" Breastplate", "");
                    //newArmorSet.name = newArmorSet.name.Replace(" Vest", "");
                    newArmorSet.category = newArmor.name.Replace(" Leggings", "");
                    newArmorSet.entries = new List<sItemLootProfileEntry>();
                    
                    armorSets.Add(newArmorSet);
                }
                allPieces.Add(newArmor);
            }
            //sLootProfileCategory alduressaArmorSet = new sLootProfileCategory();
            //alduressaArmorSet.name = "Alduressa"; //manually add Alduressa as there are many "coats" that shouldn't be added but Alduressa only has coats
            //armorSets.Add(alduressaArmorSet);

            foreach (sItemLootProfileEntry entry in allPieces)
            {
                foreach(sLootProfileCategory armorSet in armorSets)
                {
                    if (entry.name.Contains("Studded Leather"))
                    {
                        if (armorSet.category.Contains("Studded Leather"))
                        {
                            armorSet.entries.Add(entry);
                        }
                    }
                    else if (entry.name.Contains(armorSet.category))
                    {
                        if (entry.name.Contains("Leather"))
                        {
                            if (entry.name.Contains("Coat") || entry.name.Contains("Breastplate") || entry.name.Contains("Basinet"))
                                continue; //there were renamed when converted to the "new" versions
                        }
                        if (armorSet.category == "Platemail" && entry.name.Contains("Diforsa"))//"Platemail Diforsa" is not platemail
                            continue;
                        armorSet.entries.Add(entry);
                        break;
                    }
                }
            }

            string jsonString = JsonConvert.SerializeObject(armorSets, Formatting.Indented);

            StreamWriter outputFile = new StreamWriter(new FileStream("./output/listOfMutableItems.json", FileMode.Create, FileAccess.Write));
            outputFile.WriteLine(jsonString);
            outputFile.WriteLine("");
            outputFile.WriteLine("---Repeated Entries---");
            foreach (string entry in repeatEntries)
            {
                outputFile.WriteLine(entry);
            }
            outputFile.Close();
        }

        class cCreatureAnalysisInfo
        {
            public int totalItems;
            public Single totalValue;
            public int highestValue = 0;
            public int lowestValue = int.MaxValue;
            public int lowestValueId;
            public int highestValueId;
            public SortedDictionary<int, int> valueBuckets = new SortedDictionary<int, int>();

            public cCreatureAnalysisInfo()
            {
                for (int i = 0; i < 1000; i += 10)
                {
                    valueBuckets.Add(i, 0);
                }
                valueBuckets.Add(int.MaxValue, 0);
            }

            public void addEntry(int value, int id)
            {
                totalItems++;
                totalValue += value;
                if (value > highestValue)
                {
                    highestValue = value;
                    highestValueId = id;
                }
                if (value < lowestValue)
                {
                    lowestValue = value;
                    lowestValueId = id;
                }

                for (int i = 1; i < valueBuckets.Count; i++)
                {
                    int bucketMaxValue = new List<int>(valueBuckets.Keys)[i];
                    if (value > bucketMaxValue)
                        continue;
                    else
                    {
                        valueBuckets[bucketMaxValue]++;
                        break;
                    }
                }
            }
        }

        void generateCreatureSpellsList()
        {
            List<int> spells = new List<int>();
            foreach (KeyValuePair<int, cWeenie> entry in weenies)
            {
                cWeenie weenie = entry.Value;

                if (!weenie.dataFlags.HasFlag(eDataFlags.spellBook))
                    continue;

                foreach(sSpellData spell in weenie.spellBook.spellData)
                {
                    if (!spells.Contains(spell.key))
                        spells.Add(spell.key);
                }
            }

            StreamWriter outputFile = new StreamWriter(new FileStream("./output/creatureSpellList.txt", FileMode.Create, FileAccess.Write));

            foreach (int spellId in spells)
            {
                outputFile.WriteLine(spellId);
                //outputFile.WriteLine($"{spellId} - {SpellInfo.getSpellName(spellId)}");
                outputFile.Flush();
            }

            outputFile.Close();
        }

        void generateMeleeAttackInfoList()
        {
            cCreatureAnalysisInfo info = new cCreatureAnalysisInfo();

            foreach (KeyValuePair<int, cWeenie> entry in weenies)
            {
                cWeenie weenie = entry.Value;

                if (weenie.weenieType != eWeenieTypes.Creature)
                    continue;

                if (!weenie.dataFlags.HasFlag(eDataFlags.skills))
                    continue;

                if (weenie.hasStat(eBoolStat.Attackable) && weenie.getStat(eBoolStat.Attackable) == 0)
                    continue;

                int level = weenie.getStat(eIntStat.Level);
                //if (level < 30) //t1
                //if (level > 30 && level < 60) //t2
                {
                    foreach (sSkill skill in weenie.skills.skills)
                    {
                        switch (skill.key)
                        {
                            case eSkills.Axe:
                            case eSkills.Dagger:
                            case eSkills.Mace:
                            case eSkills.Spear:
                            case eSkills.Sword:
                            case eSkills.Staff:
                            case eSkills.UnarmedCombat:
                            case eSkills.HeavyWeapons:
                            case eSkills.LightWeapons:
                            case eSkills.FinesseWeapons:
                                {
                                    if (skill.init_level > 0)
                                        info.addEntry(skill.init_level, weenie.wcid);
                                    break;
                                }
                        }
                    }
                }
            }

            StreamWriter outputFile = new StreamWriter(new FileStream("./output/meleeAttackInfoList.txt", FileMode.Create, FileAccess.Write));
            outputFile.WriteLine($"Lowest Value: {info.lowestValue} ({info.lowestValueId})");
            outputFile.WriteLine($"Highest Value: {info.highestValue} ({info.highestValueId})");
            outputFile.WriteLine($"Average Value: {info.totalValue / info.totalItems}");
            outputFile.WriteLine();
            outputFile.WriteLine("Values:");
            foreach (KeyValuePair<int, int> entry in info.valueBuckets)
            {
                if (entry.Value > 0)
                    outputFile.WriteLine($"Under {entry.Key}: {entry.Value}({entry.Value * 100f / info.totalItems}%)");
            }
            outputFile.Flush();
            outputFile.Close();
        }

        void generateTreasureList()
        {
            SortedDictionary<int, List<cWeenie>> deathTreasureMap = new SortedDictionary<int, List<cWeenie>>();
            SortedDictionary<int, List<cWeenie>> wieldedTreasureMap = new SortedDictionary<int, List<cWeenie>>();

            foreach(KeyValuePair<int, cWeenie> entry in weenies)
            {
                cWeenie weenie = entry.Value;

                sDidStat valueStat;
                if (weenie.didStats.TryGetValue(eDidStat.DeathTreasureType, out valueStat))
                {
                    int value = valueStat.value;
                    List<cWeenie> list;
                    if (!deathTreasureMap.TryGetValue(value, out list))
                        list = new List<cWeenie>();
                    list.Add(weenie);
                    deathTreasureMap[value] = list;
                }

                if (weenie.didStats.TryGetValue(eDidStat.WieldedTreasureType, out valueStat))
                {
                    int value = valueStat.value;
                    List<cWeenie> list;
                    if (!wieldedTreasureMap.TryGetValue(value, out list))
                        list = new List<cWeenie>();
                    list.Add(weenie);
                    wieldedTreasureMap[value] = list;
                }
            }

            StreamWriter outputFile = new StreamWriter(new FileStream("./output/deathTreasureList.txt", FileMode.Create, FileAccess.Write));
            foreach (KeyValuePair<int, List<cWeenie>> entry in deathTreasureMap)
            {
                outputFile.WriteLine($"---{entry.Key} {(eTreasureGeneratorType)entry.Key}---");

                SortedDictionary<int, int> levelStats = new SortedDictionary<int, int>();
                foreach (cWeenie listEntry in entry.Value)
                {
                    int level = listEntry.getStat(eIntStat.Level);

                    int currentCount;
                    if (!levelStats.TryGetValue(level, out currentCount))
                        levelStats.Add(level, 1);
                    else
                        levelStats[level]++;
                }

                foreach (KeyValuePair<int, int> levelEntry in levelStats)
                {
                    outputFile.WriteLine($"level: {levelEntry.Key} - count: {levelEntry.Value}");
                }
                outputFile.WriteLine("---");

                foreach (cWeenie listEntry in entry.Value)
                {
                    outputFile.WriteLine($"{listEntry.getStat(eStringStat.Name)}({listEntry.wcid}) - level: {listEntry.getStat(eIntStat.Level)}");
                }
                outputFile.WriteLine("");
                outputFile.WriteLine("");
                outputFile.WriteLine("");
                outputFile.Flush();
            }
            outputFile.Close();

            outputFile = new StreamWriter(new FileStream("./output/wieldedTreasureList.txt", FileMode.Create, FileAccess.Write));
            foreach (KeyValuePair<int, List<cWeenie>> entry in wieldedTreasureMap)
            {
                outputFile.WriteLine($"---{entry.Key} {(eTreasureGeneratorType)entry.Key}---");

                SortedDictionary<int, int> levelStats = new SortedDictionary<int, int>();
                foreach (cWeenie listEntry in entry.Value)
                {
                    int level = listEntry.getStat(eIntStat.Level);

                    int currentCount;
                    if (!levelStats.TryGetValue(level, out currentCount))
                        levelStats.Add(level, 1);
                    else
                        levelStats[level]++;
                }

                foreach (KeyValuePair<int, int> levelEntry in levelStats)
                {
                    outputFile.WriteLine($"level: {levelEntry.Key} - count: {levelEntry.Value}");
                }
                outputFile.WriteLine("---");


                foreach (cWeenie listEntry in entry.Value)
                {
                    outputFile.WriteLine($"{listEntry.getStat(eStringStat.Name)}({listEntry.wcid}) - level: {listEntry.getStat(eIntStat.Level)}");
                }
                outputFile.WriteLine("");
                outputFile.WriteLine("");
                outputFile.WriteLine("");
                outputFile.Flush();
            }
            outputFile.Close();
        }

        void generateChestTreasureList()
        {
            SortedDictionary<int, List<cWeenie>> generatorTableTreasureMap = new SortedDictionary<int, List<cWeenie>>();
            SortedDictionary<int, List<cWeenie>> generatorTableMap = new SortedDictionary<int, List<cWeenie>>();

            foreach (KeyValuePair<int, cWeenie> entry in weenies)
            {
                cWeenie weenie = entry.Value;

                if (weenie.weenieType != eWeenieTypes.Chest)
                    continue;

                if (weenie.generatorTable.entries != null && weenie.generatorTable.entries.Count > 0)
                {
                    foreach(sGeneratorTableEntry genTableEntry in weenie.generatorTable.entries)
                    {
                        if (genTableEntry.whereCreate == eRegenLocationType.ContainTreasure_RegenLocationType)
                        {
                            List<cWeenie> list;
                            if (!generatorTableTreasureMap.TryGetValue(genTableEntry.type, out list))
                                list = new List<cWeenie>();
                            list.Add(weenie);
                            generatorTableTreasureMap[genTableEntry.type] = list;
                        }
                        else
                        {
                            List<cWeenie> list;
                            if (!generatorTableTreasureMap.TryGetValue(genTableEntry.type, out list))
                                list = new List<cWeenie>();
                            list.Add(weenie);
                            generatorTableMap[genTableEntry.type] = list;
                        }
                    }
                }
            }

            StreamWriter outputFile = new StreamWriter(new FileStream("./output/chestTreasureList.txt", FileMode.Create, FileAccess.Write));
            foreach (KeyValuePair<int, List<cWeenie>> entry in generatorTableTreasureMap)
            {
                outputFile.WriteLine($"---{entry.Key} {(eTreasureGeneratorType)entry.Key}---");

                foreach (cWeenie listEntry in entry.Value)
                {
                    outputFile.WriteLine($"{listEntry.getStat(eStringStat.Name)}({listEntry.wcid}) - class: {WeenieClassNames.getWeenieClassName(listEntry.wcid)}");
                }
                outputFile.WriteLine("");
                outputFile.WriteLine("");
                outputFile.WriteLine("");
                outputFile.Flush();
            }
            outputFile.Close();

            outputFile = new StreamWriter(new FileStream("./output/chestOtherList.txt", FileMode.Create, FileAccess.Write));
            foreach (KeyValuePair<int, List<cWeenie>> entry in generatorTableMap)
            {
                outputFile.WriteLine($"---{entry.Key} {WeenieClassNames.getWeenieClassName(entry.Key)}---");

                foreach (cWeenie listEntry in entry.Value)
                {
                    outputFile.WriteLine($"{listEntry.getStat(eStringStat.Name)}({listEntry.wcid}) - class: {WeenieClassNames.getWeenieClassName(listEntry.wcid)}");
                }
                outputFile.WriteLine("");
                outputFile.WriteLine("");
                outputFile.WriteLine("");
                outputFile.Flush();
            }
            outputFile.Close();
        }

        public sLootProfile generateRandomLoot(string file0009, string file0002, string lootProfileFilename, string outputPath, bool generateItems, bool addEntriesToCreaturesAndChests, bool writeJsonFile, bool writeRawFile)
        {
            loadWeeniesRaw(file0009);

            clothingTableManager = new cClothingTableManager();
            clothingTableManager.loadClothingTablesFromRaw("./input/clothingTables/");

            StreamReader reader = new StreamReader(new FileStream(lootProfileFilename, FileMode.Open, FileAccess.Read));
            string jsonData = reader.ReadToEnd();

            sLootProfile lootProfile = JsonConvert.DeserializeObject<sLootProfile>(jsonData, new Newtonsoft.Json.Converters.StringEnumConverter());
            lootProfile.initialize();

            creatureReplacementSpellMap = new Dictionary<int, int>();
            if (file0002 != "")
            {
                if (lootProfile.otherOptions.globalCreatureSpellDamageMultiplier != 1.0)
                    creatureReplacementSpellMap = cCache2Converter.createDuplicateWarSpellsWithMultipliedDamage(file0002, lootProfile.otherOptions.globalCreatureSpellDamageMultiplier);
                else
                    File.Copy(file0002, "./intermediate/0002.raw", true);
            }

            cItemCreator itemCreator = new cItemCreator(weenies, clothingTableManager.clothingTables);
            if(generateItems)
                itemCreator.generateLootProfile(lootProfile, writeJsonFile, outputPath);

            if (addEntriesToCreaturesAndChests)
                addLootToCreatures(itemCreator, lootProfile, writeJsonFile, writeRawFile, outputPath);

            return lootProfile;
        }

        void addLootToCreatures(cItemCreator itemCreator, sLootProfile lootProfile, bool writeJsonFile, bool writeRawFile, string outputPath)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Console.WriteLine("Adding loot entries to creatures and chests...");

            List<sLootTier> lootTiers = new List<sLootTier>();
            foreach (sLootTier tier in lootProfile.tiers)
            {
                sLootTier newTier = tier.Copy();
                newTier.lootChance = tier.lootChance * lootProfile.otherOptions.globalLootChanceMultiplier;
                newTier.miscLootChance = tier.miscLootChance * lootProfile.otherOptions.globalMiscLootChanceMultiplier;
                newTier.scrollLootChance = tier.scrollLootChance * lootProfile.otherOptions.globalScrollLootChanceMultiplier;
                newTier.chestLootAmountMultiplier = tier.chestLootAmountMultiplier * lootProfile.otherOptions.globalChestLootAmountMultiplier;

                newTier.regularLootFirstId = tier.firstId;
                newTier.regularLootLastId = tier.firstId + tier.amount - 1;

                lootTiers.Add(newTier);
            }
            foreach (sLootTier complementaryTier in lootProfile.complementaryTiers)
            {
                foreach (sLootTier tier in lootProfile.tiers)
                {
                    if (complementaryTier.template == tier.name)
                    {
                        sLootTier newTier = tier.Copy();
                        newTier.name = complementaryTier.name;
                        newTier.treasureGeneratorTypes = complementaryTier.treasureGeneratorTypes;
                        newTier.lootChance = complementaryTier.lootChance * lootProfile.otherOptions.globalComplementaryTierLootChanceMultiplier;
                        lootTiers.Add(newTier);
                        break;
                    }
                }
            }

            ConcurrentDictionary<sLootTier, int> lootAddedCounter = new ConcurrentDictionary<sLootTier, int>();
            ConcurrentDictionary<sLootTier, int> chestLootAddedCounter = new ConcurrentDictionary<sLootTier, int>();

            foreach (sLootTier tier in lootTiers)
            {
                lootAddedCounter.TryAdd(tier, 0);
                chestLootAddedCounter.TryAdd(tier, 0);
            }

            ParallelOptions parallelOptions = new ParallelOptions();
            //parallelOptions.MaxDegreeOfParallelism = 1; //debug

            Parallel.ForEach(weenies, parallelOptions, currentElement =>
            //foreach (KeyValuePair<int, cWeenie> currentElement in weenies)
            {
                cWeenie weenie = currentElement.Value;

                if(lootProfile.otherOptions.globalCreatureSpellDamageMultiplier != 1.0)
                {
                    if(weenie.dataFlags.HasFlag(eDataFlags.spellBook))
                    {
                        List<sSpellData> newSpells = new List<sSpellData>();
                        foreach(sSpellData spell in weenie.spellBook.spellData)
                        {
                            sSpellData newSpell = spell.Copy();
                            if(creatureReplacementSpellMap.ContainsKey(spell.key))
                                newSpell.key = creatureReplacementSpellMap[spell.key];
                            newSpells.Add(newSpell);
                        }
                        weenie.spellBook.spellData = newSpells;
                    }

                    if (writeJsonFile)
                        writeJson(weenie, outputPath);
                }

                if (weenie.weenieType == eWeenieTypes.Creature)
                {
                    int type = weenie.getStat(eDidStat.DeathTreasureType);

                    foreach (sLootTier tier in lootTiers)
                    {
                        if (tier.treasureGeneratorTypes.Contains(type))
                        {
                            itemCreator.addLootToCreateList(weenie, lootProfile, tier);

                            if (tier.creatureDifficulty_Multiplier != 1.0 ||
                                tier.creatureDifficulty_Multiplier_MeleeAttack != 1.0 ||
                                tier.creatureDifficulty_Multiplier_MissileAttack != 1.0 ||
                                tier.creatureDifficulty_Multiplier_MagicAttack != 1.0 ||
                                tier.creatureDifficulty_Multiplier_MeleeDefense != 1.0 ||
                                tier.creatureDifficulty_Multiplier_MissileDefense != 1.0 ||
                                tier.creatureDifficulty_Multiplier_MagicDefense != 1.0 ||
                                tier.creatureXPMultiplier != 1.0)
                            {
                                if(weenie.hasStat(eIntStat.XpOverride))
                                    weenie.addOrUpdateStat(eIntStat.XpOverride, (int)Math.Round(weenie.getStat(eIntStat.XpOverride) * tier.creatureXPMultiplier));

                                if (weenie.dataFlags.HasFlag(eDataFlags.attributes))
                                {
                                    if (weenie.attributes.attributeFlags.HasFlag(eAttributeFlags.strength))
                                        weenie.attributes.strength.init_level = (int)Math.Round(weenie.attributes.strength.init_level * tier.creatureDifficulty_Multiplier);
                                    if (weenie.attributes.attributeFlags.HasFlag(eAttributeFlags.endurance))
                                        weenie.attributes.endurance.init_level = (int)Math.Round(weenie.attributes.endurance.init_level * tier.creatureDifficulty_Multiplier);
                                    if (weenie.attributes.attributeFlags.HasFlag(eAttributeFlags.quickness))
                                        weenie.attributes.quickness.init_level = (int)Math.Round(weenie.attributes.quickness.init_level * tier.creatureDifficulty_Multiplier);
                                    if (weenie.attributes.attributeFlags.HasFlag(eAttributeFlags.coordination))
                                        weenie.attributes.coordination.init_level = (int)Math.Round(weenie.attributes.coordination.init_level * tier.creatureDifficulty_Multiplier);
                                    if (weenie.attributes.attributeFlags.HasFlag(eAttributeFlags.focus))
                                        weenie.attributes.focus.init_level = (int)Math.Round(weenie.attributes.focus.init_level * tier.creatureDifficulty_Multiplier);
                                    if (weenie.attributes.attributeFlags.HasFlag(eAttributeFlags.self))
                                        weenie.attributes.self.init_level = (int)Math.Round(weenie.attributes.self.init_level * tier.creatureDifficulty_Multiplier);
                                }

                                if (weenie.dataFlags.HasFlag(eDataFlags.skills))
                                {
                                    for (int i = 0; i < weenie.skills.skills.Count; i++)
                                    {
                                        sSkill skill = weenie.skills.skills[i];
                                        if (skill.key != eSkills.Run && skill.key != eSkills.Jump)
                                        {
                                            switch (skill.key)
                                            {
                                                case eSkills.CreatureEnchantment:
                                                case eSkills.ItemEnchantment:
                                                case eSkills.LifeMagic:
                                                case eSkills.WarMagic:
                                                    {
                                                        skill.init_level = (int)Math.Round(skill.init_level * tier.creatureDifficulty_Multiplier_MagicAttack);
                                                        weenie.skills.skills[i] = skill;
                                                        break;
                                                    }
                                                case eSkills.Axe:
                                                case eSkills.Dagger:
                                                case eSkills.Mace:
                                                case eSkills.Spear:
                                                case eSkills.Sword:
                                                case eSkills.Staff:
                                                case eSkills.UnarmedCombat:
                                                case eSkills.HeavyWeapons:
                                                case eSkills.LightWeapons:
                                                case eSkills.FinesseWeapons:
                                                    {
                                                        skill.init_level = (int)Math.Round(skill.init_level * tier.creatureDifficulty_Multiplier_MeleeAttack);
                                                        weenie.skills.skills[i] = skill;
                                                        break;
                                                    }
                                                case eSkills.Bow:
                                                case eSkills.Crossbow:
                                                case eSkills.ThrownWeapon:
                                                case eSkills.MissileWeapons:
                                                    {
                                                        skill.init_level = (int)Math.Round(skill.init_level * tier.creatureDifficulty_Multiplier_MissileAttack);
                                                        weenie.skills.skills[i] = skill;
                                                        break;
                                                    }
                                                case eSkills.MeleeDefense:
                                                    {
                                                        skill.init_level = (int)Math.Round(skill.init_level * tier.creatureDifficulty_Multiplier_MeleeDefense);
                                                        weenie.skills.skills[i] = skill;
                                                        break;
                                                    }
                                                case eSkills.MissileDefense:
                                                    {
                                                        skill.init_level = (int)Math.Round(skill.init_level * tier.creatureDifficulty_Multiplier_MissileDefense);
                                                        weenie.skills.skills[i] = skill;
                                                        break;
                                                    }
                                                case eSkills.MagicDefense:
                                                    {
                                                        skill.init_level = (int)Math.Round(skill.init_level * tier.creatureDifficulty_Multiplier_MagicDefense);
                                                        weenie.skills.skills[i] = skill;
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        skill.init_level = (int)Math.Round(skill.init_level * tier.creatureDifficulty_Multiplier);
                                                        weenie.skills.skills[i] = skill;
                                                        break;
                                                    }
                                            }
                                        }
                                    }
                                }
                            }

                            if (writeJsonFile)
                                writeJson(weenie, outputPath);
                            lootAddedCounter.AddOrUpdate(tier, 1, (key, oldValue) => oldValue + 1);
                        }
                    }
                }
                else if (weenie.weenieType == eWeenieTypes.Chest || weenie.weenieType == eWeenieTypes.Container)
                {
                    if (weenie.dataFlags.HasFlag(eDataFlags.generatorTable))
                    {
                        if (lootProfile.chestTreasureTypeReplacementTable != null && lootProfile.chestTreasureTypeReplacementTable.Count > 0)
                        {
                            List<sGeneratorTableEntry> translatedTable = new List<sGeneratorTableEntry>();
                            foreach (sGeneratorTableEntry generatorTableEntry in weenie.generatorTable.entries)
                            {
                                sGeneratorTableEntry newEntry = generatorTableEntry.Copy();
                                foreach (KeyValuePair<int, int> entry in lootProfile.chestTreasureTypeReplacementTable)
                                {
                                    if (newEntry.type == entry.Key)
                                    {
                                        newEntry.type = entry.Value;
                                        break;
                                    }
                                }
                                translatedTable.Add(newEntry);
                            }
                            weenie.generatorTable.entries = translatedTable;
                        }

                        cItemCreator.sGeneratorTableEntryData data = new cItemCreator.sGeneratorTableEntryData();
                        foreach (sGeneratorTableEntry generatorTableEntry in weenie.generatorTable.entries)
                        {
                            if (generatorTableEntry.whereCreate == eRegenLocationType.ContainTreasure_RegenLocationType)
                            {
                                eTreasureGeneratorType type = (eTreasureGeneratorType)generatorTableEntry.type;

                                foreach (sLootTier tier in lootTiers)
                                {
                                    if (tier.treasureGeneratorTypes.Contains((int)type))
                                    {
                                        itemCreator.addLootToGeneratorTable(weenie, data, type, lootProfile, tier);
                                        chestLootAddedCounter.AddOrUpdate(tier, 1, (key, oldValue) => oldValue + 1);
                                    }
                                }
                            }
                        }

                        if (writeJsonFile)
                            writeJson(weenie, outputPath);
                    }
                }

                if (lootProfile.otherOptions.fixEnchantableAmmunition && weenie.weenieType == eWeenieTypes.Ammunition)
                {
                    weenie.addOrUpdateStat(eIntStat.ResistMagic, 9999);

                    if (writeJsonFile)
                        writeJson(weenie, outputPath);
                }

                if (lootProfile.otherOptions.fixStackableItemRewards && weenie.weenieType == eWeenieTypes.Creature)
                {
                    bool replacedEntries = false;
                    if (weenie.dataFlags.HasFlag(eDataFlags.emoteTable))
                    {
                        List<sEmoteTableKey> newKeys = new List<sEmoteTableKey>();
                        foreach (sEmoteTableKey emoteTableKey in weenie.emoteTable.entries)
                        {
                            sEmoteTableKey newEmoteTableKey = emoteTableKey.Copy();
                            newEmoteTableKey.entries = new List<sEmoteTableEntry>();
                            foreach (sEmoteTableEntry emoteTableEntry in emoteTableKey.entries)
                            {
                                sEmoteTableEntry newEmoteTableEntry = emoteTableEntry.Copy();
                                newEmoteTableEntry.actions = new List<iEmoteTableAction>();
                                foreach (iEmoteTableAction emoteTableAction in emoteTableEntry.actions)
                                {
                                    if (emoteTableAction.GetType() == typeof(sEmoteTableActionCProfType))
                                    {
                                        sEmoteTableActionCProfType action = (sEmoteTableActionCProfType)emoteTableAction.Copy();
                                        int amount = action.cprof.stack_size;
                                        if(action.cprof.wcid == 273) //pyreal coins
                                            amount = (int)Math.Round(amount * lootProfile.otherOptions.pyrealRewardsMultiplier);

                                        while (amount > 0)
                                        {
                                            action = (sEmoteTableActionCProfType)emoteTableAction.Copy(); //make a new copy every loop to avoid data contamination.
                                            if (action.cprof.wcid != 273)
                                            {
                                                if (amount >= 1)
                                                {
                                                    action.cprof.stack_size = 1;
                                                    action.delay = 0.1f;
                                                    replacedEntries = true;
                                                }
                                                amount--;
                                            }
                                            else //pyreal coins - convert to trade notes
                                            {
                                                if (amount >= 250000)
                                                {
                                                    action.cprof.wcid = 20630;
                                                    amount -= 250000;
                                                }
                                                else if (amount >= 200000)
                                                {
                                                    action.cprof.wcid = 20629;
                                                    amount -= 200000;
                                                }
                                                else if (amount >= 150000)
                                                {
                                                    action.cprof.wcid = 20268;
                                                    amount -= 150000;
                                                }
                                                else if (amount >= 100000)
                                                {
                                                    action.cprof.wcid = 2627;
                                                    amount -= 100000;
                                                }
                                                else if (amount >= 75000)
                                                {
                                                    action.cprof.wcid = 7377;
                                                    amount -= 75000;
                                                }
                                                else if (amount >= 50000)
                                                {
                                                    action.cprof.wcid = 2626;
                                                    amount -= 50000;
                                                }
                                                else if (amount >= 25000)
                                                {
                                                    action.cprof.wcid = 7376;
                                                    amount -= 25000;
                                                }
                                                else if (amount >= 20000)
                                                {
                                                    action.cprof.wcid = 7375;
                                                    amount -= 20000;
                                                }
                                                else if (amount >= 15000)
                                                {
                                                    action.cprof.wcid = 7374;
                                                    amount -= 15000;
                                                }
                                                else if (amount >= 10000)
                                                {
                                                    action.cprof.wcid = 2625;
                                                    amount -= 10000;
                                                }
                                                else if (amount >= 5000)
                                                {
                                                    action.cprof.wcid = 2624;
                                                    amount -= 5000;
                                                }
                                                else if (amount >= 1000)
                                                {
                                                    action.cprof.wcid = 2623;
                                                    amount -= 1000;
                                                }
                                                else if (amount >= 500)
                                                {
                                                    action.cprof.wcid = 2622;
                                                    amount -= 500;
                                                }
                                                else if (amount >= 100)
                                                {
                                                    action.cprof.wcid = 2621;
                                                    amount -= 100;
                                                }
                                                else
                                                {
                                                    action.cprof.wcid = 2621;
                                                    amount = 0;
                                                }

                                                action.cprof.stack_size = 1;
                                                action.delay = 0.1f;
                                                replacedEntries = true;
                                            }
                                            newEmoteTableEntry.actions.Add(action);
                                        }
                                    }
                                    else
                                        newEmoteTableEntry.actions.Add(emoteTableAction);
                                }
                                newEmoteTableKey.entries.Add(newEmoteTableEntry);
                            }
                            newKeys.Add(newEmoteTableKey);
                        }
                        weenie.emoteTable.entries = newKeys;
                    }
                    if (replacedEntries && writeJsonFile)
                        writeJson(weenie, outputPath);
                }

                if (lootProfile.otherOptions.fixSpawnGeneratorTimers &&  weenie.weenieType != eWeenieTypes.Chest && weenie.weenieType != eWeenieTypes.Container)
                {
                    if (weenie.dataFlags.HasFlag(eDataFlags.generatorTable))
                    {
                        int maxGeneratedObjects = weenie.getStat(eIntStat.MaxGeneratedObjects);
                        Double regenerationInterval = weenie.getStat(eFloatStat.RegenerationInterval);
                        if (regenerationInterval != 0)
                        {
                            string className = WeenieClassNames.getWeenieClassName(weenie.wcid);

                            Double shortestDelay = Double.MaxValue;
                            foreach (sGeneratorTableEntry entry in weenie.generatorTable.entries)
                            {
                                if (entry.delay != 0 && entry.delay < shortestDelay)
                                    shortestDelay = entry.delay;
                            }

                            if (shortestDelay == Double.MaxValue)
                                shortestDelay = weenie.getStat(eFloatStat.RegenerationInterval);
                            if (shortestDelay == 0)
                                shortestDelay = 60;

                            weenie.addOrUpdateStat(eFloatStat.RegenerationInterval, shortestDelay * lootProfile.otherOptions.spawnGeneratorTimerMultiplier);
                        }

                        if (writeJsonFile)
                            writeJson(weenie, outputPath);
                    }
                }

                //add unarmed combat all all creatures so they can fight barehanded in case they are unarmed due to phatAC's lack of lootgen
                if (weenie.weenieType == eWeenieTypes.Creature)
                {
                    if (weenie.dataFlags.HasFlag(eDataFlags.skills))
                    {
                        List<sSkill> newSkills = new List<sSkill>();
                        int highestMeleeWeaponSkill = 0;
                        int skillValue;
                        for (int i = 0; i < weenie.skills.skills.Count; i++)
                        {
                            sSkill skill = weenie.skills.skills[i].Copy();
                            switch (skill.key)
                            {
                                case eSkills.Axe:
                                case eSkills.Dagger:
                                case eSkills.Mace:
                                case eSkills.Spear:
                                case eSkills.Sword:
                                case eSkills.Staff:
                                case eSkills.UnarmedCombat:
                                case eSkills.HeavyWeapons:
                                case eSkills.LightWeapons:
                                case eSkills.FinesseWeapons:
                                    skillValue = skill.init_level;
                                    if (skillValue > highestMeleeWeaponSkill)
                                        highestMeleeWeaponSkill = skillValue;
                                    break;
                            }
                            newSkills.Add(skill);
                        }

                        if (highestMeleeWeaponSkill > 0)
                        {
                            newSkills.Add(new sSkill(eSkills.UnarmedCombat, eSkillAdvancementClass.Specialized, highestMeleeWeaponSkill));
                        }

                        weenie.skills.skills = newSkills;

                        if (writeJsonFile)
                            writeJson(weenie, outputPath);
                    }
                }

                //if (weenie.weenieType == eWeenieTypes.Creature)
                //{
                //    if (weenie.dataFlags.HasFlag(eDataFlags.skills))
                //    {
                //        List<sSkill> newSkills = new List<sSkill>();
                //        int highestMeleeWeaponSkill = 0;
                //        int highestMissileWeaponSkill = 0;
                //        int skillValue;
                //        for (int i = 0; i < weenie.skills.skills.Count; i++)
                //        {
                //            sSkill skill = weenie.skills.skills[i].Copy();
                //            switch (skill.key)
                //            {
                //                case eSkills.Axe:
                //                case eSkills.Dagger:
                //                case eSkills.Mace:
                //                case eSkills.Spear:
                //                case eSkills.Sword:
                //                case eSkills.Staff:
                //                case eSkills.UnarmedCombat:
                //                case eSkills.HeavyWeapons:
                //                case eSkills.LightWeapons:
                //                case eSkills.FinesseWeapons:
                //                    skillValue = skill.init_level;
                //                    if (skillValue > highestMeleeWeaponSkill)
                //                        highestMeleeWeaponSkill = skillValue;
                //                    break;
                //                case eSkills.Bow:
                //                case eSkills.Crossbow:
                //                case eSkills.ThrownWeapon:
                //                case eSkills.MissileWeapons:
                //                    skillValue = skill.init_level;
                //                    if (skillValue > highestMissileWeaponSkill)
                //                        highestMissileWeaponSkill = skillValue;
                //                    break;
                //                default:
                //                    newSkills.Add(skill);
                //                    break;
                //            }
                //        }

                //        if (highestMeleeWeaponSkill > 0)
                //        {
                //            newSkills.Add(new sSkill(eSkills.Axe, eSkillAdvancementClass.Specialized, highestMeleeWeaponSkill));
                //            newSkills.Add(new sSkill(eSkills.Dagger, eSkillAdvancementClass.Specialized, highestMeleeWeaponSkill));
                //            newSkills.Add(new sSkill(eSkills.Mace, eSkillAdvancementClass.Specialized, highestMeleeWeaponSkill));
                //            newSkills.Add(new sSkill(eSkills.Spear, eSkillAdvancementClass.Specialized, highestMeleeWeaponSkill));
                //            newSkills.Add(new sSkill(eSkills.Sword, eSkillAdvancementClass.Specialized, highestMeleeWeaponSkill));
                //            newSkills.Add(new sSkill(eSkills.Staff, eSkillAdvancementClass.Specialized, highestMeleeWeaponSkill));
                //            newSkills.Add(new sSkill(eSkills.UnarmedCombat, eSkillAdvancementClass.Specialized, highestMeleeWeaponSkill));
                //            newSkills.Add(new sSkill(eSkills.HeavyWeapons, eSkillAdvancementClass.Specialized, highestMeleeWeaponSkill));
                //            newSkills.Add(new sSkill(eSkills.LightWeapons, eSkillAdvancementClass.Specialized, highestMeleeWeaponSkill));
                //            newSkills.Add(new sSkill(eSkills.FinesseWeapons, eSkillAdvancementClass.Specialized, highestMeleeWeaponSkill));
                //        }

                //        if (highestMissileWeaponSkill > 0)
                //        {
                //            newSkills.Add(new sSkill(eSkills.Bow, eSkillAdvancementClass.Specialized, highestMissileWeaponSkill));
                //            newSkills.Add(new sSkill(eSkills.Crossbow, eSkillAdvancementClass.Specialized, highestMissileWeaponSkill));
                //            newSkills.Add(new sSkill(eSkills.ThrownWeapon, eSkillAdvancementClass.Specialized, highestMissileWeaponSkill));
                //            newSkills.Add(new sSkill(eSkills.MissileWeapons, eSkillAdvancementClass.Specialized, highestMissileWeaponSkill));
                //        }

                //        weenie.skills.skills = newSkills;

                //        if (writeJsonFile)
                //            writeJson(weenie, outputPath);
                //    }
                //}

                if (weenie.weenieType == eWeenieTypes.Creature &&
                    (lootProfile.otherOptions.globalCreatureDifficulty_Multiplier != 1.0 ||
                    lootProfile.otherOptions.globalCreatureDifficulty_Multiplier_MeleeAttack != 1.0 ||
                    lootProfile.otherOptions.globalCreatureDifficulty_Multiplier_MissileAttack != 1.0 ||
                    lootProfile.otherOptions.globalCreatureDifficulty_Multiplier_MagicAttack != 1.0 ||
                    lootProfile.otherOptions.globalCreatureDifficulty_Multiplier_MeleeDefense != 1.0 ||
                    lootProfile.otherOptions.globalCreatureDifficulty_Multiplier_MissileDefense != 1.0 ||
                    lootProfile.otherOptions.globalCreatureDifficulty_Multiplier_MagicDefense != 1.0 ||
                    lootProfile.otherOptions.globalCreatureDifficulty_SkillBonus_MeleeAttack != 0))
                {
                    if (weenie.dataFlags.HasFlag(eDataFlags.attributes))
                    {
                        if(weenie.attributes.attributeFlags.HasFlag(eAttributeFlags.strength))
                            weenie.attributes.strength.init_level = (int)Math.Round(weenie.attributes.strength.init_level * lootProfile.otherOptions.globalCreatureDifficulty_Multiplier);
                        if (weenie.attributes.attributeFlags.HasFlag(eAttributeFlags.endurance))
                            weenie.attributes.endurance.init_level = (int)Math.Round(weenie.attributes.endurance.init_level * lootProfile.otherOptions.globalCreatureDifficulty_Multiplier);
                        if (weenie.attributes.attributeFlags.HasFlag(eAttributeFlags.quickness))
                            weenie.attributes.quickness.init_level = (int)Math.Round(weenie.attributes.quickness.init_level * lootProfile.otherOptions.globalCreatureDifficulty_Multiplier);
                        if (weenie.attributes.attributeFlags.HasFlag(eAttributeFlags.coordination))
                            weenie.attributes.coordination.init_level = (int)Math.Round(weenie.attributes.coordination.init_level * lootProfile.otherOptions.globalCreatureDifficulty_Multiplier);
                        if (weenie.attributes.attributeFlags.HasFlag(eAttributeFlags.focus))
                            weenie.attributes.focus.init_level = (int)Math.Round(weenie.attributes.focus.init_level * lootProfile.otherOptions.globalCreatureDifficulty_Multiplier);
                        if (weenie.attributes.attributeFlags.HasFlag(eAttributeFlags.self))
                            weenie.attributes.self.init_level = (int)Math.Round(weenie.attributes.self.init_level * lootProfile.otherOptions.globalCreatureDifficulty_Multiplier);
                    }

                    if (weenie.dataFlags.HasFlag(eDataFlags.skills))
                    {
                        for(int i = 0; i < weenie.skills.skills.Count; i++)
                        {
                            sSkill skill = weenie.skills.skills[i];
                            if (skill.key != eSkills.Run && skill.key != eSkills.Jump)
                            {
                                switch (skill.key)
                                {
                                    case eSkills.CreatureEnchantment:
                                    case eSkills.ItemEnchantment:
                                    case eSkills.LifeMagic:
                                    case eSkills.WarMagic:
                                        {
                                            skill.init_level = (int)Math.Round((skill.init_level + lootProfile.otherOptions.globalCreatureDifficulty_SkillBonus_MagicAttack) * lootProfile.otherOptions.globalCreatureDifficulty_Multiplier_MagicAttack);
                                            weenie.skills.skills[i] = skill;
                                            break;
                                        }
                                    case eSkills.Axe:
                                    case eSkills.Dagger:
                                    case eSkills.Mace:
                                    case eSkills.Spear:
                                    case eSkills.Sword:
                                    case eSkills.Staff:
                                    case eSkills.UnarmedCombat:
                                    case eSkills.HeavyWeapons:
                                    case eSkills.LightWeapons:
                                    case eSkills.FinesseWeapons:
                                        {
                                            skill.init_level = (int)Math.Round((skill.init_level + lootProfile.otherOptions.globalCreatureDifficulty_SkillBonus_MeleeAttack) * lootProfile.otherOptions.globalCreatureDifficulty_Multiplier_MeleeAttack);
                                            weenie.skills.skills[i] = skill;
                                            break;
                                        }
                                    case eSkills.Bow:
                                    case eSkills.Crossbow:
                                    case eSkills.ThrownWeapon:
                                    case eSkills.MissileWeapons:
                                        {
                                            skill.init_level = (int)Math.Round((skill.init_level + lootProfile.otherOptions.globalCreatureDifficulty_SkillBonus_MissileAttack) * lootProfile.otherOptions.globalCreatureDifficulty_Multiplier_MissileAttack);
                                            weenie.skills.skills[i] = skill;
                                            break;
                                        }
                                    case eSkills.MeleeDefense:
                                        {
                                            skill.init_level = (int)Math.Round((skill.init_level + lootProfile.otherOptions.globalCreatureDifficulty_SkillBonus_MeleeDefense) * lootProfile.otherOptions.globalCreatureDifficulty_Multiplier_MeleeDefense);
                                            weenie.skills.skills[i] = skill;
                                            break;
                                        }
                                    case eSkills.MissileDefense:
                                        {
                                            skill.init_level = (int)Math.Round((skill.init_level + lootProfile.otherOptions.globalCreatureDifficulty_SkillBonus_MissileDefense) * lootProfile.otherOptions.globalCreatureDifficulty_Multiplier_MissileDefense);
                                            weenie.skills.skills[i] = skill;
                                            break;
                                        }
                                    case eSkills.MagicDefense:
                                        {
                                            skill.init_level = (int)Math.Round((skill.init_level + lootProfile.otherOptions.globalCreatureDifficulty_SkillBonus_MagicDefense) * lootProfile.otherOptions.globalCreatureDifficulty_Multiplier_MagicDefense);
                                            weenie.skills.skills[i] = skill;
                                            break;
                                        }
                                    default:
                                        {
                                            skill.init_level = (int)Math.Round(skill.init_level * lootProfile.otherOptions.globalCreatureDifficulty_Multiplier);
                                            weenie.skills.skills[i] = skill;
                                            break;
                                        }
                                }
                            }
                        }
                    }

                    if (writeJsonFile)
                        writeJson(weenie, outputPath);
                }
            });

            if (lootProfile.otherOptions.disableKeyringCrafting)
            {
                //remove keyrings since they do not work and can be exploited to duplicate keys.
                cWeenie removed;
                weenies.TryRemove(23194, out removed);
                weenies.TryRemove(23195, out removed);
                weenies.TryRemove(23196, out removed);
                weenies.TryRemove(23197, out removed);
                weenies.TryRemove(23198, out removed);
                weenies.TryRemove(23199, out removed);
                weenies.TryRemove(24887, out removed);
                weenies.TryRemove(31826, out removed);
                weenies.TryRemove(34960, out removed);
                weenies.TryRemove(42347, out removed);
                weenies.TryRemove(48954, out removed);
            }

            int numCreaturesLootAdded = 0;
            int numChestsLootAdded = 0;

            foreach (KeyValuePair<sLootTier, int> entry in lootAddedCounter)
            {
                numCreaturesLootAdded += entry.Value;
            }

            foreach (KeyValuePair<sLootTier, int> entry in chestLootAddedCounter)
            {
                numChestsLootAdded += entry.Value;
            }

            Dictionary<sLootTier, int> sortedLootAddedCounter = new Dictionary<sLootTier, int>();
            Dictionary<sLootTier, int> sortedChestLootAddedCounter = new Dictionary<sLootTier, int>();
            foreach (sLootTier tier in lootTiers)
            {
                sortedLootAddedCounter.Add(tier, lootAddedCounter[tier]);
                sortedChestLootAddedCounter.Add(tier, chestLootAddedCounter[tier]);
            }

            timer.Stop();
            Console.WriteLine("Added loot to {0} weenies in {1} seconds:", numCreaturesLootAdded + numChestsLootAdded, timer.ElapsedMilliseconds / 1000f);
            Console.WriteLine(" - {0} creatures.", numCreaturesLootAdded);
            foreach (KeyValuePair<sLootTier, int> entry in sortedLootAddedCounter)
            {
                if (entry.Value > 0)
                    Console.WriteLine("    - {0} {1} creatures.", entry.Value, entry.Key.name);
            }
            Console.WriteLine(" - {0} chests.", numChestsLootAdded);
            foreach (KeyValuePair<sLootTier, int> entry in sortedChestLootAddedCounter)
            {
                if (entry.Value > 0)
                    Console.WriteLine("    - {0} {1} chests.", entry.Value, entry.Key.name);
            }

            if (writeRawFile)
                writeRaw();
        }

        void writeJson(cWeenie weenie, string outputPath, bool extended = false)
        {
            int wcid = weenie.wcid;
            string className = WeenieClassNames.getWeenieClassName(wcid);
            string name = weenie.weenieName;

            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalid)
            {
                name = name.Replace(c.ToString(), "");
                className = className.Replace(c.ToString(), "");
            }

            string type = weenie.weenieType.ToString();

            if (!Directory.Exists($"{outputPath}/{type}/"))
                Directory.CreateDirectory($"{outputPath}/{type}/");

            string jsonFilename;
            if (className != "" && name != "")
            {
                if (className.ToLower() != name.ToLower())
                    jsonFilename = $"{outputPath}/{type}/{wcid} - {name}({className}).json";
                else
                    jsonFilename = $"{outputPath}/{type}/{wcid} - {name}.json";
            }
            else if (className == "" && name != "")
                jsonFilename = $"{outputPath}/{type}/{wcid} - {name}.json";
            else if (className != "" && name == "")
                jsonFilename = $"{outputPath}/{type}/{wcid} - {className}.json";
            else
                jsonFilename = $"{outputPath}/{type}/{wcid}.json";

            StreamWriter outputFile = new StreamWriter(new FileStream(jsonFilename, FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open {0}", jsonFilename);
                return;
            }

            if(!extended)
                weenie.writeJson(outputFile, "", true);
            else
                weenie.writeExtendedJson(outputFile);

            outputFile.Flush();
            outputFile.Close();
        }

        void writeJson(string outputPath, bool singleFile, eWeenieTypes specificType, bool extended = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            int weenieCount = 0;
            if (singleFile)
            {
                Console.WriteLine("Writing {0}weenies to a single json file...", extended ? "extended " : "");
                string outputFilename = $"{outputPath}/weenies(single file).json";
                if(extended)
                    outputFilename = $"{outputPath}/extended weenies(single file).json";
                StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));
                if (outputFile == null)
                {
                    Console.WriteLine("Unable to open {0}", outputFile);
                    return;
                }

                outputFile.Write("[\n");
                bool first = true;
                foreach (KeyValuePair<int, cWeenie> entry in weenies)
                {
                    if (specificType != eWeenieTypes.Undef && entry.Value.weenieType != specificType)
                        continue;

                    if (!extended)
                    {
                        entry.Value.writeJson(outputFile, "\t", first);
                        if (first)
                            first = false;
                    }
                    else
                        entry.Value.writeExtendedJson(outputFile);
                    weenieCount++;
                }
                outputFile.Write("\n]");

                outputFile.Flush();
                outputFile.Close();
            }
            else
            {
                Console.WriteLine("Writing {0}weenies...", extended ? "extended " : "");

                foreach (KeyValuePair<int, cWeenie> entry in weenies)
                {
                    if (specificType != eWeenieTypes.Undef && entry.Value.weenieType != specificType)
                        continue;

                    writeJson(entry.Value, outputPath, extended);
                    weenieCount++;
                }
            }

            timer.Stop();
            Console.WriteLine("{0} weenies written in {1} seconds.", weenieCount, timer.ElapsedMilliseconds / 1000f);
        }

        public string buildWeenieName(int wcid)
        {
            string returnValue = "";
            cWeenie wcidWeenie = Program.cache9Converter.getWeenie(wcid);
            if (wcidWeenie != null)
                returnValue = wcidWeenie.weenieName;
            return Utils.removeWcidNameRedundancy(WeenieClassNames.getWeenieClassName(wcid), returnValue);
        }
    }
}