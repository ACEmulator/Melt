using ManagedSquish;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Drawing;

namespace Melt
{   
    class TextureConverter
    {
        static public void folderToPNG(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Unable to open {0}", path);
                return;
            }

            string[] fileEntries = Directory.GetFiles(path);
            foreach (string fileName in fileEntries)
                toPNG(fileName);
            Console.WriteLine("Done");
        }

        static public void folderBMPToPNG(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Unable to open {0}", path);
                return;
            }

            string[] fileEntries = Directory.GetFiles(path);
            foreach (string fileName in fileEntries)
                BMPtoPNG(fileName);
            Console.WriteLine("Done");
        }

        static public void toBin(string filename, uint fileid, uint format)
        {
            try
            {
                Console.WriteLine("Converting to Bin...");

                //string outputFileName = filename.Replace(".png", "_new.bin");

                //string[] splitFilename = filename.Split('\\');

                string outputFileName = fileid.ToString("x8") + ".bin";

                StreamWriter outputFile = new StreamWriter(new FileStream(outputFileName, FileMode.Create, FileAccess.Write));
                if (outputFile == null)
                {
                    Console.WriteLine("Unable to open {0}", outputFileName);
                    return;
                }

                Bitmap inputBMP = new Bitmap(filename);

                uint width = (uint)inputBMP.Size.Width;
                uint height = (uint)inputBMP.Size.Height;
                uint lenght = 0;

                switch (format)
                {
                    case 21:
                        {
                            lenght = width * height * 4;
                            outputFile.BaseStream.Write(BitConverter.GetBytes((uint)fileid), 0, 4);
                            outputFile.BaseStream.Write(BitConverter.GetBytes((uint)2), 0, 4);
                            outputFile.BaseStream.Write(BitConverter.GetBytes(width), 0, 4);
                            outputFile.BaseStream.Write(BitConverter.GetBytes(height), 0, 4);
                            outputFile.BaseStream.Write(BitConverter.GetBytes(format), 0, 4);
                            outputFile.BaseStream.Write(BitConverter.GetBytes(lenght), 0, 4);
                            outputFile.Flush();

                            for (int y = 0; y < height; y++)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    Color pixel = inputBMP.GetPixel(x, y);

                                    outputFile.BaseStream.Write(BitConverter.GetBytes(pixel.B), 0, 1);
                                    outputFile.BaseStream.Write(BitConverter.GetBytes(pixel.G), 0, 1);
                                    outputFile.BaseStream.Write(BitConverter.GetBytes(pixel.R), 0, 1);
                                    outputFile.BaseStream.Write(BitConverter.GetBytes(pixel.A), 0, 1);
                                    outputFile.Flush();
                                }
                            }
                            break;
                        }
                    case 244:
                        {
                            lenght = width * height;
                            outputFile.BaseStream.Write(BitConverter.GetBytes((uint)fileid), 0, 4);
                            outputFile.BaseStream.Write(BitConverter.GetBytes((uint)2), 0, 4);
                            outputFile.BaseStream.Write(BitConverter.GetBytes(width), 0, 4);
                            outputFile.BaseStream.Write(BitConverter.GetBytes(height), 0, 4);
                            outputFile.BaseStream.Write(BitConverter.GetBytes(format), 0, 4);
                            outputFile.BaseStream.Write(BitConverter.GetBytes(lenght), 0, 4);
                            outputFile.Flush();

                            for (int y = 0; y < height; y++)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    Color pixel = inputBMP.GetPixel(x, y);

                                    outputFile.BaseStream.Write(BitConverter.GetBytes(pixel.A), 0, 1);
                                    outputFile.Flush();
                                }
                            }
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Unsupported texture type: {0}", format);
                            break;
                        }
                }
                outputFile.Close();
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("Unable to open {0}", filename);
            }

            Console.WriteLine("Done");
        }

        static public void BMPtoPNG(string filename)
        {
            if (!filename.Contains(".bmp"))
                return;
            Bitmap bmp = new Bitmap(filename);
            string outputName = filename.Replace("bmp", "png");
            bmp.Save(outputName, System.Drawing.Imaging.ImageFormat.Png);
        }

        static public void toPNG(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return;
            }

            Console.WriteLine("Converting to PNG...");
            byte[] buffer = new byte[1024];

            uint fileHeader = Utils.ReadUInt32(buffer, inputFile);
            uint textureType = Utils.ReadUInt32(buffer, inputFile);
            uint width = Utils.ReadUInt32(buffer, inputFile);
            uint height = Utils.ReadUInt32(buffer, inputFile);
            uint format = Utils.ReadUInt32(buffer, inputFile);
            uint length = Utils.ReadUInt32(buffer, inputFile);

            if (textureType != 8)
                return;

            switch (format)
            {
                case 20: //D3DFMT_R8G8B8
                    {
                        Bitmap bmp = new Bitmap((int)width, (int)height);

                        byte r;
                        byte g;
                        byte b;

                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                b = Utils.ReadByte(buffer, inputFile);
                                g = Utils.ReadByte(buffer, inputFile);
                                r = Utils.ReadByte(buffer, inputFile);

                                bmp.SetPixel(x, y, Color.FromArgb(255, r, g, b));
                            }
                        }

                        bmp.Save(fileHeader.ToString("x8") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    }
                case 21: //D3DFMT_A8R8G8B8
                    {
                        Bitmap bmp = new Bitmap((int)width, (int)height);

                        byte a;
                        byte r;
                        byte g;
                        byte b;

                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                b = Utils.ReadByte(buffer, inputFile);
                                g = Utils.ReadByte(buffer, inputFile);
                                r = Utils.ReadByte(buffer, inputFile);
                                a = Utils.ReadByte(buffer, inputFile);

                                bmp.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                            }
                        }

                        bmp.Save(fileHeader.ToString("x8") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    }
                case 244:
                    {
                        Bitmap bmp = new Bitmap((int)width, (int)height);

                        byte a;
                        byte r;
                        byte g;
                        byte b;

                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                b = 255;
                                g = 255;
                                r = 255;
                                a = Utils.ReadByte(buffer, inputFile);

                                bmp.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                            }
                        }

                        bmp.Save(fileHeader.ToString("x8") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    }
                default:
                    if (textureType == 8)
                    {
                        byte[] buffer2 = new byte[(int)length];
                        byte[] data = Utils.ReadBytes(buffer2, inputFile, (int)length);

                        byte[] data2 = Squish.DecompressImage(data, (int)width, (int)height, SquishFlags.Dxt1);
                        Bitmap bmp = new Bitmap((int)width, (int)height);

                        byte r;
                        byte g;
                        byte b;
                        byte a;

                        int i = 0;
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                r = data2[i++];
                                g = data2[i++];
                                b = data2[i++];
                                a = data2[i++];

                                bmp.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                            }
                        }

                        bmp.Save(fileHeader.ToString("x8") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Unsupported texture type: {0}", format);
                        break;
                    }
            }
        }

        static public void darkMajestyfolderToPNG(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Unable to open {0}", path);
                return;
            }

            string[] fileEntries = Directory.GetFiles(path);
            foreach (string fileName in fileEntries)
                darkMajestyToPNG(fileName);
            Console.WriteLine("Done");
        }

        static public void darkMajestyToPNG(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return;
            }

            Console.WriteLine("Converting to PNG...");
            byte[] buffer = new byte[1024];

            uint fileHeader = Utils.ReadUInt32(buffer, inputFile);
            uint textureType = Utils.ReadUInt32(buffer, inputFile);
            uint width = Utils.ReadUInt32(buffer, inputFile);
            uint height = Utils.ReadUInt32(buffer, inputFile);

            switch (textureType)
            {
                case 4:
                    {
                        Bitmap bmp = new Bitmap((int)width, (int)height);

                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                byte rgb = Utils.ReadByte(buffer, inputFile);
                                byte alpha = Utils.ReadByte(buffer, inputFile);

                                bmp.SetPixel(x, y, Color.FromArgb(alpha, rgb, rgb, rgb));
                            }
                        }

                        bmp.Save(fileHeader.ToString("x8") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    }
                case 10:
                    {
                        Bitmap bmp = new Bitmap((int)width, (int)height);
                        List<byte> r = new List<byte>();
                        List<byte> g = new List<byte>();
                        List<byte> b = new List<byte>();

                        uint sizePerColor = width * height;
                        for (int i = 0; i < sizePerColor; i++)
                            r.Add(Utils.ReadByte(buffer, inputFile));
                        for (int i = 0; i < sizePerColor; i++)
                            g.Add(Utils.ReadByte(buffer, inputFile));
                        for (int i = 0; i < sizePerColor; i++)
                            b.Add(Utils.ReadByte(buffer, inputFile));

                        int count = 0;
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                bmp.SetPixel(x, y, Color.FromArgb(r[count], g[count], b[count]));
                                count++;
                            }
                        }

                        bmp.Save(fileHeader.ToString("x8") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    }
                case 11:
                    {
                        Bitmap bmp = new Bitmap((int)width, (int)height);
                        List<byte> a = new List<byte>();
                        List<byte> r = new List<byte>();
                        List<byte> g = new List<byte>();
                        List<byte> b = new List<byte>();

                        uint sizePerColor = width * height;
                        for (int i = 0; i < sizePerColor; i++)
                            a.Add(Utils.ReadByte(buffer, inputFile));
                        for (int i = 0; i < sizePerColor; i++)
                            r.Add(Utils.ReadByte(buffer, inputFile));
                        for (int i = 0; i < sizePerColor; i++)
                            g.Add(Utils.ReadByte(buffer, inputFile));
                        for (int i = 0; i < sizePerColor; i++)
                            b.Add(Utils.ReadByte(buffer, inputFile));

                        int count = 0;
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                bmp.SetPixel(x, y, Color.FromArgb(a[count],r[count], g[count], b[count]));
                                count++;
                            }
                        }

                        bmp.Save(fileHeader.ToString("x8") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    }
                default:
                    Console.WriteLine("{0}: Unsupported texture type: {1}", filename, textureType);
                    break;
            }

            inputFile.Close();

            Console.WriteLine("Done");
        }
    }
}

                //case 7:
                    //{ //RGB555
                    //    //reverse = WORD pixel555 = (red << 10) | (green << 5) | blue;
                    //    Bitmap bmp = new Bitmap(width, height);

                    //    ushort redMask = 0x7C00;
                    //    ushort greenMask = 0x3E0;
                    //    ushort blueMask = 0x1F;

                    //    byte r, g, b;

                    //    ushort pixelColor;
                    //    for (int y = 0; y < height; y++)
                    //    {
                    //        for (int x = 0; x < width; x++)
                    //        {
                    //            pixelColor = (ushort)Utils.ReadShort(buffer, inputFile);

                    //            r = (byte)((pixelColor & redMask) >> 10);
                    //            g = (byte)((pixelColor & greenMask) >> 5);
                    //            b = (byte)(pixelColor & blueMask);

                    //            // Expand to 8-bit values.
                    //            r = (byte)(r << 3);
                    //            g = (byte)(g << 3);
                    //            b = (byte)(b << 3);

                    //            bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
                    //        }
                    //    }

                    //    bmp.Save(fileHeader.ToString("x8") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //    break;
                    //}
                    //{ //RGB565
                    //reverse = WORD pixel565 = (red_value << 11) | (green_value << 5) | blue_value;
                    //    Bitmap bmp = new Bitmap((int)width, (int)height);

                    //    ushort redMask = 0xF800;
                    //    ushort greenMask = 0x7E0;
                    //    ushort blueMask = 0x1F;

                    //    byte r, g, b;

                    //    ushort pixelColor;
                    //    for (int y = 0; y < height; y++)
                    //    {
                    //        for (int x = 0; x < width; x++)
                    //        {
                    //            pixelColor = (ushort)Utils.ReadShort(buffer, inputFile);

                    //            r = (byte)((pixelColor & redMask) >> 11);
                    //            g = (byte)((pixelColor & greenMask) >> 5);
                    //            b = (byte)(pixelColor & blueMask);

                    //            // Expand to 8-bit values.
                    //            r = (byte)(r << 3);
                    //            g = (byte)(g << 2);
                    //            b = (byte)(b << 3);

                    //            bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
                    //        }
                    //    }

                    //    bmp.Save(fileHeader.ToString("x8") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //    break;
                    //}