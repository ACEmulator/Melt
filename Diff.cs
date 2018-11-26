using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Melt
{
    class Diff
    {
        static public void FolderDiff(string path, string path2)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Unable to open {0}", path);
                return;
            }

            if (!Directory.Exists(path2))
            {
                Console.WriteLine("Unable to open {0}", path2);
                return;
            }

            StreamWriter outputFile = new StreamWriter(new FileStream(".\\diff_results.txt", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open diff_results.txt");
                return;
            }

            string[] fileEntries = Directory.GetFiles(path);
            string[] fileEntries2 = Directory.GetFiles(path2);
            int fileCounter1 = 0;
            int fileCounter2 = 0;
            for (int i = 0; i < fileEntries.Length; i++)
            {
                if (fileEntries2.Length <= i)
                    break;

                string[] filename = fileEntries[fileCounter1].Split('\\');
                string[] filename2 = fileEntries2[fileCounter2].Split('\\');

                if (filename[filename.Length - 1] != filename2[filename2.Length - 1])
                {
                    fileCounter1++;
                    continue;
                }
                else if (!AreFilesEqual(fileEntries[fileCounter1], fileEntries2[fileCounter2]))
                {
                    string[] fileCode = filename[filename.Length - 1].Split('.');
                    outputFile.WriteLine("writedat client_portal.dat -f {0}=\"Winter\\{1}\"", fileCode[0], filename[filename.Length-1]);
                    //outputFile.WriteLine("{0} {1}", fileEntries[i], fileEntries2[i]);
                    outputFile.Flush();
                }

                fileCounter1++;
                fileCounter2++;
            }
            Console.WriteLine("Done");

            outputFile.Close();
        }

        static public bool AreFilesEqual(string file1, string file2, bool logOnlyDifferences = true)
        {
            StreamReader inputFile = new StreamReader(new FileStream(file1, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", file1);
                return false;
            }
            StreamReader inputFile2 = new StreamReader(new FileStream(file2, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", file2);
                return false;
            }

            Console.WriteLine("Comparing {0} and {1}...",file1, file2);

            if (inputFile.BaseStream.Length != inputFile2.BaseStream.Length)
            {
                return false;
            }

            for(uint i = 0; i < inputFile.BaseStream.Length; i++)
            {
                byte a = (byte)inputFile.BaseStream.ReadByte();
                byte b = (byte)inputFile2.BaseStream.ReadByte();
                if (a != b)
                {
                    return false;
                }
            }

            inputFile.Close();
            inputFile2.Close();
            //Console.WriteLine("Done");
            return true;
        }

    }
}