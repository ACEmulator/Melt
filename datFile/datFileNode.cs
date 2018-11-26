using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public class cDatFileNode //BTNode
    {
        public uint startBlockOffset;
        public int blockSize;

        //public List<uint> branches;
        public SortedDictionary<uint, cDatFileEntry> files;
        public List<cDatFileNode> subFolders;

        public List<cDatFileBlock> listOfBlocks;

        public static int sizeToD = (sizeof(uint) * 62) + sizeof(uint) + (sizeof(uint) * 6 * 61);
        public static int sizeRetail = (sizeof(uint) * 62) + sizeof(uint) + (sizeof(uint) * 3 * 61);

        public cDatFileNode(cDatFile datFile, cDatFileBlockCache blockCache)
        {
            blockSize = datFile.blockSize;

            int size;
            if (datFile.fileFormat == eDatFormat.ToD)
                size = sizeToD;
            else
                size = sizeRetail;

            listOfBlocks = blockCache.getNewListOfBlocks(size);
            startBlockOffset = listOfBlocks[0].blockOffset;

            files = new SortedDictionary<uint, cDatFileEntry>(); //max 61
            subFolders = new List<cDatFileNode>();
        }

        public cDatFileNode(byte[] buffer, StreamReader inputFile, cDatFileBlockCache blockCache, uint startBlockOffset, int blockSize, eDatFormat fileFormat)
        {
            this.startBlockOffset = startBlockOffset;
            this.blockSize = blockSize;

            int size;
            if (fileFormat == eDatFormat.ToD)
                size = sizeToD;
            else
                size = sizeRetail;

            listOfBlocks = new List<cDatFileBlock>();
            MemoryStream memoryStream = blockCache.blocksToData(startBlockOffset, size, listOfBlocks);
            memoryStream.Position = 0;
            StreamReader reader = new StreamReader(memoryStream);

            files = new SortedDictionary<uint, cDatFileEntry>(); //max 61
            subFolders = new List<cDatFileNode>(); //max 62

            List<uint> subFolderOffsets = new List<uint>();

            for (int i = 0; i < 62; i++)
                subFolderOffsets.Add(Utils.ReadUInt32(buffer, reader));
            uint entryCount = Utils.ReadUInt32(buffer, reader);

            // folder is allowed to have (files + 1) subfolders
            if (subFolderOffsets[0] != 0)
            {
                for (int i = 0; i < entryCount + 1; i++)
                {
                    cDatFileNode newDirectory = new cDatFileNode(buffer, inputFile, blockCache, subFolderOffsets[i], blockSize, fileFormat);
                    subFolders.Add(newDirectory);
                }
            }

            for (uint i = 0; i < entryCount; i++)
            {
                cDatFileEntry file = new cDatFileEntry(buffer, reader, fileFormat);
                files.Add(file.fileId, file);
            }
        }

        public void loadFilesAndAddToCache(SortedDictionary<uint, cDatFileEntry> fileCache, cDatFileBlockCache blockCache, StreamReader inputFile, int blockSize)
        {
            foreach (cDatFileNode entry in subFolders)
            {
                entry.loadFilesAndAddToCache(fileCache, blockCache, inputFile, blockSize);
            }

            foreach (KeyValuePair<uint, cDatFileEntry> entry in files)
            {
                cDatFileEntry fileEntry = entry.Value;

                fileEntry.listOfBlocks = new List<cDatFileBlock>();
                fileEntry.fileContent = blockCache.blocksToData(fileEntry.startBlockOffset, fileEntry.fileSize, fileEntry.listOfBlocks);

                fileCache[fileEntry.fileId] = fileEntry;
            }
        }

        public void updateBlockData(cDatFile datFile, cDatFileBlockCache blockCache)
        {
            MemoryStream memoryStream = new MemoryStream(new byte[sizeToD]);
            StreamWriter writer = new StreamWriter(memoryStream);

            for (int i = 0; i < 62; i++)
            {
                if (i == 0 && subFolders.Count == 0)
                    Utils.writeUInt32(0, writer);
                else if (i < subFolders.Count && i < files.Count + 1) //directory is allowed to have (files + 1) subdirectories
                {
                    cDatFileNode subFolder = subFolders[i];

                    subFolder.updateBlockData(datFile, blockCache);

                    Utils.writeUInt32(subFolder.startBlockOffset, writer);
                }
                else
                    Utils.writeUInt32(0xcdcdcdcd, writer);
            }

            Utils.writeUInt32((uint)files.Count, writer);

            foreach (KeyValuePair<uint, cDatFileEntry> entry in files)
            {
                cDatFileEntry fileEntry = entry.Value;
                if (fileEntry.fileContent != null)
                {
                    fileEntry.fileSize = (int)fileEntry.fileContent.Length;
                    blockCache.dataToBlocks(fileEntry.listOfBlocks, fileEntry.fileContent);
                    if (fileEntry.listOfBlocks.Count > 0)
                        fileEntry.startBlockOffset = fileEntry.listOfBlocks[0].blockOffset;
                }
                else
                    fileEntry.fileSize = 0;
                fileEntry.writeHeader(writer);
            }

            cDatFileEntry emptyFileEntry = new cDatFileEntry(0, datFile.fileFormat);
            for (int i = files.Count; i < 61; i++)
            {
                emptyFileEntry.writeHeader(writer);
            }

            writer.Flush();

            blockCache.dataToBlocks(listOfBlocks, memoryStream);
            if (listOfBlocks.Count > 0)
                startBlockOffset = listOfBlocks[0].blockOffset;
        }
    }
}