using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public enum eIidStat
    {
        Undef = 0,
        Owner = 1,
        Container = 2,
        Wielder = 3,
        Freezer = 4,
        Viewer = 5,
        Generator = 6,
        Scribe = 7,
        CurrentCombatTarget = 8,
        CurrentEnemy = 9,
        ProjectileLauncher = 10,
        CurrentAttacker = 11,
        CurrentDamager = 12,
        CurrentFollowTarget = 13,
        CurrentAppraisalTarget = 14,
        CurrentFellowshipAppraisalTarget = 15,
        ActivationTarget = 16,
        Creator = 17,
        Victim = 18,
        Killer = 19,
        Vendor = 20,
        Customer = 21,
        Bonded = 22,
        Wounder = 23,
        Allegiance = 24,
        Patron = 25,
        Monarch = 26,
        CombatTarget = 27,
        HealthQueryTarget = 28,
        LastUnlocker = 29,
        CrashAndTurnTarget = 30,
        AllowedActivator = 31,
        HouseOwner = 32,
        House = 33,
        Slumlord = 34,
        ManaQueryTarget = 35,
        CurrentGame = 36,
        RequestedAppraisalTarget = 37,
        AllowedWielder = 38,
        AssignedTarget = 39,
        LimboSource = 40,
        Snooper = 41,
        TeleportedCharacter = 42,
        Pet = 43,
        PetOwner = 44,
        PetDevice = 45
    }

    public struct sIidStat
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public eIidStat key;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public int value;

        public sIidStat(eIidStat key, int value)
        {
            this.key = key;
            this.value = value;
        }

        public sIidStat(byte[] buffer, StreamReader inputFile)
        {
            key = (eIidStat)Utils.ReadInt32(buffer, inputFile);

            if (!Enum.IsDefined(typeof(eIidStat), key))
                Console.WriteLine("Unknown iidStat: {0}", key);

            value = Utils.ReadInt32(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)key, outputStream);
            Utils.writeInt32(value, outputStream);
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