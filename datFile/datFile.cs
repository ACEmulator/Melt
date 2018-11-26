using BTreeNamespace;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Melt
{
    public enum eDatFormat
    {
        invalid,
        retail,
        ToD 
    }

    public class cDatFile
    {
        public eDatFormat fileFormat;

        public byte[] acTransactionRecord;

        public uint fileType;
        public int blockSize;
        public uint fileSize;
        public uint dataSet;
        public uint dataSubset;

        public uint freeHead;
        public uint freeTail;
        public uint freeCount;
        public uint rootDirectoryOffset;

        public uint youngLRU;
        public uint oldLRU;
        public uint useLRU;

        public uint masterMapId;

        public uint enginePackVersion;
        public uint gamePackVersion;
        public byte[] versionMajor;
        public uint versionMinor;

        public cDatFileNode rootDirectory;

        public cDatFileBlockCache inputBlockCache;
        public cDatFileBlockCache outputBlockCache;
        public SortedDictionary<uint, cDatFileEntry> fileCache;

        public cBTree bTree;

        public cDatFile()
        {
            fileCache = new SortedDictionary<uint, cDatFileEntry>();
        }

        public void loadFromDat(string filename)
        {
            byte[] buffer = new byte[1024];

            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile.BaseStream.Length < 1024)
            {
                Console.WriteLine("{0} is too small to be a valid dat file", filename);
                return;
            }

            Console.WriteLine("Reading data from {0}...", filename);
            Stopwatch timer = new Stopwatch();
            timer.Start();

            inputFile.BaseStream.Seek(257, SeekOrigin.Begin);
            int format = Utils.ReadInt32(buffer, inputFile);
            if (format == 0x4C50)
            {
                inputFile.BaseStream.Seek(256, SeekOrigin.Begin); //skip acVersionStr which is empty
                acTransactionRecord = Utils.ReadBytes(buffer, inputFile, 64);
                for (int i = 4; i < 64; i++)
                {
                    acTransactionRecord[i] = 0;
                }
                fileType = Utils.ReadUInt32(buffer, inputFile);
                if (fileType == 0x5442)
                {
                    fileFormat = eDatFormat.ToD;
                }
            }
            else
            {
                acTransactionRecord = new byte[64] { 0x00, 0x50, 0x4C, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0xFF, 0xFF, 0x00, 0x93, 0x1D, 0x0F, 0x0C, 0x00, 0x00, 0x00, 0x94, 0xD0, 0x9E, 0x42, 0x01, 0x00, 0x00, 0x00, 0x00, 0xAC, 0x1D, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x25, 0x00, 0x00, 0x00, 0x00, 0xAB, 0x1D, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                acTransactionRecord[0] = 0x00;
                acTransactionRecord[1] = 0x50;
                acTransactionRecord[2] = 0x4C;
                acTransactionRecord[3] = 0x00;
                for (int i = 4; i < 64; i++)
                {
                    acTransactionRecord[i] = 0;
                }
                inputFile.BaseStream.Seek(300, SeekOrigin.Begin);
                fileType = Utils.ReadUInt32(buffer, inputFile);
                if (fileType == 0x5442)
                    fileFormat = eDatFormat.retail;
            }

            if (fileFormat == eDatFormat.invalid)
            {
                Console.WriteLine("{0} is not a valid dat file.", filename);
                return;
            }

            blockSize = Utils.ReadInt32(buffer, inputFile);
            fileSize = Utils.ReadUInt32(buffer, inputFile);
            dataSet = Utils.ReadUInt32(buffer, inputFile);
            if (fileFormat == eDatFormat.ToD)
                dataSubset = Utils.ReadUInt32(buffer, inputFile);
            else
            {
                dataSet = 2;
                dataSubset = 1;
            }

            freeHead = Utils.ReadUInt32(buffer, inputFile);
            freeTail = Utils.ReadUInt32(buffer, inputFile);
            freeCount = Utils.ReadUInt32(buffer, inputFile);
            rootDirectoryOffset = Utils.ReadUInt32(buffer, inputFile);

            if (fileFormat == eDatFormat.ToD)
            {
                youngLRU = Utils.ReadUInt32(buffer, inputFile);
                oldLRU = Utils.ReadUInt32(buffer, inputFile);
                useLRU = Utils.ReadUInt32(buffer, inputFile);

                masterMapId = Utils.ReadUInt32(buffer, inputFile);

                enginePackVersion = Utils.ReadUInt32(buffer, inputFile);
                gamePackVersion = Utils.ReadUInt32(buffer, inputFile);
                versionMajor = Utils.ReadBytes(buffer, inputFile, 16);//int data1, short data2, short data3, int64 data4
                versionMinor = Utils.ReadUInt32(buffer, inputFile);
            }
            else
            {
                youngLRU = 0;
                oldLRU = 0;
                useLRU = 0xCDCDCD00;

                masterMapId = 0;

                enginePackVersion = 0x16;
                gamePackVersion = 0;
                versionMajor = new byte[] { 0xD2, 0xD7, 0xA7, 0x34, 0x2F, 0x72, 0x46, 0x4C, 0x8A, 0xB4, 0xEF, 0x51, 0x4F, 0x85, 0x6F, 0xFD };

                versionMinor = 222;
            }

            inputBlockCache = new cDatFileBlockCache(fileSize, blockSize, freeHead, freeTail, freeCount);

            cDatFileBlock.loadBlocksAndAddToDictionary(inputBlockCache, inputFile, blockSize);

            rootDirectory = new cDatFileNode(buffer, inputFile, inputBlockCache, rootDirectoryOffset, blockSize, fileFormat);
            rootDirectory.loadFilesAndAddToCache(fileCache, inputBlockCache, inputFile, blockSize);

            timer.Stop();
            Console.WriteLine("{0} blocks read in {1} seconds.", inputBlockCache.blocks.Count, timer.ElapsedMilliseconds / 1000f);
        }

        //we only write to the ToD data format.
        public void writeToDat(string filename)
        {
            fileSize = inputBlockCache.fileSize;
            freeHead = inputBlockCache.freeHead;
            freeTail = inputBlockCache.freeTail;
            freeCount = inputBlockCache.freeCount;

            byte[] buffer = new byte[1024];

            StreamWriter outputFile = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write));

            Console.WriteLine("Writing data to {0}...", filename);
            Stopwatch timer = new Stopwatch();
            timer.Start();

            ////keep some free blocks
            //if (inputBlockCache.freeCount < 1000)
            //    inputBlockCache.addNewFreeBlocks(1000 - (int)inputBlockCache.freeCount);

            //rootDirectory.updateBlockData(this, inputBlockCache);

            //foreach (KeyValuePair<uint, cDatFileBlock> entry in inputBlockCache.blocks)
            //{
            //    outputFile.BaseStream.Seek(entry.Key, SeekOrigin.Begin);
            //    entry.Value.writeToDat(outputFile, blockSize);
            //}

            //fileSize = inputBlockCache.fileSize;
            //freeHead = inputBlockCache.fileSize;
            //freeTail = inputBlockCache.fileSize;
            //freeCount = inputBlockCache.fileSize;

            bTree = new cBTree(31);

            foreach (KeyValuePair<uint, cDatFileEntry> entry in fileCache)
            {
                bTree.Insert(entry.Key);
            }

            outputBlockCache = new cDatFileBlockCache(blockSize, 1024);
            rootDirectory = buildBtreeStructure(bTree.Root, outputBlockCache);
            rootDirectory.updateBlockData(this, outputBlockCache);

            foreach (KeyValuePair<uint, cDatFileBlock> entry in outputBlockCache.blocks)
            {
                outputFile.BaseStream.Seek(entry.Key, SeekOrigin.Begin);
                entry.Value.writeToDat(outputFile, blockSize);
            }

            if (outputBlockCache.freeCount < 1000)
                outputBlockCache.addNewFreeBlocks(1000 - (int)outputBlockCache.freeCount);

            rootDirectoryOffset = rootDirectory.startBlockOffset;
            fileSize = outputBlockCache.fileSize;
            freeHead = outputBlockCache.fileSize;
            freeTail = outputBlockCache.fileSize;
            freeCount = outputBlockCache.fileSize;

            outputFile.BaseStream.Seek(256, SeekOrigin.Begin); //skip acVersionStr which is empty
            Utils.writeBytes(acTransactionRecord, outputFile);

            Utils.writeUInt32(fileType, outputFile);

            Utils.writeInt32(blockSize, outputFile);
            Utils.writeUInt32(fileSize, outputFile);
            Utils.writeUInt32(dataSet, outputFile);
            Utils.writeUInt32(dataSubset, outputFile);

            Utils.writeUInt32(freeHead, outputFile);
            Utils.writeUInt32(freeTail, outputFile);
            Utils.writeUInt32(freeCount, outputFile);
            Utils.writeUInt32(rootDirectoryOffset, outputFile);

            Utils.writeUInt32(youngLRU, outputFile);
            Utils.writeUInt32(oldLRU, outputFile);
            Utils.writeUInt32(useLRU, outputFile);

            Utils.writeUInt32(masterMapId, outputFile);

            Utils.writeUInt32(enginePackVersion, outputFile);
            Utils.writeUInt32(gamePackVersion, outputFile);
            Utils.writeBytes(versionMajor, outputFile);
            Utils.writeUInt32(versionMinor, outputFile);

            outputFile.Close();
            timer.Stop();
            Console.WriteLine("{0} blocks written in {1} seconds.", inputBlockCache.blocks.Count, timer.ElapsedMilliseconds / 1000f);

            exportDirTree(rootDirectory);
        }

        public cDatFileNode buildBtreeStructure(Node node, cDatFileBlockCache blockCache)
        {
            cDatFileNode newFolder = new cDatFileNode(this, blockCache);
            foreach (uint entry in node.Entries)
            {
                cDatFileEntry file = fileCache[entry];
                file.startBlockOffset = 0;
                file.listOfBlocks = new List<cDatFileBlock>();
                //blockCache.dataToBlocks(file.listOfBlocks, file.fileContent);
                //file.startBlockOffset = file.listOfBlocks[0].blockOffset;
                newFolder.files.Add(file.fileId, file);
            }

            if (!node.IsLeaf)
            {
                foreach (Node child in node.Children)
                {
                    cDatFileNode newSubFolder = buildBtreeStructure(child, blockCache);
                    newFolder.subFolders.Add(newSubFolder);
                }
            }

            //newFolder.updateBlockData(this, blockCache);

            return newFolder;
        }

        //public cDatFileNode getWorkFolder(cDatFileNode folder)
        //{
        //    if (folder.files.Count < 30)
        //        return folder;

        //    foreach (cDatFileNode subFolder in folder.subFolders)
        //    {
        //        if (subFolder.files.Count < 30)
        //            return subFolder;

        //        foreach (cDatFileNode subFolder2 in subFolder.subFolders)
        //        {
        //            if (subFolder2.files.Count < 30)
        //                return subFolder2;
        //        }
        //    }

        //    if (folder.subFolders.Count <= folder.files.Count)
        //    {
        //        cDatFileNode newFolder = new cDatFileNode(this, inputBlockCache);
        //        folder.subFolders.Add(newFolder);
        //        return newFolder;
        //    }

        //    foreach (cDatFileNode subFolder in folder.subFolders)
        //    {
        //        if (subFolder.subFolders.Count <= subFolder.files.Count)
        //        {
        //            cDatFileNode newFolder = new cDatFileNode(this, inputBlockCache);
        //            subFolder.subFolders.Add(newFolder);
        //            return newFolder;
        //        }
        //    }

        //    throw new ArgumentException();
        //}

        //public cDatFileFolder findValidSpotForNewFile(cDatFileFolder directory)
        //{
        //    for (int i = 0; i < directory.files.Count; i++)
        //    {
        //        if (i < directory.subFolders.Count)
        //        {
        //            if (directory.files.Count < 30)
        //            {
        //                return directory;
        //            }
        //        }
        //    }
        //    return null;
        //}

        //public cDatFileFolder findValidSpotForNewDirectory(cDatFileFolder directory)
        //{
        //    for (int i = 0; i < directory.files.Count + 1; i++)
        //    {
        //        if (i < directory.subFolders.Count)
        //        {
        //            if (directory.files.Count < 30)
        //            {
        //                return directory;
        //            }
        //        }
        //    }
        //    return null;
        //}

        //public cDatFileEntry createNewFile(uint fileId)
        //{
        //    if (inputFileCache.ContainsKey(fileId))
        //        throw new ArgumentException();

        //    cDatFileNode currentWorkDirectory = getWorkFolder(rootDirectory);
        //    cDatFileEntry newFile = new cDatFileEntry(fileId, fileFormat);

        //    currentWorkDirectory.files.Add(newFile.fileId, newFile);

        //    inputFileCache.Add(fileId, newFile);

        //    return newFile;
        //}

        public void convertSurfaceData()
        {
            Console.WriteLine("Converting surface data...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            int landBlockCounter = 0;
            byte[] buffer = new byte[1024];
            foreach (KeyValuePair<uint, cDatFileEntry> entry in fileCache)
            {
                cDatFileEntry file;
                if (!fileCache.TryGetValue(entry.Value.fileId, out file))
                    continue;

                StreamReader reader = new StreamReader(file.fileContent);

                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //surface
                {
                    cCellLandblock landblock = new cCellLandblock(buffer, reader);

                    file.fileContent.SetLength(0);
                    StreamWriter writer = new StreamWriter(file.fileContent);
                    landblock.writeToDat(writer);
                    writer.Flush();

                    landBlockCounter++;
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //surface objects
                {
                    cLandblockInfo landblockInfo = new cLandblockInfo(buffer, reader, fileFormat);

                    file.fileContent.SetLength(0);
                    StreamWriter writer = new StreamWriter(file.fileContent);
                    landblockInfo.writeToDat(writer);
                    writer.Flush();
                }
                else //dungeons and interiors
                {
                    cEnvCell envCell = new cEnvCell(buffer, reader, fileFormat);

                    file.fileContent.SetLength(0);
                    StreamWriter writer = new StreamWriter(file.fileContent);
                    envCell.writeToDat(writer);
                    writer.Flush();
                }
            }

            timer.Stop();
            Console.WriteLine("{0} landblocks converted in {1} seconds.", landBlockCounter, timer.ElapsedMilliseconds / 1000f);
        }

        public void exportDirTree(cDatFileNode directory)
        {
            StreamWriter outputFile = new StreamWriter(new FileStream(".\\dirTree.json", FileMode.Create, FileAccess.Write));
            outputFile.WriteLine($"-- root directory({directory.files.Count} files, {directory.subFolders.Count} subDirectories)-- ");
            exportSubDirTrees(directory, 0, outputFile);
        }

        private void exportSubDirTrees(cDatFileNode directory, int tabCount, StreamWriter outputFile)
        {
            string tab = "";
            for (int i = 0; i < tabCount; i++)
            {
                tab += "    ";
            }

            foreach (KeyValuePair<uint, cDatFileEntry> entry in directory.files)
            {
                cDatFileEntry file = entry.Value;
                //if ((file.fileId & 0x0000FFFF) == 0x0000FFFF)
                //{
                //    uint x = (uint)file.fileId >> 24;
                //    uint y = (uint)(file.fileId & 0x00FF0000) >> 16;

                //    outputFile.WriteLine($"{tab}file: {file.fileId.ToString("x8")} = cellLandblock {x},{y}");
                //}
                //else if((file.fileId & 0x0000FFFE) == 0x0000FFFE)
                //{
                //    uint x = (uint)file.fileId >> 24;
                //    uint y = (uint)(file.fileId & 0x00FF0000) >> 16;

                //    outputFile.WriteLine($"{tab}file: {file.fileId.ToString("x8")} = landblockInfo {x},{y}");
                //}
                //else
                //outputFile.WriteLine($"{tab}file: {file.fileId.ToString("x8")} bitFlags:{file.bitFlags.ToString("x8")}");
                outputFile.WriteLine($"{tab}file: {file.fileId.ToString("x8")}");
            }

            outputFile.Flush();

            int subDirCount = 0;
            foreach (cDatFileNode subDirectory in directory.subFolders)
            {
                outputFile.WriteLine($"{tab}-- {tabCount} subDirectory {subDirCount} ({subDirectory.files.Count} files, {subDirectory.subFolders.Count} subDirectories)-- ");
                exportSubDirTrees(subDirectory, tabCount + 1, outputFile);
                subDirCount++;
            }

            outputFile.Flush();
        }

        public void migrateSurfaceData(cDatFile otherDat)
        {
            Console.WriteLine("Migrating surface data...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            //List<uint> filesToRemove = new List<uint>();

            //foreach (KeyValuePair<uint, cDatFileEntry> entry in fileCache)
            //{
            //    if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //surface
            //    {
            //    }
            //    else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //surface objects
            //    {
            //    }
            //    else
            //    {
            //        if ((entry.Value.fileId >> 24) != 0xC6)
            //            continue;
            //        filesToRemove.Add(entry.Value.fileId);
            //    }
            //}

            //foreach (uint entry in filesToRemove)
            //{
            //    fileCache.Remove(entry);
            //}

            int landBlockCounter = 0;
            foreach (KeyValuePair<uint, cDatFileEntry> entry in otherDat.fileCache)
            {
                byte[] buffer = new byte[1024];

                cDatFileEntry otherFile = entry.Value;
                cDatFileEntry thisFile;
                if (!fileCache.TryGetValue(entry.Value.fileId, out thisFile))
                {
                    thisFile = new cDatFileEntry(entry.Value.fileId, eDatFormat.ToD);
                    fileCache.Add(entry.Value.fileId, thisFile);
                }

                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //surface
                {
                    StreamReader reader = new StreamReader(otherFile.fileContent);
                    cCellLandblock otherLandblock = new cCellLandblock(buffer, reader);

                    otherFile.fileContent.SetLength(0);
                    StreamWriter writer = new StreamWriter(otherFile.fileContent);
                    otherLandblock.writeToDat(writer);
                    writer.Flush();

                    otherFile.listOfBlocks = thisFile.listOfBlocks;
                    otherFile.startBlockOffset = thisFile.startBlockOffset;
                    thisFile.copyFrom(otherFile);

                    //StreamReader reader = new StreamReader(otherFile.fileContent);
                    //StreamReader reader2 = new StreamReader(thisFile.fileContent);
                    //cCellLandblock otherLandblock = new cCellLandblock(buffer, reader);
                    //cCellLandblock thisLandblock = new cCellLandblock(buffer, reader2);

                    ////if (entry.Value.fileId == 0xC6A9FFFF) //arwic
                    //////if (entry.Value.fileId == 0xC990FFFF) //rithwic crypt
                    ////if (entry.Value.fileId == 0xB89FFFFF) //cragstone house on the hill
                    ////    otherLandblock.HasObjects = true;
                    ////else
                    ////    otherLandblock.HasObjects = false;

                    //thisFile.fileContent.SetLength(0);
                    //StreamWriter writer = new StreamWriter(thisFile.fileContent);
                    //otherLandblock.writeToDat(writer);
                    //writer.Flush();

                    landBlockCounter++;
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //surface objects
                {
                    StreamReader reader = new StreamReader(otherFile.fileContent);
                    cLandblockInfo otherLandblockInfo = new cLandblockInfo(buffer, reader, otherFile.fileFormat);

                    //List<cBuildInfo> test = new List<cBuildInfo>(otherLandblockInfo.Buildings);
                    //otherLandblockInfo.Buildings.Clear();
                    //otherLandblockInfo.Buildings.Add(test[0]);

                    otherFile.fileContent.SetLength(0);
                    StreamWriter writer = new StreamWriter(otherFile.fileContent);
                    otherLandblockInfo.writeToDat(writer);
                    writer.Flush();

                    otherFile.listOfBlocks = thisFile.listOfBlocks;
                    otherFile.startBlockOffset = thisFile.startBlockOffset;
                    thisFile.copyFrom(otherFile);

                    //if (entry.Value.fileId != 0xC6A9FFFE)
                    //if (entry.Value.fileId != 0xC990FFFE)
                    //if (entry.Value.fileId != 0xB89FFFFE)
                    //    continue;

                    //StreamReader reader = new StreamReader(otherFile.fileContent);
                    //StreamReader reader2 = new StreamReader(thisFile.fileContent);
                    //cLandblockInfo thisLandblockInfo = new cLandblockInfo(buffer, reader2, fileFormat);
                    //cLandblockInfo otherLandblockInfo = new cLandblockInfo(buffer, reader, otherFile.fileFormat);

                    //otherLandblockInfo.Buildings.Clear();
                    //otherLandblockInfo.Objects.Clear();

                    //thisFile.fileContent.SetLength(0);
                    //StreamWriter writer = new StreamWriter(thisFile.fileContent);
                    //otherLandblockInfo.writeToDat(writer);
                    //writer.Flush();
                }
                else //dungeons and interiors
                {
                    StreamReader reader = new StreamReader(otherFile.fileContent);
                    //StreamReader reader2 = new StreamReader(thisFile.fileContent);
                    //cEnvCell thisEnvCell = new cEnvCell(buffer, reader2, fileFormat);
                    cEnvCell otherEnvCell = new cEnvCell(buffer, reader, otherFile.fileFormat);

                    otherEnvCell.Stabs.Clear();

                    otherFile.fileContent.SetLength(0);
                    StreamWriter writer = new StreamWriter(otherFile.fileContent);
                    otherEnvCell.writeToDat(writer);
                    writer.Flush();

                    otherFile.listOfBlocks = thisFile.listOfBlocks;
                    otherFile.startBlockOffset = thisFile.startBlockOffset;
                    thisFile.copyFrom(otherFile);

                    //if ((entry.Value.fileId >> 16) != 0xC6A9)
                    //if ((entry.Value.fileId >> 16) != 0xB89F)
                    //    continue;

                    //StreamReader reader = new StreamReader(otherFile.fileContent);
                    //StreamReader reader2 = new StreamReader(thisFile.fileContent);
                    //cEnvCell otherEnvCell = new cEnvCell(buffer, reader, otherDat.fileFormat);
                    //cEnvCell thisEnvCell = new cEnvCell(buffer, reader2, fileFormat);

                    ////otherEnvCell.Textures = thisEnvCell.Textures;
                    ////otherEnvCell.Stabs.Clear();

                    //thisFile.fileContent.SetLength(0);
                    //StreamWriter writer = new StreamWriter(thisFile.fileContent);
                    //otherEnvCell.writeToDat(writer);
                    //writer.Flush();
                }
            }

            timer.Stop();
            Console.WriteLine("{0} landblocks migrated in {1} seconds.", landBlockCounter, timer.ElapsedMilliseconds / 1000f);
        }

        public void duplicateTrainingAcademy()
        {
            Console.WriteLine("Duplicating traning academy data...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            List<cDatFileEntry> filesToAdd = new List<cDatFileEntry>();

            int landBlockCounter = 0;
            foreach (KeyValuePair<uint, cDatFileEntry> entry in fileCache)
            {
                byte[] buffer = new byte[1024];

                cDatFileEntry thisFile = entry.Value;

                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //surface
                {
                    if ((entry.Value.fileId >> 16) == 0x7203)
                    {
                        ushort high = (ushort)(entry.Value.fileId >> 16);
                        ushort low = (ushort)entry.Value.fileId;
                        uint newFileId = (uint)(low & 0xFFFF) | ((0x0363 & 0xFFFF) << 16);

                        cDatFileEntry newFile = cDatFileEntry.newFrom(thisFile, newFileId);
                        newFile.listOfBlocks = new List<cDatFileBlock>();

                        StreamReader reader = new StreamReader(newFile.fileContent);
                        cCellLandblock thisLandblock = new cCellLandblock(buffer, reader);
                        thisLandblock.Id = newFileId;

                        newFile.fileContent.SetLength(0);
                        StreamWriter writer = new StreamWriter(newFile.fileContent);
                        thisLandblock.writeToDat(writer);
                        writer.Flush();

                        filesToAdd.Add(newFile);
                    }
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //surface objects
                {
                    if ((entry.Value.fileId >> 16) == 0x7203)
                    {
                        ushort high = (ushort)(entry.Value.fileId >> 16);
                        ushort low = (ushort)entry.Value.fileId;
                        uint newFileId = (uint)(low & 0xFFFF) | ((0x0363 & 0xFFFF) << 16);

                        cDatFileEntry newFile = cDatFileEntry.newFrom(thisFile, newFileId);
                        newFile.listOfBlocks = new List<cDatFileBlock>();

                        StreamReader reader = new StreamReader(newFile.fileContent);
                        cLandblockInfo thisLandblockInfo = new cLandblockInfo(buffer, reader, fileFormat);
                        thisLandblockInfo.Id = newFileId;

                        newFile.fileContent.SetLength(0);
                        StreamWriter writer = new StreamWriter(newFile.fileContent);
                        thisLandblockInfo.writeToDat(writer);
                        writer.Flush();

                        filesToAdd.Add(newFile);
                    }
                }
                else //dungeons and interiors
                {
                    if ((entry.Value.fileId >> 16) == 0x7203)
                    {
                        ushort high = (ushort)(entry.Value.fileId >> 16);
                        ushort low = (ushort)entry.Value.fileId;
                        uint newFileId = (uint)(low & 0xFFFF) | ((0x0363 & 0xFFFF) << 16);

                        if (entry.Value.fileId == 0x720301DA)
                        { }

                        cDatFileEntry newFile = cDatFileEntry.newFrom(thisFile, newFileId);
                        newFile.listOfBlocks = new List<cDatFileBlock>();

                        StreamReader reader = new StreamReader(newFile.fileContent);
                        cEnvCell thisEnvCell = new cEnvCell(buffer, reader, fileFormat);
                        thisEnvCell.Id = newFileId;

                        newFile.fileContent.SetLength(0);
                        StreamWriter writer = new StreamWriter(newFile.fileContent);
                        thisEnvCell.writeToDat(writer);
                        writer.Flush();

                        filesToAdd.Add(newFile);
                    }
                }
            }

            foreach (cDatFileEntry file in filesToAdd)
            {
                if (fileCache.ContainsKey(file.fileId))
                    fileCache[file.fileId] = file;
                else
                    fileCache.Add(file.fileId, file);
            }

            timer.Stop();
            Console.WriteLine("{0} landblocks migrated in {1} seconds.", landBlockCounter, timer.ElapsedMilliseconds / 1000f);
        }

        public void migrateTrainingAcademy(cDatFile fromDat)
        {
            Console.WriteLine("Migrating traning academies data...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            List<cDatFileEntry> filesToAdd = new List<cDatFileEntry>();

            int landBlockCounter = 0;
            foreach (KeyValuePair<uint, cDatFileEntry> entry in fromDat.fileCache)
            {
                byte[] buffer = new byte[1024];

                cDatFileEntry otherFile = entry.Value;

                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //landblock
                {
                    if ((entry.Value.fileId >> 16) == 0x0363 ||
                        (entry.Value.fileId >> 16) == 0x0364 ||
                        (entry.Value.fileId >> 16) == 0x0365 ||
                        (entry.Value.fileId >> 16) == 0x0366 ||
                        (entry.Value.fileId >> 16) == 0x0367 ||
                        (entry.Value.fileId >> 16) == 0x0368)
                    {
                        cDatFileEntry newFile = cDatFileEntry.newFrom(otherFile, entry.Value.fileId);
                        newFile.listOfBlocks = new List<cDatFileBlock>();

                        StreamReader reader = new StreamReader(newFile.fileContent);
                        cCellLandblock thisLandblock = new cCellLandblock(buffer, reader);

                        newFile.fileContent.SetLength(0);
                        StreamWriter writer = new StreamWriter(newFile.fileContent);
                        thisLandblock.writeToDat(writer);
                        writer.Flush();

                        filesToAdd.Add(newFile);
                    }
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //landblock info
                {
                    if ((entry.Value.fileId >> 16) == 0x0363 ||
                        (entry.Value.fileId >> 16) == 0x0364 ||
                        (entry.Value.fileId >> 16) == 0x0365 ||
                        (entry.Value.fileId >> 16) == 0x0366 ||
                        (entry.Value.fileId >> 16) == 0x0367 ||
                        (entry.Value.fileId >> 16) == 0x0368)
                    {
                        cDatFileEntry newFile = cDatFileEntry.newFrom(otherFile, entry.Value.fileId);

                        StreamReader reader = new StreamReader(newFile.fileContent);
                        cLandblockInfo thisLandblockInfo = new cLandblockInfo(buffer, reader, newFile.fileFormat);

                        newFile.fileContent.SetLength(0);
                        StreamWriter writer = new StreamWriter(newFile.fileContent);
                        thisLandblockInfo.writeToDat(writer);
                        writer.Flush();

                        filesToAdd.Add(newFile);
                    }
                }
                else //dungeons and interiors
                {
                    if ((entry.Value.fileId >> 16) == 0x0363 ||
                        (entry.Value.fileId >> 16) == 0x0364 ||
                        (entry.Value.fileId >> 16) == 0x0365 ||
                        (entry.Value.fileId >> 16) == 0x0366 ||
                        (entry.Value.fileId >> 16) == 0x0367 ||
                        (entry.Value.fileId >> 16) == 0x0368)
                    {
                        cDatFileEntry newFile = cDatFileEntry.newFrom(otherFile, entry.Value.fileId);
                        newFile.listOfBlocks = new List<cDatFileBlock>();

                        StreamReader reader = new StreamReader(newFile.fileContent);
                        cEnvCell thisEnvCell = new cEnvCell(buffer, reader, newFile.fileFormat);

                        newFile.fileContent.SetLength(0);
                        StreamWriter writer = new StreamWriter(newFile.fileContent);
                        thisEnvCell.writeToDat(writer);
                        writer.Flush();

                        filesToAdd.Add(newFile);
                    }
                }
            }

            foreach (cDatFileEntry file in filesToAdd)
            {
                if (fileCache.ContainsKey(file.fileId))
                    fileCache[file.fileId] = file;
                else
                    fileCache.Add(file.fileId, file);
            }

            timer.Stop();
            Console.WriteLine("{0} landblocks migrated in {1} seconds.", landBlockCounter, timer.ElapsedMilliseconds / 1000f);
        }

        public void migrateDungeon(cDatFile fromDat, ushort oldDungeonId, ushort newDungeonId)
        {
            Console.WriteLine("Migrating dungeon {0} to {1}...", oldDungeonId.ToString("x4"), newDungeonId.ToString("x4"));
            Stopwatch timer = new Stopwatch();
            timer.Start();

            List<uint> filesToRemove = new List<uint>();

            foreach (KeyValuePair<uint, cDatFileEntry> entry in fileCache)
            {
                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //surface
                {
                    if ((entry.Value.fileId >> 16) == newDungeonId)
                        filesToRemove.Add(entry.Value.fileId);
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //surface objects
                {
                    if ((entry.Value.fileId >> 16) == newDungeonId)
                        filesToRemove.Add(entry.Value.fileId);
                }
                else //dungeons and interiors
                {
                    if ((entry.Value.fileId >> 16) == newDungeonId)
                        filesToRemove.Add(entry.Value.fileId);
                }
            }

            List<cDatFileEntry> filesToAdd = new List<cDatFileEntry>();

            int landBlockCounter = 0;
            foreach (KeyValuePair<uint, cDatFileEntry> entry in fromDat.fileCache)
            {
                byte[] buffer = new byte[1024];

                cDatFileEntry otherFile = entry.Value;

                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //landblock
                {
                    if ((entry.Value.fileId >> 16) == oldDungeonId)
                    {
                        cDatFileEntry newFile = cDatFileEntry.newFrom(otherFile, entry.Value.fileId);
                        newFile.listOfBlocks = new List<cDatFileBlock>();

                        StreamReader reader = new StreamReader(newFile.fileContent);
                        cCellLandblock thisLandblock = new cCellLandblock(buffer, reader);

                        ushort high = (ushort)(entry.Value.fileId >> 16);
                        ushort low = (ushort)entry.Value.fileId;
                        uint newFileId = (uint)(low & 0xFFFF) | (uint)((newDungeonId & 0xFFFF) << 16);
                        newFile.fileId = newFileId;
                        thisLandblock.Id = newFileId;

                        newFile.fileContent.SetLength(0);
                        StreamWriter writer = new StreamWriter(newFile.fileContent);
                        thisLandblock.writeToDat(writer);
                        writer.Flush();

                        filesToAdd.Add(newFile);
                    }
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //landblock info
                {
                    if ((entry.Value.fileId >> 16) == oldDungeonId)
                    {
                        cDatFileEntry newFile = cDatFileEntry.newFrom(otherFile, entry.Value.fileId);

                        StreamReader reader = new StreamReader(newFile.fileContent);
                        cLandblockInfo thisLandblockInfo = new cLandblockInfo(buffer, reader, newFile.fileFormat);

                        ushort high = (ushort)(entry.Value.fileId >> 16);
                        ushort low = (ushort)entry.Value.fileId;
                        uint newFileId = (uint)(low & 0xFFFF) | (uint)((newDungeonId & 0xFFFF) << 16);
                        newFile.fileId = newFileId;
                        thisLandblockInfo.Id = newFileId;

                        newFile.fileContent.SetLength(0);
                        StreamWriter writer = new StreamWriter(newFile.fileContent);
                        thisLandblockInfo.writeToDat(writer);
                        writer.Flush();

                        filesToAdd.Add(newFile);
                    }
                }
                else //dungeons and interiors
                {
                    if ((entry.Value.fileId >> 16) == oldDungeonId)
                    {
                        cDatFileEntry newFile = cDatFileEntry.newFrom(otherFile, entry.Value.fileId);
                        newFile.listOfBlocks = new List<cDatFileBlock>();

                        StreamReader reader = new StreamReader(newFile.fileContent);
                        cEnvCell thisEnvCell = new cEnvCell(buffer, reader, newFile.fileFormat);

                        ushort high = (ushort)(entry.Value.fileId >> 16);
                        ushort low = (ushort)entry.Value.fileId;
                        uint newFileId = (uint)(low & 0xFFFF) | (uint)((newDungeonId & 0xFFFF) << 16);
                        newFile.fileId = newFileId;
                        thisEnvCell.Id = newFileId;

                        if(newFileId == 0x02b90184)
                        { }

                        newFile.fileContent.SetLength(0);
                        StreamWriter writer = new StreamWriter(newFile.fileContent);
                        thisEnvCell.writeToDat(writer);
                        writer.Flush();

                        filesToAdd.Add(newFile);
                    }
                }
            }

            foreach (uint entry in filesToRemove)
            {
                fileCache.Remove(entry);
            }

            foreach (cDatFileEntry file in filesToAdd)
            {
                if (fileCache.ContainsKey(file.fileId))
                    fileCache[file.fileId] = file;
                else
                    fileCache.Add(file.fileId, file);
            }

            timer.Stop();
            Console.WriteLine("{0} landblocks migrated in {1} seconds.", landBlockCounter, timer.ElapsedMilliseconds / 1000f);
        }

        public void migrateEverything(cDatFile fromDat)
        {
            Console.WriteLine("Migrating data...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            //List<uint> filesToRemove = new List<uint>();

            //foreach (KeyValuePair<uint, cDatFileEntry> entry in fileCache)
            //{
            //    if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //surface
            //    {
            //        if ((entry.Value.fileId >> 16) == 0xC6A9)//arwic
            //            filesToRemove.Add(entry.Value.fileId);
            //    }
            //    else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //surface objects
            //    {
            //        if ((entry.Value.fileId >> 16) == 0xC6A9)//arwic
            //            filesToRemove.Add(entry.Value.fileId);
            //    }
            //    else //dungeons and interiors
            //    {
            //        if ((entry.Value.fileId >> 16) == 0xC6A9)//arwic
            //            filesToRemove.Add(entry.Value.fileId);
            //    }
            //}

            //foreach (uint entry in filesToRemove)
            //{
            //    fileCache.Remove(entry);
            //}

            List<cDatFileEntry> filesToAdd = new List<cDatFileEntry>();

            int landBlockCounter = 0;
            foreach (KeyValuePair<uint, cDatFileEntry> entry in fromDat.fileCache)
            {
                byte[] buffer = new byte[1024];

                cDatFileEntry otherFile = entry.Value;

                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //surface
                {
                    //if ((entry.Value.fileId >> 16) != 0xC6A9)//arwic
                    //    continue;

                    cDatFileEntry newFile = cDatFileEntry.newFrom(otherFile, entry.Value.fileId);
                    newFile.listOfBlocks = new List<cDatFileBlock>();

                    StreamReader reader = new StreamReader(newFile.fileContent);
                    cCellLandblock thisLandblock = new cCellLandblock(buffer, reader);

                    //thisLandblock.HasObjects = false;

                    newFile.fileContent.SetLength(0);
                    StreamWriter writer = new StreamWriter(newFile.fileContent);
                    thisLandblock.writeToDat(writer);
                    writer.Flush();

                    filesToAdd.Add(newFile);
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //surface objects
                {
                    //if ((entry.Value.fileId >> 28) != 0x2)
                    //if ((entry.Value.fileId >> 16) != 0x0809)//caul
                    //if ((entry.Value.fileId >> 24) != 0x08 && (entry.Value.fileId >> 24) != 0x09)//caul
                    //if ((entry.Value.fileId >> 16) != 0xC6A9)//arwic
                    //if ((entry.Value.fileId >> 24) != 0xC6)//arwic
                    //if ((entry.Value.fileId >> 28) != 0x6)//ice island
                    //    continue;

                    cDatFileEntry newFile = cDatFileEntry.newFrom(otherFile, entry.Value.fileId);

                    StreamReader reader = new StreamReader(newFile.fileContent);
                    cLandblockInfo thisLandblockInfo = new cLandblockInfo(buffer, reader, newFile.fileFormat);

                    //if ((entry.Value.fileId >> 16) == 0xC6A9)//arwic
                    //{
                    //    List<cBuildInfo> buildings = thisLandblockInfo.Buildings.Copy();

                    //    thisLandblockInfo.Buildings.Clear();
                    //    //thisLandblockInfo.Objects.Clear();

                    //    thisLandblockInfo.Buildings.Add(buildings[0]);
                    //}
                    //else
                    //    thisLandblockInfo.Buildings.Clear();

                    //thisLandblockInfo.Objects.Clear();
                    //if (thisLandblockInfo.Buildings.Count > 0)
                    //{
                    //    List<cBuildInfo> buildings = thisLandblockInfo.Buildings.Copy();
                    //thisLandblockInfo.Buildings.Clear();
                    //    //thisLandblockInfo.Buildings.Add(buildings[0]);
                    //}

                    newFile.fileContent.SetLength(0);
                    StreamWriter writer = new StreamWriter(newFile.fileContent);
                    thisLandblockInfo.writeToDat(writer);
                    writer.Flush();

                    filesToAdd.Add(newFile);
                }
                else //dungeons and interiors
                {
                    //if ((entry.Value.fileId >> 16) != 0xC6A9)//arwic
                    //if ((entry.Value.fileId >> 24) != 0xC6)//arwic
                    //if ((entry.Value.fileId >> 28) != 0x2)
                    //if ((entry.Value.fileId >> 16) != 0x0809)//caul
                    //if ((entry.Value.fileId >> 24) != 0x08 && (entry.Value.fileId >> 24) != 0x09)//caul
                    //if ((entry.Value.fileId >> 28) != 0x6)//ice island
                    //    continue;

                    //if (entry.Value.fileId == 0xC6A90102)
                    //{ }

                    cDatFileEntry newFile = cDatFileEntry.newFrom(otherFile, entry.Value.fileId);
                    newFile.listOfBlocks = new List<cDatFileBlock>();

                    StreamReader reader = new StreamReader(newFile.fileContent);
                    cEnvCell thisEnvCell = new cEnvCell(buffer, reader, newFile.fileFormat);

                    //if (entry.Value.fileId >> 4 != 0x0C6A9012)
                    //    thisEnvCell.Stabs.Clear();

                    //if (entry.Value.fileId != 0xC6A90102)
                    //    thisEnvCell.Stabs.Clear();

                    newFile.fileContent.SetLength(0);
                    StreamWriter writer = new StreamWriter(newFile.fileContent);
                    thisEnvCell.writeToDat(writer);
                    writer.Flush();

                    filesToAdd.Add(newFile);
                }
            }

            foreach (cDatFileEntry file in filesToAdd)
            {
                if (fileCache.ContainsKey(file.fileId))
                    fileCache[file.fileId] = file;
                else
                    fileCache.Add(file.fileId, file);
            }

            timer.Stop();
            Console.WriteLine("{0} landblocks migrated in {1} seconds.", landBlockCounter, timer.ElapsedMilliseconds / 1000f);
        }

        public void importNewEntries(cDatFile fromDat)
        {
            Console.WriteLine("Importing new entries...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            List<cDatFileEntry> filesToAdd = new List<cDatFileEntry>();

            int entriesCounter = 0;
            byte[] buffer = new byte[1024];
            foreach (KeyValuePair<uint, cDatFileEntry> entry in fromDat.fileCache)
            {
                if (fileCache.ContainsKey(entry.Key))
                    continue;

                entriesCounter++;

                cDatFileEntry otherFile = entry.Value;
                cDatFileEntry newFile = cDatFileEntry.newFrom(otherFile, entry.Value.fileId);
                newFile.listOfBlocks = new List<cDatFileBlock>();

                filesToAdd.Add(newFile);
            }

            foreach (cDatFileEntry file in filesToAdd)
            {
                if (fileCache.ContainsKey(file.fileId))
                    fileCache[file.fileId] = file;
                else
                    fileCache.Add(file.fileId, file);
            }

            timer.Stop();
            Console.WriteLine("{0} entries imported in {1} seconds.", entriesCounter, timer.ElapsedMilliseconds / 1000f);
        }

        Dictionary<uint, cEnvCell> thisEnvCells;
        Dictionary<uint, cEnvCell> otherEnvCells;
        SortedDictionary<ushort, ushort> textureIdMigrationTable;

        class cAmbiguousValues
        {
            public ushort newId;
            public List<ushort> alternateNewIds;
        }
        SortedDictionary<ushort, cAmbiguousValues> ambiguousList;

        List<ushort> allOldIds;
        SortedDictionary<ushort, ushort> missingList;

        public void buildTextureIdMigrationTable(cDatFile otherDat)
        {
            thisEnvCells = new Dictionary<uint, cEnvCell>();
            otherEnvCells = new Dictionary<uint, cEnvCell>();

            textureIdMigrationTable = new SortedDictionary<ushort, ushort>();
            ambiguousList = new SortedDictionary<ushort, cAmbiguousValues>();

            allOldIds = new List<ushort>();
            missingList = new SortedDictionary<ushort, ushort>();

            byte[] buffer = new byte[1024];
            foreach (KeyValuePair<uint, cDatFileEntry> entry in fileCache)
            {
                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //surface
                {
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //surface objects
                {
                }
                else //dungeons and interiors
                {
                    StreamReader reader = new StreamReader(entry.Value.fileContent);
                    cEnvCell thisEnvCell = new cEnvCell(buffer, reader, fileFormat);

                    thisEnvCells.Add(entry.Value.fileId, thisEnvCell);
                }
            }

            foreach (KeyValuePair<uint, cDatFileEntry> entry in otherDat.fileCache)
            {
                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //surface
                {
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //surface objects
                {
                }
                else //dungeons and interiors
                {
                    StreamReader reader = new StreamReader(entry.Value.fileContent);
                    cEnvCell otherEnvCell = new cEnvCell(buffer, reader, otherDat.fileFormat, false);

                    otherEnvCells.Add(entry.Value.fileId, otherEnvCell);
                }
            }

            foreach (KeyValuePair<uint, cEnvCell> entry in otherEnvCells)
            {
                cEnvCell thisEnvCell;
                cEnvCell otherEnvCell = entry.Value;

                if (!thisEnvCells.TryGetValue(otherEnvCell.Id, out thisEnvCell))
                    continue;

                for (int i = 0; i < otherEnvCell.Textures.Count; i++)
                {
                    ushort oldId = otherEnvCell.Textures[i];
                    if (!allOldIds.Contains(oldId))
                        allOldIds.Add(oldId);
                }

                if (compareEnvCells(thisEnvCell, otherEnvCell))
                {
                    for (int i = 0; i < otherEnvCell.Textures.Count; i++)
                    {
                        ushort oldId = otherEnvCell.Textures[i];
                        ushort newId = thisEnvCell.Textures[i];

                        ushort currentId = 0;
                        textureIdMigrationTable.TryGetValue(oldId, out currentId);

                        if(currentId == 0)
                            textureIdMigrationTable.Add(oldId, newId);
                        else
                        {
                            if (newId != currentId)
                            {
                                cAmbiguousValues ambiguousId;
                                if(!ambiguousList.TryGetValue(oldId, out ambiguousId))
                                {
                                    ambiguousId = new cAmbiguousValues();
                                    ambiguousId.newId = currentId;
                                    ambiguousId.alternateNewIds = new List<ushort>();
                                    ambiguousList.Add(oldId, ambiguousId);
                                    ambiguousId.alternateNewIds.Add(newId);
                                }
                                else
                                {
                                    if(!ambiguousId.alternateNewIds.Contains(newId))
                                        ambiguousId.alternateNewIds.Add(newId);
                                }
                                //throw new Exception("ambiguous texture id migration");
                            }
                        }
                    }
                }
            }

            foreach (ushort entry in allOldIds)
            {
                if(!textureIdMigrationTable.ContainsKey(entry))
                {
                    missingList.Add(entry, entry);
                }
            }

            StreamWriter outputFile = new StreamWriter(new FileStream("textureIdMigrationTable.txt", FileMode.Create, FileAccess.Write));
            foreach (KeyValuePair<ushort, ushort> entry in textureIdMigrationTable)
            {
                outputFile.WriteLine($"{entry.Key.ToString("x4")} {entry.Value.ToString("x4")}");
                outputFile.Flush();
            }
            outputFile.Close();

            outputFile = new StreamWriter(new FileStream("textureIdMigrationMissingConversions.txt", FileMode.Create, FileAccess.Write));
            foreach (KeyValuePair<ushort, ushort> entry in missingList)
            {
                outputFile.WriteLine(entry.Key.ToString("x4"));
                outputFile.Flush();
            }
            outputFile.Close();

            outputFile = new StreamWriter(new FileStream("textureIdMigrationTableAmbiguous.txt", FileMode.Create, FileAccess.Write));
            foreach (KeyValuePair<ushort, cAmbiguousValues> entry in ambiguousList)
            {
                outputFile.Write($"{entry.Key.ToString("x4")} {entry.Value.newId.ToString("x4")}");
                bool first = true;
                foreach (ushort value in entry.Value.alternateNewIds)
                {
                    if (first)
                    {
                        outputFile.Write("(");
                        first = false;
                    }
                    else
                        outputFile.Write(", ");
                    outputFile.Write(value.ToString("x4"));
                    outputFile.Flush();
                }
                outputFile.WriteLine(")");

                outputFile.Flush();
            }
            outputFile.Close();
        }

        private bool compareEnvCells(cEnvCell thisEnvCell, cEnvCell otherEnvCell)
        {
            if (thisEnvCell.Id != otherEnvCell.Id)
                return false;
            if (thisEnvCell.Bitfield != otherEnvCell.Bitfield)
                return false;
            if (thisEnvCell.EnvironmentId != otherEnvCell.EnvironmentId)
                return false;
            if (thisEnvCell.StructId != otherEnvCell.StructId)
                return false;
            if (thisEnvCell.RestrictionObj != otherEnvCell.RestrictionObj)
                return false;

            if (thisEnvCell.Position.angles.x != otherEnvCell.Position.angles.x)
                return false;
            if (thisEnvCell.Position.angles.y != otherEnvCell.Position.angles.y)
                return false;
            if (thisEnvCell.Position.angles.z != otherEnvCell.Position.angles.z)
                return false;
            if (thisEnvCell.Position.angles.w != otherEnvCell.Position.angles.w)
                return false;

            if (thisEnvCell.Position.origin.x != otherEnvCell.Position.origin.x)
                return false;
            if (thisEnvCell.Position.origin.y != otherEnvCell.Position.origin.y)
                return false;
            if (thisEnvCell.Position.origin.z != otherEnvCell.Position.origin.z)
                return false;

            if (thisEnvCell.Portals.Count != otherEnvCell.Portals.Count)
                return false;
            for (int i = 0; i < thisEnvCell.Portals.Count; i++)
            {
                cCellPortal thisPortal = thisEnvCell.Portals[i];
                cCellPortal otherPortal = otherEnvCell.Portals[i];
                if (thisPortal.Bitfield != otherPortal.Bitfield ||
                    thisPortal.EnvironmentId != otherPortal.EnvironmentId ||
                    thisPortal.OtherCellId != otherPortal.OtherCellId ||
                    thisPortal.OtherPortalId != otherPortal.OtherPortalId)
                    return false;
            }

            if (thisEnvCell.Cells.Count != otherEnvCell.Cells.Count)
                return false;
            for (int i = 0; i < thisEnvCell.Cells.Count; i++)
            {
                if(thisEnvCell.Cells[i] != otherEnvCell.Cells[i])
                    return false;
            }

            if (thisEnvCell.Stabs.Count != otherEnvCell.Stabs.Count)
                return false;
            for (int i = 0; i < thisEnvCell.Stabs.Count; i++)
            {
                cStab thisStab = thisEnvCell.Stabs[i];
                cStab otherStab = otherEnvCell.Stabs[i];

                if (thisStab.id != otherStab.id)
                    return false;

                if (thisStab.frame.angles.x != otherStab.frame.angles.x)
                    return false;
                if (thisStab.frame.angles.y != otherStab.frame.angles.y)
                    return false;
                if (thisStab.frame.angles.z != otherStab.frame.angles.z)
                    return false;
                if (thisStab.frame.angles.w != otherStab.frame.angles.w)
                    return false;

                if (thisStab.frame.origin.x != otherStab.frame.origin.x)
                    return false;
                if (thisStab.frame.origin.y != otherStab.frame.origin.y)
                    return false;
                if (thisStab.frame.origin.z != otherStab.frame.origin.z)
                    return false;
            }

            if (thisEnvCell.Textures.Count != otherEnvCell.Textures.Count)
                return false;

            //do not check textureIds cause that's what we're converting

            return true;
        }

        List<cCellLandblock> cellLandblockList;
        List<cLandblockInfo> landblockInfoList;
        List<cEnvCell> envCellList;
        public void exportCellJson(string outputPath)
        {
            Console.WriteLine("Writing cell.dat to json files...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            cellLandblockList = new List<cCellLandblock>();
            landblockInfoList = new List<cLandblockInfo>();
            envCellList = new List<cEnvCell>();
            byte[] buffer = new byte[1024];

            Console.WriteLine("Preparing files...");
            foreach (KeyValuePair<uint, cDatFileEntry> entry in fileCache)
            {
                if ((entry.Value.fileId & 0x0000FFFF) == 0x0000FFFF) //surface
                {
                    //StreamReader reader = new StreamReader(entry.Value.fileContent);
                    //cCellLandblock thisLandblock = new cCellLandblock(buffer, reader);

                    //cellLandblockList.Add(thisLandblock);
                }
                else if ((entry.Value.fileId & 0x0000FFFE) == 0x0000FFFE) //surface objects
                {
                    if ((entry.Value.fileId >> 16) != 0xC6A9)//arwic
                        continue;

                    StreamReader reader = new StreamReader(entry.Value.fileContent);
                    cLandblockInfo thisLandblockInfo = new cLandblockInfo(buffer, reader, fileFormat);

                    landblockInfoList.Add(thisLandblockInfo);
                }
                else //dungeons and interiors
                {
                    if ((entry.Value.fileId >> 16) != 0xC6A9)//arwic
                        continue;

                    StreamReader reader = new StreamReader(entry.Value.fileContent);
                    cEnvCell thisEnvCell = new cEnvCell(buffer, reader, fileFormat);

                    envCellList.Add(thisEnvCell);
                }
            }

            JsonSerializerSettings settings = new JsonSerializerSettings();
            //settings.TypeNameHandling = TypeNameHandling.Auto;
            //settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            //settings.DefaultValueHandling = DefaultValueHandling.Ignore;

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            //Console.WriteLine("Exporting landblocks...");
            //foreach (cCellLandblock entry in cellLandblockList)
            //{
            //    string outputFilename = Path.Combine(outputPath, (entry.Id >> 16).ToString("x4"));
            //    if (!Directory.Exists(outputFilename))
            //        Directory.CreateDirectory(outputFilename);
            //    outputFilename = Path.Combine(outputFilename, $"{entry.Id.ToString("x8")}.json");

            //    StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));

            //    string jsonString = JsonConvert.SerializeObject(entry, Formatting.Indented, settings);
            //    outputFile.Write(jsonString);
            //    outputFile.Close();
            //}

            Console.WriteLine("Exporting landblock info...");
            foreach (cLandblockInfo entry in landblockInfoList)
            {
                string outputFilename = Path.Combine(outputPath, (entry.Id >> 16).ToString("x4"));
                if (!Directory.Exists(outputFilename))
                    Directory.CreateDirectory(outputFilename);
                outputFilename = Path.Combine(outputFilename, $"{entry.Id.ToString("x8")}.json");

                StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));

                string jsonString = JsonConvert.SerializeObject(entry, Formatting.Indented, settings);
                outputFile.Write(jsonString);
                outputFile.Close();
            }

            Console.WriteLine("Exporting envCells...");
            foreach (cEnvCell entry in envCellList)
            {
                string outputFilename = Path.Combine(outputPath, (entry.Id >> 16).ToString("x4"));
                if (!Directory.Exists(outputFilename))
                    Directory.CreateDirectory(outputFilename);
                outputFilename = Path.Combine(outputFilename, $"{entry.Id.ToString("x8")}.json");

                StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));

                string jsonString = JsonConvert.SerializeObject(entry, Formatting.Indented, settings);
                outputFile.Write(jsonString);
                outputFile.Close();
            }

            timer.Stop();
            Console.WriteLine("Finished in {0} seconds.", timer.ElapsedMilliseconds / 1000f);
        }
    }
}