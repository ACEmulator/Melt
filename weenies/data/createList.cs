using System;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public struct sCreateListEntry
    {
        public int wcid;
        public int palette;
        public Single shade;
        public eDestinationType destination;
        public int stack_size;
        public int try_to_bond;

        public sCreateListEntry(byte[] buffer, StreamReader inputFile)
        {
            wcid = Utils.ReadInt32(buffer, inputFile);
            palette = Utils.ReadInt32(buffer, inputFile);
            shade = Utils.ReadSingle(buffer, inputFile);
            destination = (eDestinationType)Utils.ReadInt32(buffer, inputFile);
            stack_size = Utils.ReadInt32(buffer, inputFile);
            try_to_bond = Utils.ReadInt32(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32(wcid, outputStream);
            Utils.writeInt32(palette, outputStream);
            Utils.writeSingle(shade, outputStream);
            Utils.writeInt32((int)destination, outputStream);
            Utils.writeInt32(stack_size, outputStream);
            Utils.writeInt32(try_to_bond, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            cWeenie wcidWeenie = Program.cache9Converter.getWeenie(wcid);
            string wcidName = "";

            if (wcidWeenie != null)
                wcidName = wcidWeenie.weenieName;
            wcidName = Utils.removeWcidNameRedundancy(WeenieClassNames.getWeenieClassName(wcid), wcidName);

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "wcid", wcid, subEntriesTab, true, true, 11);
            Utils.writeJson(outputStream, "_comment", wcidName, "    ", false, false, 0);
            Utils.writeJson(outputStream, "palette", palette, subEntriesTab, false, true, 8);
            Utils.writeJson(outputStream, "shade", shade, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "destination", (int)destination, subEntriesTab, false, true, 4);
            Utils.writeJson(outputStream, "_comment2", destination.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "stack_size", stack_size, subEntriesTab, false, true, 5);
            Utils.writeJson(outputStream, "try_to_bond", try_to_bond, subEntriesTab, false, true, 4);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sCprof
    {
        public int wcid;
        public int palette;
        public Single shade;
        public eDestinationType destination;
        public int stack_size;
        public int try_to_bond;

        public sCprof(byte[] buffer, StreamReader inputFile)
        {
            wcid = Utils.ReadInt32(buffer, inputFile);
            palette = Utils.ReadInt32(buffer, inputFile);
            shade = Utils.ReadSingle(buffer, inputFile);
            destination = (eDestinationType)Utils.ReadInt32(buffer, inputFile);
            stack_size = Utils.ReadInt32(buffer, inputFile);
            try_to_bond = Utils.ReadInt32(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32(wcid, outputStream);
            Utils.writeInt32(palette, outputStream);
            Utils.writeSingle(shade, outputStream);
            Utils.writeInt32((int)destination, outputStream);
            Utils.writeInt32(stack_size, outputStream);
            Utils.writeInt32(try_to_bond, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";

            cWeenie wcidWeenie = Program.cache9Converter.getWeenie(wcid);
            string wcidName = "";

            if (wcidWeenie != null)
                wcidName = wcidWeenie.weenieName;
            wcidName = Utils.removeWcidNameRedundancy(WeenieClassNames.getWeenieClassName(wcid), wcidName);

            outputStream.Write("{0}\n{1}\"cprof\": {{", entryStarter, tab);
            Utils.writeJson(outputStream, "wcid", wcid, entriesTab, true, true, 11);
            Utils.writeJson(outputStream, "_comment", wcidName, "    ", false, false, 0);
            Utils.writeJson(outputStream, "palette", palette, entriesTab, false, true, 8);
            Utils.writeJson(outputStream, "shade", shade, entriesTab, false, true, 10);
            Utils.writeJson(outputStream, "destination", (int)destination, entriesTab, false, true, 4);
            Utils.writeJson(outputStream, "_comment2", destination.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "stack_size", stack_size, entriesTab, false, true, 5);
            Utils.writeJson(outputStream, "try_to_bond", try_to_bond, entriesTab, false, true, 4);
            outputStream.Write("\n{0}}}", tab);
        }
    }

    public struct sCreateList
    {
        public List<sCreateListEntry> entries;

        public sCreateList(byte[] buffer, StreamReader inputFile)
        {
            entries = new List<sCreateListEntry>();

            int count = Utils.ReadInt32(buffer, inputFile);
            for (int i = 0; i < count; i++)
            {
                entries.Add(new sCreateListEntry(buffer, inputFile));
            }
        }

        public void writeRaw(StreamWriter outputStream)
        {
            if (entries.Count > 0)
            {
                Utils.writeInt32(entries.Count, outputStream);
                foreach (sCreateListEntry entry in entries)
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
                outputStream.Write("{0}\n{1}\"createList\": [", entryStarter, tab);

                bool firstEntry = true;
                foreach (sCreateListEntry entry in entries)
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