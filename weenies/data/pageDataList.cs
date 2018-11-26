using System;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public struct sPage
    {
        public uint authorID;
        public string authorName;
        public string authorAccount;
        public Int16 unknownBookValue1;
        public Int16 unknownBookValue2;
        public int unknownBookValue3;
        public int ignoreAuthor;
        public string pageText;

        public sPage(byte[] buffer, StreamReader inputFile)
        {
            authorID = Utils.ReadUInt32(buffer, inputFile);
            authorName = Utils.ReadStringAndReplaceSpecialCharacters(buffer, inputFile);
            authorAccount = Utils.ReadStringAndReplaceSpecialCharacters(buffer, inputFile);
            unknownBookValue1 = Utils.ReadInt16(buffer, inputFile);//not used by PhatAC
            unknownBookValue2 = Utils.ReadInt16(buffer, inputFile);//not used by PhatAC
            unknownBookValue3 = Utils.ReadInt32(buffer, inputFile);//not used by PhatAC
            ignoreAuthor = Utils.ReadInt32(buffer, inputFile);
            pageText = Utils.ReadStringAndReplaceSpecialCharacters(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeUInt32(authorID, outputStream);
            Utils.writeString(Utils.RestoreStringSpecialCharacters(authorName), outputStream);
            Utils.writeString(Utils.RestoreStringSpecialCharacters(authorAccount), outputStream);
            Utils.writeInt16(unknownBookValue1, outputStream);
            Utils.writeInt16(unknownBookValue2, outputStream);
            Utils.writeInt32(unknownBookValue3, outputStream);
            Utils.writeInt32(ignoreAuthor, outputStream);
            Utils.writeString(Utils.RestoreStringSpecialCharacters(pageText), outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";
            string subSubEntriesTab = $"{tab}\t\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "authorID", authorID, subEntriesTab, true, true, 17);
            Utils.writeJson(outputStream, "authorName", authorName, subEntriesTab, false, true, authorName == " " ? 12 : 0);
            Utils.writeJson(outputStream, "authorAccount", authorAccount, subEntriesTab, false, true, 0);
            Utils.writeJson(outputStream, "ignoreAuthor", ignoreAuthor, subEntriesTab, false, true, 13);
            Utils.writeJson(outputStream, "pageText", pageText, subEntriesTab, false, true, 0);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sPageDataList
    {
        public int maxNumPages;
        public int maxNumCharsPerPage;
        public List<sPage> pages;

        public sPageDataList(byte[] buffer, StreamReader inputFile)
        {
            maxNumPages = Utils.ReadInt32(buffer, inputFile);
            maxNumCharsPerPage = Utils.ReadInt32(buffer, inputFile);

            int pageCount = Utils.ReadInt32(buffer, inputFile);
            pages = new List<sPage>();
            for (int i = 0; i < pageCount; i++)
            {
                pages.Add(new sPage(buffer, inputFile));
            }
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32(maxNumPages, outputStream);
            Utils.writeInt32(maxNumCharsPerPage, outputStream);
            Utils.writeInt32(pages.Count, outputStream);

            foreach (sPage page in pages)
            {
                page.writeRaw(outputStream);
            }
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";

            if (pages.Count > 0)
            {
                outputStream.Write("{0}\n{1}\"pageDataList\": {{", entryStarter, tab);

                Utils.writeJson(outputStream, "maxNumPages", maxNumPages, entriesTab, true, true, 11);
                Utils.writeJson(outputStream, "maxNumCharsPerPage", maxNumCharsPerPage, entriesTab, false, true, 4);
                outputStream.Write(",\n{0}\"pages\": [", entriesTab);
                bool firstEntry = true;
                foreach (sPage page in pages)
                {
                    page.writeJson(outputStream, entriesTab, firstEntry);
                    if (firstEntry)
                        firstEntry = false;
                }

                outputStream.Write("\n{0}]", entriesTab);
                outputStream.Write("\n{0}}}", tab);
            }
        }
    }
}