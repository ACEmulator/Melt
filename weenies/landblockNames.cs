using System.Collections.Generic;
using System.IO;


namespace Melt
{
    public class landblockNames
    {
        static landblockNames instance = new landblockNames();
        Dictionary<uint, string> map;

        public static string geLandblockName(uint id)
        {
            string value;
            if (instance.map.TryGetValue(id, out value))
                return Utils.ReplaceStringSpecialCharacters(value);
            return "";
        }

        public landblockNames()
        {
            map = new Dictionary<uint, string>();

            StreamReader inputFile = new StreamReader(new FileStream("./input/landblockNames.txt", FileMode.Open, FileAccess.Read));

            while (!inputFile.EndOfStream)
            {
                string line = inputFile.ReadLine();
                string[] splitLine = line.Split('\t');

                if (splitLine.Length >= 2)
                {
                    uint key = uint.Parse(splitLine[0], System.Globalization.NumberStyles.HexNumber);
                    string name = splitLine[1];

                    if (map.ContainsKey(key))
                    {
                        string currentName = map[key];
                        if (currentName.Contains(name))
                            map[key] = $"{map[key]} - {name}";
                    }
                    else
                        map.Add(key, name);
                }
            }
        }
    }
}