using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Melt
{
    class SpellsConverter
    {
        static public void toTxt(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return;
            }
            StreamWriter outputFile = new StreamWriter(new FileStream(".\\0E00000E.txt", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open 0E00000E.txt");
                return;
            }

            Console.WriteLine("Converting spells from binary to txt...");

            byte[] buffer = new byte[1024];

            int fileHeader;
            short spellCount;
            short unknown1;

            fileHeader = Utils.ReadInt32(buffer, inputFile);
            if (fileHeader != 0x0E00000E)
            {
                Console.WriteLine("Invalid header, aborting.");
                return;
            }

            spellCount = Utils.ReadInt16(buffer, inputFile);
            unknown1 = Utils.ReadInt16(buffer, inputFile);

            outputFile.WriteLine("{0}", spellCount);
            outputFile.WriteLine("{0}", unknown1);
            outputFile.Flush();

            for (int entry = 0; entry < spellCount; entry++)
            {
                int spellId;
                string spellName;
                string spellDescription;
                uint hash;
                int schoolId;
                int iconId;
                int familyId;
                int flags;
                int manaCost;
                float unknown2; //min range?
                float unknown3; //extra range per level?
                int difficulty;
                float economy;
                int generation;
                float speed;
                int spellType;
                int unknown4; //Id?
                double duration = 0d; //if(spellType = 1/7/12)
                int unknown5a = 0; //if(type = 1/12) // = 0xc4268000 for buffs/debuffs, = 0 for wars/portals -- related to duration?
                int unknown5b = 0; //if(type = 1/12) // = 0xc4268000 for buffs/debuffs, = 0 for wars/portals -- related to duration?
                int[] component = new int[8];
                int casterEffect;
                int targetEffect;
                int unknown6;
                int unknown7;
                int unknown8;
                int unknown9;
                int sortOrder;
                int targetMask;
                int unknown10; //something for fellowship spells

                spellId = Utils.ReadInt32(buffer, inputFile);

                spellName = Utils.ReadEncodedString(buffer, inputFile);
                spellDescription = Utils.ReadEncodedString(buffer, inputFile);
                hash = Utils.GetHash(spellDescription, 0xBEADCF45) + Utils.GetHash(spellName, 0x12107680);
                schoolId = Utils.ReadInt32(buffer, inputFile);
                iconId = Utils.ReadInt32(buffer, inputFile);
                familyId = Utils.ReadInt32(buffer, inputFile);
                flags = Utils.ReadInt32(buffer, inputFile);
                manaCost = Utils.ReadInt32(buffer, inputFile);
                unknown2 = Utils.ReadSingle(buffer, inputFile);
                unknown3 = Utils.ReadSingle(buffer, inputFile);
                difficulty = Utils.ReadInt32(buffer, inputFile);
                economy = Utils.ReadSingle(buffer, inputFile);
                generation = Utils.ReadInt32(buffer, inputFile);
                speed = Utils.ReadSingle(buffer, inputFile);
                spellType = Utils.ReadInt32(buffer, inputFile);
                unknown4 = Utils.ReadInt32(buffer, inputFile);

                switch (spellType)
                {
                    case 1:
                        duration = Utils.ReadDouble(buffer, inputFile);
                        unknown5a = Utils.ReadInt32(buffer, inputFile);
                        unknown5b = Utils.ReadInt32(buffer, inputFile);
                        break;
                    case 7:
                        duration = Utils.ReadDouble(buffer, inputFile);
                        break;
                    case 12:
                        duration = Utils.ReadDouble(buffer, inputFile);
                        unknown5a = Utils.ReadInt32(buffer, inputFile);
                        unknown5b = Utils.ReadInt32(buffer, inputFile);
                        break;
                    default:
                        break;
                }

                for (int i = 0; i < 8; i++)
                {
                    int hashedComponent = Utils.ReadInt32(buffer, inputFile);
                    if (hashedComponent != 0)
                        component[i] = (int)(hashedComponent - hash);
                    else
                        component[i] = 0;
                }

                casterEffect = Utils.ReadInt32(buffer, inputFile);
                targetEffect = Utils.ReadInt32(buffer, inputFile);

                unknown6 = Utils.ReadInt32(buffer, inputFile);
                unknown7 = Utils.ReadInt32(buffer, inputFile);
                unknown8 = Utils.ReadInt32(buffer, inputFile);
                unknown9 = Utils.ReadInt32(buffer, inputFile);

                sortOrder = Utils.ReadInt32(buffer, inputFile);
                targetMask = Utils.ReadInt32(buffer, inputFile);
                unknown10 = Utils.ReadInt32(buffer, inputFile);

                outputFile.WriteLine("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|{20}|{21}|{22}|{23}|{24}|{25}|{26}|{27}|{28}|{29}|{30}|{31}|{32}|{33}|{34}|{35}",
                    spellId, spellName, spellDescription, schoolId, iconId, familyId, flags,
                    manaCost, unknown2, unknown3, difficulty, economy, generation, speed, spellType,
                    unknown4, duration, unknown5a, unknown5b, component[0], component[1], component[2], component[3],
                    component[4], component[5], component[6], component[7], casterEffect, targetEffect,
                    unknown6, unknown7, unknown8, unknown9, sortOrder, targetMask, unknown10);

                //if (difficulty < 1)
                //    difficulty = 10;
                //if (spellName.Contains("Minor ") && difficulty < 150)
                //    difficulty = 150;
                //if (spellName.Contains("Moderate ") && difficulty < 175)
                //    difficulty = 175;
                //else if (spellName.Contains("Major ") && difficulty < 200)
                //    difficulty = 200;
                //else if (spellName.Contains("Epic ") && difficulty < 250)
                //    difficulty = 250;
                //else if (spellName.Contains("Legendary ") && difficulty < 300)
                //    difficulty = 300;
                //outputFile.WriteLine("map.Add({0}, new sSpellInfo({0},\"{1}\",{2},{3}));", spellId, spellName, manaCost, difficulty);

                //outputFile.WriteLine("map.Add({0}, new sSpellInfo({0},\"{1}\"));", spellId, spellName);

                //if(spellType == 2)
                //{
                //    if(spellDescription == "CREATURE MAGIC ONLY!")
                //        outputFile.WriteLine("creatureOnlySpellList.Add({0}); //{1}", spellId, spellName);
                //    else
                //        outputFile.WriteLine("replaceSpellList.Add({0}); //{1}", spellId, spellName);
                //}

                outputFile.Flush();
            }

            //unknown data
            while (true)
            {
                int bytesRead = inputFile.BaseStream.Read(buffer, 0, 4);
                if (bytesRead != 4)
                    break;
                int unknown11 = BitConverter.ToInt32(buffer, 0);
                outputFile.WriteLine("{0}", unknown11);
                outputFile.Flush();
            }

            inputFile.Close();
            outputFile.Close();
            Console.WriteLine("Done");
        }

        public static void convertStringToByteArrayNoEncode(string text, ref byte[] byteArray, int startIndex, int length)
        {
            for (int i = startIndex; i < startIndex + length; i++)
            {
                byte nextByte = (byte)text[i];
                byteArray[i] = nextByte;
            }

            byte fillerByte = 0x00;
            for (int i = startIndex + length; i < startIndex + length + 4; i++)
            {

                byteArray[i] = fillerByte;
            }
        }

        public static void convertStringToByteArray(string text, ref byte[] byteArray, int startIndex, int length)
        {
            for (int i = startIndex; i < startIndex + length; i++)
            {
                byte nextByte = (byte)((text[i] << 4) ^ (text[i] >> 4));
                byteArray[i] = nextByte;
            }

            byte fillerByte = (byte)((0 << 4) ^ (0 >> 4));
            for (int i = startIndex + length; i < startIndex + length + 4; i++)
            {

                byteArray[i] = fillerByte;
            }
        }

        static public void toBin(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return;
            }
            StreamWriter outputFile = new StreamWriter(new FileStream(".\\0E00000E.bin", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open 0E00000E.bin");
                return;
            }

            Console.WriteLine("Converting spells from txt to binary...");

            string line;
            string[] spell;
            byte[] buffer = new byte[1024];
            char[] textArray = new char[1024];

            int fileHeader = 0x0E00000E;
            short spellCount;
            short unknown1;

            line = inputFile.ReadLine();
            spellCount = Convert.ToInt16(line);

            line = inputFile.ReadLine();
            unknown1 = Convert.ToInt16(line);

            outputFile.BaseStream.Write(BitConverter.GetBytes(fileHeader), 0, 4);
            outputFile.BaseStream.Write(BitConverter.GetBytes(spellCount), 0, 2);
            outputFile.BaseStream.Write(BitConverter.GetBytes(unknown1), 0, 2);
            outputFile.Flush();

            for (int entry = 0; entry < spellCount; entry++)
            {
                line = inputFile.ReadLine();
                spell = line.Split('|');

                int spellId = Convert.ToInt32(spell[0]);
                string spellName = spell[1];
                string spellDescription = spell[2];
                uint hash;
                int schoolId = Convert.ToInt32(spell[3]);
                int iconId = Convert.ToInt32(spell[4]);
                int familyId = Convert.ToInt32(spell[5]);
                int flags = Convert.ToInt32(spell[6]);
                int manaCost = Convert.ToInt32(spell[7]);
                float unknown2 = Convert.ToSingle(spell[8]);
                float unknown3 = Convert.ToSingle(spell[9]);
                int difficulty = Convert.ToInt32(spell[10]);
                float economy = Convert.ToSingle(spell[11]);
                int generation = Convert.ToInt32(spell[12]);
                float speed = Convert.ToSingle(spell[13]);
                int spellType = Convert.ToInt32(spell[14]);
                int unknown4 = Convert.ToInt32(spell[15]);

                double duration = 0d; //if(spellType = 1/7/12)
                int unknown5a = 0; //if(type = 1/12) // = 0xc4268000 for buffs/debuffs, = 0 for wars/portals -- related to duration?
                int unknown5b = 0; //if(type = 1/12) // = 0xc4268000 for buffs/debuffs, = 0 for wars/portals -- related to duration?
                switch (spellType)
                {
                    case 1:
                        duration = Convert.ToDouble(spell[16]);
                        unknown5a = Convert.ToInt32(spell[17]);
                        unknown5b = Convert.ToInt32(spell[18]);
                        break;
                    case 7:
                        duration = Convert.ToDouble(spell[16]);
                        break;
                    case 12:
                        duration = Convert.ToDouble(spell[16]);
                        unknown5a = Convert.ToInt32(spell[17]);
                        unknown5b = Convert.ToInt32(spell[18]);
                        break;
                    default:
                        break;
                }

                hash = Utils.GetHash(spellDescription, 0xBEADCF45) + Utils.GetHash(spellName, 0x12107680);

                int[] component = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    int hashedComponent = Convert.ToInt32(spell[19 + i]);
                    if (hashedComponent != 0)
                        component[i] = (int)(hashedComponent + hash);
                    else
                        component[i] = 0;
                }

                int casterEffect = Convert.ToInt32(spell[27]);
                int targetEffect = Convert.ToInt32(spell[28]);
                int unknown6 = Convert.ToInt32(spell[29]);
                int unknown7 = Convert.ToInt32(spell[30]);
                int unknown8 = Convert.ToInt32(spell[31]);
                int unknown9 = Convert.ToInt32(spell[32]);
                int sortOrder = Convert.ToInt32(spell[33]);
                int targetMask = Convert.ToInt32(spell[34]);
                int unknown10 = Convert.ToInt32(spell[35]);

                outputFile.BaseStream.Write(BitConverter.GetBytes(spellId), 0, 4);

                outputFile.BaseStream.Write(BitConverter.GetBytes((short)spellName.Length), 0, 2);
                convertStringToByteArray(spellName, ref buffer, 0, spellName.Length);
                int startIndex = (int)outputFile.BaseStream.Position;
                int endIndex = (int)outputFile.BaseStream.Position + spellName.Length + 2;
                int alignedIndex = Utils.Align4(endIndex - startIndex);
                int newIndex = startIndex + alignedIndex;
                int bytesNeededToReachAlignment = newIndex - endIndex;
                outputFile.BaseStream.Write(buffer, 0, spellName.Length + bytesNeededToReachAlignment);

                outputFile.BaseStream.Write(BitConverter.GetBytes((short)spellDescription.Length), 0, 2);
                convertStringToByteArray(spellDescription, ref buffer, 0, spellDescription.Length);
                startIndex = (int)outputFile.BaseStream.Position;
                endIndex = (int)outputFile.BaseStream.Position + spellDescription.Length + 2;
                alignedIndex = Utils.Align4(endIndex - startIndex);
                newIndex = startIndex + alignedIndex;
                bytesNeededToReachAlignment = newIndex - endIndex;
                outputFile.BaseStream.Write(buffer, 0, spellDescription.Length + bytesNeededToReachAlignment);

                outputFile.BaseStream.Write(BitConverter.GetBytes(schoolId), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(iconId), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(familyId), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(flags), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(manaCost), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown2), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown3), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(difficulty), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(economy), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(generation), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(speed), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(spellType), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown4), 0, 4);

                switch (spellType)
                {
                    case 1:
                        outputFile.BaseStream.Write(BitConverter.GetBytes(duration), 0, 8);
                        outputFile.BaseStream.Write(BitConverter.GetBytes(unknown5a), 0, 4);
                        outputFile.BaseStream.Write(BitConverter.GetBytes(unknown5b), 0, 4);
                        break;
                    case 7:
                        outputFile.BaseStream.Write(BitConverter.GetBytes(duration), 0, 8);
                        break;
                    case 12:
                        outputFile.BaseStream.Write(BitConverter.GetBytes(duration), 0, 8);
                        outputFile.BaseStream.Write(BitConverter.GetBytes(unknown5a), 0, 4);
                        outputFile.BaseStream.Write(BitConverter.GetBytes(unknown5b), 0, 4);
                        break;
                    default:
                        break;
                }

                for (int i = 0; i < 8; i++)
                {
                    outputFile.BaseStream.Write(BitConverter.GetBytes(component[i]), 0, 4);
                }

                outputFile.BaseStream.Write(BitConverter.GetBytes(casterEffect), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(targetEffect), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown6), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown7), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown8), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown9), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(sortOrder), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(targetMask), 0, 4);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown10), 0, 4);

                outputFile.Flush();
            }

            //unknown data
            while (!inputFile.EndOfStream)
            {
                line = inputFile.ReadLine();
                int unknown11 = Convert.ToInt32(line);
                outputFile.BaseStream.Write(BitConverter.GetBytes(unknown11), 0, 4);
                outputFile.Flush();
            }

            inputFile.Close();
            outputFile.Close();
            Console.WriteLine("Done");
        }

        static public void revertWeaponMasteries(string filename1, string originalSpellsFilename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename1, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename1);
                return;
            }
            StreamReader inputFile2 = new StreamReader(new FileStream(originalSpellsFilename, FileMode.Open, FileAccess.Read));
            if (inputFile2 == null)
            {
                Console.WriteLine("Unable to open {0}", originalSpellsFilename);
                return;
            }

            StreamWriter outputFile = new StreamWriter(new FileStream(".\\0E00000E.txt", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open 0E00000E.txt");
                return;
            }

            Console.WriteLine("Reverting weapon masteries spells...");

            string line;
            string line2;
            string[] spell;
            string[] spellOld;

            byte[] buffer = new byte[1024];
            char[] textArray = new char[1024];

            //int fileHeader = 0x0E00000E;
            short spellCount;
            short unknown1;

            short spellCount2;
            short unknown1_2;

            line = inputFile.ReadLine();
            spellCount = Convert.ToInt16(line);
            outputFile.WriteLine(line);

            line = inputFile.ReadLine();
            unknown1 = Convert.ToInt16(line);
            outputFile.WriteLine(line);

            outputFile.Flush();

            line2 = inputFile2.ReadLine();
            spellCount2 = Convert.ToInt16(line2);

            line2 = inputFile2.ReadLine();
            unknown1_2 = Convert.ToInt16(line2);

            for (int entry = 0; entry < spellCount; entry++)
            {
                line = inputFile.ReadLine();
                spell = line.Split('|');

                int spellId = Convert.ToInt32(spell[0]);
                string spellName = spell[1];
                string spellDescription = spell[2];
                int schoolId = Convert.ToInt32(spell[3]);
                int iconId = Convert.ToInt32(spell[4]);
                int familyId = Convert.ToInt32(spell[5]);
                eSpellIndex flags = (eSpellIndex)Convert.ToInt32(spell[6]);
                int baseManaCost = Convert.ToInt32(spell[7]);
                float unknown2 = Convert.ToSingle(spell[8]);
                float unknown3 = Convert.ToSingle(spell[9]);
                int power = Convert.ToInt32(spell[10]);
                float economy = Convert.ToSingle(spell[11]);
                int generation = Convert.ToInt32(spell[12]);
                float speed = Convert.ToSingle(spell[13]);
                eSpellType spellType = (eSpellType)Convert.ToInt32(spell[14]);
                int unknown4 = Convert.ToInt32(spell[15]);

                double duration = 0d; //if(spellType = 1/7/12)
                int unknown5a = 0; //if(type = 1/12) // = 0xc4268000 for buffs/debuffs, = 0 for wars/portals -- related to duration?
                int unknown5b = 0; //if(type = 1/12) // = 0xc4268000 for buffs/debuffs, = 0 for wars/portals -- related to duration?
                switch (spellType)
                {
                    case eSpellType.Enchantment_SpellType:
                        duration = Convert.ToDouble(spell[16]);
                        unknown5a = Convert.ToInt32(spell[17]);
                        unknown5b = Convert.ToInt32(spell[18]);
                        break;
                    case eSpellType.PortalSummon_SpellType:
                        duration = Convert.ToDouble(spell[16]);
                        break;
                    case eSpellType.FellowEnchantment_SpellType:
                        duration = Convert.ToDouble(spell[16]);
                        unknown5a = Convert.ToInt32(spell[17]);
                        unknown5b = Convert.ToInt32(spell[18]);
                        break;
                    default:
                        break;
                }

                int[] component = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    component[i] = Convert.ToInt32(spell[19 + i]);
                }

                int casterEffect = Convert.ToInt32(spell[27]);
                int targetEffect = Convert.ToInt32(spell[28]);
                int fizzleEffect = Convert.ToInt32(spell[29]);
                int unknown7 = Convert.ToInt32(spell[30]);
                int unknown8 = Convert.ToInt32(spell[31]);
                int unknown9 = Convert.ToInt32(spell[32]);
                int sortOrder = Convert.ToInt32(spell[33]);
                eItemType targetMask = (eItemType)Convert.ToInt32(spell[34]);
                int unknown10 = Convert.ToInt32(spell[35]);

                if (entry < spellCount2)
                {
                    line2 = inputFile2.ReadLine();
                    spellOld = line2.Split('|');

                    int spellIdOld = Convert.ToInt32(spellOld[0]);
                    string spellNameOld = spellOld[1];
                    string spellDescriptionOld = spellOld[2];
                    int schoolIdOld = Convert.ToInt32(spellOld[3]);
                    int iconIdOld = Convert.ToInt32(spellOld[4]);
                    int casterEffectOld = Convert.ToInt32(spellOld[27]);
                    int targetEffectOld = Convert.ToInt32(spellOld[28]);
                    int fizzleEffectOld = Convert.ToInt32(spellOld[29]);

                    eItemType targetMaskOld = (eItemType)Convert.ToInt32(spellOld[34]);
                    eSpellIndex flagsOld = (eSpellIndex)Convert.ToInt32(spell[6]);


                    if (targetMaskOld != targetMask)
                    {
                        flags &= ~(eSpellIndex.SelfTargeted_SpellIndex);

                        for (int i = 0; i < 8; i++)
                        {
                            //60 = rowan talisman(creature enchantment self)
                            //49 = poplar talisman(creature enchantment other)
                            if (component[i] == 60 || component[i] == 49)
                                component[i] = 57; //change rowan and poplar talisman to ashwood talisman
                        }                        

                        spellName = spellNameOld;
                        spellDescription = spellDescriptionOld;
                        iconId = iconIdOld;
                        casterEffect = casterEffectOld;
                        targetEffect = targetEffectOld;
                        fizzleEffect = fizzleEffectOld;
                        targetMask = targetMaskOld;
                    }

                    if (spellName.Contains("Light Weapon") ||
                        spellName.Contains("Finesse Weapon") ||
                        spellName.Contains("Heavy Weapon") ||
                        spellName.Contains("Light Weapon") ||
                        spellName.Contains("Missile Weapon") ||
                        spellName.Contains("Cascade"))
                    {
                        spellName = spellNameOld;
                        spellDescription = spellDescriptionOld;
                        iconId = iconIdOld;
                        casterEffect = casterEffectOld;
                        targetEffect = targetEffectOld;
                        fizzleEffect = fizzleEffectOld;
                    }
                }

                outputFile.WriteLine("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|{20}|{21}|{22}|{23}|{24}|{25}|{26}|{27}|{28}|{29}|{30}|{31}|{32}|{33}|{34}|{35}",
                    spellId, spellName, spellDescription, schoolId, iconId, familyId, (int)flags,
                    baseManaCost, unknown2, unknown3, power, economy, generation, speed, (int)spellType,
                    unknown4, duration, unknown5a, unknown5b, component[0], component[1], component[2], component[3],
                    component[4], component[5], component[6], component[7], casterEffect, targetEffect,
                    fizzleEffect, unknown7, unknown8, unknown9, sortOrder, (int)targetMask, unknown10);
                outputFile.Flush();
            }

            //unknown data
            while (!inputFile.EndOfStream)
            {
                line = inputFile.ReadLine();
                outputFile.WriteLine(line);
                outputFile.Flush();
            }

            inputFile.Close();
            outputFile.Close();
            Console.WriteLine("Done");
        }
    }
}
