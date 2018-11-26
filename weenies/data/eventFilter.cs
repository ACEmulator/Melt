using System;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public struct sEventFilter
    {
        public List<int> events;

        public sEventFilter(byte[] buffer, StreamReader inputFile)
        {
            events = new List<int>();

            int count = Utils.ReadInt32(buffer, inputFile);
            for (int i = 0; i < count; i++)
            {
                events.Add(Utils.ReadInt32(buffer, inputFile));
            }
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32(events.Count, outputStream);
            foreach(int entry in events)
            {
                Utils.writeInt32(entry, outputStream);
            }
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";

            if (events.Count > 0)
            {
                outputStream.Write("{0}\n{1}\"eventFilter\": {{ \"events\": [", entryStarter, tab);

                bool firstEntry = true;
                foreach (int eventEntry in events)
                {
                    string subEntryStarter = firstEntry ? "" : ",";
                    outputStream.Write("{0} {1}", subEntryStarter, eventEntry);
                    if (firstEntry)
                        firstEntry = false;
                }

                outputStream.Write(" ] }");
            }
        }
    }
}