using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public enum eStringStat
    {
        Undef = 0,
        Name = 1,
        Title = 2,
        Sex = 3,
        HeritageGroup = 4,
        Template = 5,
        AttackersName = 6,
        Inscription = 7,
        ScribeName = 8,
        VendorsName = 9,
        Fellowship = 10,
        MonarchsName = 11,
        LockCode = 12,
        KeyCode = 13,
        Use = 14,
        ShortDesc = 15,
        LongDesc = 16,
        ActivationTalk = 17,
        UseMessage = 18,
        ItemHeritageGroupRestriction = 19,
        PluralName = 20,
        MonarchsTitle = 21,
        ActivationFailure = 22,
        ScribeAccount = 23,
        TownName = 24,
        CraftsmanName = 25,
        UsePkServerError = 26,
        ScoreCachedText = 27,
        ScoreDefaultEntryFormat = 28,
        ScoreFirstEntryFormat = 29,
        ScoreLastEntryFormat = 30,
        ScoreOnlyEntryFormat = 31,
        ScoreNoEntry = 32,
        Quest = 33,
        GeneratorEvent = 34,
        PatronsTitle = 35,
        HouseOwnerName = 36,
        QuestRestriction = 37,
        AppraisalPortalDestination = 38,
        TinkerName = 39,
        ImbuerName = 40,
        HouseOwnerAccount = 41,
        DisplayName = 42,
        DateOfBirth = 43,
        ThirdPartyApi = 44,
        KillQuest = 45,
        Afk = 46,
        AllegianceName = 47,
        AugmentationAddQuest = 48,
        KillQuest2 = 49,
        KillQuest3 = 50,
        UseSendsSignal = 51,
        GearPlatingName = 52
    }

    public struct sStringStat
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public eStringStat key;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public string value;

        public sStringStat(eStringStat key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public sStringStat(byte[] buffer, StreamReader inputFile)
        {
            key = (eStringStat)Utils.ReadInt32(buffer, inputFile);

            if (!Enum.IsDefined(typeof(eStringStat), key))
                Console.WriteLine("Unknown stringStat: {0}", key);

            value = Utils.ReadStringAndReplaceSpecialCharacters(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)key, outputStream);
            Utils.writeString(Utils.RestoreStringSpecialCharacters(value), outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            outputStream.Write("{0}\n{1}{{", entryStarter, tab);

            Utils.writeJson(outputStream, "key", (int)key, "", true, false, 3);
            Utils.writeJson(outputStream, "value", value, "    ", false, false, 0);
            Utils.writeJson(outputStream, "_comment", key.ToString(), "    ", false, false, 0);
            outputStream.Write("}");
        }
    }
}