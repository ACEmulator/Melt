using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Melt
{
    class TextureIdDictionary
    {
        static public void folderExtractTextureFromHeader(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Unable to open {0}", path);
                return;
            }

            string[] fileEntries = Directory.GetFiles(path);

            StreamWriter outputFile = new StreamWriter(new FileStream("./TextureIdDictionary.json", FileMode.Create, FileAccess.Write));

            Console.WriteLine("Creating export list...");
            Dictionary<uint, uint> dictionary = new Dictionary<uint, uint>();

            foreach (string fileName in fileEntries)
            {
                string keyString = fileName.Replace(".bin", string.Empty);
                keyString = keyString.Substring(keyString.LastIndexOf('/') + 1);
                uint key = uint.Parse(keyString, System.Globalization.NumberStyles.HexNumber);
                uint value = extractTextureFromHeader(fileName);
                dictionary.Add(key, value);
            }

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;
            settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            string jsonString = JsonConvert.SerializeObject(dictionary, Formatting.Indented, settings);

            outputFile.Write(jsonString);
            outputFile.Close();
            Console.WriteLine("Done");
        }

        static public uint extractTextureFromHeader(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));

            byte[] buffer = new byte[1024];
            uint fileHeader = Utils.ReadUInt32(buffer, inputFile);
            uint textureId = Utils.ReadUInt32(buffer, inputFile);
            return textureId;
        }
    }
}