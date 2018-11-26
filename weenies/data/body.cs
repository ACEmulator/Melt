using System;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public struct sAcache
    {
        public int base_armor;
        public int armor_vs_slash;
        public int armor_vs_pierce;
        public int armor_vs_bludgeon;
        public int armor_vs_cold;
        public int armor_vs_fire;
        public int armor_vs_acid;
        public int armor_vs_electric;
        public int armor_vs_nether;

        public sAcache(byte[] buffer, StreamReader inputFile)
        {
            base_armor = Utils.ReadInt32(buffer, inputFile);
            armor_vs_slash = Utils.ReadInt32(buffer, inputFile);
            armor_vs_pierce = Utils.ReadInt32(buffer, inputFile);
            armor_vs_bludgeon = Utils.ReadInt32(buffer, inputFile);
            armor_vs_cold = Utils.ReadInt32(buffer, inputFile);
            armor_vs_fire = Utils.ReadInt32(buffer, inputFile);
            armor_vs_acid = Utils.ReadInt32(buffer, inputFile);
            armor_vs_electric = Utils.ReadInt32(buffer, inputFile);
            armor_vs_nether = Utils.ReadInt32(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32(base_armor, outputStream);
            Utils.writeInt32(armor_vs_slash, outputStream);
            Utils.writeInt32(armor_vs_pierce, outputStream);
            Utils.writeInt32(armor_vs_bludgeon, outputStream);
            Utils.writeInt32(armor_vs_cold, outputStream);
            Utils.writeInt32(armor_vs_fire, outputStream);
            Utils.writeInt32(armor_vs_acid, outputStream);
            Utils.writeInt32(armor_vs_electric, outputStream);
            Utils.writeInt32(armor_vs_nether, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";

            outputStream.Write("{0}\n{1}\"acache\": {{", entryStarter, tab);
            Utils.writeJson(outputStream, "base_armor", base_armor, entriesTab, true, true, 10);
            Utils.writeJson(outputStream, "armor_vs_slash", armor_vs_slash, entriesTab, false, true, 6);
            Utils.writeJson(outputStream, "armor_vs_pierce", armor_vs_pierce, entriesTab, false, true, 5);
            Utils.writeJson(outputStream, "armor_vs_bludgeon", armor_vs_bludgeon, entriesTab, false, true, 3);
            Utils.writeJson(outputStream, "armor_vs_cold", armor_vs_cold, entriesTab, false, true, 7);
            Utils.writeJson(outputStream, "armor_vs_fire", armor_vs_fire, entriesTab, false, true, 7);
            Utils.writeJson(outputStream, "armor_vs_acid", armor_vs_acid, entriesTab, false, true, 7);
            Utils.writeJson(outputStream, "armor_vs_electric", armor_vs_acid, entriesTab, false, true, 3);
            Utils.writeJson(outputStream, "armor_vs_nether", armor_vs_acid, entriesTab, false, true, 5);
            outputStream.Write("\n{0}}}", tab);
        }
    }

    public struct sBpsd
    {
        public Single HLF;
        public Single MLF;
        public Single LLF;
        public Single HRF;
        public Single MRF;
        public Single LRF;
        public Single HLB;
        public Single MLB;
        public Single LLB;
        public Single HRB;
        public Single MRB;
        public Single LRB;

        public sBpsd(byte[] buffer, StreamReader inputFile)
        {
            HLF = Utils.ReadSingle(buffer, inputFile);
            MLF = Utils.ReadSingle(buffer, inputFile);
            LLF = Utils.ReadSingle(buffer, inputFile);
            HRF = Utils.ReadSingle(buffer, inputFile);
            MRF = Utils.ReadSingle(buffer, inputFile);
            LRF = Utils.ReadSingle(buffer, inputFile);
            HLB = Utils.ReadSingle(buffer, inputFile);
            MLB = Utils.ReadSingle(buffer, inputFile);
            LLB = Utils.ReadSingle(buffer, inputFile);
            HRB = Utils.ReadSingle(buffer, inputFile);
            MRB = Utils.ReadSingle(buffer, inputFile);
            LRB = Utils.ReadSingle(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeSingle(HLF, outputStream);
            Utils.writeSingle(MLF, outputStream);
            Utils.writeSingle(LLF, outputStream);
            Utils.writeSingle(HRF, outputStream);
            Utils.writeSingle(MRF, outputStream);
            Utils.writeSingle(LRF, outputStream);
            Utils.writeSingle(HLB, outputStream);
            Utils.writeSingle(MLB, outputStream);
            Utils.writeSingle(LLB, outputStream);
            Utils.writeSingle(HRB, outputStream);
            Utils.writeSingle(MRB, outputStream);
            Utils.writeSingle(LRB, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";

            outputStream.Write("{0}\n{1}\"bpsd\": {{", entryStarter, tab);
            Utils.writeJson(outputStream, "HLF", HLF, entriesTab, true, true, 5);
            Utils.writeJson(outputStream, "MLF", MLF, entriesTab, false, true, 5);
            Utils.writeJson(outputStream, "LLF", LLF, entriesTab, false, true, 5);
            Utils.writeJson(outputStream, "HRF", HRF, entriesTab, false, true, 5);
            Utils.writeJson(outputStream, "MRF", MRF, entriesTab, false, true, 5);
            Utils.writeJson(outputStream, "LRF", LRF, entriesTab, false, true, 5);
            Utils.writeJson(outputStream, "HLB", HLB, entriesTab, false, true, 5);
            Utils.writeJson(outputStream, "MLB", MLB, entriesTab, false, true, 5);
            Utils.writeJson(outputStream, "LLB", LLB, entriesTab, false, true, 5);
            Utils.writeJson(outputStream, "HRB", HRB, entriesTab, false, true, 5);
            Utils.writeJson(outputStream, "MRB", MRB, entriesTab, false, true, 5);
            Utils.writeJson(outputStream, "LRB", LRB, entriesTab, false, true, 5);
            outputStream.Write("\n{0}}}", tab);
        }
    }

    public struct sBodyData
    {
        public int key;
        public int unknownBodyValue;
        public eDamageType dtype;
        public int dval;
        public Single dvar;
        public sAcache acache;
        public eBodyHeight bh;
        public sBpsd bpsd;

        public sBodyData(byte[] buffer, StreamReader inputFile)
        {
            key = Utils.ReadInt32(buffer, inputFile);
            unknownBodyValue = Utils.ReadInt32(buffer, inputFile);
            dtype = (eDamageType)Utils.ReadInt32(buffer, inputFile);
            dval = Utils.ReadInt32(buffer, inputFile);
            dvar = Utils.ReadSingle(buffer, inputFile);
            acache = new sAcache(buffer, inputFile);
            bh = (eBodyHeight)Utils.ReadInt32(buffer, inputFile);
            bpsd = new sBpsd(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32(key, outputStream);
            Utils.writeInt32(unknownBodyValue, outputStream);
            Utils.writeInt32((int)dtype, outputStream);
            Utils.writeInt32(dval, outputStream);
            Utils.writeSingle(dvar, outputStream);
            acache.writeRaw(outputStream);
            Utils.writeInt32((int)bh, outputStream);
            bpsd.writeRaw(outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, tab);
            Utils.writeJson(outputStream, "key", key, entriesTab, true, true, 3);
            outputStream.Write(",\n{0}\"value\": {{", entriesTab);
            Utils.writeJson(outputStream, "dtype", (int)dtype, subEntriesTab, true, true, 3);
            Utils.writeJson(outputStream, "_comment1", $"Damage Type = {dtype.ToString()}", "    ", false, false, 0);
            Utils.writeJson(outputStream, "bh", (int)bh, subEntriesTab, false, true, 6);
            Utils.writeJson(outputStream, "_comment2", $"Body Height = {bh.ToString()}", "    ", false, false, 0);
            Utils.writeJson(outputStream, "dval", dval, subEntriesTab, false, true, 4);
            Utils.writeJson(outputStream, "_comment3", $"Damage Value", "    ", false, false, 0);
            Utils.writeJson(outputStream, "dvar", dval, subEntriesTab, false, true, 4);
            Utils.writeJson(outputStream, "_comment4", $"Damage Variance", "    ", false, false, 0);
            acache.writeJson(outputStream, subEntriesTab, false);
            bpsd.writeJson(outputStream, subEntriesTab, false);
            outputStream.Write("\n{0}}}", entriesTab);
            outputStream.Write("\n{0}}}", tab);
        }
    }

    public struct sBody
    {
        public List<sBodyData> entries;

        public sBody(byte[] buffer, StreamReader inputFile)
        {
            entries = new List<sBodyData>();

            short amount = 0;
            int sectionHeader = Utils.ReadInt32(buffer, inputFile);
            if (sectionHeader >> 16 == 0x40)
            {
                amount = (short)sectionHeader;
                for (int i = 0; i < amount; i++)
                {
                    entries.Add(new sBodyData(buffer, inputFile));
                }
            }
        }

        public void writeRaw(StreamWriter outputStream)
        {
            int sectionHeader = (short)entries.Count;
            sectionHeader |= 0x40 << 16;

            Utils.writeInt32(sectionHeader, outputStream);

            if (entries.Count > 0)
            {
                foreach (sBodyData entry in entries)
                {
                    entry.writeRaw(outputStream);
                }
            }
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";
            string subSubEntriesTab = $"{tab}\t\t\t";

            if (entries.Count > 0)
            {
                outputStream.Write("{0}\n{1}\"body\": {{", entryStarter, tab);
                outputStream.Write("\n{0}\"body_part_table\": [", entriesTab);

                bool firstEntry = true;
                foreach (sBodyData entry in entries)
                {
                    entry.writeJson(outputStream, subEntriesTab, firstEntry);
                    if (firstEntry)
                        firstEntry = false;
                }

                outputStream.Write("\n{0}]", entriesTab);
                outputStream.Write("\n{0}}}", tab);
            }
        }
    }
}