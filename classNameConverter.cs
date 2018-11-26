using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Melt
{
    class ClassNameConverter
    {
        static public void convert()
        {
            StreamReader inputFile = new StreamReader(new FileStream(".\\weenie class names.dump", FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open weenie class names.dump");
                return;
            }
            StreamWriter outputFile = new StreamWriter(new FileStream(".\\weenie class names.txt", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open weenie class names.txt");
                return;
            }

            Console.WriteLine("Converting weenie class names...");

            string line;
            string[] splitLine;
            while(!inputFile.EndOfStream)
            {
                line = inputFile.ReadLine();
                splitLine = line.Split(',');
                string idString = splitLine[splitLine.Length - 1];
                idString = idString.Replace("h", "");
                int id = int.Parse(idString, System.Globalization.NumberStyles.HexNumber);
                //int id = Convert.ToInt32(idString);

                inputFile.ReadLine();
                inputFile.ReadLine();
                inputFile.ReadLine();
                inputFile.ReadLine();

                line = inputFile.ReadLine();
                splitLine = line.Split(',');
                string name = splitLine[splitLine.Length - 1];
                name = name.Replace("_CLASS\"", "");
                name = name.Replace("\"W_", "");
                name = name.ToLower();
                name = name.Trim();

                outputFile.WriteLine($"map.Add({id}, \"{name}\");");

                outputFile.Flush();

                inputFile.ReadLine();
            }


            inputFile.Close();
            outputFile.Close();
            Console.WriteLine("Done");
        }
    }
}