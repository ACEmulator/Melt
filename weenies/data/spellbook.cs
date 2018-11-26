using System;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public struct sSpellData
    {
        public int key;
        public Single casting_likelihood;

        public sSpellData(int spell, Single castingLikelyhood)
        {
            key = spell;
            casting_likelihood = castingLikelyhood;
        }

        public sSpellData(byte[] buffer, StreamReader inputFile)
        {
            key = Utils.ReadInt32(buffer, inputFile);
            casting_likelihood = Utils.ReadSingle(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32(key, outputStream);
            Utils.writeSingle(casting_likelihood, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";
            string subSubEntriesTab = $"{tab}\t\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "key", key, subEntriesTab, true, true, 4);
            Utils.writeJson(outputStream, "_comment", SpellInfo.getSpellName(key), "    ", false, false, 0);
            outputStream.Write(",\n{0}\"value\": {{", subEntriesTab);
            Utils.writeJson(outputStream, "casting_likelihood", casting_likelihood, subSubEntriesTab, true, true, 3);
            outputStream.Write("\n{0}}}", subEntriesTab);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sSpellBook
    {
        public List<sSpellData> spellData;

        public sSpellBook(byte[] buffer, StreamReader inputFile)
        {
            spellData = new List<sSpellData>();

            int sectionHeader = Utils.ReadInt32(buffer, inputFile);

            if (sectionHeader >> 16 == 0x40)
            {
                short amount = (short)sectionHeader;
                for (int i = 0; i < amount; i++)
                {
                    spellData.Add(new sSpellData(buffer, inputFile));
                }
            }
        }

        public void writeRaw(StreamWriter outputStream)
        {
            int sectionHeader = (short)spellData.Count;
            sectionHeader |= 0x40 << 16;

            Utils.writeInt32(sectionHeader, outputStream);

            foreach (sSpellData spell in spellData)
            {
                spell.writeRaw(outputStream);
            }
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";

            if (spellData.Count > 0)
            {
                outputStream.Write("{0}\n{1}\"spellbook\": [", entryStarter, tab);

                bool firstEntry = true;
                foreach (sSpellData spell in spellData)
                {
                    spell.writeJson(outputStream, tab, firstEntry);
                    if (firstEntry)
                        firstEntry = false;
                }

                outputStream.Write("\n{0}]", tab);
            }
        }
    }
}