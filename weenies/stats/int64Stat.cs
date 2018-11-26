using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public enum eInt64Stat
    {
        Undef = 0,
        TotalExperience = 1,
        AvailableExperience = 2,
        AugmentationCost = 3,
        ItemTotalXp = 4,
        ItemBaseXp = 5,
        AvailableLuminance = 6,
        MaximumLuminance = 7,
        InteractionReqs = 8,
        DeleteTime = 9001
    }

    public struct sInt64Stat
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public eInt64Stat key;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public Int64 value;

        public sInt64Stat(eInt64Stat key, Int64 value)
        {
            this.key = key;
            this.value = value;
        }

        public sInt64Stat(byte[] buffer, StreamReader inputFile)
        {
            key = (eInt64Stat)Utils.ReadInt32(buffer, inputFile);

            if (!Enum.IsDefined(typeof(eInt64Stat), key))
                Console.WriteLine("Unknown int64Stat: {0}", key);

            value = Utils.ReadInt64(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)key, outputStream);
            Utils.writeInt64(value, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            outputStream.Write("{0}\n{1}{{", entryStarter, tab);

            Utils.writeJson(outputStream, "key", (int)key, "", true, false, 3);
            Utils.writeJson(outputStream, "value", value, "    ", false, false, 10);
            Utils.writeJson(outputStream, "_comment", key.ToString(), "    ", false, false, 0);
            outputStream.Write("}");
        }
    }
}