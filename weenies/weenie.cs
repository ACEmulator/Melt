using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public class cWeenie
    {
        public int wcid;
        public uint entryHeader1;
        public string weenieName;
        public uint entryHeader2;
        [JsonConverter(typeof(StringEnumConverter))]
        public eStatFlags statFlags;
        public eWeenieTypes weenieType;

        public Dictionary<eIntStat, sIntStat> intStats = new Dictionary<eIntStat, sIntStat>();
        public Dictionary<eInt64Stat, sInt64Stat> int64Stats = new Dictionary<eInt64Stat, sInt64Stat>();
        public Dictionary<eBoolStat, sBoolStat> boolStats = new Dictionary<eBoolStat, sBoolStat>();
        public Dictionary<eFloatStat, sFloatStat> floatStats = new Dictionary<eFloatStat,sFloatStat>();
        public Dictionary<eStringStat, sStringStat> stringStats = new Dictionary<eStringStat, sStringStat>();
        public Dictionary<eDidStat, sDidStat> didStats = new Dictionary<eDidStat, sDidStat>();
        public List<sPosStat> posStats = new List<sPosStat>();
        public Dictionary<eIidStat, sIidStat> iidStats = new Dictionary<eIidStat, sIidStat>();

        [JsonConverter(typeof(StringEnumConverter))]
        public eDataFlags dataFlags;

        public sAttributes attributes;
        public sSkills skills;
        public sBody body;
        public sSpellBook spellBook;
        public sEventFilter eventFilter;
        public sEmoteTable emoteTable;
        public sCreateList createList;
        public sPageDataList pageDataList;
        public sGeneratorTable generatorTable;
        public sObjDesc objDesc;

        public cWeenie()
        {
        }

        public cWeenie(byte[] buffer, StreamReader inputFile)
        {
            wcid = Utils.ReadInt32(buffer, inputFile);
            entryHeader1 = Utils.ReadUInt32(buffer, inputFile);
            weenieName = Utils.ReadStringAndReplaceSpecialCharacters(buffer, inputFile);
            entryHeader2 = Utils.ReadUInt32(buffer, inputFile);

            statFlags = (eStatFlags)Utils.ReadInt32(buffer, inputFile);
            weenieType = (eWeenieTypes)Utils.ReadInt32(buffer, inputFile);

            if (statFlags.HasFlag(eStatFlags.intStats))
                parseIntStats(buffer, inputFile);
            if (statFlags.HasFlag(eStatFlags.int64Stats))
                parseInt64Stats(buffer, inputFile);
            if (statFlags.HasFlag(eStatFlags.boolStats))
                parseBoolStats(buffer, inputFile);
            if (statFlags.HasFlag(eStatFlags.floatStats))
                parseFloatStats(buffer, inputFile);
            if (statFlags.HasFlag(eStatFlags.stringStats))
                parseStringStats(buffer, inputFile);
            if (statFlags.HasFlag(eStatFlags.didStats))
                parseDidStats(buffer, inputFile);
            if (statFlags.HasFlag(eStatFlags.posStats))
                parsePosStats(buffer, inputFile);
            if (statFlags.HasFlag(eStatFlags.iidStat))
                parseIidStats(buffer, inputFile);

            dataFlags = (eDataFlags)Utils.ReadInt32(buffer, inputFile);

            int repeatWcid = Utils.ReadInt32(buffer, inputFile);

            if (wcid != repeatWcid)
                Console.WriteLine("Wcids do not match: {0} and {1}", wcid, repeatWcid);

            if (dataFlags.HasFlag(eDataFlags.attributes))
                attributes = new sAttributes(buffer, inputFile);
            if (dataFlags.HasFlag(eDataFlags.skills))
                skills = new sSkills(buffer, inputFile);
            if (dataFlags.HasFlag(eDataFlags.body))
                body = new sBody(buffer, inputFile);
            if (dataFlags.HasFlag(eDataFlags.spellBook))
                spellBook = new sSpellBook(buffer, inputFile);
            if (dataFlags.HasFlag(eDataFlags.eventFilter))
                eventFilter = new sEventFilter(buffer, inputFile);
            if (dataFlags.HasFlag(eDataFlags.emoteTable))
                emoteTable = new sEmoteTable(buffer, inputFile);
            if (dataFlags.HasFlag(eDataFlags.createList))
                createList = new sCreateList(buffer, inputFile);
            if (dataFlags.HasFlag(eDataFlags.pageDataList))
                pageDataList = new sPageDataList(buffer, inputFile);
            if (dataFlags.HasFlag(eDataFlags.generatorTable))
                generatorTable = new sGeneratorTable(buffer, inputFile);

            objDesc = new sObjDesc(buffer, inputFile);
            byte entryDelimiter = Utils.ReadByte(buffer, inputFile);

            if (entryDelimiter != 0x01)
                Console.WriteLine("Error reading weenie at {0}", inputFile.BaseStream.Position);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32(wcid, outputStream);
            Utils.writeUInt32(entryHeader1, outputStream);
            Utils.writeString(Utils.RestoreStringSpecialCharacters(weenieName), outputStream);
            Utils.writeUInt32(entryHeader2, outputStream);

            Utils.writeInt32((int)statFlags, outputStream);
            Utils.writeInt32((int)weenieType, outputStream);

            if (statFlags.HasFlag(eStatFlags.intStats))
                writeIntStatsRaw(outputStream);
            if (statFlags.HasFlag(eStatFlags.int64Stats))
                writeInt64StatsRaw(outputStream);
            if (statFlags.HasFlag(eStatFlags.boolStats))
                writeBoolStatsRaw(outputStream);
            if (statFlags.HasFlag(eStatFlags.floatStats))
                writeFloatStatsRaw(outputStream);
            if (statFlags.HasFlag(eStatFlags.stringStats))
                writeStringStatsRaw(outputStream);
            if (statFlags.HasFlag(eStatFlags.didStats))
                writeDidStatsRaw(outputStream);
            if (statFlags.HasFlag(eStatFlags.posStats))
                writePosStatsRaw(outputStream);
            if (statFlags.HasFlag(eStatFlags.iidStat))
                writeIidStatsRaw(outputStream);

            Utils.writeInt32((int)dataFlags, outputStream);

            Utils.writeInt32(wcid, outputStream);

            if (dataFlags.HasFlag(eDataFlags.attributes))
                attributes.writeRaw(outputStream);
            if (dataFlags.HasFlag(eDataFlags.skills))
                skills.writeRaw(outputStream);
            if (dataFlags.HasFlag(eDataFlags.body))
                body.writeRaw(outputStream);
            if (dataFlags.HasFlag(eDataFlags.spellBook))
                spellBook.writeRaw(outputStream);
            if (dataFlags.HasFlag(eDataFlags.eventFilter))
                eventFilter.writeRaw(outputStream);
            if (dataFlags.HasFlag(eDataFlags.emoteTable))
                emoteTable.writeRaw(outputStream);
            if (dataFlags.HasFlag(eDataFlags.createList))
                createList.writeRaw(outputStream);
            if (dataFlags.HasFlag(eDataFlags.pageDataList))
                pageDataList.writeRaw(outputStream);
            if (dataFlags.HasFlag(eDataFlags.generatorTable))
                generatorTable.writeRaw(outputStream);

            objDesc.writeRaw(outputStream);
            Utils.writeByte(0x01, outputStream);
        }

        public void writeExtendedJson(StreamWriter outputStream)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;
            settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            settings.Converters.Add(new singleConverter());
            settings.Converters.Add(new doubleConverter());

            outputStream.Write(JsonConvert.SerializeObject(this, Formatting.Indented, settings));
        }

        public void writeJson(StreamWriter outputStream, string tab = "", bool isFirst = true)
        {
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            string entryStarter = isFirst ? "" : ",\n";
            outputStream.Write("{0}{1}{{", entryStarter, tab);

            Utils.writeJson(outputStream, "wcid", wcid, entriesTab, true, true);

            cWeenie wcidWeenie = Program.cache9Converter.getWeenie(wcid);
            string wcidName = "";

            if (wcidWeenie != null)
                wcidName = wcidWeenie.weenieName;
            wcidName = Utils.removeWcidNameRedundancy(WeenieClassNames.getWeenieClassName(wcid), wcidName);

            Utils.writeJson(outputStream, "_comment", $"{wcidName}", "    ", false, false);
            //Utils.writeJson(outputStream, "weenieName", weenieName, entriesTab, false, true);
            Utils.writeJson(outputStream, "weenieType", (int)weenieType, entriesTab, false, true);
            Utils.writeJson(outputStream, "_comment2", weenieType.ToString(),"    ",false, false);

            if (statFlags.HasFlag(eStatFlags.stringStats))
                writeStringStatsJson(outputStream, entriesTab);
            if (statFlags.HasFlag(eStatFlags.intStats))
                writeIntStatsJson(outputStream, entriesTab);
            if (statFlags.HasFlag(eStatFlags.int64Stats))
                writeInt64StatsJson(outputStream, entriesTab);
            if (statFlags.HasFlag(eStatFlags.boolStats))
                writeBoolStatsJson(outputStream, entriesTab);
            if (statFlags.HasFlag(eStatFlags.floatStats))
                writeFloatStatsJson(outputStream, entriesTab);
            if (statFlags.HasFlag(eStatFlags.didStats))
                writeDidStatsJson(outputStream, entriesTab);
            if (statFlags.HasFlag(eStatFlags.posStats))
                writePosStatsJson(outputStream, entriesTab);
            if (statFlags.HasFlag(eStatFlags.iidStat))
                writeIidStatsJson(outputStream, entriesTab);

            if (dataFlags.HasFlag(eDataFlags.attributes))
                attributes.writeJson(outputStream, entriesTab, false);
            if (dataFlags.HasFlag(eDataFlags.skills))
                skills.writeJson(outputStream, entriesTab, false);
            if (dataFlags.HasFlag(eDataFlags.body))
                body.writeJson(outputStream, entriesTab, false);
            if (dataFlags.HasFlag(eDataFlags.spellBook))
                spellBook.writeJson(outputStream, entriesTab, false);
            if (dataFlags.HasFlag(eDataFlags.eventFilter))
                eventFilter.writeJson(outputStream, entriesTab, false);
            if (dataFlags.HasFlag(eDataFlags.emoteTable))
                emoteTable.writeJson(outputStream, entriesTab, false);
            if (dataFlags.HasFlag(eDataFlags.createList))
                createList.writeJson(outputStream, entriesTab, false);
            if (dataFlags.HasFlag(eDataFlags.pageDataList))
                pageDataList.writeJson(outputStream, entriesTab, false);
            if (dataFlags.HasFlag(eDataFlags.generatorTable))
                generatorTable.writeJson(outputStream, entriesTab, false);

            outputStream.Write("\n{0}}}", tab);
        }

        void writeIntStatsRaw(StreamWriter outputStream, string tab = "")
        {
            int sectionHeader = (short)intStats.Count;
            sectionHeader |= 0x40 << 16;
            Utils.writeInt32(sectionHeader, outputStream);

            foreach (KeyValuePair<eIntStat, sIntStat> stat in intStats)
            {
                stat.Value.writeRaw(outputStream);
            }
        }

        void writeInt64StatsRaw(StreamWriter outputStream, string tab = "")
        {
            int sectionHeader = (short)int64Stats.Count;
            sectionHeader |= 0x40 << 16;
            Utils.writeInt32(sectionHeader, outputStream);

            foreach (KeyValuePair<eInt64Stat, sInt64Stat> stat in int64Stats)
            {
                stat.Value.writeRaw(outputStream);
            }
        }

        void writeBoolStatsRaw(StreamWriter outputStream, string tab = "")
        {
            int sectionHeader = (short)boolStats.Count;
            sectionHeader |= 0x40 << 16;
            Utils.writeInt32(sectionHeader, outputStream);

            foreach (KeyValuePair<eBoolStat, sBoolStat> stat in boolStats)
            {
                stat.Value.writeRaw(outputStream);
            }
        }

        void writeFloatStatsRaw(StreamWriter outputStream, string tab = "")
        {
            int sectionHeader = (short)floatStats.Count;
            sectionHeader |= 0x40 << 16;
            Utils.writeInt32(sectionHeader, outputStream);

            foreach (KeyValuePair<eFloatStat, sFloatStat> stat in floatStats)
            {
                stat.Value.writeRaw(outputStream);
            }
        }

        void writeStringStatsRaw(StreamWriter outputStream, string tab = "")
        {
            int sectionHeader = (short)stringStats.Count;
            sectionHeader |= 0x40 << 16;
            Utils.writeInt32(sectionHeader, outputStream);

            foreach (KeyValuePair<eStringStat, sStringStat> stat in stringStats)
            {
                stat.Value.writeRaw(outputStream);
            }
        }

        void writeDidStatsRaw(StreamWriter outputStream, string tab = "")
        {
            int sectionHeader = (short)didStats.Count;
            sectionHeader |= 0x40 << 16;
            Utils.writeInt32(sectionHeader, outputStream);

            foreach (KeyValuePair<eDidStat, sDidStat> stat in didStats)
            {
                stat.Value.writeRaw(outputStream);
            }
        }

        void writePosStatsRaw(StreamWriter outputStream, string tab = "")
        {
            int sectionHeader = (short)posStats.Count;
            sectionHeader |= 0x40 << 16;
            Utils.writeInt32(sectionHeader, outputStream);

            foreach (sPosStat stat in posStats)
            {
                stat.writeRaw(outputStream);
            }
        }

        void writeIidStatsRaw(StreamWriter outputStream, string tab = "")
        {
            int sectionHeader = (short)iidStats.Count;
            sectionHeader |= 0x40 << 16;
            Utils.writeInt32(sectionHeader, outputStream);

            foreach (KeyValuePair<eIidStat, sIidStat> stat in iidStats)
            {
                stat.Value.writeRaw(outputStream);
            }
        }

        void writeIntStatsJson(StreamWriter outputStream, string tab = "")
        {
            string entriesTab = $"{tab}\t";

            outputStream.Write(",\n{0}\"intStats\":[", tab);
            bool first = true;
            foreach (KeyValuePair<eIntStat, sIntStat> stat in intStats)
            {
                stat.Value.writeJson(outputStream, entriesTab, first);
                if (first)
                    first = false;
            }
            outputStream.Write("\n{0}]", tab);
        }

        void writeInt64StatsJson(StreamWriter outputStream, string tab = "")
        {
            string entriesTab = $"{tab}\t";

            outputStream.Write(",\n{0}\"int64Stats\":[", tab);
            bool first = true;
            foreach (KeyValuePair<eInt64Stat, sInt64Stat> stat in int64Stats)
            {
                stat.Value.writeJson(outputStream, entriesTab, first);
                if (first)
                    first = false;
            }
            outputStream.Write("\n{0}]", tab);
        }

        void writeBoolStatsJson(StreamWriter outputStream, string tab = "")
        {
            string entriesTab = $"{tab}\t";

            outputStream.Write(",\n{0}\"boolStats\":[", tab);
            bool first = true;
            foreach (KeyValuePair<eBoolStat, sBoolStat> stat in boolStats)
            {
                stat.Value.writeJson(outputStream, entriesTab, first);
                if (first)
                    first = false;
            }
            outputStream.Write("\n{0}]", tab);
        }

        void writeFloatStatsJson(StreamWriter outputStream, string tab = "")
        {
            string entriesTab = $"{tab}\t";

            outputStream.Write(",\n{0}\"floatStats\":[", tab);
            bool first = true;
            foreach (KeyValuePair<eFloatStat, sFloatStat> stat in floatStats)
            {
                stat.Value.writeJson(outputStream, entriesTab, first);
                if (first)
                    first = false;
            }
            outputStream.Write("\n{0}]", tab);
        }

        void writeStringStatsJson(StreamWriter outputStream, string tab = "")
        {
            string entriesTab = $"{tab}\t";

            outputStream.Write(",\n{0}\"stringStats\":[", tab);
            bool first = true;
            foreach (KeyValuePair<eStringStat, sStringStat> stat in stringStats)
            {
                stat.Value.writeJson(outputStream, entriesTab, first);
                if (first)
                    first = false;
            }
            outputStream.Write("\n{0}]", tab);
        }

        void writeDidStatsJson(StreamWriter outputStream, string tab = "")
        {
            string entriesTab = $"{tab}\t";

            outputStream.Write(",\n{0}\"didStats\":[", tab);
            bool first = true;
            foreach (KeyValuePair<eDidStat, sDidStat> stat in didStats)
            {
                stat.Value.writeJson(outputStream, entriesTab, first);
                if (first)
                    first = false;
            }
            outputStream.Write("\n{0}]", tab);
        }

        void writePosStatsJson(StreamWriter outputStream, string tab = "")
        {
            string entriesTab = $"{tab}\t";

            outputStream.Write(",\n{0}\"posStats\":[", tab);
            bool first = true;
            foreach (sPosStat stat in posStats)
            {
                stat.writeJson(outputStream, entriesTab, first);
                if (first)
                    first = false;
            }
            outputStream.Write("\n{0}]", tab);
        }

        void writeIidStatsJson(StreamWriter outputStream, string tab = "")
        {
            string entriesTab = $"{tab}\t";

            outputStream.Write(",\n{0}\"iidStats\":[", tab);
            bool first = true;
            foreach (KeyValuePair<eIidStat, sIidStat> stat in iidStats)
            {
                stat.Value.writeJson(outputStream, entriesTab, first);
                if (first)
                    first = false;
            }
            outputStream.Write("\n{0}]", tab);
        }

        void parseIntStats(byte[] buffer, StreamReader inputFile)
        {
            short amount = 0;
            int sectionHeader = Utils.ReadInt32(buffer, inputFile);
            if (sectionHeader >> 16 == 0x40)
            {
                amount = (short)sectionHeader;

                for (int i = 0; i < amount; i++)
                {
                    sIntStat newStat = new sIntStat(buffer, inputFile);
                    intStats.Add(newStat.key, newStat);
                }
            }
        }

        void parseInt64Stats(byte[] buffer, StreamReader inputFile)
        {
            short amount = 0;
            int sectionHeader = Utils.ReadInt32(buffer, inputFile);
            if (sectionHeader >> 16 == 0x40)
            {
                amount = (short)sectionHeader;

                for (int i = 0; i < amount; i++)
                {
                    sInt64Stat newStat = new sInt64Stat(buffer, inputFile);
                    int64Stats.Add(newStat.key, newStat);
                }
            }
        }

        void parseBoolStats(byte[] buffer, StreamReader inputFile)
        {
            short amount = 0;
            int sectionHeader = Utils.ReadInt32(buffer, inputFile);
            if (sectionHeader >> 16 == 0x40)
            {
                amount = (short)sectionHeader;

                for (int i = 0; i < amount; i++)
                {
                    sBoolStat newStat = new sBoolStat(buffer, inputFile);
                    boolStats.Add(newStat.key, newStat);
                }
            }
        }

        void parseFloatStats(byte[] buffer, StreamReader inputFile)
        {
            short amount = 0;
            int sectionHeader = Utils.ReadInt32(buffer, inputFile);
            if (sectionHeader >> 16 == 0x40)
            {
                amount = (short)sectionHeader;

                for (int i = 0; i < amount; i++)
                {
                    sFloatStat newStat = new sFloatStat(buffer, inputFile);
                    floatStats.Add(newStat.key, newStat);
                }
            }
        }

        void parseStringStats(byte[] buffer, StreamReader inputFile)
        {
            short amount = 0;
            int sectionHeader = Utils.ReadInt32(buffer, inputFile);
            if (sectionHeader >> 16 == 0x40)
            {
                amount = (short)sectionHeader;

                for (int i = 0; i < amount; i++)
                {
                    sStringStat newStat = new sStringStat(buffer, inputFile);
                    stringStats.Add(newStat.key, newStat);
                }
            }
        }

        void parseDidStats(byte[] buffer, StreamReader inputFile)
        {
            short amount = 0;
            int sectionHeader = Utils.ReadInt32(buffer, inputFile);
            if (sectionHeader >> 16 == 0x40)
            {
                amount = (short)sectionHeader;

                for (int i = 0; i < amount; i++)
                {
                    sDidStat newStat = new sDidStat(buffer, inputFile);
                    didStats.Add(newStat.key, newStat);
                }
            }
        }

        void parsePosStats(byte[] buffer, StreamReader inputFile)
        {
            short amount = 0;
            int sectionHeader = Utils.ReadInt32(buffer, inputFile);
            if (sectionHeader >> 16 == 0x40)
            {
                amount = (short)sectionHeader;

                for (int i = 0; i < amount; i++)
                {
                    posStats.Add(new sPosStat(buffer, inputFile));
                }
            }
        }

        void parseIidStats(byte[] buffer, StreamReader inputFile)
        {
            short amount = 0;
            int sectionHeader = Utils.ReadInt32(buffer, inputFile);
            if (sectionHeader >> 16 == 0x40)
            {
                amount = (short)sectionHeader;

                for (int i = 0; i < amount; i++)
                {
                    sIidStat newStat = new sIidStat(buffer, inputFile);
                    iidStats.Add(newStat.key, newStat);
                }
            }
        }

        public void addOrUpdateStat(eIntStat stat, int value)
        {
            sIntStat entry;
            if (intStats.TryGetValue(stat, out entry))
            {
                entry.value = value;
                intStats[stat] = entry;
            }
            else
            {
                entry = new sIntStat(stat, value);
                intStats.Add(stat, entry);
            }
        }

        public void addOrUpdateStat(eInt64Stat stat, Int64 value)
        {
            sInt64Stat entry;
            if (int64Stats.TryGetValue(stat, out entry))
            {
                entry.value = value;
                int64Stats[stat] = entry;
            }
            else
            {
                entry = new sInt64Stat(stat, value);
                int64Stats.Add(stat, entry);
            }
        }

        public void addOrUpdateStat(eBoolStat stat, int value)
        {
            sBoolStat entry;
            if (boolStats.TryGetValue(stat, out entry))
            {
                entry.value = value;
                boolStats[stat] = entry;
            }
            else
            {
                entry = new sBoolStat(stat, value);
                boolStats.Add(stat, entry);
            }
        }

        public void addOrUpdateStat(eFloatStat stat, double value)
        {
            sFloatStat entry;
            if (floatStats.TryGetValue(stat, out entry))
            {
                entry.value = value;
                floatStats[stat] = entry;
            }
            else
            {
                entry = new sFloatStat(stat, value);
                floatStats.Add(stat, entry);
            }
        }

        public void addOrUpdateStat(eStringStat stat, string value)
        {
            sStringStat entry;
            if (stringStats.TryGetValue(stat, out entry))
            {
                entry.value = value;
                stringStats[stat] = entry;
            }
            else
            {
                entry = new sStringStat(stat, value);
                stringStats.Add(stat, entry);
            }
        }

        public void addOrUpdateStat(eDidStat stat, int value)
        {
            sDidStat entry;
            if (didStats.TryGetValue(stat, out entry))
            {
                entry.value = value;
                didStats[stat] = entry;
            }
            else
            {
                entry = new sDidStat(stat, value);
                didStats.Add(stat, entry);
            }
        }

        public void addOrUpdateStat(eIidStat stat, int value)
        {
            sIidStat entry;
            if (iidStats.TryGetValue(stat, out entry))
            {
                entry.value = value;
                iidStats[stat] = entry;
            }
            else
            {
                entry = new sIidStat(stat, value);
                iidStats.Add(stat, entry);
            }
        }

        public int getStat(eIntStat stat)
        {
            sIntStat entry;
            if (intStats.TryGetValue(stat, out entry))
                return entry.value;
            else
                return 0;
        }

        public Int64 getStat(eInt64Stat stat)
        {
            sInt64Stat entry;
            if (int64Stats.TryGetValue(stat, out entry))
                return entry.value;
            else
                return 0;
        }

        public int getStat(eBoolStat stat)
        {
            sBoolStat entry;
            if (boolStats.TryGetValue(stat, out entry))
                return entry.value;
            else
                return 0;
        }

        public string getStat(eStringStat stat)
        {
            sStringStat entry;
            if (stringStats.TryGetValue(stat, out entry))
                return entry.value;
            else
                return "";
        }

        public double getStat(eFloatStat stat)
        {
            sFloatStat entry;
            if (floatStats.TryGetValue(stat, out entry))
                return entry.value;
            else
                return 0;
        }

        public int getStat(eDidStat stat)
        {
            sDidStat entry;
            if (didStats.TryGetValue(stat, out entry))
                return entry.value;
            else
                return 0;
        }

        public int getStat(eIidStat stat)
        {
            sIidStat entry;
            if (iidStats.TryGetValue(stat, out entry))
                return entry.value;
            else
                return 0;
        }

        public void removeStat(eIntStat stat)
        {
            if (intStats.ContainsKey(stat))
                intStats.Remove(stat);
        }

        public void removeStat(eInt64Stat stat)
        {
            if (int64Stats.ContainsKey(stat))
                int64Stats.Remove(stat);
        }

        public void removeStat(eBoolStat stat)
        {
            if (boolStats.ContainsKey(stat))
                boolStats.Remove(stat);
        }

        public void removeStat(eStringStat stat)
        {
            if (stringStats.ContainsKey(stat))
                stringStats.Remove(stat);
        }

        public void removeStat(eFloatStat stat)
        {
            if (floatStats.ContainsKey(stat))
                floatStats.Remove(stat);
        }

        public void removeStat(eDidStat stat)
        {
            if (didStats.ContainsKey(stat))
                didStats.Remove(stat);
        }

        public void removeStat(eIidStat stat)
        {
            if (iidStats.ContainsKey(stat))
                iidStats.Remove(stat);
        }

        public bool hasStat(eIntStat stat)
        {
            return intStats.ContainsKey(stat);
        }

        public bool hasStat(eInt64Stat stat)
        {
            return int64Stats.ContainsKey(stat);
        }

        public bool hasStat(eBoolStat stat)
        {
            return boolStats.ContainsKey(stat);
        }

        public bool hasStat(eStringStat stat)
        {
            return stringStats.ContainsKey(stat);
        }

        public bool hasStat(eFloatStat stat)
        {
            return floatStats.ContainsKey(stat);
        }

        public bool hasStat(eDidStat stat)
        {
            return didStats.ContainsKey(stat);
        }

        public bool hasStat(eIidStat stat)
        {
            return iidStats.ContainsKey(stat);
        }
    }
}