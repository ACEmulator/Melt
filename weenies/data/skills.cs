using System;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public enum eSkills
    {
        None = 0,
        Axe = 1,
        Bow = 2,
        Crossbow = 3,
        Dagger = 4,
        Mace = 5,
        MeleeDefense = 6,
        MissileDefense = 7,
        Sling = 8,
        Spear = 9,
        Staff = 10,
        Sword = 11,
        ThrownWeapon = 12,
        UnarmedCombat = 13,
        ArcaneLore = 14,
        MagicDefense = 15,
        ManaConversion = 16,
        SpellCraft = 17,
        ItemAppraisal = 18,
        PersonalAppraisal = 19,
        Deception = 20,
        Healing = 21,
        Jump = 22,
        Lockpick = 23,
        Run = 24,
        Awareness = 25,
        ArmsAndArmorRepair = 26,
        CreatureAppraisal = 27,
        WeaponAppraisal = 28,
        ArmorAppraisal = 29,
        MagicItemAppraisal = 30,
        CreatureEnchantment = 31,
        ItemEnchantment = 32,
        LifeMagic = 33,
        WarMagic = 34,
        Leadership = 35,
        Loyalty = 36,
        Fletching = 37,
        Alchemy = 38,
        Cooking = 39,
        Salvaging = 40,
        TwoHandedCombat = 41,
        GearCraft = 42,
        VoidMagic = 43,
        HeavyWeapons = 44,
        LightWeapons = 45,
        FinesseWeapons = 46,
        MissileWeapons = 47,
        Shield = 48,
        DualWield = 49,
        Recklessness = 50,
        SneakAttack = 51,
        DirtyFighting = 52,
        Challenge = 53,
        Summoning = 54
    }

    public enum eSkillAdvancementClass
    {
        Undef = 0,
        Untrained = 1,
        Trained = 2,
        Specialized = 3,
        NUM_SKILL_ADVANCEMENT_CLASSES = 4,
        FORCE_SKILL_ADVANCEMENT_CLASS_32_BIT = 0x7FFFFFFF
    }

    public struct sSkill
    {
        public eSkills key;
        public int resistance_of_last_check;
        public eSkillAdvancementClass sac;
        public int pp;
        public int init_level;
        public int level_from_pp;
        public Double last_used_time;

        public sSkill(eSkills skill, eSkillAdvancementClass skillAdvancementClass, int value)
        {
            key = skill;
            sac = skillAdvancementClass;
            init_level = value;
            last_used_time = 269.30424052;
            level_from_pp = 0;
            pp = 0;
            resistance_of_last_check = 65536;
        }

        public sSkill(byte[] buffer, StreamReader inputFile)
        {
            key = (eSkills)Utils.ReadInt32(buffer, inputFile);
            resistance_of_last_check = Utils.ReadInt32(buffer, inputFile);
            sac = (eSkillAdvancementClass)Utils.ReadInt32(buffer, inputFile);
            pp = Utils.ReadInt32(buffer, inputFile);
            init_level = Utils.ReadInt32(buffer, inputFile);
            level_from_pp = Utils.ReadInt32(buffer, inputFile);
            last_used_time = Utils.ReadDouble(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)key, outputStream);
            Utils.writeInt32(resistance_of_last_check, outputStream);
            Utils.writeInt32((int)sac, outputStream);
            Utils.writeInt32(pp, outputStream);
            Utils.writeInt32(init_level, outputStream);
            Utils.writeInt32(level_from_pp, outputStream);
            Utils.writeDouble(last_used_time, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";
            string subSubEntriesTab = $"{tab}\t\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "key", (int)key, subEntriesTab, true, true, 0);
            Utils.writeJson(outputStream, "_comment", key.ToString(), " ", false, false, 0);
            outputStream.Write(",\n{0}\"value\": {{", subEntriesTab);
            Utils.writeJson(outputStream, "resistance_of_last_check", resistance_of_last_check, subSubEntriesTab, true, true, 5);
            Utils.writeJson(outputStream, "sac", (int)sac, subSubEntriesTab, false, true, 26);
            Utils.writeJson(outputStream, "_comment", sac.ToString(), " ", false, false, 0);
            Utils.writeJson(outputStream, "pp", pp, subSubEntriesTab, false, true, 27);
            Utils.writeJson(outputStream, "init_level", init_level, subSubEntriesTab, false, true, 19);
            Utils.writeJson(outputStream, "level_from_pp", level_from_pp, subSubEntriesTab, false, true, 16);
            Utils.writeJson(outputStream, "last_used_time", last_used_time, subSubEntriesTab, false, true, 15);
            outputStream.Write("\n{0}}}", subEntriesTab);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sSkills
    {
        public List<sSkill> skills;

        public sSkills(byte[] buffer, StreamReader inputFile)
        {
            skills = new List<sSkill>();

            int sectionHeader = Utils.ReadInt32(buffer, inputFile);
            if (sectionHeader >> 16 == 0x40)
            {
                short amount = (short)sectionHeader;
                for (int i = 0; i < amount; i++)
                {
                    skills.Add(new sSkill(buffer, inputFile));
                }
            }
        }

        public void writeRaw(StreamWriter outputStream)
        {
            int sectionHeader = (short)skills.Count;
            sectionHeader |= 0x40 << 16;

            Utils.writeInt32(sectionHeader, outputStream);

            foreach (sSkill skill in skills)
            {
                skill.writeRaw(outputStream);
            }
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";

            if (skills.Count > 0)
            {
                outputStream.Write("{0}\n{1}\"skills\": [", entryStarter, tab);

                bool firstEntry = true;
                foreach (sSkill skill in skills)
                {
                    skill.writeJson(outputStream, tab, firstEntry);
                    if (firstEntry)
                        firstEntry = false;
                }

                outputStream.Write("\n{0}]", tab);
            }
        }
    }
}