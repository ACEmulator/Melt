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
    public class cDatFileEntry //BTEntry
    {
        public eDatFormat fileFormat;
        public uint bitFlags;
        public uint fileId;
        public int fileSize;
        public uint startBlockOffset;
        public uint timeStamp;
        public uint version;

        public MemoryStream fileContent;
        public List<cDatFileBlock> listOfBlocks;

        //public static cDatFileEntry newFrom(cDatFileEntry other)
        //{
        //    return newFrom(other, other.fileId);
        //}

        public static cDatFileEntry newFrom(cDatFileEntry other, uint newFileId)
        {
            cDatFileEntry newFile = other.Copy();
            newFile.fileId = newFileId;
            return newFile;
        }

        public void copyFrom(cDatFileEntry other)
        {
            copyFrom(other, other.fileId);
        }

        public void copyFrom(cDatFileEntry other, uint newFileId)
        {
            fileFormat = other.fileFormat;
            bitFlags = other.bitFlags;
            fileSize = other.fileSize;
            startBlockOffset = other.startBlockOffset;
            timeStamp = other.timeStamp;
            version = other.version;

            fileId = newFileId;
            fileContent = other.fileContent;
            listOfBlocks = new List<cDatFileBlock>();
        }

        public cDatFileEntry(uint fileId, eDatFormat fileFormat)
        {
            this.fileId = fileId;
            this.fileFormat = fileFormat;

            startBlockOffset = 0;
            fileSize = 0;
            bitFlags = 0x00020000;
            timeStamp = 1117234133;
            version = 1;

            fileContent = new MemoryStream();
            listOfBlocks = new List<cDatFileBlock>();
        }

        public cDatFileEntry(byte[] buffer, StreamReader inputFile, eDatFormat fileFormat)
        {
            this.fileFormat = fileFormat;

            if (fileFormat == eDatFormat.ToD)
            {
                bitFlags = Utils.ReadUInt32(buffer, inputFile);//0x10000 = empty 0x20000 = has data
                fileId = Utils.ReadUInt32(buffer, inputFile);
                startBlockOffset = Utils.ReadUInt32(buffer, inputFile);
                fileSize = Utils.ReadInt32(buffer, inputFile);
                timeStamp = Utils.ReadUInt32(buffer, inputFile);
                version = Utils.ReadUInt32(buffer, inputFile);

                //DateTimeOffset convertedTimeStamp = DateTimeOffset.FromUnixTimeSeconds(timeStamp);
            }
            else
            {
                fileId = Utils.ReadUInt32(buffer, inputFile);
                startBlockOffset = Utils.ReadUInt32(buffer, inputFile);
                fileSize = Utils.ReadInt32(buffer, inputFile);
                bitFlags = 0x00020000;
                timeStamp = (uint)DateTimeOffset.Now.ToUnixTimeSeconds();
                version = 1;
            }

            fileContent = new MemoryStream();
        }

        public void writeHeader(StreamWriter outputFile)
        {
            Utils.writeUInt32(bitFlags, outputFile);
            Utils.writeUInt32(fileId, outputFile);
            Utils.writeUInt32(startBlockOffset, outputFile);
            Utils.writeInt32(fileSize, outputFile);
            Utils.writeUInt32(timeStamp, outputFile);
            Utils.writeUInt32(version, outputFile);
        }
    }
}