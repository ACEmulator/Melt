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
    public class cDatFileBlock
    {
        public uint blockOffset;
        public uint nextBlockOffset;
        public byte[] data;
        public bool isFree;

        public cDatFileBlock(uint blockOffset, uint nextBlockOffset, int blockSize, bool isFree)
        {
            this.blockOffset = blockOffset;
            this.nextBlockOffset = nextBlockOffset;
            if (isFree)
            {
                this.isFree = isFree;
                nextBlockOffset = (nextBlockOffset ^ 0x80000000);
            }
            int blockSizeWithoutHeader = blockSize - 4;
            data = new byte[blockSizeWithoutHeader];
        }

        public cDatFileBlock(StreamReader inputFile, int blockSize)
        {
            int blockSizeWithoutHeader = blockSize - 4;
            byte[] buffer = new byte[blockSizeWithoutHeader];

            blockOffset = (uint)inputFile.BaseStream.Position;
            nextBlockOffset = Utils.ReadUInt32(buffer, inputFile);
            if ((nextBlockOffset & 0x80000000) > 0)
            {
                isFree = true;
                nextBlockOffset = (nextBlockOffset ^ 0x80000000);
            }
            data = Utils.ReadBytes(buffer, inputFile, blockSizeWithoutHeader);
        }

        public void writeToDat(StreamWriter outputFile, int blockSize)
        {
            if(isFree)
                Utils.writeUInt32((nextBlockOffset ^ 0x80000000), outputFile);
            else
                Utils.writeUInt32(nextBlockOffset, outputFile);
            Utils.writeBytes(data, outputFile);
        }

        public static void loadBlocksAndAddToDictionary(cDatFileBlockCache blockCache, StreamReader inputFile, int blockSize)
        {
            inputFile.BaseStream.Seek(1024, SeekOrigin.Begin);

            while(inputFile.BaseStream.Position < inputFile.BaseStream.Length)
            {
                cDatFileBlock newBlock = new cDatFileBlock(inputFile, blockSize);
                blockCache.addBlock(newBlock);
            }
        }
    }
}