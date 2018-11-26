using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Melt
{
    class TextureHeader
    {
        static public void folderExtractTextureFromHeader(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Unable to open {0}", path);
                return;
            }

            string[] fileEntries = Directory.GetFiles(path);

            StreamWriter outputFile = new StreamWriter(new FileStream("./Export List from Headers.txt", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Export List from Headers.txt");
                return;
            }

            Console.WriteLine("Creating export list...");

            foreach (string fileName in fileEntries)
            {
                //outputFile.WriteLine("readdat -f client_portal_latest_unmodified.dat {0}", extractTextureFromHeader(fileName));
                //outputFile.WriteLine("writedat client_portal.dat -f {0}={0}.bin", extractTextureFromHeader(fileName));
                outputFile.WriteLine("{0}", extractTextureFromHeader(fileName));
                outputFile.Flush();
            }

            outputFile.Close();
            Console.WriteLine("Done");
        }

        static public string extractTextureFromHeader(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return "";
            }

            byte[] buffer = new byte[1024];
            uint fileHeader = Utils.ReadUInt32(buffer, inputFile);
            uint unknown1 = Utils.ReadUInt32(buffer, inputFile);
            byte amountOfTextures = Utils.ReadByte(buffer, inputFile);
            uint unknown2 = Utils.ReadUInt32(buffer, inputFile);
            uint textureId = Utils.ReadUInt32(buffer, inputFile);
            uint textureId2;
            if (amountOfTextures == 2)
                textureId2 = Utils.ReadUInt32(buffer, inputFile);
            else
                textureId2 = textureId;
            return fileHeader.ToString("x8") + " " + textureId2.ToString("x8");
            //return textureId2.ToString("x8");
        }
    }
}