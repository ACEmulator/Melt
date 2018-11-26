using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static Melt.Utils;

namespace Melt
{
    public struct sOtherOptions
    {
        public string copyOutputToFolder;
        public bool debug;
        public bool fixStackableItemRewards;
        public Single pyrealRewardsMultiplier;

        public bool disableKeyringCrafting;
        public bool fixEnchantableAmmunition;

        public bool extraMutations;

        public Single globalCreatureSpellDamageMultiplier;

        public bool fixSpawnGeneratorTimers;
        public Single spawnGeneratorTimerMultiplier;

        public Single globalCreatureDifficulty_Multiplier;
        public Single globalCreatureDifficulty_Multiplier_MeleeAttack;
        public Single globalCreatureDifficulty_Multiplier_MissileAttack;
        public Single globalCreatureDifficulty_Multiplier_MagicAttack;
        public Single globalCreatureDifficulty_Multiplier_MeleeDefense;
        public Single globalCreatureDifficulty_Multiplier_MissileDefense;
        public Single globalCreatureDifficulty_Multiplier_MagicDefense;

        public int globalCreatureDifficulty_SkillBonus_MeleeAttack;
        public int globalCreatureDifficulty_SkillBonus_MissileAttack;
        public int globalCreatureDifficulty_SkillBonus_MagicAttack;
        public int globalCreatureDifficulty_SkillBonus_MeleeDefense;
        public int globalCreatureDifficulty_SkillBonus_MissileDefense;
        public int globalCreatureDifficulty_SkillBonus_MagicDefense;

        public Single globalLootChanceMultiplier;
        public Single globalMiscLootChanceMultiplier;
        public Single globalScrollLootChanceMultiplier;
        public Single globalChestLootAmountMultiplier;
        public Single globalComplementaryTierLootChanceMultiplier;
    }

    public struct sSpellProperties
    {
        public List<int> spellPower;
        public List<int> spellMana;
        public List<int> cantripPower;
        public List<int> cantripMana;
    }

    public struct sMaterialProperties
    {
        public Single valueMultiplier;
        public List<int> palettes;
        public int gemValue;
        public string gemName;
        public string gemPluralName;
    }

    public struct sWorkmanshipProperties
    {
        public Single valueMultiplier;
        public Single gemChance;
        public int maxGemCount;
    }

    public struct sItemLootProfileEntry
    {
        public string name;
        public int wcid;
        public int maxAmount;
        public List<eMaterialType> possibleMaterials;
        public List<int> elementalVariants;
        public eSkills weaponSkill;
    }

    public enum eSpellCategory
    {
        none,
        magicSkillMastery,
        weaponSkillMastery
    }

    public struct sPossibleSpells
    {
        public string spellName;
        public eSpellCategory spellCategory;
        public List<int> spells;
        public List<int> cantrips;

        public int id;
    }

    public struct sWieldTier
    {
        public int weaponSkillRequired;
        public Single maxAttackSkillBonus;
        public Single maxMeleeDefenseBonus;
        public Single maxMissileDefenseBonus;
        public Single maxMagicDefenseBonus;
        public Single attackSkillBonusChance;
        public Single meleeDefenseBonusChance;
        public Single missileDefenseBonusChance;
        public Single magicDefenseBonusChance;
        public Single elementalChance;

        public Single slayer_Chance;
        public Single slayer_MinDamageBonus;
        public Single slayer_MaxDamageBonus;
        public List<eCreatureType> slayer_Types;

        public int maxPropertyAmount;

        public Single crushingBlow_Chance;
        public Single crushingBlow_MinCriticalMultiplier;
        public Single crushingBlow_MaxCriticalMultiplier;

        public Single bitingStrike_Chance;
        public Single bitingStrike_MinCriticalFrequency;
        public Single bitingStrike_MaxCriticalFrequency;

        //melee weapons
        public int minDamageBonus;
        public int maxDamageBonus;
        public Single highVariance;
        public Single lowVariance;

        //melee and missile weapons
        public Single slowSpeedMod;
        public Single fastSpeedMod;

        //missile weapons
        public Single minDamageModBonus;
        public Single maxDamageModBonus;
        public int minElementalDamageBonus;
        public int maxElementalDamageBonus;

        //casters
        public Single maxManaConversionBonus;
        public Single minElementalDamageMod;
        public Single maxElementalDamageMod;
        public Single manaConversionBonusChance;

        //armor
        public int armorWieldTier;
        public int meleeDefenseSkillRequired;
        public int missileDefenseSkillRequired;
        public int magicDefenseSkillRequired;
        public int minArmorBonus;
        public int maxArmorBonus;
        public Single minBulkMod;
        public Single maxBulkMod;
        public int minShieldArmorBonus;
        public int maxShieldArmorBonus;
        public int minLevel;
    }

    public struct sLootProfileCategory
    {
        public string category;
        public List<eMaterialType> possibleMaterials;
        public List<sWieldTier> wieldTiers;
        public List<sItemLootProfileEntry> entries;
    }

    public struct sLootArmorProfile
    {
        public List<sWieldTier> wieldTiers;
        public List<sLootProfileCategory> categories;
    }

    public struct sLootTier
    {
        public string name;
        public string template;
        public int firstId;
        public int lootEntriesPerWeenie;
        public int amount;
        public int regularLootFirstId;
        public int regularLootLastId;
        public int qualityLootFirstId;
        public int qualityLootLastId;
        public int allLootFirstId;
        public int allLootLastId;
        public List<string> miscItemsCategories;
        public List<string> scrollCategories;
        public List<int> treasureGeneratorTypes;
        public Single lootChance;
        public Single miscLootChance;
        public Single scrollLootChance;
        public int scrollEntriesPerWeenie;
        public Single chestLootAmountMultiplier;
        public int qualityLootLevelThreshold;
        public double qualityLootModifier;

        public Single creatureDifficulty_Multiplier;
        public Single creatureDifficulty_Multiplier_MeleeAttack;
        public Single creatureDifficulty_Multiplier_MissileAttack;
        public Single creatureDifficulty_Multiplier_MagicAttack;
        public Single creatureDifficulty_Multiplier_MeleeDefense;
        public Single creatureDifficulty_Multiplier_MissileDefense;
        public Single creatureDifficulty_Multiplier_MagicDefense;

        public Single creatureXPMultiplier;

        public int minMeleeWeaponWieldTier;
        public int maxMeleeWeaponWieldTier;
        public int minMissileWeaponWieldTier;
        public int maxMissileWeaponWieldTier;
        public int minCasterWieldTier;
        public int maxCasterWieldTier;

        public int minArmorWieldTier;
        public int maxArmorWieldTier;
        public int minArmorMeleeWieldTier;
        public int maxArmorMeleeWieldTier;
        public int minArmorMissileWieldTier;
        public int maxArmorMissileWieldTier;
        public int minArmorMagicWieldTier;
        public int maxArmorMagicWieldTier;

        public int meleeWeaponsLootProportion;
        public int missileWeaponsLootProportion;
        public int castersLootProportion;
        public int armorLootProportion;
        public int clothingLootProportion;
        public int jewelryLootProportion;

        public List<string> commonArmorCategories;
        public List<string> rareArmorCategories;
        public Single rareArmorChance;

        public int minWorkmanship;
        public int maxWorkmanship;

        public int maxAmountOfSpells;
        public Single chanceOfSpells;
        public int minSpellLevel;
        public int maxSpellLevel;
        public int preferredSpellLevel;
        public Single preferredSpellLevelStrength;

        public int maxAmountOfCantrips;
        public Single chanceOfCantrips;
        public int minCantripLevel;
        public int maxCantripLevel;
        public int preferredCantripLevel;
        public Single preferredCantripLevelStrength;

        public Single heritageRequirementChance;
        public int minSpellsForHeritageRequirement;
        public Single allegianceRankRequirementChance;
        public int minSpellsForAllegianceRankRequirement;
        public int maxAllegianceRankRequired;

        public List<eMaterialType> materialsCeramic;
        public List<eMaterialType> materialsCloth;
        public List<eMaterialType> materialsGem;
        public List<eMaterialType> materialsLeather;
        public List<eMaterialType> materialsMetal;
        public List<eMaterialType> materialsStone;
        public List<eMaterialType> materialsWood;
    }

    public struct sLootProfile
    {
        public List<sLootTier> tiers;
        public List<sLootTier> complementaryTiers;

        public List<sLootProfileCategory> meleeWeapons;
        public List<sLootProfileCategory> missileWeapons;
        public sLootProfileCategory casters;
        public sLootArmorProfile armor;
        public List<sLootProfileCategory> clothing;
        public List<sLootProfileCategory> jewelry;
        public List<sLootProfileCategory> miscItems;
        public List<sLootProfileCategory> scrolls;

        public sSpellProperties spellProperties;

        public List<sPossibleSpells> meleeWeaponSpells;
        public List<sPossibleSpells> missileWeaponSpells;
        public List<sPossibleSpells> casterSpells;
        public List<sPossibleSpells> shieldSpells;
        public List<sPossibleSpells> jewelrySpells;

        public List<sPossibleSpells> clothingSpells;
        public List<sPossibleSpells> clothingHeadSpells;
        public List<sPossibleSpells> clothingHandsSpells;
        public List<sPossibleSpells> clothingFeetSpells;

        public List<sPossibleSpells> armorItemSpells;
        public List<sPossibleSpells> armorHeadSpells;
        public List<sPossibleSpells> armorUpperBodySpells;
        public List<sPossibleSpells> armorHandsSpells;
        public List<sPossibleSpells> armorLowerBodySpells;
        public List<sPossibleSpells> armorFeetSpells;

        public Dictionary<eMaterialType, sMaterialProperties> materialProperties;
        public Dictionary<int, sWorkmanshipProperties> workmanshipProperties;

        public Dictionary<int, int> chestTreasureTypeReplacementTable;

        public sOtherOptions otherOptions;

        int nextSpellId;
        public Dictionary<string, int> spellNameToIdMap;
        public Dictionary<int, sPossibleSpells> spells;

        public void computeUniqueSpellIds(List<sPossibleSpells> spellList)
        {
            for (int i = 0; i < spellList.Count; i++)
            {
                sPossibleSpells spell = spellList[i];
                int id;
                if (!spellNameToIdMap.TryGetValue(spell.spellName, out id))
                {
                    id = nextSpellId++;
                    spell.id = id;
                    spellNameToIdMap.Add(spell.spellName, id);
                    spells.Add(id, spell);
                }
                spell.id = id;
                spellList[i] = spell;
            }
        }

        bool isInitialized;
        public void initialize()
        {
            if (isInitialized)
                return;

            isInitialized = true;

            nextSpellId = 0;
            spellNameToIdMap = new Dictionary<string, int>();
            spells = new Dictionary<int, sPossibleSpells>();

            computeUniqueSpellIds(meleeWeaponSpells);
            computeUniqueSpellIds(missileWeaponSpells);
            computeUniqueSpellIds(casterSpells);
            computeUniqueSpellIds(shieldSpells);
            computeUniqueSpellIds(clothingSpells);
            computeUniqueSpellIds(jewelrySpells);

            computeUniqueSpellIds(armorItemSpells);
            computeUniqueSpellIds(armorHeadSpells);
            computeUniqueSpellIds(armorUpperBodySpells);
            computeUniqueSpellIds(armorHandsSpells);
            computeUniqueSpellIds(armorLowerBodySpells);
            computeUniqueSpellIds(armorFeetSpells);

            computeUniqueSpellIds(clothingHeadSpells);
            computeUniqueSpellIds(clothingHandsSpells);
            computeUniqueSpellIds(clothingFeetSpells);

            List<sLootTier> lootTiers = new List<sLootTier>();
            foreach (sLootTier tier in tiers)
            {
                sLootTier newTier = tier.Copy();

                int regularLootAmount = ((tier.amount / 4) * 3) + tier.amount % 4;
                int qualityLootAmount = tier.amount - regularLootAmount;

                newTier.regularLootFirstId = tier.firstId;
                newTier.regularLootLastId = tier.firstId + regularLootAmount - 1;

                newTier.qualityLootFirstId = newTier.regularLootLastId + 1;
                newTier.qualityLootLastId = newTier.qualityLootFirstId + qualityLootAmount - 1;

                newTier.allLootFirstId = newTier.regularLootFirstId;
                newTier.allLootLastId = newTier.qualityLootLastId;

                lootTiers.Add(newTier);
            }

            tiers = lootTiers;
        }
    }

    class tierAnalysisInfo
    {
        public int totalItems;
        public Single totalValue;
        public int highestValue = 0;
        public int lowestValue = int.MaxValue;
        public int lowestValueId;
        public int highestValueId;
        public SortedDictionary<int, int> valueBuckets = new SortedDictionary<int, int>();

        public int totalMagicItems;
        public Single totalArcaneLoreReq;
        public int highestArcaneLoreReq = 0;
        public int lowestArcaneLoreReq = int.MaxValue;
        public int lowestArcaneLoreId;
        public int highestArcaneLoreId;
        public SortedDictionary<int, int> arcaneLoreBuckets = new SortedDictionary<int, int>();

