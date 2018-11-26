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
    public struct sQuestFlag
    {
        public string name;
        public int repeatTimerSeconds;
        public string comment;
        public int maxRepetitions;
        public string description;

        //json writer will look into this to decide
        public bool ShouldSerializecomment()
        {
            return comment.Length > 0;
        }

        public sQuestFlag(byte[] buffer, StreamReader inputFile)
        {
            name = Utils.ReadString(buffer, inputFile);
            repeatTimerSeconds = Utils.ReadInt32(buffer, inputFile);
            maxRepetitions = Utils.ReadInt32(buffer, inputFile);
            description = Utils.ReadEncodedString(buffer, inputFile);

            comment = "";
            TimeSpan time = TimeSpan.FromSeconds(repeatTimerSeconds);
            string plural = "";
            if (time.Seconds > 0)
            {
                plural = time.Seconds > 1 ? "s" : "";
                comment = $"{time.Seconds} second{plural}";
            }
            if (time.Minutes > 0)
            {
                plural = time.Minutes > 1 ? "s" : "";
                if (comment.Length > 0)
                    comment = $"{time.Minutes} minute{plural}, {comment}";
                else
                    comment = $"{time.Minutes} minute{plural}";
            }
            if (time.Hours > 0)
            {
                plural = time.Hours > 1 ? "s" : "";
                if (comment.Length > 0)
                    comment = $"{time.Hours} hour{plural}, {comment}";
                else
                    comment = $"{time.Hours} hour{plural}";
            }
            if (time.Days > 0)
            {
                plural = time.Days > 1 ? "s" : "";
                if (comment.Length > 0)
                    comment = $"{time.Days} day{plural}, {comment}";
                else
                    comment = $"{time.Days} day{plural}";
            }
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeString(name, outputStream);
            Utils.writeInt32(repeatTimerSeconds, outputStream);
            Utils.writeInt32(maxRepetitions, outputStream);
            Utils.writeEncodedString(description, outputStream);
        }
    }

    public class cCache8Converter
    {
        List<sQuestFlag> questFlags;
        byte[] buffer = new byte[4096];

        public cCache8Converter()
        {
        }

        public void loadFromRaw(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            Console.WriteLine("Reading quest flags from 0008.raw...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            questFlags = new List<sQuestFlag>();

            short count = Utils.ReadInt16(buffer, inputFile);
            short unknown = Utils.ReadInt16(buffer, inputFile);

            short questFlagCount;
            for (questFlagCount = 0; questFlagCount < count; questFlagCount++)
            {
                sQuestFlag newTimerEntry = new sQuestFlag(buffer, inputFile);
                questFlags.Add(newTimerEntry);
            }

            inputFile.Close();
            timer.Stop();
            Console.WriteLine("{0} quest flags read in {1} seconds.", questFlagCount, timer.ElapsedMilliseconds / 1000f);
        }

        public void loadFromJson(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            Console.WriteLine("Reading timers from {0}...", filename);
            Stopwatch timer = new Stopwatch();
            timer.Start();

            questFlags = new List<sQuestFlag>();

            StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            string jsonData = reader.ReadToEnd();

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;
            settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            //settings.DefaultValueHandling = DefaultValueHandling.Ignore;

            questFlags = JsonConvert.DeserializeObject<List<sQuestFlag>>(jsonData, settings);

            inputFile.Close();
            timer.Stop();
            Console.WriteLine("{0} timers read in {1} seconds.", questFlags.Count, timer.ElapsedMilliseconds / 1000f);
        }

        public void writeJson(string outputPath)
        {
            Console.WriteLine("Writing quest flags to json file..");
            Stopwatch timer = new Stopwatch();
            timer.Start();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;
            settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            //settings.DefaultValueHandling = DefaultValueHandling.Ignore;

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            string outputFilename = Path.Combine(outputPath, "questFlags.json");
            StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));

            string jsonString = JsonConvert.SerializeObject(questFlags, Formatting.Indented, settings);
            outputFile.Write(jsonString);
            outputFile.Close();

            timer.Stop();
            Console.WriteLine("{0} quest flags written in {1} seconds.", questFlags.Count, timer.ElapsedMilliseconds / 1000f);
        }

        public void writeRaw(string outputPath)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Console.WriteLine("Writing \"0008.raw\"");
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            string outputFilename = Path.Combine(outputPath, "0008.raw");
            StreamWriter outputFile = new StreamWriter(new FileStream(outputFilename, FileMode.Create, FileAccess.Write));
            Utils.writeInt16((short)questFlags.Count, outputFile);
            Utils.writeInt16(32, outputFile); //unknown

            foreach(sQuestFlag timerEntry in questFlags)
            {
                timerEntry.writeRaw(outputFile);
            }

            outputFile.Close();
            timer.Stop();
            Console.WriteLine("{0} quest flags written in {1} seconds.", questFlags.Count, timer.ElapsedMilliseconds / 1000f);
        }
    }
}