using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melt
{
    struct sWarSpellData
    {
        public int spellId;
        public string spellName;
        public string spellDescription;
        public int schoolId;
        public int iconId;
        public int familyId;
        public int flags;
        public int manaCost;
        public float unknown1;
        public float unknown3;
        public int difficulty;
        public float economy;
        public int generation;
        public float speed;
        public eSpellType spellType;
        public int unknown4;

        public eDamageType damageType;
        public int minDamage;
        public int damageVariance;
        public int[] unknownProjectileValues;

        public int[] component;
        public int casterEffect;
        public int targetEffect;
        public int unknown6;
        public int unknown7;
        public int unknown8;
        public int unknown9;
        public int sortOrder;
        public int targetMask;
        public int unknown10;
    }

    public class cCache2Converter
    {
        public static Dictionary<int, int> createDuplicateWarSpellsWithMultipliedDamage(string filename, float creatureDamageMultiplier)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return null;
            }
            StreamWriter outputFile = new StreamWriter(new FileStream("./intermediate/0002.raw", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open ./intermediate/0002.raw");
                return null;
            }

            Stopwatch timer = new Stopwatch();
            timer.Start();

            Console.WriteLine("Adjusting spell damage...");

            List<sWarSpellData> warSpells = new List<sWarSpellData>();

            byte[] buffer = new byte[1024];

            int fileHeader;

            fileHeader = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile);
            if (fileHeader != 0x10000E9F)
            {
                Console.WriteLine("Invalid header, aborting.");
                return null;
            }

            List<int> sharedSpellList = new List<int>();
            sharedSpellList.Add(27); //Flame Bolt I
            sharedSpellList.Add(28); //Frost Bolt I
            sharedSpellList.Add(58); //Acid Stream I
            sharedSpellList.Add(59); //Acid Stream II
            sharedSpellList.Add(60); //Acid Stream III
            sharedSpellList.Add(61); //Acid Stream IV
            sharedSpellList.Add(62); //Acid Stream V
            sharedSpellList.Add(63); //Acid Stream VI
            sharedSpellList.Add(64); //Shock Wave I
            sharedSpellList.Add(65); //Shock Wave II
            sharedSpellList.Add(66); //Shock Wave III
            sharedSpellList.Add(67); //Shock Wave IV
            sharedSpellList.Add(68); //Shock Wave V
            sharedSpellList.Add(69); //Shock Wave VI
            sharedSpellList.Add(70); //Frost Bolt II
            sharedSpellList.Add(71); //Frost Bolt III
            sharedSpellList.Add(72); //Frost Bolt IV
            sharedSpellList.Add(73); //Frost Bolt V
            sharedSpellList.Add(74); //Frost Bolt VI
            sharedSpellList.Add(75); //Lightning Bolt I
            sharedSpellList.Add(76); //Lightning Bolt II
            sharedSpellList.Add(77); //Lightning Bolt III
            sharedSpellList.Add(78); //Lightning Bolt IV
            sharedSpellList.Add(79); //Lightning Bolt V
            sharedSpellList.Add(80); //Lightning Bolt VI
            sharedSpellList.Add(81); //Flame Bolt II
            sharedSpellList.Add(82); //Flame Bolt III
            sharedSpellList.Add(83); //Flame Bolt IV
            sharedSpellList.Add(84); //Flame Bolt V
            sharedSpellList.Add(85); //Flame Bolt VI
            sharedSpellList.Add(86); //Force Bolt I
            sharedSpellList.Add(87); //Force Bolt II
            sharedSpellList.Add(88); //Force Bolt III
            sharedSpellList.Add(89); //Force Bolt IV
            sharedSpellList.Add(90); //Force Bolt V
            sharedSpellList.Add(91); //Force Bolt VI
            sharedSpellList.Add(92); //Whirling Blade I
            sharedSpellList.Add(93); //Whirling Blade II
            sharedSpellList.Add(94); //Whirling Blade III
            sharedSpellList.Add(95); //Whirling Blade IV
            sharedSpellList.Add(96); //Whirling Blade V
            sharedSpellList.Add(97); //Whirling Blade VI
            sharedSpellList.Add(99); //Acid Blast III
            sharedSpellList.Add(102); //Acid Blast VI
            sharedSpellList.Add(103); //Shock Blast III
            sharedSpellList.Add(104); //Shock Blast IV
            sharedSpellList.Add(105); //Shock Blast V
            sharedSpellList.Add(106); //Shock Blast VI
            sharedSpellList.Add(107); //Frost Blast III
            sharedSpellList.Add(108); //Frost Blast IV
            sharedSpellList.Add(109); //Frost Blast V
            sharedSpellList.Add(111); //Lightning Blast III
            sharedSpellList.Add(113); //Lightning Blast V
            sharedSpellList.Add(115); //Flame Blast III
            sharedSpellList.Add(120); //Force Blast IV
            sharedSpellList.Add(121); //Force Blast V
            sharedSpellList.Add(123); //Blade Blast III
            sharedSpellList.Add(124); //Blade Blast IV
            sharedSpellList.Add(125); //Blade Blast V
            sharedSpellList.Add(126); //Blade Blast VI
            sharedSpellList.Add(127); //Acid Volley III
            sharedSpellList.Add(128); //Acid Volley IV
            sharedSpellList.Add(129); //Acid Volley V
            sharedSpellList.Add(130); //Acid Volley VI
            sharedSpellList.Add(131); //Bludgeoning Volley III
            sharedSpellList.Add(132); //Bludgeoning Volley IV
            sharedSpellList.Add(133); //Bludgeoning Volley V
            sharedSpellList.Add(134); //Bludgeoning Volley VI
            sharedSpellList.Add(135); //Frost Volley III
            sharedSpellList.Add(136); //Frost Volley IV
            sharedSpellList.Add(137); //Frost Volley V
            sharedSpellList.Add(138); //Frost Volley VI
            sharedSpellList.Add(139); //Lightning Volley III
            sharedSpellList.Add(140); //Lightning Volley IV
            sharedSpellList.Add(141); //Lightning Volley V
            sharedSpellList.Add(142); //Lightning Volley VI
            sharedSpellList.Add(143); //Flame Volley III
            sharedSpellList.Add(144); //Flame Volley IV
            sharedSpellList.Add(145); //Flame Volley V
            sharedSpellList.Add(146); //Flame Volley VI
            sharedSpellList.Add(147); //Force Volley III
            sharedSpellList.Add(148); //Force Volley IV
            sharedSpellList.Add(149); //Force Volley V
            sharedSpellList.Add(150); //Force Volley VI
            sharedSpellList.Add(151); //Blade Volley III
            sharedSpellList.Add(152); //Blade Volley IV
            sharedSpellList.Add(153); //Blade Volley V
            sharedSpellList.Add(154); //Blade Volley VI
            sharedSpellList.Add(1783); //Searing Disc
            sharedSpellList.Add(1784); //Horizon's Blades
            sharedSpellList.Add(1785); //Cassius' Ring of Fire
            sharedSpellList.Add(1786); //Nuhmudira's Spines
            sharedSpellList.Add(1787); //Halo of Frost
            sharedSpellList.Add(1788); //Eye of the Storm
            sharedSpellList.Add(1789); //Tectonic Rifts
            sharedSpellList.Add(1792); //Acid Streak III
            sharedSpellList.Add(1793); //Acid Streak IV
            sharedSpellList.Add(1794); //Acid Streak V
            sharedSpellList.Add(1795); //Acid Streak VI
            sharedSpellList.Add(1797); //Flame Streak II
            sharedSpellList.Add(1798); //Flame Streak III
            sharedSpellList.Add(1799); //Flame Streak IV
            sharedSpellList.Add(1800); //Flame Streak V
            sharedSpellList.Add(1801); //Flame Streak VI
            sharedSpellList.Add(1807); //Force Streak VI
            sharedSpellList.Add(1808); //Frost Streak I
            sharedSpellList.Add(1810); //Frost Streak III
            sharedSpellList.Add(1811); //Frost Streak IV
            sharedSpellList.Add(1812); //Frost Streak V
            sharedSpellList.Add(1813); //Frost Streak VI
            sharedSpellList.Add(1814); //Lightning Streak I
            sharedSpellList.Add(1815); //Lightning Streak II
            sharedSpellList.Add(1816); //Lightning Streak III
            sharedSpellList.Add(1817); //Lightning Streak IV
            sharedSpellList.Add(1818); //Lightning Streak V
            sharedSpellList.Add(1819); //Lightning Streak VI
            sharedSpellList.Add(1825); //Shock Wave Streak VI
            sharedSpellList.Add(1829); //Whirling Blade Streak IV
            sharedSpellList.Add(1830); //Whirling Blade Streak V
            sharedSpellList.Add(1831); //Whirling Blade Streak VI
            sharedSpellList.Add(1834); //Firestorm
            sharedSpellList.Add(1836); //Avalanche
            sharedSpellList.Add(1839); //Blistering Creeper
            sharedSpellList.Add(1840); //Bed of Blades
            sharedSpellList.Add(1841); //Slithering Flames
            sharedSpellList.Add(1842); //Spike Strafe
            sharedSpellList.Add(1843); //Foon-Ki's Glacial Floe
            sharedSpellList.Add(1844); //Os' Wall
            sharedSpellList.Add(2120); //Dissolving Vortex
            sharedSpellList.Add(2121); //Corrosive Flash
            sharedSpellList.Add(2122); //Disintegration
            sharedSpellList.Add(2123); //Celdiseth's Searing
            sharedSpellList.Add(2124); //Sau Kolin's Sword
            sharedSpellList.Add(2125); //Flensing Wings
            sharedSpellList.Add(2126); //Thousand Fists
            sharedSpellList.Add(2128); //Ilservian's Flame
            sharedSpellList.Add(2129); //Sizzling Fury
            sharedSpellList.Add(2130); //Infernae
            sharedSpellList.Add(2132); //The Spike
            sharedSpellList.Add(2133); //Outlander's Insolence
            sharedSpellList.Add(2134); //Fusillade
            sharedSpellList.Add(2136); //Icy Torment
            sharedSpellList.Add(2137); //Sudden Frost
            sharedSpellList.Add(2138); //Blizzard
            sharedSpellList.Add(2140); //Alset's Coil
            sharedSpellList.Add(2141); //Lhen's Flare
            sharedSpellList.Add(2142); //Tempest
            sharedSpellList.Add(2143); //Pummeling Storm
            sharedSpellList.Add(2144); //Crushing Shame
            sharedSpellList.Add(2145); //Cameron's Curse
            sharedSpellList.Add(2146); //Evisceration
            sharedSpellList.Add(2715); //Acid Arc V
            sharedSpellList.Add(2716); //Acid Arc VI
            sharedSpellList.Add(2717); //Acid Arc VII
            sharedSpellList.Add(2722); //Force Arc V
            sharedSpellList.Add(2723); //Force Arc VI
            sharedSpellList.Add(2724); //Force Arc VII
            sharedSpellList.Add(2728); //Frost Arc IV
            sharedSpellList.Add(2730); //Frost Arc VI
            sharedSpellList.Add(2731); //Frost Arc VII
            sharedSpellList.Add(2735); //Lightning Arc IV
            sharedSpellList.Add(2737); //Lightning Arc VI
            sharedSpellList.Add(2738); //Lightning Arc VII
            sharedSpellList.Add(2743); //Flame Arc V
            sharedSpellList.Add(2744); //Flame Arc VI
            sharedSpellList.Add(2745); //Flame Arc VII
            sharedSpellList.Add(2750); //Shock Arc V
            sharedSpellList.Add(2751); //Shock Arc VI
            sharedSpellList.Add(2752); //Shock Arc VII
            sharedSpellList.Add(2755); //Blade Arc III
            sharedSpellList.Add(2756); //Blade Arc IV
            sharedSpellList.Add(2757); //Blade Arc V
            sharedSpellList.Add(2758); //Blade Arc VI
            sharedSpellList.Add(2759); //Blade Arc VII

            List<int> creatureOnlySpellList = new List<int>();
            creatureOnlySpellList.Add(1097); //Flaming Missile
            creatureOnlySpellList.Add(1481); //Flaming Missile Volley
            creatureOnlySpellList.Add(2030); //Flaming Blaze
            creatureOnlySpellList.Add(2031); //Steel Thorns
            creatureOnlySpellList.Add(2032); //Electric Blaze
            creatureOnlySpellList.Add(2033); //Acidic Spray
            creatureOnlySpellList.Add(2035); //Electric Discharge
            creatureOnlySpellList.Add(2036); //Fuming Acid
            creatureOnlySpellList.Add(2037); //Flaming Irruption
            creatureOnlySpellList.Add(2039); //Sparking Fury
            creatureOnlySpellList.Add(2042); //Demon's Tongues
            creatureOnlySpellList.Add(2045); //Demon Fists
            creatureOnlySpellList.Add(2147); //Rending Wind
            creatureOnlySpellList.Add(2672); //Ring of True Pain
            creatureOnlySpellList.Add(2673); //Ring of Unspeakable Agony
            creatureOnlySpellList.Add(2674); //Vicious Rebuke
            creatureOnlySpellList.Add(2699); //Auroric Whip
            creatureOnlySpellList.Add(2700); //Corrosive Cloud
            creatureOnlySpellList.Add(2701); //Elemental Fury
            creatureOnlySpellList.Add(2702); //Elemental Fury
            creatureOnlySpellList.Add(2703); //Elemental Fury
            creatureOnlySpellList.Add(2704); //Elemental Fury
            creatureOnlySpellList.Add(2710); //Volcanic Blast
            creatureOnlySpellList.Add(2781); //Lesser Elemental Fury
            creatureOnlySpellList.Add(2782); //Lesser Elemental Fury
            creatureOnlySpellList.Add(2783); //Lesser Elemental Fury
            creatureOnlySpellList.Add(2784); //Lesser Elemental Fury
            creatureOnlySpellList.Add(2934); //Tusker Fists
            creatureOnlySpellList.Add(3025); //Shriek
            creatureOnlySpellList.Add(3107); //Flay Soul
            creatureOnlySpellList.Add(3108); //Flay Soul
            creatureOnlySpellList.Add(3109); //Liquefy Flesh
            creatureOnlySpellList.Add(3110); //Sear Flesh
            creatureOnlySpellList.Add(3111); //Soul Hammer
            creatureOnlySpellList.Add(3112); //Soul Spike
            creatureOnlySpellList.Add(3113); //Flay Soul
            creatureOnlySpellList.Add(3114); //Liquefy Flesh
            creatureOnlySpellList.Add(3115); //Sear Flesh
            creatureOnlySpellList.Add(3116); //Soul Hammer
            creatureOnlySpellList.Add(3117); //Soul Spike
            creatureOnlySpellList.Add(3118); //Liquefy Flesh
            creatureOnlySpellList.Add(3119); //Sear Flesh
            creatureOnlySpellList.Add(3120); //Soul Hammer
            creatureOnlySpellList.Add(3121); //Soul Spike
            creatureOnlySpellList.Add(3426); //Greater Withering
            creatureOnlySpellList.Add(3427); //Lesser Withering
            creatureOnlySpellList.Add(3428); //Withering
            creatureOnlySpellList.Add(3451); //Concussive Belch
            creatureOnlySpellList.Add(3452); //Concussive Wail
            creatureOnlySpellList.Add(3455); //Koruu Cloud
            creatureOnlySpellList.Add(3456); //Koruu's Wrath
            creatureOnlySpellList.Add(3457); //Mana Bolt
            creatureOnlySpellList.Add(3458); //Mana Purge
            creatureOnlySpellList.Add(3459); //Mucor Cloud
            creatureOnlySpellList.Add(3460); //Dissolving Vortex

            bool writeSpellToFile = true;
            Dictionary<int, int> creatureReplacementSpellMap = new Dictionary<int, int>();
            int spellCounter = 0;
            //int duplicatedSpellsCounter = 0;
            int changedSpellsCounter = 0;
            while (inputFile.BaseStream.Position < inputFile.BaseStream.Length)
            {
                spellCounter++;
                sWarSpellData spell = new sWarSpellData();

                spell.spellId = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile);

                //int replacementStartIndex = 3679;
                //if (spell.spellId >= replacementStartIndex && spell.spellId < Math.Min(replacementStartIndex + warSpells.Count, 3745))
                //{
                //    duplicatedSpellsCounter++;
                //    int warSpellIndex = spell.spellId - replacementStartIndex;
                //    sWarSpellData replacementSpell = warSpells[warSpellIndex];
                //    creatureReplacementSpellMap.Add(replacementSpell.spellId, spell.spellId);

                //    Utils.writeString(replacementSpell.spellName, outputFile);
                //    Utils.writeString(replacementSpell.spellDescription, outputFile);
                //    Utils.writeInt32(replacementSpell.schoolId, outputFile);
                //    Utils.writeInt32(replacementSpell.iconId, outputFile);
                //    Utils.writeInt32(replacementSpell.familyId, outputFile);
                //    Utils.writeInt32(replacementSpell.flags, outputFile);
                //    Utils.writeInt32(replacementSpell.manaCost, outputFile);
                //    Utils.writeSingle(replacementSpell.unknown1, outputFile);
                //    Utils.writeSingle(replacementSpell.unknown3, outputFile);
                //    Utils.writeInt32(replacementSpell.difficulty, outputFile);
                //    Utils.writeSingle(replacementSpell.economy, outputFile);
                //    Utils.writeInt32(replacementSpell.generation, outputFile);
                //    Utils.writeSingle(replacementSpell.speed, outputFile);
                //    Utils.writeInt32((int)replacementSpell.spellType, outputFile);
                //    Utils.writeInt32(replacementSpell.unknown4, outputFile);

                //    replacementSpell.minDamage = (int)Math.Round(replacementSpell.minDamage * creatureDamageMultiplier);
                //    replacementSpell.damageVariance = (int)Math.Round(replacementSpell.damageVariance * creatureDamageMultiplier);

                //    Utils.writeInt32((int)replacementSpell.damageType, outputFile);
                //    Utils.writeInt32(replacementSpell.minDamage, outputFile);
                //    Utils.writeInt32(replacementSpell.damageVariance, outputFile);

                //    for (int i = 0; i < 29; i++)
                //    {
                //        Utils.writeInt32(replacementSpell.unknownProjectileValues[i], outputFile);
                //    }

                //    for (int i = 0; i < 8; i++)
                //    {
                //        Utils.writeInt32(replacementSpell.component[i], outputFile);
                //    }

                //    Utils.writeInt32(replacementSpell.casterEffect, outputFile);
                //    Utils.writeInt32(replacementSpell.targetEffect, outputFile);

                //    Utils.writeInt32(replacementSpell.unknown6, outputFile);
                //    Utils.writeInt32(replacementSpell.unknown7, outputFile);
                //    Utils.writeInt32(replacementSpell.unknown8, outputFile);
                //    Utils.writeInt32(replacementSpell.unknown9, outputFile);

                //    Utils.writeInt32(replacementSpell.sortOrder, outputFile);
                //    Utils.writeInt32(replacementSpell.targetMask, outputFile);
                //    Utils.writeInt32(replacementSpell.unknown10, outputFile);

                //    outputFile.Flush();
                //    writeSpellToFile = false;
                //}

                spell.spellName = Utils.ReadAndWriteString(buffer, inputFile, outputFile, writeSpellToFile);
                spell.spellDescription = Utils.ReadAndWriteString(buffer, inputFile, outputFile, writeSpellToFile);
                spell.schoolId = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                spell.iconId = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                spell.familyId = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                spell.flags = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                spell.manaCost = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                spell.unknown1 = Utils.ReadAndWriteSingle(buffer, inputFile, outputFile, writeSpellToFile);
                spell.unknown3 = Utils.ReadAndWriteSingle(buffer, inputFile, outputFile, writeSpellToFile);
                spell.difficulty = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                spell.economy = Utils.ReadAndWriteSingle(buffer, inputFile, outputFile, writeSpellToFile);
                spell.generation = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                spell.speed = Utils.ReadAndWriteSingle(buffer, inputFile, outputFile, writeSpellToFile);
                spell.spellType = (eSpellType)Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                spell.unknown4 = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);

                switch (spell.spellType)
                {
                    case eSpellType.Enchantment_SpellType:
                    case eSpellType.Transfer_SpellType:
                    case eSpellType.PortalSending_SpellType:
                        Double duration = Utils.ReadAndWriteDouble(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);

                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        break;
                    case eSpellType.Projectile_SpellType:
                        spell.damageType = (eDamageType)Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        spell.minDamage = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, false);
                        spell.damageVariance = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, false);

                        if (writeSpellToFile)
                        {
                            //if (creatureOnlySpellList.Contains(spell.spellId))
                            //{
                            int minDamage = (int)Math.Round(spell.minDamage * creatureDamageMultiplier);
                            int damageVariance = (int)Math.Round(spell.damageVariance * creatureDamageMultiplier);

                            Utils.writeInt32(minDamage, outputFile);
                            Utils.writeInt32(damageVariance, outputFile);
                            changedSpellsCounter++;
                            //}
                            //else
                            //{
                            //    Utils.writeInt32(spell.minDamage, outputFile);
                            //    Utils.writeInt32(spell.damageVariance, outputFile);
                            //}
                        }

                        spell.unknownProjectileValues = new int[29];
                        for (int i = 0; i < 29; i++)
                        {
                            spell.unknownProjectileValues[i] = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        }
                        break;
                    case eSpellType.Boost_SpellType:
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        int minAmount = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        int maxAmount = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        break;
                    case eSpellType.PortalLink_SpellType:
                    case eSpellType.PortalRecall_SpellType:
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        break;
                    case eSpellType.PortalSummon_SpellType:
                        duration = Utils.ReadAndWriteDouble(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        break;
                    case eSpellType.Dispel_SpellType:
                    case eSpellType.FellowDispel_SpellType:
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        break;
                    case eSpellType.LifeProjectile_SpellType:
                        eDamageType damageType = (eDamageType)Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        for (int i = 0; i < 33; i++)
                            Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        break;
                    case eSpellType.FellowBoost_SpellType:
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        break;
                    case eSpellType.FellowEnchantment_SpellType:
                        duration = Utils.ReadAndWriteDouble(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        break;
                    case eSpellType.FellowPortalSending_SpellType:
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                        break;
                    default:
                        break;
                }

                spell.component = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    spell.component[i] = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                }

                spell.casterEffect = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                spell.targetEffect = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);

                spell.unknown6 = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                spell.unknown7 = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                spell.unknown8 = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                spell.unknown9 = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);

                spell.sortOrder = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                spell.targetMask = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);
                spell.unknown10 = Utils.ReadAndWriteInt32(buffer, inputFile, outputFile, writeSpellToFile);

                if (spell.spellType == eSpellType.Projectile_SpellType && sharedSpellList.Contains(spell.spellId))
                    warSpells.Add(spell);

                outputFile.Flush();
            }

            //int spellId = 6341;
            //foreach (sWarSpellData spell in warSpells)
            //{
            //    duplicatedSpellsCounter++;
            //    spellCounter++;

            //    creatureReplacementSpellMap.Add(spell.spellId, spellId);

            //    Utils.writeInt32(spellId, outputFile);
            //    Utils.writeString(spell.spellName, outputFile);
            //    Utils.writeString(spell.spellDescription, outputFile);
            //    Utils.writeInt32(spell.schoolId, outputFile);
            //    Utils.writeInt32(spell.iconId, outputFile);
            //    Utils.writeInt32(spell.familyId, outputFile);
            //    Utils.writeInt32(spell.flags, outputFile);
            //    Utils.writeInt32(spell.manaCost, outputFile);
            //    Utils.writeSingle(spell.unknown1, outputFile);
            //    Utils.writeSingle(spell.unknown3, outputFile);
            //    Utils.writeInt32(spell.difficulty, outputFile);
            //    Utils.writeSingle(spell.economy, outputFile);
            //    Utils.writeInt32(spell.generation, outputFile);
            //    Utils.writeSingle(spell.speed, outputFile);
            //    Utils.writeInt32((int)spell.spellType, outputFile);
            //    Utils.writeInt32(spell.unknown4, outputFile);

            //    Utils.writeInt32((int)spell.damageType, outputFile);

            //    int minDamage = (int)Math.Round(spell.minDamage * creatureDamageMultiplier);
            //    int damageVariance = (int)Math.Round(spell.damageVariance * creatureDamageMultiplier);

            //    Utils.writeInt32(minDamage, outputFile);
            //    Utils.writeInt32(damageVariance, outputFile);

            //    for (int i = 0; i < 29; i++)
            //    {
            //        Utils.writeInt32(spell.unknownProjectileValues[i], outputFile);
            //    }

            //    for (int i = 0; i < 8; i++)
            //    {
            //        Utils.writeInt32(spell.component[i], outputFile);
            //    }

            //    Utils.writeInt32(spell.casterEffect, outputFile);
            //    Utils.writeInt32(spell.targetEffect, outputFile);

            //    Utils.writeInt32(spell.unknown6, outputFile);
            //    Utils.writeInt32(spell.unknown7, outputFile);
            //    Utils.writeInt32(spell.unknown8, outputFile);
            //    Utils.writeInt32(spell.unknown9, outputFile);

            //    Utils.writeInt32(spell.sortOrder, outputFile);
            //    Utils.writeInt32(spell.targetMask, outputFile);
            //    Utils.writeInt32(spell.unknown10, outputFile);

            //    outputFile.Flush();
            //    spellId++;
            //}

            inputFile.Close();
            outputFile.Close();
            timer.Stop();
            Console.WriteLine("{0} spells written in {1} seconds.", spellCounter, timer.ElapsedMilliseconds / 1000f);
            //Console.WriteLine(" - {0} spells duplicated and adjusted.", duplicatedSpellsCounter, timer.ElapsedMilliseconds / 1000f);
            Console.WriteLine(" - {0} spells adjusted.", changedSpellsCounter, timer.ElapsedMilliseconds / 1000f);

            return creatureReplacementSpellMap;
        }
    }
}