        public tierAnalysisInfo()
        {
            valueBuckets.Add(500, 0);
            valueBuckets.Add(1000, 0);
            valueBuckets.Add(2500, 0);
            valueBuckets.Add(5000, 0);
            valueBuckets.Add(7500, 0);
            valueBuckets.Add(10000, 0);
            valueBuckets.Add(12500, 0);
            valueBuckets.Add(15000, 0);
            valueBuckets.Add(20000, 0);
            valueBuckets.Add(25000, 0);
            valueBuckets.Add(30000, 0);
            valueBuckets.Add(35000, 0);
            valueBuckets.Add(40000, 0);
            valueBuckets.Add(45000, 0);
            valueBuckets.Add(50000, 0);
            valueBuckets.Add(75000, 0);
            valueBuckets.Add(100000, 0);
            valueBuckets.Add(150000, 0);
            valueBuckets.Add(200000, 0);
            valueBuckets.Add(int.MaxValue, 0);

            arcaneLoreBuckets.Add(10, 0);
            arcaneLoreBuckets.Add(20, 0);
            arcaneLoreBuckets.Add(30, 0);
            arcaneLoreBuckets.Add(40, 0);
            arcaneLoreBuckets.Add(50, 0);
            arcaneLoreBuckets.Add(60, 0);
            arcaneLoreBuckets.Add(70, 0);
            arcaneLoreBuckets.Add(80, 0);
            arcaneLoreBuckets.Add(90, 0);
            arcaneLoreBuckets.Add(100, 0);
            arcaneLoreBuckets.Add(110, 0);
            arcaneLoreBuckets.Add(120, 0);
            arcaneLoreBuckets.Add(130, 0);
            arcaneLoreBuckets.Add(140, 0);
            arcaneLoreBuckets.Add(150, 0);
            arcaneLoreBuckets.Add(160, 0);
            arcaneLoreBuckets.Add(170, 0);
            arcaneLoreBuckets.Add(180, 0);
            arcaneLoreBuckets.Add(190, 0);
            arcaneLoreBuckets.Add(200, 0);
            arcaneLoreBuckets.Add(210, 0);
            arcaneLoreBuckets.Add(220, 0);
            arcaneLoreBuckets.Add(230, 0);
            arcaneLoreBuckets.Add(240, 0);
            arcaneLoreBuckets.Add(250, 0);
            arcaneLoreBuckets.Add(260, 0);
            arcaneLoreBuckets.Add(270, 0);
            arcaneLoreBuckets.Add(280, 0);
            arcaneLoreBuckets.Add(290, 0);
            arcaneLoreBuckets.Add(300, 0);
            arcaneLoreBuckets.Add(310, 0);
            arcaneLoreBuckets.Add(320, 0);
            arcaneLoreBuckets.Add(330, 0);
            arcaneLoreBuckets.Add(int.MaxValue, 0);
        }

        public void addArcaneLoreEntry(int value, int id)
        {
            lock (this)
            {
                totalMagicItems++;
                totalArcaneLoreReq += value;
                if (value > highestArcaneLoreReq)
                {
                    highestArcaneLoreReq = value;
                    highestArcaneLoreId = id;
                }
                if (value < lowestArcaneLoreReq)
                {
                    lowestArcaneLoreReq = value;
                    lowestArcaneLoreId = id;
                }

                for (int i = 0; i < arcaneLoreBuckets.Count; i++)
                {
                    int bucketMaxValue = new List<int>(arcaneLoreBuckets.Keys)[i];
                    if (value > bucketMaxValue)
                        continue;
                    else
                    {
                        arcaneLoreBuckets[bucketMaxValue]++;
                        break;
                    }
                }
            }
        }

        public void addValueEntry(int value, int id)
        {
            lock (this)
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

                for (int i = 0; i < valueBuckets.Count; i++)
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
    }

    class cItemCreator
    {
        //SortedDictionary<string, tierAnalysisInfo> infoMap = new SortedDictionary<string, tierAnalysisInfo>();

        int numGeneratedItems;
        int numMundaneItems;
        int numMagicItems;
        int numFailedGenerations;
        ConcurrentDictionary<int, cWeenie> weenies;
        Dictionary<uint, sClothingTable> clothingTables;

        public cItemCreator(ConcurrentDictionary<int, cWeenie> weenies, Dictionary<uint, sClothingTable> clothingTables)
        {
            this.weenies = weenies;
            this.clothingTables = clothingTables;
        }

        public void generateLootProfile(sLootProfile lootProfile, bool writeJsonFile, string outputPath)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Console.WriteLine("Generating items...");
            numGeneratedItems = 0;
            numMundaneItems = 0;
            numMagicItems = 0;
            numFailedGenerations = 0;

            ParallelOptions parallelOptions = new ParallelOptions();
            //parallelOptions.MaxDegreeOfParallelism = 1; //debug

            Parallel.ForEach(lootProfile.tiers, parallelOptions, tier =>
            //foreach(sLootTier tier in lootProfile.tiers)
            {
                generateLootTier(tier, lootProfile, writeJsonFile, outputPath, tier.regularLootFirstId, tier.regularLootLastId, 0);
                generateLootTier(tier, lootProfile, writeJsonFile, outputPath, tier.qualityLootFirstId, tier.qualityLootLastId, tier.qualityLootModifier);
            });

            timer.Stop();
            Console.WriteLine("Generated {0} items in {1} seconds.", numGeneratedItems, timer.ElapsedMilliseconds / 1000f);
            Console.WriteLine(" - {0} mundane items.", numMundaneItems);
            Console.WriteLine(" - {0} magic items.", numMagicItems);
            if (numFailedGenerations > 0)
                Console.WriteLine(" - {0} items failed item generation.", numFailedGenerations);

            //StreamWriter outputFile = new StreamWriter(new FileStream("./output/generationInfo.txt", FileMode.Create, FileAccess.Write));
            //foreach (KeyValuePair<string, tierAnalysisInfo> analisysInfo in infoMap)
            //{
            //    outputFile.WriteLine($"--{analisysInfo.Key}--");
            //    //outputFile.WriteLine($"Lowest Value: {analisysInfo.Value.lowestValue} ({analisysInfo.Value.lowestValueId})");
            //    //outputFile.WriteLine($"Highest Value: {analisysInfo.Value.highestValue} ({analisysInfo.Value.highestValueId})");
            //    //outputFile.WriteLine($"Average Value: {analisysInfo.Value.totalValue / analisysInfo.Value.totalItems}");
            //    //outputFile.WriteLine();
            //    //outputFile.WriteLine("Values:");
            //    //foreach (KeyValuePair<int, int> entry in analisysInfo.Value.valueBuckets)
            //    //{
            //    //    if (entry.Value > 0)
            //    //        outputFile.WriteLine($"Under {entry.Key}: {entry.Value}({entry.Value * 100f / analisysInfo.Value.totalItems}%)");
            //    //}
            //    outputFile.WriteLine();
            //    outputFile.WriteLine($"Lowest Arcane Lore Req: {analisysInfo.Value.lowestArcaneLoreReq} ({analisysInfo.Value.lowestArcaneLoreId})");
            //    outputFile.WriteLine($"Highest Arcane Lore Req: {analisysInfo.Value.highestArcaneLoreReq} ({analisysInfo.Value.highestArcaneLoreId})");
            //    outputFile.WriteLine($"Average Arcane Lore Req: {analisysInfo.Value.totalArcaneLoreReq / analisysInfo.Value.totalMagicItems}");
            //    outputFile.WriteLine();
            //    outputFile.WriteLine("Arcane Lore Reqs:");
            //    foreach (KeyValuePair<int, int> entry in analisysInfo.Value.arcaneLoreBuckets)
            //    {
            //        if (entry.Value > 0)
            //            outputFile.WriteLine($"Under {entry.Key}: {entry.Value}({entry.Value * 100f / analisysInfo.Value.totalMagicItems}%)");
            //    }
            //    outputFile.WriteLine();
            //    outputFile.WriteLine();
            //    outputFile.WriteLine("---------------------");
            //    outputFile.WriteLine();
            //    outputFile.WriteLine();
            //    outputFile.Flush();
            //}
            //outputFile.Close();
        }

        public void generateLootTier(sLootTier tier, sLootProfile lootProfile, bool writeJsonFile, string outputPath, int firstId, int lastId, double qualityModifier)
        {
            List<sLootProfileCategory> commonArmor = new List<sLootProfileCategory>();
            List<sLootProfileCategory> rareArmor = new List<sLootProfileCategory>();

            foreach (sLootProfileCategory armorCategory in lootProfile.armor.categories)
            {
                foreach (string commonArmorCategory in tier.commonArmorCategories)
                {
                    if (armorCategory.category == commonArmorCategory)
                        commonArmor.Add(armorCategory);
                }
                foreach (string rareArmorCategory in tier.rareArmorCategories)
                {
                    if (armorCategory.category == rareArmorCategory)
                        rareArmor.Add(armorCategory);
                }
            }

            int nextAvailableId = firstId;

            //for (int i = 0; i < tier.amount; i++)
            //    generateItem(lootProfile, tier, lootProfile.meleeWeapons[0]);

            int amountOfTypes = (tier.meleeWeaponsLootProportion * lootProfile.meleeWeapons.Count) +
                                (tier.missileWeaponsLootProportion * lootProfile.missileWeapons.Count) +
                                tier.castersLootProportion +
                                tier.armorLootProportion +
                                tier.clothingLootProportion +
                                tier.jewelryLootProportion;

            int amount = lastId - firstId + 1;
            int amountOfIterations = amount / amountOfTypes;
            int rest = amount % amountOfTypes;

            Dictionary<int, sLootProfileCategory> generationList = new Dictionary<int, sLootProfileCategory>();

            for (int i = 0; i < amountOfIterations; i++)
            {
                for (int j = 0; j < tier.meleeWeaponsLootProportion; j++)
                {
                    for (int k = 0; k < lootProfile.meleeWeapons.Count; k++)
                        generationList.Add(nextAvailableId++, lootProfile.meleeWeapons[k]);
                }

                for (int j = 0; j < tier.missileWeaponsLootProportion; j++)
                {
                    for (int k = 0; k < lootProfile.missileWeapons.Count; k++)
                        generationList.Add(nextAvailableId++, lootProfile.missileWeapons[k]);
                }

                for (int j = 0; j < tier.castersLootProportion; j++)
                {
                    generationList.Add(nextAvailableId++, lootProfile.casters);
                }

                for (int j = 0; j < tier.armorLootProportion; j++)
                {
                    if (rareArmor.Count > 0 && Utils.getRandomNumberExclusive(100) < tier.rareArmorChance * 100)
                        generationList.Add(nextAvailableId++, rareArmor[Utils.getRandomNumberExclusive(rareArmor.Count)]);
                    else if (commonArmor.Count > 0)
                        generationList.Add(nextAvailableId++, commonArmor[Utils.getRandomNumberExclusive(commonArmor.Count)]);
                    else
                        generationList.Add(nextAvailableId++, commonArmor[Utils.getRandomNumberExclusive(commonArmor.Count)]);
                }

                for (int j = 0; j < tier.clothingLootProportion; j++)
                {
                    generationList.Add(nextAvailableId++, lootProfile.clothing[Utils.getRandomNumberExclusive(lootProfile.clothing.Count)]);
                }

                for (int j = 0; j < tier.jewelryLootProportion; j++)
                {
                    generationList.Add(nextAvailableId++, lootProfile.jewelry[Utils.getRandomNumberExclusive(lootProfile.jewelry.Count)]);
                }
            }

            //The remaining entrances are filled with armor
            for (int i = 0; i < rest; i++)
            {
                if (rareArmor.Count > 0 && Utils.getRandomNumberExclusive(100) < tier.rareArmorChance * 100)
                    generationList.Add(nextAvailableId++, rareArmor[Utils.getRandomNumberExclusive(rareArmor.Count)]);
                else if (commonArmor.Count > 0)
                    generationList.Add(nextAvailableId++, commonArmor[Utils.getRandomNumberExclusive(commonArmor.Count)]);
                else
                    generationList.Add(nextAvailableId++, commonArmor[Utils.getRandomNumberExclusive(commonArmor.Count)]);
            }

            ParallelOptions parallelOptions = new ParallelOptions();
            //parallelOptions.MaxDegreeOfParallelism = 1; //debug
            Parallel.ForEach(generationList, parallelOptions, entry =>
            //foreach (KeyValuePair<int, sLootProfileCategory> entry in generationList)
            {
                generateItem(lootProfile, tier, entry.Value, entry.Key, qualityModifier, writeJsonFile, outputPath);
            });
        }

        private struct sItemCreationInfo
        {
            public double qualityModifier;
            public bool isMagical;
            public int totalPower;
            public int totalMana;
            public int highestPower;
            public int totalSpellsCount;
            public int spellAmountMultiplier;
            public sPossibleSpells favoredMagicSchoolMasterySpell;
            public List<sPossibleSpells> otherMagicSchoolMasterySpells;
            public bool hasAlreadyReplacedSpell;
            public bool hasAlreadyReplacedCantrip;
            public List<sPossibleSpells> spells;
            public List<int> spellIds;
            public List<sPossibleSpells> cantrips;
            public List<int> cantripIds;
        }

        public void generateItem(sLootProfile lootProfile, sLootTier tier, sLootProfileCategory category, int wcid, double qualityModifier, bool writeJsonFile, string outputPath)
        {
            int retryCount = 0;

            while (true)
            {
                sItemLootProfileEntry randomLootProfile = category.entries[Utils.getRandomNumberExclusive(category.entries.Count)];
                //sItemLootProfileEntry randomLootProfile = category.entries[0];

                cWeenie baseItem;
                if (weenies.TryGetValue(randomLootProfile.wcid, out baseItem))
                {
                    cWeenie newItem = baseItem.Copy();
                    newItem.wcid = wcid;
                    newItem.weenieName = $"GeneratedItem{newItem.wcid}";

                    newItem.removeStat(eIntStat.TsysMutationData);
                    newItem.removeStat(eDidStat.MutateFilter);
                    newItem.removeStat(eDidStat.TsysMutationFilter);

                    sItemCreationInfo creationInfo = new sItemCreationInfo();
                    creationInfo.qualityModifier = qualityModifier;
                    creationInfo.spells = new List<sPossibleSpells>();
                    creationInfo.spellIds = new List<int>();
                    creationInfo.cantrips = new List<sPossibleSpells>();
                    creationInfo.cantripIds = new List<int>();

                    bool success = mutateItem(newItem, baseItem, ref creationInfo, lootProfile, tier, category, randomLootProfile);

                    if (!success)
                    {
                        retryCount++;
                        if (retryCount > 10)
                        {
                            Console.WriteLine("Failed to mutate 10 times in a row! wcid: {0} (entry: {1} - category: {2})", randomLootProfile.wcid, randomLootProfile.name, category.category);
                            Interlocked.Increment(ref numFailedGenerations);
                            break;
                        }
                    }
                    else
                    {
                        if (writeJsonFile)
                        {
                            if (!Directory.Exists($"{outputPath}/MutatedItems/{tier.name}"))
                                Directory.CreateDirectory($"{outputPath}/MutatedItems/{tier.name}");

                            string fileName = $"{outputPath}/MutatedItems/{tier.name}/{newItem.wcid}.json";
                            StreamWriter outputFile = new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write));
                            if (outputFile == null)
                                Console.WriteLine($"Unable to open {fileName}");
                            else
                            {
                                newItem.writeJson(outputFile);
                                outputFile.Close();
                            }
                        }

                        weenies.TryAdd(newItem.wcid, newItem);
                        Interlocked.Increment(ref numGeneratedItems);
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("Unknown wcid: {0} (entry: {1} - category: {2})", randomLootProfile.wcid, randomLootProfile.name, category.category);
                    Interlocked.Increment(ref numFailedGenerations);
                    break;
                }
            }
        }

