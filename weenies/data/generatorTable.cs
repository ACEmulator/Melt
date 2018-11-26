using System;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public struct sGeneratorTableEntry
    {
        public Single probability;
        public int type;
        public Double delay;
        public int initCreate;
        public int maxNum;
        public eRegenerationType whenCreate;
        public eRegenLocationType whereCreate;
        public int stackSize;
        public int ptid;
        public int shade;
        public int slot;
        public sFrame frame;
        public int objcell_id;

        public sGeneratorTableEntry(byte[] buffer, StreamReader inputFile)
        {
            probability = Utils.ReadSingle(buffer, inputFile);
            type = Utils.ReadInt32(buffer, inputFile);
            delay = Utils.ReadDouble(buffer, inputFile);
            initCreate = Utils.ReadInt32(buffer, inputFile);
            maxNum = Utils.ReadInt32(buffer, inputFile);
            whenCreate = (eRegenerationType)Utils.ReadInt32(buffer, inputFile);
            whereCreate = (eRegenLocationType)Utils.ReadInt32(buffer, inputFile);
            stackSize = Utils.ReadInt32(buffer, inputFile);
            ptid = Utils.ReadInt32(buffer, inputFile);
            shade = Utils.ReadInt32(buffer, inputFile);
            objcell_id = Utils.ReadInt32(buffer, inputFile);
            frame = new sFrame(buffer, inputFile);
            slot = Utils.ReadInt32(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeSingle(probability, outputStream);
            Utils.writeInt32(type, outputStream);
            Utils.writeDouble(delay, outputStream);
            Utils.writeInt32(initCreate, outputStream);
            Utils.writeInt32(maxNum, outputStream);
            Utils.writeInt32((int)whenCreate, outputStream);
            Utils.writeInt32((int)whereCreate, outputStream);
            Utils.writeInt32(stackSize, outputStream);
            Utils.writeInt32(ptid, outputStream);
            Utils.writeInt32(shade, outputStream);
            Utils.writeInt32(objcell_id, outputStream);
            frame.writeRaw(outputStream);
            Utils.writeInt32(slot, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            cWeenie wcidWeenie = Program.cache9Converter.getWeenie(type);
            string wcidName = "";

            if (wcidWeenie != null)
                wcidName = wcidWeenie.weenieName;
            wcidName = Utils.removeWcidNameRedundancy(WeenieClassNames.getWeenieClassName(type), wcidName);

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", type, subEntriesTab, true, true, 16);
            if((int)whereCreate < 0x40)
                Utils.writeJson(outputStream, "_comment", wcidName, "    ", false, false, 0);
            else
                Utils.writeJson(outputStream, "_comment", ((eTreasureGeneratorType)type).ToString(), "    ", false, false, 0);
                //Utils.writeJson(outputStream, "_comment", "Unknown treasure table value. Not working in phatAC", "    ", false, false, 0);
            Utils.writeJson(outputStream, "probability", probability, subEntriesTab, false, true, 9);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 15);
            Utils.writeJson(outputStream, "initCreate", initCreate, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "maxNum", maxNum, subEntriesTab, false, true, 14);
            Utils.writeJson(outputStream, "whenCreate", (int)whenCreate, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "_comment2", whenCreate.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "whereCreate", (int)whereCreate, subEntriesTab, false, true, 9);
            Utils.writeJson(outputStream, "_comment3", whereCreate.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "stackSize", stackSize, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "ptid", ptid, subEntriesTab, false, true, 16);
            Utils.writeJson(outputStream, "shade", shade, subEntriesTab, false, true, 15);
            Utils.writeJson(outputStream, "slot", slot, subEntriesTab, false, true, 16);
            Utils.writeJson(outputStream, "objcell_id", objcell_id, subEntriesTab, false, true, 10);
            frame.writeJson(outputStream, subEntriesTab, false);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sGeneratorTable
    {
        public List<sGeneratorTableEntry> entries;

        public sGeneratorTable(byte[] buffer, StreamReader inputFile)
        {
            entries = new List<sGeneratorTableEntry>();

            int count = Utils.ReadInt32(buffer, inputFile);
            for (int i = 0; i < count; i++)
            {
                entries.Add(new sGeneratorTableEntry(buffer, inputFile));
            }
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32(entries.Count, outputStream);

            if(entries.Count > 0)
            {
                foreach (sGeneratorTableEntry entry in entries)
                {
                    entry.writeRaw(outputStream);
                }
            }
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";

            if (entries.Count > 0)
            {
                outputStream.Write("{0}\n{1}\"generatorTable\": [", entryStarter, tab);

                bool firstEntry = true;
                foreach (sGeneratorTableEntry entry in entries)
                {
                    entry.writeJson(outputStream, tab, firstEntry);
                    if (firstEntry)
                        firstEntry = false;
                }

                outputStream.Write("\n{0}]", tab);
            }
        }
    }
}