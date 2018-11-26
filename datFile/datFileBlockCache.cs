using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melt
{
    public class cDatFileBlockCache
    {
        public uint fileSize;
        public int blockSize;

        public uint freeHead;
        public uint freeTail;
        public uint freeCount;
        public Dictionary<uint, cDatFileBlock> blocks;

        public cDatFileBlockCache(int blockSize, uint startOffset)
        {
            this.fileSize = startOffset + (2 * (uint)blockSize);
            this.blockSize = blockSize;
            this.freeHead = startOffset;
            this.freeTail = startOffset + (uint)blockSize;
            this.freeCount = 2;
            blocks = new Dictionary<uint, cDatFileBlock>();

            addBlock(new cDatFileBlock(freeHead, freeTail, blockSize, true));
            addBlock(new cDatFileBlock(freeTail, 0, blockSize, true));
        }

        public cDatFileBlockCache(uint fileSize, int blockSize, uint freeHead, uint freeTail, uint freeCount)
        {
            this.fileSize = fileSize;
            this.blockSize = blockSize;
            this.freeHead = freeHead;
            this.freeTail = freeTail;
            this.freeCount = freeCount;
            blocks = new Dictionary<uint, cDatFileBlock>();
        }

        public void addBlock(cDatFileBlock block)
        {
            blocks.Add(block.blockOffset, block);
        }

        public void addNewFreeBlocks(int amount)
        {
            cDatFileBlock previousBlock = blocks[freeTail];
            for (int i = 1; i <= amount; i++)
            {
                cDatFileBlock newBlock = new cDatFileBlock(fileSize, 0, blockSize, true);
                blocks.Add(newBlock.blockOffset, newBlock);

                previousBlock.nextBlockOffset = newBlock.blockOffset;
                previousBlock = newBlock;
                freeCount++;
                fileSize += (uint)blockSize;
            }
            freeTail = previousBlock.blockOffset;
        }

        public List<cDatFileBlock> getNewBlocks(int amount)
        {
            if (amount >= freeCount)
                addNewFreeBlocks(amount + 1000);

            List<cDatFileBlock> list = new List<cDatFileBlock>();

            cDatFileBlock freeBlock;
            for (int i = 1; i <= amount; i++)
            {
                if (freeCount <= 0)
                    throw new ArgumentException();

                freeBlock = blocks[freeHead];
                freeBlock.isFree = false;

                freeHead = freeBlock.nextBlockOffset;
                freeCount--;

                if (i == amount)
                    freeBlock.nextBlockOffset = 0;
                list.Add(freeBlock);
            }

            return list;
        }

        public static int getAmountOfBlockNeeded(int size, int blockSize)
        {
            int blockSizeWithoutHeader = blockSize - 4;
            return (int)Math.Ceiling(size / (double)blockSizeWithoutHeader);
        }

        public List<cDatFileBlock> getNewListOfBlocks(int size)
        {
            int blocksNeeded = getAmountOfBlockNeeded(size, blockSize);

            List<cDatFileBlock> newBlocks = getNewBlocks(blocksNeeded);
            return newBlocks;
        }

        public void expandListOfBlocks(List<cDatFileBlock> listOfBlocks, int amountOfBlocksToAdd)
        {
            cDatFileBlock lastBlock = null;
            if (listOfBlocks.Count > 0)
                lastBlock = listOfBlocks[listOfBlocks.Count - 1];

            List<cDatFileBlock> newBlocks = getNewBlocks(amountOfBlocksToAdd);

            if(lastBlock != null)
                lastBlock.nextBlockOffset = newBlocks[0].blockOffset;

            listOfBlocks.AddRange(newBlocks);
        }

        public MemoryStream blocksToData(uint startOffset, int size, List<cDatFileBlock> listOfBlocks)
        {
            MemoryStream data = new MemoryStream();
            StreamWriter writer = new StreamWriter(data);

            cDatFileBlock block;

            int blockSizeWithoutHeader = blockSize - 4;
            uint nextBlockOffset = startOffset;
            int dataOffset = 0;
            int remainingSize = size;
            while (remainingSize > 0)
            {
                if (nextBlockOffset == 0 || !blocks.TryGetValue(nextBlockOffset, out block))
                    throw new ArgumentException();
                nextBlockOffset = block.nextBlockOffset;
                if (remainingSize <= blockSizeWithoutHeader)
                {
                    Utils.writeBytes(block.data, 0, remainingSize, writer);
                    //Buffer.BlockCopy(block.data, 0, data, dataOffset, remainingSize);
                    listOfBlocks.Add(block);
                    writer.Flush();
                    data.Position = 0;
                    return data;
                }
                else
                {
                    Utils.writeBytes(block.data, writer);
                    //Buffer.BlockCopy(block.data, 0, data, dataOffset, block.data.Length);
                    remainingSize -= block.data.Length;
                    dataOffset += block.data.Length;
                    listOfBlocks.Add(block);
                }
            }
            writer.Flush();
            data.Position = 0;
            return data;
        }

        public void dataToBlocks(List<cDatFileBlock> listOfBlocks, MemoryStream data)
        {
            data.Position = 0;
            byte[] dataArray = data.ToArray();
            int size = dataArray.Length;
            int blocksNeeded = getAmountOfBlockNeeded(size, blockSize);
            int blockSizeWithoutHeader = blockSize - 4;

            if (blocksNeeded > listOfBlocks.Count)
            {
                expandListOfBlocks(listOfBlocks, blocksNeeded - listOfBlocks.Count);
            }
            else if (blocksNeeded < listOfBlocks.Count)
            {
                listOfBlocks[blocksNeeded - 1].nextBlockOffset = 0;
                //to do: add remaining blocks to the free blocks chain
            }

            uint nextOffset = listOfBlocks[0].blockOffset;

            List<byte[]> bufferList = new List<byte[]>();
            for (int i = 0; i < blocksNeeded; i++)
            {
                byte[] newBlockData = new byte[blockSizeWithoutHeader];
                nextOffset = listOfBlocks[i].blockOffset;
                if (i < blocksNeeded)
                {
                    Utils.writeToByteArray(nextOffset, newBlockData, 0);
                }
                int newBlockStartPositon = (i * (blockSizeWithoutHeader));
                int newBlockDataEndPosition = newBlockStartPositon + blockSizeWithoutHeader;
                if (newBlockDataEndPosition > size)
                    newBlockDataEndPosition = size;
                int newBlockSize = newBlockDataEndPosition - newBlockStartPositon;
                Buffer.BlockCopy(dataArray, newBlockStartPositon, newBlockData, 0, newBlockSize);
                listOfBlocks[i].data = newBlockData;

                blocks[listOfBlocks[i].blockOffset] = listOfBlocks[i];
            }
        }
    }
}