        void mutateWeapon(cWeenie newItem, sWieldTier wieldTier, ref sItemCreationInfo creationInfo, sLootProfile lootProfile, sLootTier tier, sLootProfileCategory category, sItemLootProfileEntry entry)
        {
            if(entry.weaponSkill != eSkills.None)
            {
                newItem.addOrUpdateStat(eIntStat.WeaponSkill, (int)entry.weaponSkill);
            }

            if (wieldTier.weaponSkillRequired > 0)
            {
                newItem.addOrUpdateStat(eIntStat.WieldRequirements, (int)eWieldRequirements.baseSkill);
                newItem.addOrUpdateStat(eIntStat.WieldDifficulty, wieldTier.weaponSkillRequired);
                newItem.addOrUpdateStat(eIntStat.WieldSkilltype, newItem.getStat(eIntStat.WeaponSkill));
            }

            if (wieldTier.maxMagicDefenseBonus > 0 && Utils.getRandomNumberExclusive(100) < wieldTier.magicDefenseBonusChance * 100 * (1 + (creationInfo.qualityModifier * 2)))
                newItem.addOrUpdateStat(eFloatStat.WeaponMagicDefense, Math.Round(Utils.getRandomDouble(1, 1 + (wieldTier.maxMagicDefenseBonus / 100), eRandomFormula.favorMid, 2, creationInfo.qualityModifier), 2));

            if (wieldTier.maxMissileDefenseBonus > 0 && Utils.getRandomNumberExclusive(100) < wieldTier.missileDefenseBonusChance * 100 * (1 + (creationInfo.qualityModifier * 2)))
                newItem.addOrUpdateStat(eFloatStat.WeaponMissileDefense, Math.Round(Utils.getRandomDouble(1, 1 + (wieldTier.maxMissileDefenseBonus / 100), eRandomFormula.favorMid, 2, creationInfo.qualityModifier), 2));

            if (wieldTier.maxMeleeDefenseBonus > 0 && Utils.getRandomNumberExclusive(100) < wieldTier.meleeDefenseBonusChance * 100 * (1 + (creationInfo.qualityModifier * 2)))
                newItem.addOrUpdateStat(eFloatStat.WeaponDefense, Math.Round(Utils.getRandomDouble(1, 1 + (wieldTier.maxMeleeDefenseBonus / 100), eRandomFormula.favorMid, 2, creationInfo.qualityModifier), 2));

            if (lootProfile.otherOptions.extraMutations)
            {
                int propertyCount = 0;
                if (propertyCount < wieldTier.maxPropertyAmount && wieldTier.slayer_Types.Count > 0 && wieldTier.slayer_MaxDamageBonus > 0.0 && Utils.getRandomNumberExclusive(100) < wieldTier.slayer_Chance * 100 * (1 + (creationInfo.qualityModifier * 2)))
                {
                    propertyCount++;
                    eCreatureType slayerType = wieldTier.slayer_Types[Utils.getRandomNumberExclusive(wieldTier.slayer_Types.Count)];
                    newItem.addOrUpdateStat(eIntStat.SlayerCreatureType, (int)slayerType);

                    newItem.addOrUpdateStat(eFloatStat.SlayerDamageBonus, Math.Round(Utils.getRandomDouble(wieldTier.slayer_MinDamageBonus, wieldTier.slayer_MaxDamageBonus, eRandomFormula.favorLow, 1.5, creationInfo.qualityModifier), 2));
                }

                if (propertyCount < wieldTier.maxPropertyAmount && Utils.getRandomNumberExclusive(100) < wieldTier.crushingBlow_Chance * 100 * (1 + (creationInfo.qualityModifier * 2)))
                {
                    propertyCount++;
                    newItem.addOrUpdateStat(eFloatStat.CriticalMultiplier, Math.Round(Utils.getRandomDouble(wieldTier.crushingBlow_MinCriticalMultiplier, wieldTier.crushingBlow_MaxCriticalMultiplier, eRandomFormula.favorLow, 1.5, creationInfo.qualityModifier), 2));
                }

                if (propertyCount < wieldTier.maxPropertyAmount && Utils.getRandomNumberExclusive(100) < wieldTier.bitingStrike_Chance * 100 * (1 + (creationInfo.qualityModifier * 2)))
                {
                    propertyCount++;
                    newItem.addOrUpdateStat(eFloatStat.CriticalFrequency, Math.Round(Utils.getRandomDouble(wieldTier.bitingStrike_MinCriticalFrequency, wieldTier.bitingStrike_MaxCriticalFrequency, eRandomFormula.favorLow, 1.5, creationInfo.qualityModifier), 2));
                }
            }
        }

