using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public class validPortalDatEntries
    {
        static validPortalDatEntries instance = new validPortalDatEntries();

        List<uint> validStabs;
        List<uint> validEnvironments;
        Dictionary<ushort, ushort> textureIdMigrationTable;

        public static ushort translateTextureId(ushort id)
        {
            if(instance.textureIdMigrationTable.ContainsKey(id))
                return instance.textureIdMigrationTable[id];
            else
                return 0x032a; //placeholder texture
        }

        public static bool isValidStabEntry(uint id)
        {
            return instance.validStabs.Contains(id);
        }

        public static bool isValidEnvironmentEntry(uint id)
        {
            return instance.validEnvironments.Contains(id);
        }

        public validPortalDatEntries()
        {
            validStabs = new List<uint>();
            validEnvironments = new List<uint>();
            textureIdMigrationTable = new Dictionary<ushort, ushort>();

            StreamReader inputFile = new StreamReader(new FileStream("./input/validStabs.txt", FileMode.Open, FileAccess.Read));

            while (!inputFile.EndOfStream)
            {
                string line = inputFile.ReadLine();
                uint value = uint.Parse(line, System.Globalization.NumberStyles.HexNumber);

                if (value != 0)
                    validStabs.Add(value);
            }

            inputFile = new StreamReader(new FileStream("./input/validEnvironments.txt", FileMode.Open, FileAccess.Read));

            while (!inputFile.EndOfStream)
            {
                string line = inputFile.ReadLine();
                uint value = uint.Parse(line, System.Globalization.NumberStyles.HexNumber);

                if (value != 0)
                    validEnvironments.Add(value);
            }

            inputFile = new StreamReader(new FileStream("./input/textureIdMigrationTable.txt", FileMode.Open, FileAccess.Read));

            while (!inputFile.EndOfStream)
            {
                string line = inputFile.ReadLine();
                string[] splitLine = line.Split(' ');

                if (splitLine.Length >= 2)
                {
                    textureIdMigrationTable.Add(ushort.Parse(splitLine[0], System.Globalization.NumberStyles.HexNumber), ushort.Parse(splitLine[1], System.Globalization.NumberStyles.HexNumber));
                }
            }
        }
    }
}