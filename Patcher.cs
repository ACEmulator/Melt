using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace Melt
{
    class Patcher
    {
        static public void patch()
        {
            Console.WriteLine("Patching...");
            StreamReader mainFile = new StreamReader(new FileStream(".\\patch.dif", FileMode.Open, FileAccess.Read));
            if (mainFile == null)
            {
                Console.WriteLine("Unable to open patch.dif");
                return;
            }

            //header
            mainFile.ReadLine();
            mainFile.ReadLine();

            string fileName = mainFile.ReadLine();

            BinaryReader inputFile = new BinaryReader(new FileStream(String.Format(".\\{0}", fileName), FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", fileName);
                return;
            }

            BinaryWriter outputFile = new BinaryWriter(new FileStream(String.Format(".\\Patched_{0}", fileName), FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open Patched_{0}", fileName);
                return;
            }

            while (!mainFile.EndOfStream)
            {
                string line = mainFile.ReadLine();
                line = line.Replace(":", "");
                string[] parameters = line.Split(' ');

                long address = long.Parse(parameters[0], NumberStyles.HexNumber);
                byte oldValue = byte.Parse(parameters[1], NumberStyles.HexNumber );
                byte newValue = byte.Parse(parameters[2], NumberStyles.HexNumber);

                while (inputFile.BaseStream.Position < address)
                {
                    outputFile.Write(inputFile.ReadByte());
                }

                if (inputFile.BaseStream.Position == address)
                {
                    byte currentByte = (byte)inputFile.ReadByte();
                    if (currentByte == oldValue)
                        outputFile.Write(newValue);
                    else
                    {
                        Console.WriteLine("Error: File mismatch.");
                        return;
                    }
                }
            }

            while (inputFile.BaseStream.Position < inputFile.BaseStream.Length)
            {
                outputFile.Write(inputFile.ReadByte());
            }

            Console.WriteLine("Done");
        }
    }
}