        void mutateMeleeWeapon(cWeenie newItem, sWieldTier wieldTier, ref sItemCreationInfo creationInfo, sLootProfile lootProfile, sLootTier tier, sLootProfileCategory category, sItemLootProfileEntry entry)
        {
            newItem.addOrUpdateStat(eIntStat.Damage, newItem.getStat(eIntStat.Damage) + Utils.getRandomNumber(wieldTier.minDamageBonus, wieldTier.maxDamageBonus, eRandomFormula.favorMid, 2, creationInfo.qualityModifier));
            newItem.addOrUpdateStat(eFloatStat.DamageVariance, Math.Round(Utils.getRandomDouble(1f - wieldTier.highVariance, 1f - wieldTier.lowVariance, eRandomFormula.favorMid, 2, creationInfo.qualityModifier), 2));

            if (wieldTier.slowSpeedMod > 0 && wieldTier.fastSpeedMod > 0)
            {
                int weaponTime = newItem.getStat(eIntStat.WeaponTime);
                Double weaponTimeMod = Utils.getRandomDouble(wieldTier.fastSpeedMod, wieldTier.slowSpeedMod, eRandomFormula.favorHigh, 2, creationInfo.qualityModifier);
                newItem.addOrUpdateStat(eIntStat.WeaponTime, (int)Math.Round(weaponTime * weaponTimeMod));
            }

            if (wieldTier.maxAttackSkillBonus > 0 && Utils.getRandomNumberExclusive(100) < wieldTier.attackSkillBonusChance * 100 * (1 + (creationInfo.qualityModifier * 2)))
                newItem.addOrUpdateStat(eFloatStat.WeaponOffense, Math.Round(Utils.getRandomDouble(1, 1 + (wieldTier.maxAttackSkillBonus / 100), eRandomFormula.favorMid, 2, creationInfo.qualityModifier), 2));

            if(entry.elementalVariants.Count > 0)
            {
                if (Utils.getRandomNumberExclusive(100) < wieldTier.elementalChance * 100)
                {
                    int variantId = Utils.getRandomNumberExclusive(entry.elementalVariants.Count);

                    cWeenie elementalVariant;
                    if (weenies.TryGetValue(entry.elementalVariants[variantId], out elementalVariant))
                    {
                        eDamageType elementalType = (eDamageType)elementalVariant.getStat(eIntStat.DamageType);
                        newItem.addOrUpdateStat(eDidStat.Setup, elementalVariant.getStat(eDidStat.Setup));

                        if(elementalVariant.wcid == 3750) //acid battleaxe has the wrong graphics
                            newItem.addOrUpdateStat(eDidStat.Setup, 33555690);

                        switch (elementalType)
                        {
                            case eDamageType.Acid_Damage_Type:
                                {
                                    newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Acid_Damage_Type);
                                    newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Acid));
                                    newItem.addOrUpdateStat(eStringStat.Name, $"Acid {newItem.getStat(eStringStat.Name)}");
                                    break;
                                }
                            case eDamageType.Cold_Damage_Type:
                                {
                                    newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Cold_Damage_Type);
                                    newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Frost));
                                    newItem.addOrUpdateStat(eStringStat.Name, $"Frost {newItem.getStat(eStringStat.Name)}");
                                    break;
                                }
                            case eDamageType.Fire_Damage_Type:
                                {
                                    newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Fire_Damage_Type);
                                    newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Fire));
                                    newItem.addOrUpdateStat(eStringStat.Name, $"Flaming {newItem.getStat(eStringStat.Name)}");
                                    break;
                                }
                            case eDamageType.Electric_Damage_Type:
                                {
                                    newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Electric_Damage_Type);
                                    newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Lightning));
                                    newItem.addOrUpdateStat(eStringStat.Name, $"Lightning {newItem.getStat(eStringStat.Name)}");
                                    break;
                                }
                        }
                    }
                }
            }
        }

        void mutateMissileWeapon(cWeenie newItem, sWieldTier wieldTier, ref sItemCreationInfo creationInfo, sLootProfile lootProfile, sLootTier tier, sLootProfileCategory category, sItemLootProfileEntry entry)
        {
            newItem.addOrUpdateStat(eFloatStat.DamageMod, Math.Round(newItem.getStat(eFloatStat.DamageMod) + Utils.getRandomDouble(wieldTier.minDamageModBonus, wieldTier.maxDamageModBonus, eRandomFormula.favorMid, 2, creationInfo.qualityModifier), 2));

            if (wieldTier.slowSpeedMod > 0 && wieldTier.fastSpeedMod > 0)
            {
                int weaponTime = newItem.getStat(eIntStat.WeaponTime);
                Double weaponTimeMod = Utils.getRandomDouble(wieldTier.fastSpeedMod, wieldTier.slowSpeedMod, eRandomFormula.favorHigh, 2, creationInfo.qualityModifier);
                newItem.addOrUpdateStat(eIntStat.WeaponTime, (int)Math.Round(weaponTime * weaponTimeMod));
            }

            //if (Utils.getRandomNumberExclusive(100) < wieldTier.elementalChance * 100)
            if (wieldTier.maxElementalDamageBonus > 0)
            {
                int elementalDamage = Utils.getRandomNumber(wieldTier.minElementalDamageBonus, wieldTier.maxElementalDamageBonus, eRandomFormula.favorMid, 2);

                if (elementalDamage > 0)
                {
                    eElements elementalType = (eElements)Utils.getRandomNumber(1, 3);
                    if (Utils.getRandomNumberExclusive(100) < wieldTier.elementalChance * 100)
                        elementalType = (eElements)Utils.getRandomNumber(4, 7);

                    newItem.addOrUpdateStat(eIntStat.ElementalDamageBonus, elementalDamage);

                    switch (elementalType)
                    {
                        case eElements.acid:
                            {
                                newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Acid_Damage_Type);
                                newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Acid));
                                newItem.addOrUpdateStat(eStringStat.Name, $"Acid {newItem.getStat(eStringStat.Name)}");
                                break;
                            }
                        case eElements.cold:
                            {
                                newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Cold_Damage_Type);
                                newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Frost));
                                newItem.addOrUpdateStat(eStringStat.Name, $"Frost {newItem.getStat(eStringStat.Name)}");
                                break;
                            }
                        case eElements.fire:
                            {
                                newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Fire_Damage_Type);
                                newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Fire));
                                newItem.addOrUpdateStat(eStringStat.Name, $"Fire {newItem.getStat(eStringStat.Name)}");
                                break;
                            }
                        case eElements.lightning:
                            {
                                newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Electric_Damage_Type);
                                newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Lightning));
                                newItem.addOrUpdateStat(eStringStat.Name, $"Electric {newItem.getStat(eStringStat.Name)}");
                                break;
                            }
                        case eElements.bludgeoning:
                            {
                                newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Bludgeon_Damage_Type);
                                newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Bludgeoning));
                                newItem.addOrUpdateStat(eStringStat.Name, $"Blunt {newItem.getStat(eStringStat.Name)}");
                                break;
                            }
                        case eElements.piercing:
                            {
                                newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Pierce_Damage_Type);
                                newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Piercing));
                                newItem.addOrUpdateStat(eStringStat.Name, $"Piercing {newItem.getStat(eStringStat.Name)}");
                                break;
                            }
                        case eElements.slashing:
                            {
                                newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Slash_Damage_Type);
                                newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Slashing));
                                newItem.addOrUpdateStat(eStringStat.Name, $"Slashing {newItem.getStat(eStringStat.Name)}");
                                break;
                            }
                    }
                }
            }
        }

        void mutateCaster(cWeenie newItem, sWieldTier wieldTier, ref sItemCreationInfo creationInfo, sLootProfile lootProfile, sLootTier tier, sLootProfileCategory category, sItemLootProfileEntry entry)
        {
            if(wieldTier.weaponSkillRequired > 0)
            {
                List<eSkills> possibleRequirements = new List<eSkills>();
                //possibleRequirements.Add(eSkills.CreatureEnchantment);
                //possibleRequirements.Add(eSkills.ItemEnchantment);
                //possibleRequirements.Add(eSkills.LifeMagic);
                possibleRequirements.Add(eSkills.WarMagic);
                eSkills skill = possibleRequirements[getRandomNumberExclusive(possibleRequirements.Count)];
                newItem.addOrUpdateStat(eIntStat.WieldSkilltype, (int)(skill));

                creationInfo.otherMagicSchoolMasterySpells = new List<sPossibleSpells>();
                switch (skill)
                {
                    case eSkills.CreatureEnchantment:
                        creationInfo.favoredMagicSchoolMasterySpell = lootProfile.spells[lootProfile.spellNameToIdMap["Creature Enchantment Mastery"]];
                        creationInfo.otherMagicSchoolMasterySpells.Add(lootProfile.spells[lootProfile.spellNameToIdMap["Item Enchantment Mastery"]]);
                        creationInfo.otherMagicSchoolMasterySpells.Add(lootProfile.spells[lootProfile.spellNameToIdMap["Life Magic Mastery"]]);
                        creationInfo.otherMagicSchoolMasterySpells.Add(lootProfile.spells[lootProfile.spellNameToIdMap["War Magic Mastery"]]);
                        break;
                    case eSkills.ItemEnchantment:
                        creationInfo.favoredMagicSchoolMasterySpell = lootProfile.spells[lootProfile.spellNameToIdMap["Item Enchantment Mastery"]];
                        creationInfo.otherMagicSchoolMasterySpells.Add(lootProfile.spells[lootProfile.spellNameToIdMap["Creature Enchantment Mastery"]]);
                        creationInfo.otherMagicSchoolMasterySpells.Add(lootProfile.spells[lootProfile.spellNameToIdMap["Life Magic Mastery"]]);
                        creationInfo.otherMagicSchoolMasterySpells.Add(lootProfile.spells[lootProfile.spellNameToIdMap["War Magic Mastery"]]);
                        break;
                    case eSkills.LifeMagic:
                        creationInfo.favoredMagicSchoolMasterySpell = lootProfile.spells[lootProfile.spellNameToIdMap["Life Magic Mastery"]];
                        creationInfo.otherMagicSchoolMasterySpells.Add(lootProfile.spells[lootProfile.spellNameToIdMap["Creature Enchantment Mastery"]]);
                        creationInfo.otherMagicSchoolMasterySpells.Add(lootProfile.spells[lootProfile.spellNameToIdMap["Item Enchantment Mastery"]]);
                        creationInfo.otherMagicSchoolMasterySpells.Add(lootProfile.spells[lootProfile.spellNameToIdMap["War Magic Mastery"]]);
                        break;
                    case eSkills.WarMagic:
                        creationInfo.favoredMagicSchoolMasterySpell = lootProfile.spells[lootProfile.spellNameToIdMap["War Magic Mastery"]];
                        creationInfo.otherMagicSchoolMasterySpells.Add(lootProfile.spells[lootProfile.spellNameToIdMap["Creature Enchantment Mastery"]]);
                        creationInfo.otherMagicSchoolMasterySpells.Add(lootProfile.spells[lootProfile.spellNameToIdMap["Item Enchantment Mastery"]]);
                        creationInfo.otherMagicSchoolMasterySpells.Add(lootProfile.spells[lootProfile.spellNameToIdMap["Life Magic Mastery"]]);
                        break;
                }
            }

            if (wieldTier.maxManaConversionBonus > 0 && Utils.getRandomNumberExclusive(100) < wieldTier.manaConversionBonusChance * 100 * (1 + (creationInfo.qualityModifier * 2)))
            {
                double manaConversionMod = Math.Round(Utils.getRandomDouble(0, wieldTier.maxManaConversionBonus / 100, eRandomFormula.favorMid, 2, creationInfo.qualityModifier), 1);
                if (manaConversionMod > 0)
                    newItem.addOrUpdateStat(eFloatStat.ManaConversionMod, manaConversionMod);
            }

            //if (Utils.getRandomNumber(100, eRandomFormula.equalDistribution) < wieldTier.elementalChance * 100)
            if (wieldTier.maxElementalDamageMod > 0)
            {
                double elementalDamageMod = Math.Round(1 + Utils.getRandomDouble(wieldTier.minElementalDamageMod / 100, wieldTier.maxElementalDamageMod / 100, eRandomFormula.favorMid, 2), 2);

                if (elementalDamageMod > 1.0)
                {
                    eElements elementalType = (eElements)Utils.getRandomNumber(4, 7);
                    if (Utils.getRandomNumberExclusive(100) < wieldTier.elementalChance * 100)
                        elementalType = (eElements)Utils.getRandomNumber(1, 3);
                    newItem.addOrUpdateStat(eFloatStat.ElementalDamageMod, elementalDamageMod);

                    switch (elementalType)
                    {
                        case eElements.acid:
                            {
                                newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Acid_Damage_Type);
                                newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Acid));
                                newItem.addOrUpdateStat(eStringStat.Name, $"Searing {newItem.getStat(eStringStat.Name)}");
                                break;
                            }
                        case eElements.cold:
                            {
                                newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Cold_Damage_Type);
                                newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Frost));
                                newItem.addOrUpdateStat(eStringStat.Name, $"Freezing {newItem.getStat(eStringStat.Name)}");
                                break;
                            }
                        case eElements.fire:
                            {
                                newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Fire_Damage_Type);
                                newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Fire));
                                newItem.addOrUpdateStat(eStringStat.Name, $"Flaming {newItem.getStat(eStringStat.Name)}");
                                break;
                            }
                        case eElements.lightning:
                            {
                                newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Electric_Damage_Type);
                                newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Lightning));
                                newItem.addOrUpdateStat(eStringStat.Name, $"Zapping {newItem.getStat(eStringStat.Name)}");
                                break;
                            }
                        case eElements.bludgeoning:
                            {
                                newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Bludgeon_Damage_Type);
                                newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Bludgeoning));
                                newItem.addOrUpdateStat(eStringStat.Name, $"Smashing {newItem.getStat(eStringStat.Name)}");
                                break;
                            }
                        case eElements.piercing:
                            {
                                newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Pierce_Damage_Type);
                                newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Piercing));
                                newItem.addOrUpdateStat(eStringStat.Name, $"Prickly {newItem.getStat(eStringStat.Name)}");
                                break;
                            }
                        case eElements.slashing:
                            {
                                newItem.addOrUpdateStat(eIntStat.DamageType, (int)eDamageType.Slash_Damage_Type);
                                newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Slashing));
                                newItem.addOrUpdateStat(eStringStat.Name, $"Slicing {newItem.getStat(eStringStat.Name)}");
                                break;
                            }
                    }
                }
            }
        }

        void mutateArmor(cWeenie newItem, sWieldTier wieldTier, ref sItemCreationInfo creationInfo, sLootProfile lootProfile, sLootTier tier, sLootProfileCategory category, sItemLootProfileEntry entry)
        {
            newItem.addOrUpdateStat(eFloatStat.BulkMod, Math.Round(Utils.getRandomDouble(wieldTier.minBulkMod, wieldTier.maxBulkMod, eRandomFormula.favorMid, 1.4d, -creationInfo.qualityModifier), 2));

            double resistanceAcid = newItem.getStat(eFloatStat.ArmorModVsAcid);
            double resistanceElectric = newItem.getStat(eFloatStat.ArmorModVsElectric);
            double resistanceFire = newItem.getStat(eFloatStat.ArmorModVsFire);
            double resistanceCold = newItem.getStat(eFloatStat.ArmorModVsCold);
            double resistanceSlash = newItem.getStat(eFloatStat.ArmorModVsSlash);
            double resistancePierce = newItem.getStat(eFloatStat.ArmorModVsPierce);
            double resistanceBludgeon = newItem.getStat(eFloatStat.ArmorModVsBludgeon);

            resistanceAcid = Math.Min(resistanceAcid + (Utils.getRandomNumber(1, 2, eRandomFormula.favorLow, 1.4d, creationInfo.qualityModifier) / 10), 2);
            resistanceElectric = Math.Min(resistanceElectric + (Utils.getRandomNumber(1, 2, eRandomFormula.favorLow, 1.4d, creationInfo.qualityModifier) / 10), 2);
            resistanceFire = Math.Min(resistanceFire + (Utils.getRandomNumber(1, 2, eRandomFormula.favorLow, 1.4d, creationInfo.qualityModifier) / 10), 2);
            resistanceCold = Math.Min(resistanceCold + (Utils.getRandomNumber(1, 2, eRandomFormula.favorLow, 1.4d, creationInfo.qualityModifier) / 10), 2);
            resistanceSlash = Math.Min(resistanceSlash + (Utils.getRandomNumber(1, 2, eRandomFormula.favorLow, 1.4d, creationInfo.qualityModifier) / 10), 2);
            resistancePierce = Math.Min(resistancePierce + (Utils.getRandomNumber(1, 2, eRandomFormula.favorLow, 1.4d, creationInfo.qualityModifier) / 10), 2);
            resistanceBludgeon = Math.Min(resistanceBludgeon + (Utils.getRandomNumber(1, 2, eRandomFormula.favorLow, 1.4d, creationInfo.qualityModifier) / 10), 2);

            if (Utils.getRandomNumberExclusive(100) < 25 * (1 + (creationInfo.qualityModifier * 2)))
                newItem.addOrUpdateStat(eFloatStat.ArmorModVsAcid, resistanceAcid);
            if (Utils.getRandomNumberExclusive(100) < 25 * (1 + (creationInfo.qualityModifier * 2)))
                newItem.addOrUpdateStat(eFloatStat.ArmorModVsElectric, resistanceElectric);
            if (Utils.getRandomNumberExclusive(100) < 25 * (1 + (creationInfo.qualityModifier * 2)))
                newItem.addOrUpdateStat(eFloatStat.ArmorModVsFire, resistanceFire);
            if (Utils.getRandomNumberExclusive(100) < 25 * (1 + (creationInfo.qualityModifier * 2)))
                newItem.addOrUpdateStat(eFloatStat.ArmorModVsCold, resistanceCold);
            if (Utils.getRandomNumberExclusive(100) < 25 * (1 + (creationInfo.qualityModifier * 2)))
                newItem.addOrUpdateStat(eFloatStat.ArmorModVsSlash, resistanceSlash);
            if (Utils.getRandomNumberExclusive(100) < 25 * (1 + (creationInfo.qualityModifier * 2)))
                newItem.addOrUpdateStat(eFloatStat.ArmorModVsPierce, resistancePierce);
            if (Utils.getRandomNumberExclusive(100) < 25 * (1 + (creationInfo.qualityModifier * 2)))
                newItem.addOrUpdateStat(eFloatStat.ArmorModVsBludgeon, resistanceBludgeon);

            eInventoryLocations coveredAreas = (eInventoryLocations)newItem.getStat(eIntStat.ValidLocations);
            int armorLevel = newItem.getStat(eIntStat.ArmorLevel);
            if (coveredAreas.HasFlag(eInventoryLocations.Shield_Loc))
                armorLevel += Utils.getRandomNumber(wieldTier.minShieldArmorBonus, wieldTier.maxShieldArmorBonus, eRandomFormula.favorMid, 1.4d, creationInfo.qualityModifier);
            else
                armorLevel += Utils.getRandomNumber(wieldTier.minArmorBonus, wieldTier.maxArmorBonus, eRandomFormula.favorMid, 1.4d, creationInfo.qualityModifier);
            newItem.addOrUpdateStat(eIntStat.ArmorLevel, armorLevel);

            List<eSkills> possibleRequirementTypes = new List<eSkills>();
            if (wieldTier.meleeDefenseSkillRequired != 0)
                possibleRequirementTypes.Add(eSkills.MeleeDefense);
            if (wieldTier.missileDefenseSkillRequired != 0)
                possibleRequirementTypes.Add(eSkills.MissileDefense);
            if (wieldTier.magicDefenseSkillRequired != 0)
                possibleRequirementTypes.Add(eSkills.MagicDefense);

            bool hasRequirement1 = false;
            if (possibleRequirementTypes.Count > 0)
            {
                eSkills requirementType = possibleRequirementTypes[getRandomNumberExclusive(possibleRequirementTypes.Count)];
                newItem.addOrUpdateStat(eIntStat.WieldRequirements, (int)eWieldRequirements.baseSkill);
                newItem.addOrUpdateStat(eIntStat.WieldSkilltype, (int)requirementType);
                hasRequirement1 = true;
                switch (requirementType)
                {
                    case eSkills.MeleeDefense:
                        newItem.addOrUpdateStat(eIntStat.WieldDifficulty, (int)wieldTier.meleeDefenseSkillRequired);
                        break;
                    case eSkills.MissileDefense:
                        newItem.addOrUpdateStat(eIntStat.WieldDifficulty, (int)wieldTier.missileDefenseSkillRequired);
                        break;
                    case eSkills.MagicDefense:
                        newItem.addOrUpdateStat(eIntStat.WieldDifficulty, (int)wieldTier.magicDefenseSkillRequired);
                        break;
                }
            }

            if (lootProfile.otherOptions.extraMutations)
            {
                if (wieldTier.minLevel > 0)
                {
                    if (hasRequirement1)
                    {
                        newItem.addOrUpdateStat(eIntStat.WieldRequirements2, (int)eWieldRequirements.level);
                        newItem.addOrUpdateStat(eIntStat.WieldSkilltype2, 1);
                        newItem.addOrUpdateStat(eIntStat.WieldDifficulty2, wieldTier.minLevel);
                    }
                    else
                    {
                        newItem.addOrUpdateStat(eIntStat.WieldRequirements, (int)eWieldRequirements.level);
                        newItem.addOrUpdateStat(eIntStat.WieldSkilltype, 1);
                        newItem.addOrUpdateStat(eIntStat.WieldDifficulty, wieldTier.minLevel);
                    }
                }
            }

        }

        bool mutateItem(cWeenie newItem, cWeenie baseitem, ref sItemCreationInfo creationInfo, sLootProfile lootProfile, sLootTier tier, sLootProfileCategory category, sItemLootProfileEntry entry)
        {
            eItemType type = (eItemType)newItem.getStat(eIntStat.ItemType);
            creationInfo.isMagical = false;

            if (type != eItemType.Type_Melee_Weapon &&
                type != eItemType.Type_Missile_Weapon &&
                type != eItemType.Type_Caster &&
                type != eItemType.Type_Armor &&
                type != eItemType.Type_Clothing &&
                type != eItemType.Type_Jewelry)
            {
                Console.WriteLine("Unmutable item type: {0} (wcid: {1} - entry: {2} - category: {3})", type.ToString(), entry.wcid, entry.name, category.category);
                return false;
            }

            int itemWorkmanship = Utils.getRandomNumber(tier.minWorkmanship, tier.maxWorkmanship, eRandomFormula.favorMid, 2, creationInfo.qualityModifier);
            newItem.addOrUpdateStat(eIntStat.ItemWorkmanship, itemWorkmanship);

            if (type == eItemType.Type_Melee_Weapon ||
                type == eItemType.Type_Missile_Weapon ||
                type == eItemType.Type_Caster)
            {
                if (category.wieldTiers != null && category.wieldTiers.Count > 0)
                {
                    List<sWieldTier> validWieldTiers = new List<sWieldTier>();
                    sWieldTier wieldTier;
                    if (type == eItemType.Type_Melee_Weapon || type == eItemType.Type_Missile_Weapon || type == eItemType.Type_Caster)
                    {
                        foreach (sWieldTier possibleWieldTier in category.wieldTiers)
                        {
                            if (type == eItemType.Type_Melee_Weapon &&
                                possibleWieldTier.weaponSkillRequired >= tier.minMeleeWeaponWieldTier &&
                                possibleWieldTier.weaponSkillRequired <= tier.maxMeleeWeaponWieldTier)
                            {
                                validWieldTiers.Add(possibleWieldTier);
                            }
                            else if (type == eItemType.Type_Missile_Weapon &&
                                     possibleWieldTier.weaponSkillRequired >= tier.minMissileWeaponWieldTier &&
                                     possibleWieldTier.weaponSkillRequired <= tier.maxMissileWeaponWieldTier)
                            {
                                validWieldTiers.Add(possibleWieldTier);
                            }
                            else if (type == eItemType.Type_Caster &&
                                     possibleWieldTier.weaponSkillRequired >= tier.minCasterWieldTier &&
                                     possibleWieldTier.weaponSkillRequired <= tier.maxCasterWieldTier)
                            {
                                validWieldTiers.Add(possibleWieldTier);
                            }
                        }
                        wieldTier = validWieldTiers[Utils.getRandomNumberExclusive(validWieldTiers.Count, eRandomFormula.favorMid, 2)];

                        mutateWeapon(newItem, wieldTier, ref creationInfo, lootProfile, tier, category, entry);
                        if (type == eItemType.Type_Melee_Weapon)
                            mutateMeleeWeapon(newItem, wieldTier, ref creationInfo, lootProfile, tier, category, entry);
                        else if (type == eItemType.Type_Missile_Weapon)
                            mutateMissileWeapon(newItem, wieldTier, ref creationInfo, lootProfile, tier, category, entry);
                        else if (type == eItemType.Type_Caster)
                            mutateCaster(newItem, wieldTier, ref creationInfo, lootProfile, tier, category, entry);
                    }
                }
            }
            else if (type == eItemType.Type_Armor)
            {
                List<sWieldTier> wieldTiers;
                List<sWieldTier> validWieldTiers = new List<sWieldTier>();
                sWieldTier wieldTier;
                if (category.wieldTiers != null && category.wieldTiers.Count > 0)
                    wieldTiers = category.wieldTiers;
                else
                    wieldTiers = lootProfile.armor.wieldTiers;
                foreach (sWieldTier possibleWieldTier in wieldTiers)
                {
                    if ((possibleWieldTier.armorWieldTier >= tier.minArmorWieldTier && possibleWieldTier.armorWieldTier <= tier.maxArmorWieldTier) ||
                       (possibleWieldTier.meleeDefenseSkillRequired != 0 && possibleWieldTier.meleeDefenseSkillRequired >= tier.minMeleeWeaponWieldTier && possibleWieldTier.meleeDefenseSkillRequired <= tier.maxMeleeWeaponWieldTier) &&
                       (possibleWieldTier.missileDefenseSkillRequired != 0 && possibleWieldTier.missileDefenseSkillRequired >= tier.minArmorMissileWieldTier && possibleWieldTier.missileDefenseSkillRequired <= tier.maxArmorMissileWieldTier) &&
                       (possibleWieldTier.magicDefenseSkillRequired != 0 && possibleWieldTier.magicDefenseSkillRequired >= tier.minArmorMagicWieldTier && possibleWieldTier.magicDefenseSkillRequired <= tier.maxArmorMagicWieldTier))
                    {
                        eInventoryLocations coveredAreas = (eInventoryLocations)newItem.getStat(eIntStat.ValidLocations);
                        if (coveredAreas.HasFlag(eInventoryLocations.Shield_Loc))
                        {
                            //if both shieldMinArmorBonus and shieldMaxArmorBonus are 0 it means there's no shield in this wieldTier.
                            if (possibleWieldTier.minShieldArmorBonus != 0 || possibleWieldTier.maxShieldArmorBonus != 0)
                                validWieldTiers.Add(possibleWieldTier);
                        }
                        else
                            validWieldTiers.Add(possibleWieldTier);
                    }
                }
                if (validWieldTiers.Count == 0)
                    return false;
                wieldTier = validWieldTiers[Utils.getRandomNumberExclusive(validWieldTiers.Count, eRandomFormula.favorMid, 2)];
                mutateArmor(newItem, wieldTier, ref creationInfo, lootProfile, tier, category, entry);
            }

            eMaterialType materialType = 0;
            List<eMaterialType> possibleMaterialsList = new List<eMaterialType>();
            if (entry.possibleMaterials != null && entry.possibleMaterials.Count > 0)
            {
                if (entry.possibleMaterials.Contains(eMaterialType.ceramic))
                    possibleMaterialsList.AddRange(tier.materialsCeramic);
                if (entry.possibleMaterials.Contains(eMaterialType.cloth))
                    possibleMaterialsList.AddRange(tier.materialsCloth);
                if (entry.possibleMaterials.Contains(eMaterialType.gem))
                    possibleMaterialsList.AddRange(tier.materialsGem);
                if (entry.possibleMaterials.Contains(eMaterialType.leather))
                    possibleMaterialsList.AddRange(tier.materialsLeather);
                if (entry.possibleMaterials.Contains(eMaterialType.metal))
                    possibleMaterialsList.AddRange(tier.materialsMetal);
                if (entry.possibleMaterials.Contains(eMaterialType.stone))
                    possibleMaterialsList.AddRange(tier.materialsStone);
                if (entry.possibleMaterials.Contains(eMaterialType.wood))
                    possibleMaterialsList.AddRange(tier.materialsWood);
            }
            else if (category.possibleMaterials != null && category.possibleMaterials.Count > 0)
            { //if we do not have our own materials entry try the category
                if (category.possibleMaterials.Contains(eMaterialType.ceramic))
                    possibleMaterialsList.AddRange(tier.materialsCeramic);
                if (category.possibleMaterials.Contains(eMaterialType.cloth))
                    possibleMaterialsList.AddRange(tier.materialsCloth);
                if (category.possibleMaterials.Contains(eMaterialType.gem))
                    possibleMaterialsList.AddRange(tier.materialsGem);
                if (category.possibleMaterials.Contains(eMaterialType.leather))
                    possibleMaterialsList.AddRange(tier.materialsLeather);
                if (category.possibleMaterials.Contains(eMaterialType.metal))
                    possibleMaterialsList.AddRange(tier.materialsMetal);
                if (category.possibleMaterials.Contains(eMaterialType.stone))
                    possibleMaterialsList.AddRange(tier.materialsStone);
                if (category.possibleMaterials.Contains(eMaterialType.wood))
                    possibleMaterialsList.AddRange(tier.materialsWood);
            }

            sClothingTable clothingTable;
            int ClothingBase = newItem.getStat(eDidStat.ClothingBase);
            if (ClothingBase != 0)
            {
                if (clothingTables.TryGetValue((uint)ClothingBase, out clothingTable))
                {
                    int palette = 0;

                    if ((type == eItemType.Type_Armor || type == eItemType.Type_Clothing) && baseitem.wcid != 296) //wcid 296 is the crown, which is treated like jewelry as far as color is concerned
                    {
                        List<int> validEntries = new List<int>();
                        foreach (KeyValuePair<uint, sClothingTableSubPaletteEffect> subPaletteEffect in clothingTable.subPaletteEffects)
                        {
                            if (subPaletteEffect.Key < 84 && subPaletteEffect.Key != 25 && subPaletteEffect.Key != 27 && subPaletteEffect.Key != 28)
                                validEntries.Add((int)subPaletteEffect.Key);
                        }

                        int materialIndex = Utils.getRandomNumberExclusive(possibleMaterialsList.Count);
                        materialType = possibleMaterialsList[materialIndex];

                        int paletteId = Utils.getRandomNumberExclusive(validEntries.Count);
                        palette = validEntries[paletteId];
                    }
                    else
                    {
                        Dictionary<eMaterialType, uint> materialsWithMatchedPalettes = new Dictionary<eMaterialType, uint>();
                        List<eMaterialType> missingMaterials = new List<eMaterialType>();
                        foreach (eMaterialType material in possibleMaterialsList)
                        {
                            bool found = false;
                            foreach (KeyValuePair<uint, sClothingTableSubPaletteEffect> subPaletteEffect in clothingTable.subPaletteEffects)
                            {
                                if (lootProfile.materialProperties[material].palettes.Contains((int)subPaletteEffect.Key))
                                {
                                    found = true;
                                    materialsWithMatchedPalettes.Add(material, subPaletteEffect.Key);
                                    break;
                                }
                            }
                            if (!found)
                                missingMaterials.Add(material);
                        }

                        if (missingMaterials.Count > 0)
                        {
                            Console.WriteLine("Item with missing material->palette conversion: {0} - {1}", newItem.wcid, newItem.getStat(eStringStat.Name));
                            foreach(eMaterialType material in missingMaterials)
                            {
                                Console.WriteLine(" - {0}", material.ToString());
                            }
                        }

                        List<eMaterialType> validEntries = new List<eMaterialType>(materialsWithMatchedPalettes.Keys);
                        int materialIndex = Utils.getRandomNumberExclusive(validEntries.Count);
                        materialType = validEntries[materialIndex];
                        palette = (int)materialsWithMatchedPalettes[validEntries[materialIndex]];
                    }

                    if (materialType != 0 && materialType != eMaterialType.leather) //for generic leather we don't need to setup the materialType at all, otherwise it will not display correctly in the client.
                        newItem.addOrUpdateStat(eIntStat.MaterialType, (int)materialType);

                    newItem.addOrUpdateStat(eIntStat.PaletteTemplate, palette);
                    newItem.addOrUpdateStat(eFloatStat.Shade, Utils.getRandomDouble(1));
                    newItem.addOrUpdateStat(eFloatStat.Shade2, Utils.getRandomDouble(1));
                    newItem.addOrUpdateStat(eFloatStat.Shade3, Utils.getRandomDouble(1));
                    newItem.addOrUpdateStat(eFloatStat.Shade4, Utils.getRandomDouble(1));
                }
            }

            //make sure we clean the item of any existing spells
            if (newItem.dataFlags.HasFlag(eDataFlags.spellBook))
                newItem.dataFlags &= ~(eDataFlags.spellBook);
            newItem.spellBook = new sSpellBook();
            addSpells(newItem, ref creationInfo, lootProfile, tier, category, entry);

            Single value = newItem.getStat(eIntStat.Value);
            value *= lootProfile.workmanshipProperties[itemWorkmanship].valueMultiplier;
            value *= lootProfile.materialProperties[materialType].valueMultiplier;

            if(creationInfo.isMagical)
                value += creationInfo.totalPower * 5;

            if(type == eItemType.Type_Melee_Weapon)
            {
                eDamageType damageType = (eDamageType)newItem.getStat(eIntStat.DamageType);
                if (damageType == eDamageType.Acid_Damage_Type || damageType == eDamageType.Fire_Damage_Type || damageType == eDamageType.Cold_Damage_Type || damageType == eDamageType.Electric_Damage_Type)
                    value *= 1.25f;
            }
            else if (type == eItemType.Type_Missile_Weapon)
            {
                int elementalDamageBonus = newItem.getStat(eIntStat.ElementalDamageBonus);
                if(elementalDamageBonus > 0)
                    value *= 1.25f;
            }
            else if (type == eItemType.Type_Caster)
            {
                Double elementalDamageMod = newItem.getStat(eFloatStat.ElementalDamageMod);
                if (elementalDamageMod > 0)
                    value *= 1.25f;
            }

            int gemCount = 0;
            eMaterialType gem = 0;
            int maxGemCount = lootProfile.workmanshipProperties[itemWorkmanship].maxGemCount;
            if(maxGemCount > 0)
            {
                if (Utils.getRandomNumberExclusive(100) < lootProfile.workmanshipProperties[itemWorkmanship].gemChance * 100)
                {
                    gemCount = Utils.getRandomNumber(1, maxGemCount, eRandomFormula.favorLow, 2);

                    int gemTypeIndex = Utils.getRandomNumberExclusive(tier.materialsGem.Count - 1); //-1 because ivory is the last gem type in the profile and we don't want that
                    gem = tier.materialsGem[gemTypeIndex];
                    value += lootProfile.materialProperties[gem].gemValue * gemCount;
                    newItem.addOrUpdateStat(eIntStat.GemCount, gemCount);
                    newItem.addOrUpdateStat(eIntStat.GemType, (int)gem);
                }
            }

            value = (Single)Utils.getRandomDouble(value * 0.9, value * 1.1);
            newItem.addOrUpdateStat(eIntStat.Value, (int)value);

            string longDesc = "";
            if (lootProfile.otherOptions.extraMutations)
            {
                if (type == eItemType.Type_Caster || type == eItemType.Type_Melee_Weapon || type == eItemType.Type_Missile_Weapon)
                {
                    if (newItem.hasStat(eFloatStat.SlayerDamageBonus))
                    {
                        if (longDesc != "")
                            longDesc += "\\n";
                        longDesc += $"Slayer Damage Multiplier: {(newItem.getStat(eFloatStat.SlayerDamageBonus) * 100).ToString("0")}%";
                    }

                    if (newItem.hasStat(eFloatStat.CriticalFrequency))
                    {
                        if (longDesc != "")
                            longDesc += "\\n";
                        longDesc += $"Critical Hit Chance: {(newItem.getStat(eFloatStat.CriticalFrequency) * 100).ToString("0")}%";
                    }
                    else
                    {
                        if (longDesc != "")
                            longDesc += "\\n";
                        if (type == eItemType.Type_Caster)
                            longDesc += "Critical Hit Chance: 5%";
                        else
                            longDesc += "Critical Hit Chance: 10%";
                    }

                    if (newItem.hasStat(eFloatStat.CriticalMultiplier))
                    {
                        if (longDesc != "")
                            longDesc += "\\n";
                        if (type == eItemType.Type_Caster)
                            longDesc += $"Critical Hit Damage Multiplier: {((newItem.getStat(eFloatStat.CriticalMultiplier) / 0.5 ) * 100 * 1.25)}%";
                        else
                            longDesc += $"Critical Hit Damage Multiplier: {((newItem.getStat(eFloatStat.CriticalMultiplier) / 0.5) * 100 * 1.5)}%";

                    }
                    else
                    {
                        if (longDesc != "")
                            longDesc += "\\n";
                        if (type == eItemType.Type_Caster)
                            longDesc += "Critical Hit Damage Multiplier: 125%";
                        else
                            longDesc += "Critical Hit Damage Multiplier: 150%";
                    }
                }

                if (longDesc != "")
                    longDesc += "\\n";
                longDesc += $"Tier: {tier.name.Replace("Tier ", string.Empty)}";
                if (creationInfo.qualityModifier > 0)
                    longDesc += $"(Quality)"; //{creationInfo.qualityModifier.ToString("0.00")}";

                if (gemCount > 0)
                {
                    if (longDesc != "")
                        longDesc += "\\n";

                    string gemName = gem.ToString();
                    if (lootProfile.materialProperties[gem].gemName != null)
                        gemName = lootProfile.materialProperties[gem].gemName;

                    if(gemCount > 1)
                    {
                        if (lootProfile.materialProperties[gem].gemPluralName != null)
                            gemName = lootProfile.materialProperties[gem].gemPluralName;
                        else
                            gemName += "s";
                    }

                    gemName = gemName[0].ToString().ToUpper() + gemName.Substring(1);

                    longDesc += $"Gem: {gemCount} {gemName}";
                }

                newItem.addOrUpdateStat(eStringStat.LongDesc, longDesc);
                newItem.removeStat(eIntStat.AppraisalLongDescDecoration);
            }
            else if (gemCount > 0)
            {
                longDesc = newItem.getStat(eStringStat.Name);
                newItem.addOrUpdateStat(eStringStat.LongDesc, longDesc);
                newItem.addOrUpdateStat(eIntStat.AppraisalLongDescDecoration, (int)(eAppraisalLongDescDecorations.prependWorkmanship | eAppraisalLongDescDecorations.prependMaterial | eAppraisalLongDescDecorations.appendGemInfo));
            }


            ////info gathering
            //if (!infoMap.ContainsKey(tier.name))
            //    infoMap.Add(tier.name, new tierAnalysisInfo());

                //infoMap[tier.name].addValueEntry((int)value, newItem.wcid);
                //if (creationInfo.isMagical)
                //{
                //    infoMap[tier.name].addArcaneLoreEntry(newItem.getStat(eIntStat.ItemDifficulty), newItem.wcid);
                //}

            return true;
        }

        List<sPossibleSpells> mergeSpellLists(List<sPossibleSpells> list1, List<sPossibleSpells> list2)
        {
            List<sPossibleSpells> mergedList = new List<sPossibleSpells>(list1);

            foreach (sPossibleSpells spell2 in list2)
            {
                bool isRepeat = false;
                foreach (sPossibleSpells spell1 in list1)
                {
                    if (spell1.id == spell2.id)
                    {
                        isRepeat = true;
                        break;
                    }
                }
                if (!isRepeat)
                    mergedList.Add(spell2);
            }

            return mergedList;
        }

        void addSpells(cWeenie newItem, ref sItemCreationInfo creationInfo, sLootProfile lootProfile, sLootTier tier, sLootProfileCategory category, sItemLootProfileEntry entry)
        {
            eItemType type = (eItemType)newItem.getStat(eIntStat.ItemType);
            eInventoryLocations coveredAreas = (eInventoryLocations)newItem.getStat(eIntStat.ValidLocations);

            creationInfo.spellAmountMultiplier = 1;
            switch (type)
            {
                case eItemType.Type_Melee_Weapon:
                    addSpell(newItem, lootProfile.meleeWeaponSpells, ref creationInfo, lootProfile, tier, category, entry);
                    break;
                case eItemType.Type_Missile_Weapon:
                    addSpell(newItem, lootProfile.missileWeaponSpells, ref creationInfo, lootProfile, tier, category, entry);
                    break;
                case eItemType.Type_Caster:
                    addSpell(newItem, lootProfile.casterSpells, ref creationInfo, lootProfile, tier, category, entry);
                    break;
                case eItemType.Type_Clothing:
                    if (coveredAreas.HasFlag(eInventoryLocations.Head_Wear_Loc))
                        addSpell(newItem, lootProfile.clothingHeadSpells, ref creationInfo, lootProfile, tier, category, entry);
                    else if (coveredAreas.HasFlag(eInventoryLocations.Hand_Wear_Loc))
                        addSpell(newItem, lootProfile.clothingHandsSpells, ref creationInfo, lootProfile, tier, category, entry);
                    else if (coveredAreas.HasFlag(eInventoryLocations.Foot_Wear_Loc))
                        addSpell(newItem, lootProfile.clothingFeetSpells, ref creationInfo, lootProfile, tier, category, entry);
                    else
                        addSpell(newItem, lootProfile.clothingSpells, ref creationInfo, lootProfile, tier, category, entry);
                    break;
                case eItemType.Type_Jewelry:
                    addSpell(newItem, lootProfile.jewelrySpells, ref creationInfo, lootProfile, tier, category, entry);
                    break;
                case eItemType.Type_Armor:
                    int spellAmountMultiplier = 0;
                    List<sPossibleSpells> armorSpells = new List<sPossibleSpells>(lootProfile.armorItemSpells);

                    if (coveredAreas.HasFlag(eInventoryLocations.Shield_Loc))
                    {
                        armorSpells = mergeSpellLists(armorSpells, lootProfile.shieldSpells);
                        spellAmountMultiplier++;
                    }

                    if (coveredAreas.HasFlag(eInventoryLocations.Head_Wear_Loc))
                    {
                        armorSpells = mergeSpellLists(armorSpells, lootProfile.armorHeadSpells);
                        spellAmountMultiplier++;
                    }

                    if (coveredAreas.HasFlag(eInventoryLocations.Chest_Armor_Loc))
                    {
                        armorSpells = mergeSpellLists(armorSpells, lootProfile.armorUpperBodySpells);
                        spellAmountMultiplier++;
                    }

                    if (coveredAreas.HasFlag(eInventoryLocations.Upper_Arm_Armor_Loc))
                    {
                        armorSpells = mergeSpellLists(armorSpells, lootProfile.armorUpperBodySpells);
                        spellAmountMultiplier++;
                    }

                    if (coveredAreas.HasFlag(eInventoryLocations.Lower_Arm_Armor_Loc))
                    {
                        armorSpells = mergeSpellLists(armorSpells, lootProfile.armorUpperBodySpells);
                        spellAmountMultiplier++;
                    }

                    if (coveredAreas.HasFlag(eInventoryLocations.Abdomen_Armor_Loc))
                    {
                        //leg armor that also cover the abdomen do not have abdomen specific spells
                        if (!coveredAreas.HasFlag(eInventoryLocations.Upper_Leg_Armor_Loc) && !coveredAreas.HasFlag(eInventoryLocations.Lower_Leg_Armor_Loc))
                        {
                            armorSpells = mergeSpellLists(armorSpells, lootProfile.armorUpperBodySpells);
                            spellAmountMultiplier++;
                        }
                    }

                    if (coveredAreas.HasFlag(eInventoryLocations.Hand_Wear_Loc))
                    {
                        armorSpells = mergeSpellLists(armorSpells, lootProfile.armorHandsSpells);
                        spellAmountMultiplier++;
                    }

                    if (coveredAreas.HasFlag(eInventoryLocations.Upper_Leg_Armor_Loc))
                    {
                        armorSpells = mergeSpellLists(armorSpells, lootProfile.armorLowerBodySpells);
                        spellAmountMultiplier++;
                    }

                    if (coveredAreas.HasFlag(eInventoryLocations.Lower_Leg_Armor_Loc))
                    {
                        armorSpells = mergeSpellLists(armorSpells, lootProfile.armorLowerBodySpells);
                        spellAmountMultiplier++;
                    }

                    if (coveredAreas.HasFlag(eInventoryLocations.Foot_Wear_Loc))
                    {
                        armorSpells = mergeSpellLists(armorSpells, lootProfile.armorFeetSpells);
                        spellAmountMultiplier++;
                    }

                    creationInfo.spellAmountMultiplier = spellAmountMultiplier;

                    addSpell(newItem, armorSpells, ref creationInfo, lootProfile, tier, category, entry);
                    break;
                default:
                    break;
            }

            if (creationInfo.isMagical)
            {
                Interlocked.Increment(ref numMagicItems);
                if (!newItem.dataFlags.HasFlag(eDataFlags.spellBook))
                    newItem.dataFlags |= eDataFlags.spellBook;

                if (newItem.getStat(eIntStat.UiEffects) == 0)
                    newItem.addOrUpdateStat(eIntStat.UiEffects, (int)(eUIEffectType.UI_Effect_Magical));

                newItem.addOrUpdateStat(eIntStat.ItemSpellcraft, creationInfo.totalPower);
                newItem.addOrUpdateStat(eIntStat.ItemMaxMana, creationInfo.totalMana);
                newItem.addOrUpdateStat(eIntStat.ItemCurMana, Utils.getRandomNumber(creationInfo.totalMana / 2, creationInfo.totalMana, eRandomFormula.favorMid, 2));

                double averagePower = (double)creationInfo.totalPower / creationInfo.totalSpellsCount;

                double manaRate = Math.Min(-1 * creationInfo.totalSpellsCount * averagePower / 5000, -0.0166);
                manaRate = Math.Min(Utils.getRandomDouble(manaRate * 1.1, manaRate * 0.9, eRandomFormula.favorMid, 2), -0.0166);
                newItem.addOrUpdateStat(eFloatStat.ManaRate, manaRate);

                Double arcaneLoreModifier = 1.0;
                if (creationInfo.totalSpellsCount >= tier.minSpellsForHeritageRequirement)
                {
                    if (Utils.getRandomNumberExclusive(100) < tier.heritageRequirementChance * 100)
                    {
                        eHeritageGroups heritageRequirement = (eHeritageGroups)Utils.getRandomNumber(1, 3);
                        newItem.addOrUpdateStat(eIntStat.HeritageGroup, (int)heritageRequirement);
                        arcaneLoreModifier -= 0.20;
                    }
                }

                if (creationInfo.totalSpellsCount >= tier.minSpellsForAllegianceRankRequirement && tier.maxAllegianceRankRequired > 0)
                {
                    if (Utils.getRandomNumberExclusive(100) < tier.allegianceRankRequirementChance * 100)
                    {
                        int favoredRank = (int)Math.Ceiling(tier.maxAllegianceRankRequired * 0.4);
                        favoredRank = Math.Max(favoredRank, 1);
                        int allegianceRankRequirement = Utils.getRandomNumber(1, tier.maxAllegianceRankRequired, eRandomFormula.favorSpecificValue, favoredRank, 3);
                        newItem.addOrUpdateStat(eIntStat.ItemAllegianceRankLimit, allegianceRankRequirement);
                        arcaneLoreModifier -= (allegianceRankRequirement * 0.08);
                    }
                }

                int cappedTotalSpellsCount = creationInfo.totalSpellsCount;
                if(creationInfo.totalSpellsCount > tier.maxAmountOfSpells)
                    cappedTotalSpellsCount = tier.maxAmountOfSpells; //cap amount of spells influence on arcane lore on items that cover more than one body part.

                int arcaneLoreRequirement = (int)Math.Round((Math.Pow(averagePower, 2) + ((creationInfo.totalSpellsCount - 1) * averagePower * 15))/200);
                arcaneLoreRequirement = (int)Math.Round(Utils.getRandomDouble(arcaneLoreRequirement * 0.9, arcaneLoreRequirement * 1.1, eRandomFormula.favorMid, 2));
                arcaneLoreRequirement = (int)Math.Round(arcaneLoreRequirement * arcaneLoreModifier);
                arcaneLoreRequirement = Math.Max(arcaneLoreRequirement, 5);
                newItem.addOrUpdateStat(eIntStat.ItemDifficulty, arcaneLoreRequirement);
            }
            else
                Interlocked.Increment(ref numMundaneItems);
        }

        void addSpell(cWeenie newItem, List<sPossibleSpells> possibleSpellsTemplate, ref sItemCreationInfo creationInfo, sLootProfile lootProfile, sLootTier tier, sLootProfileCategory category, sItemLootProfileEntry entry)
        {
            if (possibleSpellsTemplate == null || possibleSpellsTemplate.Count == 0)
                return;

            if (tier.maxAmountOfSpells > 0 && tier.minSpellLevel > 0 && tier.maxSpellLevel > 0)
            {
                //we have possible spells
                eItemType itemType = (eItemType)newItem.getStat(eIntStat.ItemType);
                //make it so clothing and jewelry always have at least 1 spell
                if (Utils.getRandomNumberExclusive(100) < tier.chanceOfSpells * 100 * (1 + (creationInfo.qualityModifier * 2)) || itemType == eItemType.Type_Jewelry || itemType == eItemType.Type_Clothing)
                {
                    List<sPossibleSpells> possibleSpells = new List<sPossibleSpells>(possibleSpellsTemplate);
                    int spellCount = Utils.getRandomNumber(1, tier.maxAmountOfSpells, eRandomFormula.favorLow, 3, creationInfo.qualityModifier);
                    for (int i = 1; i < creationInfo.spellAmountMultiplier; i++)
                        spellCount += Utils.getRandomNumber(0, tier.maxAmountOfSpells, eRandomFormula.favorLow, 3, creationInfo.qualityModifier);

                    for (int i = 0; i < spellCount; i++)
                    {
                        int spellRoll = Utils.getRandomNumberExclusive(possibleSpells.Count);
                        sPossibleSpells possibleSpell = possibleSpells[spellRoll];

                        if (!creationInfo.hasAlreadyReplacedSpell && possibleSpell.spellCategory == eSpellCategory.magicSkillMastery && creationInfo.favoredMagicSchoolMasterySpell.id != 0 && creationInfo.otherMagicSchoolMasterySpells != null && creationInfo.otherMagicSchoolMasterySpells.Count > 0)
                        {
                            //replace magic school mastery spell for the one that the item has a requirement for.
                            foreach (sPossibleSpells spellToBeReplaced in creationInfo.otherMagicSchoolMasterySpells)
                            {
                                if (possibleSpell.id == spellToBeReplaced.id)
                                {
                                    possibleSpell = creationInfo.favoredMagicSchoolMasterySpell;
                                    creationInfo.hasAlreadyReplacedSpell = true;
                                    break;
                                }
                            }
                        }

                        List<int> spellLevelVariants = possibleSpell.spells;
                        int minSpellVariantId = tier.minSpellLevel - 1;
                        int maxSpellVariantId = Math.Min(tier.maxSpellLevel, spellLevelVariants.Count) - 1;

                        int spellVariantIndex = Utils.getRandomNumber(minSpellVariantId, maxSpellVariantId, eRandomFormula.favorSpecificValue, tier.preferredSpellLevel - 1, tier.preferredSpellLevelStrength, creationInfo.qualityModifier);
                        int spell = spellLevelVariants[spellVariantIndex];

                        if (spell == 0)
                        {
                            //this spell doesnt exit at this level
                            continue;
                        }

                        possibleSpells.RemoveAt(spellRoll);//remove this from the list of possible spells so we do not readd it
                        if (possibleSpell.spellCategory == eSpellCategory.weaponSkillMastery)
                        {
                            //remove other weapon masteries from the possible spells list.
                            List<sPossibleSpells> updatedPossibleSpells = new List<sPossibleSpells>();
                            foreach (sPossibleSpells otherSpell in possibleSpells)
                            {
                                if (otherSpell.spellCategory != eSpellCategory.weaponSkillMastery)
                                    updatedPossibleSpells.Add(otherSpell);
                            }
                            possibleSpells = updatedPossibleSpells;
                        }

                        if (!SpellInfo.isValidSpell(spell))
                        {
                            Console.WriteLine("Invalid spell id: {0}", spell);
                            continue;
                        }

                        if (newItem.spellBook.spellData == null)
                            newItem.spellBook.spellData = new List<sSpellData>();
                        newItem.spellBook.spellData.Add(new sSpellData(spell, 2));

                        creationInfo.spellIds.Add(spell);
                        creationInfo.spells.Add(possibleSpell);

                        creationInfo.isMagical = true;
                        creationInfo.totalSpellsCount++;
                        int spellPower = lootProfile.spellProperties.spellPower[spellVariantIndex];
                        creationInfo.totalPower += spellPower;
                        creationInfo.totalMana += lootProfile.spellProperties.spellMana[spellVariantIndex];
                        if (creationInfo.highestPower < spellPower)
                            creationInfo.highestPower = spellPower;
                    }
                }
            }

            if (tier.maxAmountOfCantrips > 0 && tier.minCantripLevel > 0 && tier.maxCantripLevel > 0)
            {
                //we have possible cantrips
                if (Utils.getRandomNumberExclusive(100) < tier.chanceOfCantrips * 100 * (1 + (creationInfo.qualityModifier * 2)))
                {
                    List<sPossibleSpells> possibleCantrips = new List<sPossibleSpells>(possibleSpellsTemplate);

                    if (creationInfo.spells.Count > 0)
                    {
                        //if we also have spells, coordinate cantrips with them
                        bool hasWeaponMastery = false;
                        List<sPossibleSpells> updatedPossibleCantrips = new List<sPossibleSpells>();
                        List<sPossibleSpells> magicMasterySpells = new List<sPossibleSpells>();
                        foreach (sPossibleSpells spell in creationInfo.spells)
                        {
                            if (spell.spellCategory == eSpellCategory.weaponSkillMastery)
                            {
                                hasWeaponMastery = true;
                            }
                            else if (spell.spellCategory == eSpellCategory.magicSkillMastery)
                            {
                                if (newItem.weenieType != eWeenieTypes.Caster || newItem.getStat(eIntStat.WieldSkilltype) == 0)
                                {
                                    magicMasterySpells.Add(spell);
                                }
                            }
                        }
                        foreach (sPossibleSpells cantrip in possibleCantrips)
                        {
                            if (hasWeaponMastery && cantrip.spellCategory == eSpellCategory.weaponSkillMastery)
                            {
                                foreach (sPossibleSpells spell in creationInfo.spells)
                                {
                                    if (spell.id == cantrip.id)
                                    {
                                        updatedPossibleCantrips.Add(cantrip);
                                        break;
                                    }
                                }
                            }
                            else
                                updatedPossibleCantrips.Add(cantrip);
                        }
                        possibleCantrips = updatedPossibleCantrips;

                        if (magicMasterySpells.Count > 0)
                        {
                            creationInfo.favoredMagicSchoolMasterySpell = magicMasterySpells[Utils.getRandomNumberExclusive(magicMasterySpells.Count)];

                            creationInfo.otherMagicSchoolMasterySpells = new List<sPossibleSpells>();
                            if (creationInfo.favoredMagicSchoolMasterySpell.spellName != "Creature Enchantment Mastery")
                                creationInfo.otherMagicSchoolMasterySpells.Add(lootProfile.spells[lootProfile.spellNameToIdMap["Creature Enchantment Mastery"]]);
                            if (creationInfo.favoredMagicSchoolMasterySpell.spellName != "Item Enchantment Mastery")
                                creationInfo.otherMagicSchoolMasterySpells.Add(lootProfile.spells[lootProfile.spellNameToIdMap["Item Enchantment Mastery"]]);
                            if (creationInfo.favoredMagicSchoolMasterySpell.spellName != "Life Magic Mastery")
                                creationInfo.otherMagicSchoolMasterySpells.Add(lootProfile.spells[lootProfile.spellNameToIdMap["Life Magic Mastery"]]);
                            if (creationInfo.favoredMagicSchoolMasterySpell.spellName != "War Magic Mastery")
                                creationInfo.otherMagicSchoolMasterySpells.Add(lootProfile.spells[lootProfile.spellNameToIdMap["War Magic Mastery"]]);
                        }
                    }

                    int cantripCount = Utils.getRandomNumber(1, tier.maxAmountOfCantrips, eRandomFormula.favorLow, 10.0);
                    for (int i = 1; i < creationInfo.spellAmountMultiplier; i++)
                        cantripCount += Utils.getRandomNumber(0, tier.maxAmountOfCantrips, eRandomFormula.favorLow, 10.0);

                    for (int i = 0; i < cantripCount; i++)
                    {
                        int cantripRoll = Utils.getRandomNumberExclusive(possibleCantrips.Count);
                        sPossibleSpells possibleCantrip = possibleCantrips[cantripRoll];

                        if (!creationInfo.hasAlreadyReplacedCantrip && possibleCantrip.spellCategory == eSpellCategory.magicSkillMastery && creationInfo.favoredMagicSchoolMasterySpell.id != 0 && creationInfo.otherMagicSchoolMasterySpells != null && creationInfo.otherMagicSchoolMasterySpells.Count > 0)
                        {
                            //replace magic school mastery cantrip for the one that the item has a requirement for.
                            foreach(sPossibleSpells spellToBeReplaced in creationInfo.otherMagicSchoolMasterySpells)
                            {
                                if(possibleCantrip.id == spellToBeReplaced.id)
                                {
                                    possibleCantrip = creationInfo.favoredMagicSchoolMasterySpell;
                                    creationInfo.hasAlreadyReplacedCantrip = true;
                                    break;
                                }
                            }
                        }

                        List<int> cantripLevelVariants = possibleCantrip.cantrips;
                        int minCantripVariantId = tier.minCantripLevel - 1;
                        int maxCantripVariantId = Math.Min(tier.maxCantripLevel, cantripLevelVariants.Count) - 1;

                        int cantripVariantIndex = Utils.getRandomNumber(minCantripVariantId, maxCantripVariantId, eRandomFormula.favorSpecificValue, tier.preferredCantripLevel - 1, tier.preferredCantripLevelStrength, creationInfo.qualityModifier / 2);
                        int cantrip = cantripLevelVariants[cantripVariantIndex];
                        if (cantrip == 0)
                        {
                            //this cantrip doesnt exit at this level
                            continue;
                        }

                        possibleCantrips.RemoveAt(cantripRoll);//remove this from the list of possible cantrips so we do not readd it
                        if (possibleCantrip.spellCategory == eSpellCategory.weaponSkillMastery)
                        {
                            //remove other weapon masteries from the possible cantrips list.
                            List<sPossibleSpells> updatedPossibleCantrips = new List<sPossibleSpells>();
                            foreach (sPossibleSpells otherCantrip in possibleCantrips)
                            {
                                if (otherCantrip.spellCategory != eSpellCategory.weaponSkillMastery)
                                    updatedPossibleCantrips.Add(otherCantrip);
                            }
                            possibleCantrips = updatedPossibleCantrips;
                        }

                        if (!SpellInfo.isValidSpell(cantrip))
                        {
                            Console.WriteLine("Invalid cantrip id: {0}", cantrip);
                            continue;
                        }

                        if (newItem.spellBook.spellData == null)
                            newItem.spellBook.spellData = new List<sSpellData>();
                        newItem.spellBook.spellData.Add(new sSpellData(cantrip, 2));

                        creationInfo.cantripIds.Add(cantrip);
                        creationInfo.cantrips.Add(possibleCantrip);

                        creationInfo.isMagical = true;
                        creationInfo.totalSpellsCount++;
                        int cantripPower = lootProfile.spellProperties.cantripPower[cantripVariantIndex];
                        creationInfo.totalPower += cantripPower;
                        creationInfo.totalMana += lootProfile.spellProperties.cantripMana[cantripVariantIndex];
                        if (creationInfo.highestPower < cantripPower)
                            creationInfo.highestPower = cantripPower;
                    }
                }
            }
        }

        List<sItemLootProfileEntry> mergeLootEntriesLists(List<sItemLootProfileEntry> list1, List<sItemLootProfileEntry> list2)
        {
            List<sItemLootProfileEntry> mergedList = new List<sItemLootProfileEntry>();
            mergedList.AddRange(list1);

            foreach (sItemLootProfileEntry item2 in list2)
            {
                bool isRepeat = false;
                foreach (sItemLootProfileEntry item1 in list1)
                {
                    if (item1.wcid == item2.wcid)
                    {
                        isRepeat = true;
                        break;
                    }
                }
                if (!isRepeat)
                    mergedList.Add(item2);
            }

            return mergedList;
        }

        public void addLootToCreateList(cWeenie weenie, sLootProfile lootProfile, sLootTier tier)
        {
            int numItems = Math.Min(tier.lootEntriesPerWeenie, tier.amount);

            int level = weenie.getStat(eIntStat.Level);

            if (!weenie.dataFlags.HasFlag(eDataFlags.createList))
            {
                weenie.dataFlags |= eDataFlags.createList;
                weenie.createList.entries = new List<sCreateListEntry>();
            }

            List<int> idList;

            if (weenie.hasStat(eIntStat.Level) && weenie.getStat(eIntStat.Level) >= tier.qualityLootLevelThreshold)
                idList = Utils.getRandomNumbersNoRepeat(numItems, tier.qualityLootFirstId, tier.qualityLootLastId);
            else
                idList = Utils.getRandomNumbersNoRepeat(numItems, tier.allLootFirstId, tier.allLootLastId);

            for (int i = 0; i < numItems; i++)
            {
                sCreateListEntry lootEntry = new sCreateListEntry();
                lootEntry.wcid = idList[i];
                lootEntry.palette = 0;
                lootEntry.shade = tier.lootChance / numItems;
                lootEntry.destination = eDestinationType.ContainTreasure_DestinationType;
                lootEntry.stack_size = 0;
                lootEntry.try_to_bond = 0;
                weenie.createList.entries.Add(lootEntry);
            }

            List<sItemLootProfileEntry> validMiscItemEntries = new List<sItemLootProfileEntry>();
            foreach (sLootProfileCategory miscItemCategory in lootProfile.miscItems)
            {
                foreach (string tierMiscItemCategoryName in tier.miscItemsCategories)
                {
                    if (miscItemCategory.category == tierMiscItemCategoryName)
                    {
                        validMiscItemEntries = mergeLootEntriesLists(validMiscItemEntries, miscItemCategory.entries);
                    }
                }
            }

            foreach (sItemLootProfileEntry miscItemEntry in validMiscItemEntries)
            {
                sCreateListEntry lootEntry = new sCreateListEntry();
                lootEntry.wcid = miscItemEntry.wcid;
                lootEntry.palette = 0;
                lootEntry.shade = tier.miscLootChance / validMiscItemEntries.Count;
                lootEntry.destination = eDestinationType.ContainTreasure_DestinationType;
                if (miscItemEntry.maxAmount > 0)
                    lootEntry.stack_size = Utils.getRandomNumber(1, miscItemEntry.maxAmount, eRandomFormula.favorMid, 2);
                else
                    lootEntry.stack_size = 0;
                lootEntry.try_to_bond = 0;
                weenie.createList.entries.Add(lootEntry);
            }

            List<sItemLootProfileEntry> validScrollEntries = new List<sItemLootProfileEntry>();
            foreach (sLootProfileCategory scrollCategory in lootProfile.scrolls)
            {
                foreach (string tierScrollCategoryName in tier.scrollCategories)
                {
                    if (scrollCategory.category == tierScrollCategoryName)
                    {
                        validScrollEntries = mergeLootEntriesLists(validScrollEntries, scrollCategory.entries);
                    }
                }
            }

            for (int i = 0; i < tier.scrollEntriesPerWeenie; i++)
            {
                sItemLootProfileEntry scroll = validScrollEntries[Utils.getRandomNumberExclusive(validScrollEntries.Count)];
                sCreateListEntry lootEntry = new sCreateListEntry();
                lootEntry.wcid = scroll.wcid;
                lootEntry.palette = 0;
                lootEntry.shade = tier.scrollLootChance / tier.scrollEntriesPerWeenie;
                lootEntry.destination = eDestinationType.ContainTreasure_DestinationType;
                lootEntry.stack_size = 0;
                lootEntry.try_to_bond = 0;
                weenie.createList.entries.Add(lootEntry);
            }
        }

        public struct sGeneratorTableEntryData
        {
            public bool originalValuesInitialized;
            public bool allTreasureEntriesAreMinusOneProbability;
            public int originalEntryCount;
        }

        enum eHasNonTreasure
        {
            no,
            onlyMinusOne,
            yes
        }

        public void addLootToGeneratorTable(cWeenie weenie, sGeneratorTableEntryData entryData, eTreasureGeneratorType treasureGeneratorType, sLootProfile lootProfile, sLootTier tier)
        {
            int treasureTypeCount = 0;
            bool allTreasureTheSameAndMinusOneProbability = true;
            eHasNonTreasure hasNonTreasureLoot = eHasNonTreasure.no;
            int lowestTreasureSlot = int.MaxValue;
            Double shortestDelay = Double.MaxValue;
            int amountOfMinusOneTreasureEntries = 0;

            foreach (sGeneratorTableEntry entry in weenie.generatorTable.entries)
            {
                if (entry.delay != 0 && entry.delay < shortestDelay)
                    shortestDelay = entry.delay;

                if (entry.type == (int)treasureGeneratorType)
                    treasureTypeCount++;

                if (entry.whereCreate != eRegenLocationType.ContainTreasure_RegenLocationType)
                {
                    if (entry.probability == -1)
                        hasNonTreasureLoot = eHasNonTreasure.onlyMinusOne;
                    else
                        hasNonTreasureLoot = eHasNonTreasure.yes;
                }
                else
                {
                    if (entry.type != (int)treasureGeneratorType ||
                        entry.probability != -1f)
                    {
                        allTreasureTheSameAndMinusOneProbability = false;
                    }

                    if (entry.probability == -1f)
                    {
                        if (entry.slot < lowestTreasureSlot)
                            lowestTreasureSlot = entry.slot;
                        amountOfMinusOneTreasureEntries++;
                    }
                }
            }

            if (!entryData.originalValuesInitialized)
            {
                entryData.originalValuesInitialized = true;
                entryData.originalEntryCount = weenie.generatorTable.entries.Count;
                if (amountOfMinusOneTreasureEntries == entryData.originalEntryCount)
                    entryData.allTreasureEntriesAreMinusOneProbability = true;
            }

            if (treasureTypeCount == 0)
                return;

            int numItems = Math.Min(tier.lootEntriesPerWeenie, tier.amount);

            if (!weenie.dataFlags.HasFlag(eDataFlags.generatorTable))
            {
                Console.WriteLine("Tried to add a generatorTable entry to a weenie that doesn't have a generatorTable. ({0} - {1})", weenie.wcid, WeenieClassNames.getWeenieClassName(weenie.wcid));
                return; //if we don't have a generatorTable we won't have the other variables needed, skip this entry for now.
            }

            //PhatAC does not yet regenerates items according to their delay, so force regenerate it by resetting it completely.
            //and set the initial amount of items to the max possible amount.
            int maxGeneratedObjects = weenie.getStat(eIntStat.MaxGeneratedObjects);

            //Chest looting timers do not seem to be function in PhatAC.
            //As a stopgap measure increase the respawn timer to 1 hour(up from 10 seconds).
            //This is used by runed chests
            if (weenie.getStat(eStringStat.Quest) != "")
            {
                shortestDelay = 3600;
                maxGeneratedObjects = 10;
            }

            //also increase amount of items on some chests that have just 1 max item. Originally AC chests had loot
            //one tier higher the the monsters around it, that might have been a good idea before the loot tiers were changed
            //to have wield requirements but after that what happens is someone getting higher level loot that he can't yet equip.
            //we're changing that to chests having the same tier loot as the monsters around it but in larger quantities.
            string lockCode = weenie.getStat(eStringStat.LockCode);
            if (lockCode == "keychesthigh" ||
               lockCode == "keychestextreme" ||
               lockCode == "VirindiSingularityKey" ||
               lockCode == "VirindiDirectiveKey" ||
               lockCode == "VirindiMasterKey" ||
               lockCode == "KeyChestVoDLow" ||
               lockCode == "KeyChestVoDHigh")
            {
                maxGeneratedObjects = 5;
            }
            else if (maxGeneratedObjects == 1)
               maxGeneratedObjects = 3;

            if (shortestDelay == Double.MaxValue)
                shortestDelay = weenie.getStat(eFloatStat.RegenerationInterval);
            if (shortestDelay == 0)
                shortestDelay = 60;

            //remove RegenerationInterval as having both it and ResetInterval causes a dupe bug
            weenie.removeStat(eFloatStat.RegenerationInterval);
            weenie.addOrUpdateStat(eFloatStat.ResetInterval, shortestDelay);

            maxGeneratedObjects = (int)Math.Round(maxGeneratedObjects * tier.chestLootAmountMultiplier);
            weenie.addOrUpdateStat(eIntStat.InitGeneratedObjects, maxGeneratedObjects);
            weenie.addOrUpdateStat(eIntStat.MaxGeneratedObjects, maxGeneratedObjects);

            if (lootProfile.otherOptions.debug)
            {
                //DEBUG - show chest tier in the chest name
                string name = weenie.getStat(eStringStat.Name);
                weenie.addOrUpdateStat(eStringStat.Name, $"{tier.name} {name}({weenie.wcid})");
            }

            for (int treasureEntryCounter = 0; treasureEntryCounter < treasureTypeCount; treasureEntryCounter++)
            {
                Dictionary<int, Single> probabilitiesMap = new Dictionary<int, float>();
                foreach (sGeneratorTableEntry entry in weenie.generatorTable.entries)
                {
                    probabilitiesMap.Add(entry.slot, entry.probability);
                }

                sGeneratorTable newGeneratorTable = new sGeneratorTable();
                newGeneratorTable.entries = new List<sGeneratorTableEntry>();
                List<sGeneratorTableEntry> tempStorageForTreasureEntries = new List<sGeneratorTableEntry>();
                int originalTreasureGeneratorTypeSlot = int.MaxValue;
                int forceMoveToSlot = -1;
                Single forceMoveOriginalOccupantProbability = 0f;
                bool moveEntriesToEndSlotsUnchanged = false;

                foreach (sGeneratorTableEntry entry in weenie.generatorTable.entries)
                {
                    if (entry.type == (int)treasureGeneratorType)
                    {
                        originalTreasureGeneratorTypeSlot = entry.slot;
                        int slotOffset = entry.slot;

                        Single previousEntryProbability = 0;
                        Single minProbability = 0;
                        Single maxProbability = 0;
                        Single probabilityIncrement = 0;

                        if(hasNonTreasureLoot == eHasNonTreasure.yes)
                            moveEntriesToEndSlotsUnchanged = true;

                        if (weenie.wcid == 8999)
                        {
                            //the steel chest is a special case where there's a random loot entry with
                            //a -1.0 probability, this means always drop but since we're adding a bunch
                            //of entries it would be absurd to "always drop" all of them.
                            //As a workaround we're merging the chance of getting the item to the superb
                            //mana stone that has the highest chance of dropping.
                            forceMoveToSlot = 30;
                            moveEntriesToEndSlotsUnchanged = false; //otherwise we won't get random items ever due to the titan mana charge.
                        }

                        if (weenie.wcid == 30792 || weenie.wcid == 30793 || weenie.wcid == 30794 || weenie.wcid == 30795 || weenie.wcid == 30796 || weenie.wcid == 30797)
                        {
                            //Black Marrow Reliquaries also has a -1.0 probability random loot entry, but it
                            //has plenty of "empty" chance space available for use so we just use that.
                            minProbability = 0.03f;
                            maxProbability = 1f;
                            probabilityIncrement = (maxProbability - minProbability) / numItems;
                        }
                        else if (allTreasureTheSameAndMinusOneProbability && hasNonTreasureLoot != eHasNonTreasure.yes)
                        {
                            minProbability = 0f;
                            maxProbability = 1f;

                            probabilityIncrement = (maxProbability - minProbability) / numItems;

                            originalTreasureGeneratorTypeSlot = lowestTreasureSlot;
                            slotOffset = lowestTreasureSlot;
                        }
                        else if (entryData.originalValuesInitialized && entryData.allTreasureEntriesAreMinusOneProbability)
                        {
                            Single entryProbabilityIncrement = 1f / entryData.originalEntryCount;

                            previousEntryProbability = entry.slot * entryProbabilityIncrement;

                            minProbability = previousEntryProbability;
                            maxProbability = (entry.slot + 1) * entryProbabilityIncrement;

                            probabilityIncrement = (maxProbability - minProbability) / numItems;
                        }
                        else if (forceMoveToSlot == -1)
                        {
                            if (entry.slot > 0)
                                previousEntryProbability = probabilitiesMap[entry.slot - 1];

                            minProbability = previousEntryProbability;
                            maxProbability = entry.probability;
                            probabilityIncrement = (maxProbability - minProbability) / numItems;
                        }
                        else
                        {
                            if (entry.slot > 0)
                                previousEntryProbability = probabilitiesMap[forceMoveToSlot - 1];

                            minProbability = previousEntryProbability;
                            maxProbability = probabilitiesMap[forceMoveToSlot];
                            probabilityIncrement = (maxProbability - minProbability) / numItems;
                            minProbability += probabilityIncrement;
                            forceMoveOriginalOccupantProbability = minProbability;

                            slotOffset = forceMoveToSlot;
                        }

                        if (treasureGeneratorType == eTreasureGeneratorType.T1_ChestGreenmire1 || treasureGeneratorType == eTreasureGeneratorType.T1_ChestGreenmire2)
                        {//green mire chests have their quest reward included in their random loot profile, so we need to keep it.
                            sGeneratorTableEntry questEntry = new sGeneratorTableEntry();
                            questEntry.type = entry.type;
                            questEntry.probability = -1;
                            questEntry.delay = entry.delay;
                            questEntry.initCreate = entry.initCreate;
                            questEntry.maxNum = entry.maxNum;
                            questEntry.whenCreate = entry.whenCreate;
                            questEntry.whereCreate = entry.whereCreate;
                            questEntry.stackSize = entry.stackSize;
                            questEntry.ptid = entry.ptid;
                            questEntry.shade = entry.shade;
                            questEntry.objcell_id = entry.objcell_id;
                            questEntry.frame = entry.frame;
                            questEntry.slot = slotOffset++;
                            newGeneratorTable.entries.Add(questEntry);
                        }

                        List<int> idList = Utils.getRandomNumbersNoRepeat(numItems, tier.qualityLootFirstId, tier.qualityLootLastId);

                        for (int i = 0; i < numItems; i++)
                        {
                            sGeneratorTableEntry lootEntry = new sGeneratorTableEntry();
                            lootEntry.type = idList[i];
                            lootEntry.probability = minProbability + (probabilityIncrement * (i + 1));
                            lootEntry.delay = entry.delay;
                            lootEntry.initCreate = entry.initCreate;
                            lootEntry.maxNum = entry.maxNum;
                            lootEntry.whenCreate = entry.whenCreate;
                            lootEntry.whereCreate = eRegenLocationType.Contain_RegenLocationType;
                            lootEntry.stackSize = entry.stackSize;
                            lootEntry.ptid = entry.ptid;
                            lootEntry.shade = entry.shade;
                            lootEntry.objcell_id = entry.objcell_id;
                            lootEntry.frame = entry.frame;
                            lootEntry.slot = slotOffset++;

                            tempStorageForTreasureEntries.Add(lootEntry);
                        }
                        break; //expand only one instance per loop
                    }
                }

                foreach (sGeneratorTableEntry entry in weenie.generatorTable.entries)
                {
                    if (forceMoveToSlot == -1)
                    {
                        if (entry.slot < originalTreasureGeneratorTypeSlot)
                            newGeneratorTable.entries.Add(entry);
                        else if (entry.slot > originalTreasureGeneratorTypeSlot)
                        {
                            sGeneratorTableEntry newEntry = entry;
                            if (!allTreasureTheSameAndMinusOneProbability || entry.type != (int)treasureGeneratorType)
                            {
                                if (newEntry.slot > originalTreasureGeneratorTypeSlot)
                                    newEntry.slot += numItems - 1;
                                newGeneratorTable.entries.Add(newEntry);
                            }
                        }
                        else if (entry.slot == originalTreasureGeneratorTypeSlot && !moveEntriesToEndSlotsUnchanged)
                            newGeneratorTable.entries.AddRange(tempStorageForTreasureEntries);
                    }
                    else
                    {
                        if (entry.slot == originalTreasureGeneratorTypeSlot)
                            continue;
                        else if (entry.slot < forceMoveToSlot)
                        {
                            sGeneratorTableEntry newEntry = entry;
                            if (newEntry.slot > originalTreasureGeneratorTypeSlot)
                                newEntry.slot -= 1;
                            newGeneratorTable.entries.Add(newEntry);
                        }
                        else if (entry.slot > forceMoveToSlot)
                        {
                            sGeneratorTableEntry newEntry = entry;
                            if (newEntry.slot > forceMoveToSlot)
                                newEntry.slot += numItems;
                            if (newEntry.slot > originalTreasureGeneratorTypeSlot)
                                newEntry.slot -= 1;
                            newGeneratorTable.entries.Add(newEntry);
                        }
                        else if (entry.slot == forceMoveToSlot)
                        {
                            sGeneratorTableEntry newEntry = entry;
                            if (newEntry.slot > originalTreasureGeneratorTypeSlot)
                                newEntry.slot -= 1;
                            newEntry.probability = forceMoveOriginalOccupantProbability;
                            newGeneratorTable.entries.Add(newEntry);

                            if(!moveEntriesToEndSlotsUnchanged)
                                newGeneratorTable.entries.AddRange(tempStorageForTreasureEntries);
                        }
                    }
                }
                if(moveEntriesToEndSlotsUnchanged)
                    newGeneratorTable.entries.AddRange(tempStorageForTreasureEntries);
                weenie.generatorTable = newGeneratorTable;
                if (allTreasureTheSameAndMinusOneProbability)
                    return;
            }
        }
    }
}