using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Melt
{
    class Program
    {
        //public static cCache4Converter cache4Converter = new cCache4Converter();
        public static cCache6Converter cache6Converter = new cCache6Converter();
        public static cCache8Converter cache8Converter = new cCache8Converter();
        public static cCache9Converter cache9Converter = new cCache9Converter();

        static void Main(string[] args)
        {
            //CharGen charGen = new CharGen("./input/0E000002.bin");
            //charGen.modify();
            //charGen.save("./0E000002 - CharGen - Classic.bin");
            //Console.ReadLine();
            //return;

            ////TextureConverter.folderToPNG("textures ToD");
            ////TextureConverter.folderBMPToPNG("C:/Users/Dekaru/Desktop/Research/Textures Retail");
            ////TextureConverter.toPNG("textures ToD/06003afb.bin");
            ////Console.ReadLine();
            ////return;

            ////TextureIdDictionary.folderExtractTextureFromHeader("./input/textureHeaders/");
            ////return;

            //RegionConverter.convert("Region/13000000.bin");
            //RegionConverterDM.convert("Region/130F0000 Bael.bin");
            //RegionConverterDM.convert("Region/130F0000 test.bin");

            cDatFile datFile = new cDatFile();
            datFile.loadFromDat("./input/dats/client_cell_1.dat");

            ////cDatFile datFileOld = new cDatFile();
            ////datFileOld.loadFromDat("./input/cell - 2000-08.dat");

            cDatFile datFileOld = new cDatFile();
            datFileOld.loadFromDat("./input/dats/DM retail/cell.dat");

            //datFileOld.convertSurfaceData();
            //datFile.migrateSurfaceData(datFileOld);
            ////datFile.migrateEverything(datFileOld);
            datFile.migrateEverything(datFileOld);
            //datFile.duplicateTrainingAcademy();
            //datFile.migrateTrainingAcademy(datFileOld);
            //datFile.buildTextureIdMigrationTable(datFileOld);
            ////datFile.migrateDungeon(datFileOld, 0x02B9, 0x02B9);
            ////datFile.migrateDungeon(datFileOld, 0x018A, 0x018A);

            //datFile.writeToDat("client_cell_1.dat");
            ////datFileOld.writeToDat("client_cell_1.dat");
            datFile.writeToDat("client_cell_1.dat");

            //cDatFile datFile = new cDatFile();
            //datFile.loadFromDat("./input/client_portal - ToD.dat");

            //cDatFile datFileOther = new cDatFile();
            //datFileOther.loadFromDat("./input/client_portal_latest_unmodified.dat");

            //datFile.importNewEntries(datFileOther);

            ////datFile.writeToDat("client_portal.dat");

            //cDatFile pdatFile = new cDatFile();
            //pdatFile.loadFromDat("./input/dats/client_portal.dat");

            //cDatFile pdatFileOther = new cDatFile();
            //pdatFileOther.loadFromDat("./input/dats/103199 beta/portal.dat");

            //pdatFile.importNewEntries(pdatFileOther);

            //pdatFile.writeToDat("client_portal.dat");

            //Console.ReadLine();
            //return;

            //cCellDat cellDat = new cCellDat();
            //////cellDat.loadFromDat(datFileOld);
            //cellDat.loadFromDat(datFile);
            //cMapDrawer mapDrawer = new cMapDrawer(cellDat);
            //mapDrawer.draw();

            cCellDat cellDat = new cCellDat();
            cellDat.loadFromDat(datFile);
            cMapDrawer mapDrawer = new cMapDrawer(cellDat);
            mapDrawer.draw();
            mapDrawer.draw(true);

            //testRandomValueGenerator(5);
            //Console.ReadLine();
            //return;

            //cache4Converter.loadFromRaw("./input/0004.raw");
            ////cache8Converter.loadFromJson("./output/questFlags.json");
            ////cache8Converter.writeRaw("./output/");
            //cache4Converter.writeJson("./output/");
            //Console.ReadLine();
            //return;

            //testing
            //args = new string[3];
            //args[0] = "cached";
            //args[1] = "./input/";
            //args[2] = "./input/lootProfile.json";
            //testing

            //Patcher.patch();
            //return;

            //SpellsConverter.toTxt("Spells/0E00000E - 2010.bin");
            //SpellsConverter.toTxt("Spells/0E00000E - latest.bin");
            //SpellsConverter.raw0002toTxt("intermediate/0002.raw");
            //return;
            //SpellsConverter.to0002raw("Spells/0E00000E - latest.txt");
            //SpellsConverter.toBin("Spells/0E00000E - Reversed.txt");
            //SpellsConverter.toTxt("Spells/0E00000E - Reversed.bin");
            //SpellsConverter.revertWeaponMasteries("Spells/0E00000E - Latest.txt", "Spells/0E00000E - 2010.txt");
            //SpellsConverter.toBin("0E00000E.txt");
            //SpellsConverter.toBin("Spells/0E00000E - Reversed plus removed auras.txt");
            //Diff.FolderDiff("E:\\Downloads\\Asheron's Call\\AC Utilities\\Dat Patcher\\All\\Summer", "E:\\Downloads\\Asheron's Call\\AC Utilities\\Dat Patcher\\All\\Winter");
            //TextureConverter.toPNG("06006D40.bin");
            //TextureConverter.darkMajestyfolderToPNG("Landscape Texture Conversion/DM/Textures");
            //TextureConverter.darkMajestyfolderToPNG("Landscape Texture Conversion/DM/Detail Textures");
            //TextureConverter.darkMajestyfolderToPNG("Landscape Texture Conversion/DM/Alpha Maps");
            //TextureConverter.darkMajestyToPNG("Landscape Texture Conversion/DM/Textures/05001c3c.bin");
            //TextureConverter.folderToPNG("Landscape Texture Conversion/ToD/Detail Textures");
            //TextureConverter.folderToPNG("Landscape Texture Conversion/ToD/Textures");
            //TextureConverter.folderToPNG("Landscape Texture Conversion/ToD/Alpha Maps");
            //RegionConverter.convert("Region/13000000.bin");
            //RegionConverterDM.convert("Region/130F0000 Bael.bin");
            //RegionComparer.compare("Region/130F0000 DM.bin", false, "Region/130F0000 Bael.bin", false);
            //TextureHeader.folderExtractTextureFromHeader("Landscape Texture Conversion/ToD/Alpha Maps/Headers");
            //TextureConverter.toBin("Landscape Texture Conversion/ToD/Alpha Maps/06006d6b.png", 0x06006d6b, 244);
            //DMtoToDTexture.convert();
            //cCache9Converter.writeJson("./PhatAC Cache/0009.raw", true);
            //cCache9Converter.writeJson("./input/0009.raw", "./output/weenies/", false);
            //cCache9Converter.writeJson("./PhatAC Cache/0009.raw", true, eWeenieTypes.Creature);
            //ClassNameConverter.convert();

            //cCache9Converter.generateRandomLoot("./PhatAC Cache/0009.raw", "./input/lootProfile2.json", false, true, false, true);//write creature entries to raw
            //cCache9Converter.generateRandomLoot("./PhatAC Cache/0009.raw", "./input/lootProfile2.json", true, false, true, false);//write items to json

            //cCache9Converter.generateRandomLoot("./PhatAC Cache/0009.raw", "./input/lootProfile.json", false, true, false, true);//write creature entries to raw
            //cCache9Converter.generateRandomLoot("./PhatAC Cache/0009.raw", "./input/lootProfile.json", true, false, true, false);//write items to json
            //cCache9Converter.generateRandomLoot("./PhatAC Cache/0009.raw", "./input/lootProfile.json", true, true, true, false);//write everything to json

            //cCache9Converter.generateRandomLoot("../input/0009.raw", "../input/lootProfile.json", "../output/cached/", true, true, false, true);//write everything to raw

            //cCache9Converter.writeExtendedJson("./PhatAC Cache/0009.raw", false);
            //cCache9Converter.writeRawFromExtendedJson("./output/extended weenies/");

            //cCache9Converter.writeRawFromRaw("./PhatAC Cache/0009.raw");

            //cCache9Converter.writeJson("./input/0009.raw", false);
            //cCache9Converter.writeExtendedJson("./input/0009.raw", false);
            //cCache9Converter.writeRawFromExtendedJson("./input/extended weenies/");

            Console.WriteLine("Done");
            Console.ReadLine();
            return;

            //bool invalidArgs = false;

            //if (args.Length == 0)
            //    invalidArgs = true;

            //if (!invalidArgs)
            //{
            //    switch (args[0].ToLower())
            //    {
            //        case "cached":
            //            {
            //                string fileLootProfile;
            //                string file0002;
            //                string file0009;

            //                if (args.Length >= 3)
            //                {
            //                    file0002 = Path.Combine(args[1], "0002.raw");
            //                    file0009 = Path.Combine(args[1], "0009.raw");
            //                    fileLootProfile = args[2];

            //                    if (!File.Exists(fileLootProfile))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", fileLootProfile);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0009))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0009);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0002))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0002);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }

            //                sLootProfile lootProfile = cache9Converter.generateRandomLoot(file0009, file0002, fileLootProfile, "./output/cached/", true, true, false, true);//write everything to raw

            //                Console.WriteLine("Waiting while CachePwn builds cache.bin...");
            //                Stopwatch timer = new Stopwatch();
            //                timer.Start();
            //                Process cachePwnProcess = new Process();
            //                string path = AppDomain.CurrentDomain.BaseDirectory;
            //                cachePwnProcess.StartInfo.FileName = $"{path}CachePwn.exe";
            //                cachePwnProcess.StartInfo.Arguments = "2 .\\intermediate .\\output\\cached";
            //                cachePwnProcess.Start();
            //                cachePwnProcess.WaitForExit();
            //                timer.Stop();
            //                Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);

            //                if (lootProfile.otherOptions.copyOutputToFolder != "")
            //                {
            //                    if (!Directory.Exists(lootProfile.otherOptions.copyOutputToFolder))
            //                        Console.WriteLine("Invalid copyOutputToFolder: {0}", lootProfile.otherOptions.copyOutputToFolder);
            //                    else
            //                    {
            //                        Console.WriteLine("Copying output to \"{0}\"...", lootProfile.otherOptions.copyOutputToFolder);
            //                        timer.Reset();
            //                        timer.Start();

            //                        Utils.copyDirectory(".\\output\\cached", lootProfile.otherOptions.copyOutputToFolder, true, true);

            //                        timer.Stop();
            //                        Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            //                    }
            //                }
            //                return;
            //            }
            //        case "split":
            //            {
            //                string fileLootProfile;
            //                string file0002;
            //                string file0009;

            //                if (args.Length >= 3)
            //                {
            //                    file0002 = Path.Combine(args[1], "0002.raw");
            //                    file0009 = Path.Combine(args[1], "0009.raw");
            //                    fileLootProfile = args[2];

            //                    if (!File.Exists(fileLootProfile))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", fileLootProfile);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0009))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0009);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0002))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0002);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }
            //                sLootProfile lootProfile = cache9Converter.generateRandomLoot(file0009, file0002, fileLootProfile, "./output/split/", false, true, false, true);//write creature entries to raw
            //                cache9Converter.generateRandomLoot(file0009, "", fileLootProfile, "./output/split/json/weenies/", true, false, true, false);//write items to json

            //                Console.WriteLine("Waiting while CachePwn builds cache.bin...");
            //                Stopwatch timer = new Stopwatch();
            //                timer.Start();
            //                Process cachePwnProcess = new Process();
            //                string path = AppDomain.CurrentDomain.BaseDirectory;
            //                cachePwnProcess.StartInfo.FileName = $"{path}CachePwn.exe";
            //                cachePwnProcess.StartInfo.Arguments = "2 .\\intermediate .\\output\\split";
            //                cachePwnProcess.Start();
            //                cachePwnProcess.WaitForExit();
            //                timer.Stop();
            //                Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);

            //                if (lootProfile.otherOptions.copyOutputToFolder != "")
            //                {
            //                    if (!Directory.Exists(lootProfile.otherOptions.copyOutputToFolder))
            //                        Console.WriteLine("Invalid copyOutputToFolder: {0}", lootProfile.otherOptions.copyOutputToFolder);
            //                    else
            //                    {
            //                        Console.WriteLine("Copying output to \"{0}\"...", lootProfile.otherOptions.copyOutputToFolder);
            //                        timer.Reset();
            //                        timer.Start();

            //                        Utils.copyDirectory(".\\output\\split", lootProfile.otherOptions.copyOutputToFolder, true, true);

            //                        timer.Stop();
            //                        Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            //                    }
            //                }
            //                return;
            //            }
            //        case "json":
            //            {
            //                string fileLootProfile;
            //                string file0002;
            //                string file0009;

            //                if (args.Length >= 3)
            //                {
            //                    file0002 = Path.Combine(args[1], "0002.raw");
            //                    file0009 = Path.Combine(args[1], "0009.raw");
            //                    fileLootProfile = args[2];

            //                    if (!File.Exists(fileLootProfile))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", fileLootProfile);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0009))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0009);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0002))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0002);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }
            //                cache9Converter.generateRandomLoot(file0009, file0002, fileLootProfile, "./output/json/", true, true, true, false);//write everything to json
            //                return;
            //            }
            //        case "loottablesjson":
            //            {
            //                string fileLootProfile;
            //                string file0002;
            //                string file0009;

            //                if (args.Length >= 3)
            //                {
            //                    file0002 = Path.Combine(args[1], "0002.raw");
            //                    file0009 = Path.Combine(args[1], "0009.raw");
            //                    fileLootProfile = args[2];

            //                    if (!File.Exists(fileLootProfile))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", fileLootProfile);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0009))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0009);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0002))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0002);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }
            //                cache9Converter.generateRandomLoot(file0009, file0002, fileLootProfile, "./output/json/", false, true, true, false);//write loot tables to json
            //                return;
            //            }
            //        case "loottables":
            //            {
            //                string fileLootProfile;
            //                string file0002;
            //                string file0009;

            //                if (args.Length >= 3)
            //                {
            //                    file0002 = Path.Combine(args[1], "0002.raw");
            //                    file0009 = Path.Combine(args[1], "0009.raw");
            //                    fileLootProfile = args[2];

            //                    if (!File.Exists(fileLootProfile))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", fileLootProfile);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0009))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0009);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0002))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0002);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }
            //                cache9Converter.generateRandomLoot(file0009, file0002, fileLootProfile, "./output/split/", false, true, false, true);//write loot tables to raw
            //                return;
            //            }
            //        case "weenies":
            //            {
            //                string file0009;

            //                if (args.Length >= 2)
            //                {
            //                    file0009 = Path.Combine(args[1], "0009.raw");

            //                    if (!File.Exists(file0009))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0009);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }
            //                cache9Converter.writeJson(file0009, "./output/weenies/", false);
            //                return;
            //            }
            //        case "landblocks":
            //            {
            //                string file0006;
            //                string file0009;

            //                if (args.Length >= 2)
            //                {
            //                    file0006 = Path.Combine(args[1], "0006.raw");
            //                    file0009 = Path.Combine(args[1], "0009.raw");

            //                    if (!File.Exists(file0006))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0006);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                    else if (!File.Exists(file0009))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0009);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }
            //                cache9Converter.loadWeeniesRaw(file0009);
            //                cache6Converter.loadFromRaw(file0006);
            //                cache6Converter.writeJson("./output/landblocks");
            //                return;
            //            }
            //        case "questflags":
            //            {
            //                string file0008;

            //                if (args.Length >= 2)
            //                {
            //                    file0008 = Path.Combine(args[1], "0008.raw");

            //                    if (!File.Exists(file0008))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0008);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }
            //                cache8Converter.loadFromRaw(file0008);
            //                cache8Converter.writeJson("./output");
            //                return;
            //            }
            //        case "raw2json":
            //            {
            //                string file0009;

            //                if (args.Length >= 2)
            //                {
            //                    file0009 = Path.Combine(args[1], "0009.raw");

            //                    if (!File.Exists(file0009))
            //                    {
            //                        Console.WriteLine("Invalid file: {0}", file0009);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }
            //                cache9Converter.writeExtendedJson(file0009, "./output/extended weenies/", false);
            //                return;
            //            }
            //        case "json2raw":
            //            {
            //                string folderExtendedWeenies;

            //                if (args.Length >= 2)
            //                {
            //                    folderExtendedWeenies = args[1];

            //                    if (!Directory.Exists(folderExtendedWeenies))
            //                    {
            //                        Console.WriteLine("Invalid folder: {0}", folderExtendedWeenies);
            //                        Console.ReadLine();
            //                        return;
            //                    }
            //                }
            //                else
            //                {
            //                    invalidArgs = true;
            //                    break;
            //                }

            //                cache9Converter.writeRawFromExtendedJson(folderExtendedWeenies);

            //                Console.WriteLine("Waiting while CachePwn builds cache.bin...");
            //                Stopwatch timer = new Stopwatch();
            //                timer.Start();
            //                Process cachePwnProcess = new Process();
            //                string path = AppDomain.CurrentDomain.BaseDirectory;
            //                cachePwnProcess.StartInfo.FileName = $"{path}CachePwn.exe";
            //                cachePwnProcess.StartInfo.Arguments = "2 .\\intermediate \".\\output\\extended weenies\"";
            //                cachePwnProcess.Start();
            //                cachePwnProcess.WaitForExit();
            //                timer.Stop();
            //                Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
            //                return;
            //            }
            //    }
            //}

            //if (invalidArgs)
            //{
            //    Console.WriteLine("Invalid arguments.");
            //    Console.WriteLine("Valid Modes:");
            //    Console.WriteLine("    Cached: produces a cache.bin file that contains both the generated loot and modified creatures and chests entries. Due to a limitation this file can only store up to 26550 new entries.");
            //    Console.WriteLine("        Usage: (mode) (inputFolder) (profileFile)");
            //    Console.WriteLine("    Split: produces a cache.bin file that contains the modified creatures and chests entries, and json files for the generated loot. Thus avoiding the limitation mentioned above but increasing server starting times considerably.");
            //    Console.WriteLine("        Usage: (mode) (inputFolder) (profileFile)");
            //    Console.WriteLine("    lootTables");
            //    Console.WriteLine("        Usage: (mode) (inputFolder) (profileFile)");
            //    Console.WriteLine("    Json: no cache.bin is produced, instead all modified entries are created as json.");
            //    Console.WriteLine("        Usage: (mode) (inputFolder) (profileFile)");
            //    Console.WriteLine("    lootTablesJson");
            //    Console.WriteLine("        Usage: (mode) (inputFolder) (profileFile)");
            //    Console.WriteLine("    raw2json: generate extended weenies that can later be reconverted into cached files.");
            //    Console.WriteLine("        Usage: (mode) (inputFolder)");
            //    Console.WriteLine("    json2raw: generate cache.bin file from extended weenies");
            //    Console.WriteLine("        Usage: (mode) (inputFolder)");
            //    Console.WriteLine("    Weenies: generate commented weenies files.");
            //    Console.WriteLine("        Usage: (mode) (inputFolder)");
            //    Console.WriteLine("    Landblocks: generate landblock files.");
            //    Console.WriteLine("        Usage: (mode) (inputFolder)");
            //    Console.ReadLine();
            //}
        }

        static void testRandomValueGenerator(double preferredValue)
        {
            int testRolls = 10000;
            SortedDictionary<int, int> valueDistribution = new SortedDictionary<int, int>();
            for (int i = 0; i < testRolls; i++)
            {
                int test = Utils.getRandomNumberExclusive(100);
                //int test = (int)Math.Floor(Utils.getRandomDouble(0.5, 4.0, eRandomFormula.favorLow, 1.5) * 10);
                //int test = Utils.getRandomNumber(5, 6, eRandomFormula.favorSpecificValue, 5, 4);
                //int test = Utils.getRandomNumber(5, 6, eRandomFormula.favorSpecificValue, preferredValue, 1.8d);
                //int test = Utils.getRandomNumber(1, 10, eRandomFormula.favorSpecificValue, 4, 3);
                //int test = Utils.getRandomNumber(1, eRandomFormula.favorLow, 2d);
                //int test = Utils.getRandomNumber(1, eRandomFormula.favorLow, 2d);
                //int test = Utils.getRandomNumber(1, 2, eRandomFormula.favorSpecificValue, 1.2, 1.8d);
                //int test = Utils.getRandomNumberExclusive(10, eRandomFormula.equalDistribution);
                //int test = (int)Math.Round(Utils.getRandomDouble(0.7d, 1.0d, eRandomFormula.favorMid, 1.4d)*100);
                if (valueDistribution.ContainsKey(test))
                    valueDistribution[test]++;
                else
                    valueDistribution.Add(test, 1);
            }

            foreach (KeyValuePair<int, int> entry in valueDistribution)
            {
                Console.WriteLine($"value: {entry.Key} amount: {entry.Value} percent: {entry.Value * 100d / testRolls}%");
            }

        }
    }
}
