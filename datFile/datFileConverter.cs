//using Newtonsoft.Json;
//using System;
//using System.Collections;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Melt
//{
//    public class cDatFileConverter
//    {
//        public static byte[] ReadDat(StreamReader stream, uint offset, int size, int blockSize)
//        {
//            var buffer = new byte[size];

//            stream.BaseStream.Seek(offset, SeekOrigin.Begin);

//            // Dat "file" is broken up into sectors that are not neccessarily congruous. Next address is stored in first four bytes of each sector.
//            uint nextAddress = GetNextAddress(stream, 0);

//            int bufferOffset = 0;

//            while (size > 0)
//            {
//                if (size < blockSize)
//                {
//                    stream.BaseStream.Read(buffer, bufferOffset, Convert.ToInt32(size));
//                    size = 0; // We know we've read the only/last sector, so just set this to zero to proceed.
//                }
//                else
//                {
//                    stream.BaseStream.Read(buffer, bufferOffset, Convert.ToInt32(blockSize) - 4); // Read in our sector into the buffer[]
//                    bufferOffset += Convert.ToInt32(blockSize) - 4; // Adjust this so we know where in our buffer[] the next sector gets appended to
//                    stream.BaseStream.Seek(nextAddress, SeekOrigin.Begin); // Move the file pointer to the start of the next sector we read above.
//                    nextAddress = GetNextAddress(stream, 0); // Get the start location of the next sector.
//                    size -= (blockSize - 4); // Decrease this by the amount of data we just read into buffer[] so we know how much more to go
//                }
//            }

//            return buffer;
//        }

//        private static uint GetNextAddress(StreamReader stream, int relOffset)
//        {
//            // The location of the start of the next sector is the first four bytes of the current sector. This should be 0x00000000 if no next sector.
//            byte[] nextAddressBytes = new byte[4];

//            if (relOffset != 0)
//                stream.BaseStream.Seek(relOffset, SeekOrigin.Current); // To be used to back up 4 bytes from the origin at the start

//            stream.BaseStream.Read(nextAddressBytes, 0, 4);

//            return BitConverter.ToUInt32(nextAddressBytes, 0);
//        }

//        public static uint WriteDatEmptyBlocks(StreamWriter stream, uint offset, int blockSize, int amount)
//        {
//            uint nextOffset = offset;

//            List<byte[]> bufferList = new List<byte[]>();
//            for (int i = 0; i < amount; i++)
//            {
//                byte[] newBlockData = new byte[blockSize];
//                nextOffset = (uint)(nextOffset + blockSize);
//                if (i < amount)
//                {
//                    Utils.writeToByteArray(nextOffset, newBlockData, 0);
//                }
//                bufferList.Add(newBlockData);
//            }

//            stream.BaseStream.Seek(offset, SeekOrigin.Begin);

//            foreach (byte[] buffer in bufferList)
//            {
//                stream.BaseStream.Write(buffer, 0, buffer.Length);
//            }

//            stream.BaseStream.Flush();

//            return nextOffset;
//        }

//        public static uint WriteDat(StreamWriter stream, uint offset, int size, int blockSize, byte[] data)
//        {
//            int blocksNeeded = (int)Math.Ceiling(size / (double)blockSize);
//            uint nextOffset = offset;

//            List<byte[]> bufferList = new List<byte[]>();
//            for(int i = 0; i < blocksNeeded; i++)
//            {
//                byte[] newBlockData = new byte[blockSize];
//                nextOffset = (uint)(nextOffset + blockSize);
//                if (i < blocksNeeded)
//                {
//                    Utils.writeToByteArray(nextOffset, newBlockData, 0);
//                }
//                int newBlockStartPositon = (i * (blockSize - 4));
//                int newBlockDataEndPosition = newBlockStartPositon + blockSize;
//                if (newBlockDataEndPosition > size)
//                    newBlockDataEndPosition = size;
//                int newBlockSize = newBlockDataEndPosition - newBlockStartPositon - 4;
//                Buffer.BlockCopy(data, newBlockStartPositon, newBlockData, 4, newBlockSize);
//                bufferList.Add(newBlockData);
//            }

//            stream.BaseStream.Seek(offset, SeekOrigin.Begin);

//            foreach(byte[] buffer in bufferList)
//            {
//                stream.BaseStream.Write(buffer, 0, buffer.Length);
//            }

//            stream.BaseStream.Flush();

//            return nextOffset;
//        }
//    }
//}