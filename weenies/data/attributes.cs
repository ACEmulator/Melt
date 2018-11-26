using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    [Flags]
    public enum eAttributeFlags
    {
        none = 0,
        strength = 1,
        endurance = 2,
        quickness = 4,
        coordination = 8,
        focus = 16,
        self = 32,
        health = 64,
        stamina = 128,
        mana = 256,
    }

    public struct sAttribute
    {
        public int level_from_cp;
        public int init_level;
        public int cp_spent;

        public sAttribute(byte[] buffer, StreamReader inputFile)
        {
            level_from_cp = Utils.ReadInt32(buffer, inputFile);
            init_level = Utils.ReadInt32(buffer, inputFile);
            cp_spent = Utils.ReadInt32(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32(level_from_cp, outputStream);
            Utils.writeInt32(init_level, outputStream);
            Utils.writeInt32(cp_spent, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";

            outputStream.Write(" {");
            Utils.writeJson(outputStream, "level_from_cp", level_from_cp, entriesTab, true, true, 3);
            Utils.writeJson(outputStream, "init_level", init_level, entriesTab, false, true, 6);
            Utils.writeJson(outputStream, "cp_spent", cp_spent, entriesTab, false, true, 8);
            outputStream.Write("\n{0}}}", tab);
        }
    }

    public struct sVital
    {
        public int level_from_cp;
        public int init_level;
        public int cp_spent;
        public int current;

        public sVital(byte[] buffer, StreamReader inputFile)
        {
            level_from_cp = Utils.ReadInt32(buffer, inputFile);
            init_level = Utils.ReadInt32(buffer, inputFile);
            cp_spent = Utils.ReadInt32(buffer, inputFile);
            current = Utils.ReadInt32(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32(level_from_cp, outputStream);
            Utils.writeInt32(init_level, outputStream);
            Utils.writeInt32(cp_spent, outputStream);
            Utils.writeInt32(current, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";

            outputStream.Write(" {");
            Utils.writeJson(outputStream, "level_from_cp", level_from_cp, entriesTab, true, true, 3);
            Utils.writeJson(outputStream, "init_level", init_level, entriesTab, false, true, 6);
            Utils.writeJson(outputStream, "cp_spent", cp_spent, entriesTab, false, true, 8);
            Utils.writeJson(outputStream, "current", current, entriesTab, false, true, 9);
            outputStream.Write("\n{0}}}", tab);
        }
    }

    public struct sAttributes
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public eAttributeFlags attributeFlags;

        public sAttribute strength;
        public sAttribute endurance;
        public sAttribute quickness;
        public sAttribute coordination;
        public sAttribute focus;
        public sAttribute self;

        public sVital health;
        public sVital stamina;
        public sVital mana;

        public sAttributes(byte[] buffer, StreamReader inputFile)
        {
            attributeFlags = (eAttributeFlags)Utils.ReadInt32(buffer, inputFile);

            if (attributeFlags.HasFlag(eAttributeFlags.strength))
                strength = new sAttribute(buffer, inputFile);
            else
                strength = new sAttribute();

            if (attributeFlags.HasFlag(eAttributeFlags.endurance))
                endurance = new sAttribute(buffer, inputFile);
            else
                endurance = new sAttribute();

            if (attributeFlags.HasFlag(eAttributeFlags.quickness))
                quickness = new sAttribute(buffer, inputFile);
            else
                quickness = new sAttribute();

            if (attributeFlags.HasFlag(eAttributeFlags.coordination))
                coordination = new sAttribute(buffer, inputFile);
            else
                coordination = new sAttribute();

            if (attributeFlags.HasFlag(eAttributeFlags.focus))
                focus = new sAttribute(buffer, inputFile);
            else
                focus = new sAttribute();

            if (attributeFlags.HasFlag(eAttributeFlags.self))
                self = new sAttribute(buffer, inputFile);
            else
                self = new sAttribute();

            if (attributeFlags.HasFlag(eAttributeFlags.health))
                health = new sVital(buffer, inputFile);
            else
                health = new sVital();

            if (attributeFlags.HasFlag(eAttributeFlags.stamina))
                stamina = new sVital(buffer, inputFile);
            else
                stamina = new sVital();

            if (attributeFlags.HasFlag(eAttributeFlags.mana))
                mana = new sVital(buffer, inputFile);
            else
                mana = new sVital();
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)attributeFlags, outputStream);
            if (attributeFlags.HasFlag(eAttributeFlags.strength))
                strength.writeRaw(outputStream);

            if (attributeFlags.HasFlag(eAttributeFlags.endurance))
                endurance.writeRaw(outputStream);

            if (attributeFlags.HasFlag(eAttributeFlags.quickness))
                quickness.writeRaw(outputStream);

            if (attributeFlags.HasFlag(eAttributeFlags.coordination))
                coordination.writeRaw(outputStream);

            if (attributeFlags.HasFlag(eAttributeFlags.focus))
                focus.writeRaw(outputStream);

            if (attributeFlags.HasFlag(eAttributeFlags.self))
                self.writeRaw(outputStream);

            if (attributeFlags.HasFlag(eAttributeFlags.health))
                health.writeRaw(outputStream);

            if (attributeFlags.HasFlag(eAttributeFlags.stamina))
                stamina.writeRaw(outputStream);

            if (attributeFlags.HasFlag(eAttributeFlags.mana))
                mana.writeRaw(outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            bool firstEntry = true;

            outputStream.Write("{0}\n{1}\"attributes\": {{", entryStarter, tab);

            if (attributeFlags.HasFlag(eAttributeFlags.strength))
            {
                string subEntryStarter = firstEntry ? "" : ",";
                outputStream.Write("{0}\n{1}\"strength\":", subEntryStarter, entriesTab);
                strength.writeJson(outputStream, entriesTab, true);
                if (firstEntry)
                    firstEntry = false;
            }
            if (attributeFlags.HasFlag(eAttributeFlags.endurance))
            {
                string subEntryStarter = firstEntry ? "" : ",";
                outputStream.Write("{0}\n{1}\"endurance\":", subEntryStarter, entriesTab);
                endurance.writeJson(outputStream, entriesTab, true);
                if (firstEntry)
                    firstEntry = false;
            }
            if (attributeFlags.HasFlag(eAttributeFlags.quickness))
            {
                string subEntryStarter = firstEntry ? "" : ",";
                outputStream.Write("{0}\n{1}\"quickness\":", subEntryStarter, entriesTab);
                quickness.writeJson(outputStream, entriesTab, true);
                if (firstEntry)
                    firstEntry = false;
            }
            if (attributeFlags.HasFlag(eAttributeFlags.coordination))
            {
                string subEntryStarter = firstEntry ? "" : ",";
                outputStream.Write("{0}\n{1}\"coordination\":", subEntryStarter, entriesTab);
                coordination.writeJson(outputStream, entriesTab, true);
                if (firstEntry)
                    firstEntry = false;
            }
            if (attributeFlags.HasFlag(eAttributeFlags.focus))
            {
                string subEntryStarter = firstEntry ? "" : ",";
                outputStream.Write("{0}\n{1}\"focus\":", subEntryStarter, entriesTab);
                focus.writeJson(outputStream, entriesTab, true);
                if (firstEntry)
                    firstEntry = false;
            }
            if (attributeFlags.HasFlag(eAttributeFlags.self))
            {
                string subEntryStarter = firstEntry ? "" : ",";
                outputStream.Write("{0}\n{1}\"self\":", subEntryStarter, entriesTab);
                self.writeJson(outputStream, entriesTab, true);
                if (firstEntry)
                    firstEntry = false;
            }

            if (attributeFlags.HasFlag(eAttributeFlags.health))
            {
                string subEntryStarter = firstEntry ? "" : ",";
                outputStream.Write("{0}\n{1}\"health\":", subEntryStarter, entriesTab);
                health.writeJson(outputStream, entriesTab, true);
                if (firstEntry)
                    firstEntry = false;
            }
            if (attributeFlags.HasFlag(eAttributeFlags.stamina))
            {
                string subEntryStarter = firstEntry ? "" : ",";
                outputStream.Write("{0}\n{1}\"stamina\":", subEntryStarter, entriesTab);
                stamina.writeJson(outputStream, entriesTab, true);
                if (firstEntry)
                    firstEntry = false;
            }
            if (attributeFlags.HasFlag(eAttributeFlags.mana))
            {
                string subEntryStarter = firstEntry ? "" : ",";
                outputStream.Write("{0}\n{1}\"mana\":", subEntryStarter, entriesTab);
                mana.writeJson(outputStream, entriesTab, true);
                if (firstEntry)
                    firstEntry = false;
            }

            outputStream.Write("\n{0}}}", tab);
        }
    }
}