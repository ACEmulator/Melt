using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melt
{
    struct sLandblockWeenie
    {
        public string comment;
        public int id;
        public int wcid;
        public sPosition pos;

        public sLandblockWeenie(byte[] buffer, StreamReader inputFile)
        {
            wcid = Utils.ReadInt32(buffer, inputFile);
            pos = new sPosition(buffer, inputFile);
            id = Utils.ReadInt32(buffer, inputFile);

            comment = Program.cache9Converter.buildWeenieName(wcid);
        }
    }

    struct sLandblockLink
    {
        public string comment;
        public int source;
        public int target;

        public sLandblockLink(byte[] buffer, StreamReader inputFile)
        {
            comment = "";
            source = Utils.ReadInt32(buffer, inputFile);
            target = Utils.ReadInt32(buffer, inputFile);
        }
    }

    class cLandblock
    {
        [JsonIgnore]
        public uint key;
        [JsonIgnore]
        public string friendlyName;

        public List<sLandblockWeenie> weenies;
        public List<sLandblockLink> links;

        //json writer will look into this to decide
        public bool ShouldSerializeweenies()
        {
            return weenies.Count > 0;
        }

        //json writer will look into this to decide
        public bool ShouldSerializelinks()
        {
            return links.Count > 0;
        }

        public cLandblock()
        {
            weenies = new List<sLandblockWeenie>();
            links = new List<sLandblockLink>();
        }

        public cLandblock(byte[] buffer, StreamReader inputFile)
        {
            weenies = new List<sLandblockWeenie>();
            links = new List<sLandblockLink>();

            key = Utils.ReadUInt32(buffer, inputFile);

            friendlyName = landblockNames.geLandblockName(key);
            if (friendlyName.Length == 0)
            {
                friendlyName = $"{key}({key.ToString("X8")})";
                friendlyName = friendlyName.Replace("0000)", ")");
            }
            else
            {
                friendlyName = $"{key}({key.ToString("X8")}) - {friendlyName}";
                friendlyName = friendlyName.Replace("0000)", ")");
            }

            int weeniesCount = Utils.ReadInt32(buffer, inputFile);
            for (int i = 0; i < weeniesCount; i++)
            {
                sLandblockWeenie landBlockWeenie = new sLandblockWeenie(buffer, inputFile);
                weenies.Add(landBlockWeenie);
            }

            int linksCount = Utils.ReadInt32(buffer, inputFile);
            for (int i = 0; i < linksCount; i++)
            {
                sLandblockLink landBlockLink = new sLandblockLink(buffer, inputFile);

                int sourceWcid = getWcidFromId(landBlockLink.source);
                int targetWcid = getWcidFromId(landBlockLink.target);

                string source = Program.cache9Converter.buildWeenieName(sourceWcid);
                string target = Program.cache9Converter.buildWeenieName(targetWcid);

                landBlockLink.comment = $"{source}(wcid: {sourceWcid}) -> {target}(wcid: {targetWcid})";

                links.Add(landBlockLink);
            }
        }

        public int getWcidFromId(int id)
        {
            foreach(sLandblockWeenie weenie in weenies)
            {
                if (weenie.id == id)
                    return weenie.wcid;
            }

            return 0;
        }
    }

    public class cCache6Converter
    {
        Dictionary<uint, cLandblock> landblocks;
        byte[] buffer = new byte[4096];

        public cCache6Converter()
        {
        }

        public void loadFromRaw(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            Console.WriteLine("Reading landblocks from 0006.raw...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            short totalLandblocks = Utils.ReadInt16(buffer, inputFile);
            short landscapeGeneratorsCount = Utils.ReadInt16(buffer, inputFile);
            landblocks = new Dictionary<uint, cLandblock>();

            short landblockCount;
            for (landblockCount = 0; landblockCount < totalLandblocks; landblockCount++)
            {
                cLandblock landblock = new cLandblock(buffer, inputFile);
                landblocks.Add(landblock.key, landblock);
            }

            inputFile.Close();
            timer.Stop();
            Console.WriteLine("{0} landblocks read in {1} seconds.", landblockCount, timer.ElapsedMilliseconds / 1000f);
        }

        public void writeJson(string outputPath)
        {
            Console.WriteLine("Writing landblocks to json files...");
            Stopwatch timer = new Stopwatch();
            timer.Start();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;
            settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            foreach (KeyValuePair<uint, cLandblock> landblock in landblocks)
            {
                string outputFilename = Path.Combine(outputPath, $"{landblock.Value.friendlyName}.json");
                StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));

                string jsonString = JsonConvert.SerializeObject(landblock, Formatting.Indented, settings);
                jsonString = jsonString.Replace("Key", "key");
                jsonString = jsonString.Replace("Value", "value");
                outputFile.Write(jsonString);
                outputFile.Close();
            }

            timer.Stop();
            Console.WriteLine("{0} landblocks written in {1} seconds.", landblocks.Count, timer.ElapsedMilliseconds / 1000f);
        }
    }
}