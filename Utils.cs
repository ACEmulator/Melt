using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Melt
{
    public enum eRandomFormula
    {
        favorSpecificValue,
        favorLow,
        favorMid,
        favorHigh,
        equalDistribution,
    }

    class Utils
    {
        static public byte ReadByte(byte[] buffer, StreamReader data)
        {
            data.BaseStream.Read(buffer, 0, 1);
            return (byte)BitConverter.ToChar(buffer, 0);
        }

        static public byte[] ReadBytes(byte[] buffer, StreamReader data, int amount)
        {
            byte[] bytes = new byte[amount];
            for (int i = 0; i < amount; i++)
                bytes[i] = ReadByte(buffer, data);
            return bytes;
        }

        static public Int16 ReadInt16(byte[] buffer, StreamReader data)
        {
            data.BaseStream.Read(buffer, 0, 2);
            return BitConverter.ToInt16(buffer, 0);
        }

        static public Int32 ReadInt32(byte[] buffer, StreamReader data)
        {
            data.BaseStream.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        static public Int64 ReadInt64(byte[] buffer, StreamReader data)
        {
            data.BaseStream.Read(buffer, 0, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        static public bool ReadBool(byte[] buffer, StreamReader data)
        {
            data.BaseStream.Read(buffer, 0, 1);
            return BitConverter.ToBoolean(buffer, 0);
        }

        static public UInt16 ReadUInt16(byte[] buffer, StreamReader data)
        {
            data.BaseStream.Read(buffer, 0, 2);
            return BitConverter.ToUInt16(buffer, 0);
        }

        static public UInt32 ReadUInt32(byte[] buffer, StreamReader data)
        {
            data.BaseStream.Read(buffer, 0, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        static public UInt32 ReadCompressedUInt32(byte[] buffer, StreamReader data)
        {
            var b0 = ReadByte(buffer, data);
            if ((b0 & 0x80) == 0)
                return b0;

            var b1 = ReadByte(buffer, data);
            if ((b0 & 0x40) == 0)
                return (uint)(((b0 & 0x7F) << 8) | b1);

            var s = ReadUInt16(buffer, data);
            return (uint)(((((b0 & 0x3F) << 8) | b1) << 16) | s);
        }

        static public UInt32 ReadPackedUInt32(byte[] buffer, StreamReader data, uint typeSize = 0)
        {
            //uint testValue = 100000;
            //uint packedValue = (testValue << 16) | ((testValue >> 16) | 0x8000);
            //uint unpackedValue = (packedValue >> 16) | ((packedValue ^ 0x8000) << 16);

            ushort value = ReadUInt16(buffer, data);
            if (value >> 12 != 0x08)
            {
                return value + typeSize;
            }
            else
            {
                data.BaseStream.Seek(-2, SeekOrigin.Current);
                uint packedValue = ReadUInt32(buffer, data);
                uint unpackedValue = (packedValue >> 16) | ((packedValue ^ 0x8000) << 16);
                return unpackedValue + typeSize;
            }
        }

        static public UInt64 ReadUInt64(byte[] buffer, StreamReader data)
        {
            data.BaseStream.Read(buffer, 0, 8);
            return BitConverter.ToUInt64(buffer, 0);
        }

        static public float ReadSingle(byte[] buffer, StreamReader data)
        {
            data.BaseStream.Read(buffer, 0, 4);
            return BitConverter.ToSingle(buffer, 0);
        }

        static public double ReadDouble(byte[] buffer, StreamReader data)
        {
            data.BaseStream.Read(buffer, 0, 8);
            return BitConverter.ToDouble(buffer, 0);
        }

        static public void Align(StreamWriter data)
        {
            int alignedPosition = (int)(data.BaseStream.Position);
            if (alignedPosition % 4 != 0)
            {
                alignedPosition = alignedPosition + (4 - alignedPosition % 4);

                int difference = alignedPosition - (int)(data.BaseStream.Position);

                for (int i = 0; i < difference; i++)
                {
                    data.BaseStream.WriteByte(0);
                }
            }
        }

        static public void Align(StreamReader data)
        {
            int alignedPosition = (int)(data.BaseStream.Position);
            if (alignedPosition % 4 != 0)
            {
                alignedPosition = alignedPosition + (4 - alignedPosition % 4);
                data.BaseStream.Seek(alignedPosition, 0);
            }
        }

        static public int Align4(int index)
        {
            return (index + 3) & 0xFFFFFC;
        }

        static public string ReplaceStringSpecialCharacters(string text)
        {
            text = text.Replace("\\n", "<tempLineBreak>");
            text = text.Replace("\\t", "<tempTabulation>");
            text = text.Replace("\\\"", "<tempQuote>");

            text = text.Replace("\n", "\\n");
            text = text.Replace("\t", "\\t");
            text = text.Replace("\"", "\\\"");

            text = text.Replace("<tempLineBreak>", "\\\\n");
            text = text.Replace("<tempTabulation>", "\\\\t");
            text = text.Replace("<tempQuote>", "\\\"");

            return text;
        }

        static public string RestoreStringSpecialCharacters(string text)
        {
            text = text.Replace("\\\\n", "<tempLineBreak>");
            text = text.Replace("\\\\t", "<tempTabulation>");
            text = text.Replace("\\\\\"", "<tempQuote>");

            text = text.Replace("\\n", "\n");
            text = text.Replace("\\t", "\t");
            text = text.Replace("\\\"", "\"");

            text = text.Replace("<tempLineBreak>", "\\n");
            text = text.Replace("<tempTabulation>", "\\t");
            text = text.Replace("<tempQuote>", "\\\"");

            return text;
        }

        static public string ReadStringAndReplaceSpecialCharacters(byte[] buffer, StreamReader data)
        {
            int startIndex = (int)data.BaseStream.Position;
            string text = "";
            int letterCount = ReadInt16(buffer, data);

            data.BaseStream.Read(buffer, 0, letterCount);
            for (int i = 0; i < letterCount; i++)
            {
                byte nextByte = (byte)(buffer[i]);
                text += (char)nextByte;
            }
            Align(data);

            return ReplaceStringSpecialCharacters(text);
        }

        static public string ReadSerializedString(byte[] buffer, StreamReader data)
        {
            int startIndex = (int)data.BaseStream.Position;
            string text = "";
            uint letterCount = ReadCompressedUInt32(buffer, data);

            data.BaseStream.Read(buffer, 0, (int)letterCount);
            for (int i = 0; i < letterCount; i++)
            {
                byte nextByte = buffer[i];
                text += (char)nextByte;
            }

            return text;
        }

        static public string ReadString(byte[] buffer, StreamReader data)
        {
            int startIndex = (int)data.BaseStream.Position;
            string text = "";
            int letterCount = ReadInt16(buffer, data);

            data.BaseStream.Read(buffer, 0, letterCount);
            for (int i = 0; i < letterCount; i++)
            {
                byte nextByte = buffer[i];
                text += (char)nextByte;
            }
            Align(data);

            return text;
        }

        static public string ReadStringNoAlign(byte[] buffer, StreamReader data)
        {
            int startIndex = (int)data.BaseStream.Position;
            string text = "";
            int letterCount = ReadInt16(buffer, data);

            data.BaseStream.Read(buffer, 0, letterCount);
            for (int i = 0; i < letterCount; i++)
            {
                byte nextByte = (byte)(buffer[i]);
                text += (char)nextByte;
            }

            return text;
        }

        static public string ReadEncodedString(byte[] buffer, StreamReader data)
        {
            int startIndex = (int)data.BaseStream.Position;
            string text = "";
            int letterCount = ReadInt16(buffer, data);

            data.BaseStream.Read(buffer, 0, letterCount);
            for (int i = 0; i < letterCount; i++)
            {
                byte nextByte = (byte)((buffer[i] >> 4) ^ (buffer[i] << 4));
                text += (char)nextByte;
            }

            Align(data);
            return text;
        }

        static public uint GetHash(string value, uint seed)
        {
            uint r = 0;
            for (int i = 0; i < value.Length; i++)
            {
                int c = value[i];
                r = (uint)((r << 4) + c) & 0xFFFFFFFF;
                int t = (int)(r >> 28);
                if (t != 0)
                {
                    r = (uint)((r & 0xFFFFFFF) ^ (t << 4));
                }
            }
            return (uint)(r % seed);
        }

        public static void convertStringToEncodedByteArray(string text, ref byte[] byteArray, int startIndex, int length)
        {
            for (int i = startIndex; i < startIndex + length; i++)
            {
                byte nextByte = (byte)((text[i] << 4) ^ (text[i] >> 4));
                byteArray[i] = nextByte;
            }

            byte fillerByte = (byte)((0 << 4) ^ (0 >> 4));
            for (int i = startIndex + length; i < startIndex + length + 4; i++)
            {

                byteArray[i] = fillerByte;
            }
        }

        public static void convertStringToByteArray(string text, ref byte[] byteArray, int startIndex, int length)
        {
            for (int i = startIndex; i < startIndex + length; i++)
            {
                byte nextByte = (byte)text[i];
                byteArray[i] = nextByte;
            }

            byte fillerByte = 0x00;
            for (int i = startIndex + length; i < startIndex + length + 4; i++)
            {

                byteArray[i] = fillerByte;
            }
        }

        static public byte ReadAndWriteByte(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
        {
            data.BaseStream.Read(buffer, 0, 1);
            if (write)
                outputData.BaseStream.Write(buffer, 0, 1);
            return (byte)BitConverter.ToChar(buffer, 0);
        }

        static public short ReadAndWriteShort(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
        {
            data.BaseStream.Read(buffer, 0, 2);
            if (write)
                outputData.BaseStream.Write(buffer, 0, 2);
            return BitConverter.ToInt16(buffer, 0);
        }

        static public Int32 ReadAndWriteInt32(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
        {
            data.BaseStream.Read(buffer, 0, 4);
            if (write)
                outputData.BaseStream.Write(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        static public Int64 ReadAndWriteInt64(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
        {
            data.BaseStream.Read(buffer, 0, 8);
            if (write)
                outputData.BaseStream.Write(buffer, 0, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        static public UInt32 ReadAndWriteUInt32(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
        {
            data.BaseStream.Read(buffer, 0, 4);
            if (write)
                outputData.BaseStream.Write(buffer, 0, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        static public UInt64 ReadAndWriteUInt64(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
        {
            data.BaseStream.Read(buffer, 0, 8);
            if (write)
                outputData.BaseStream.Write(buffer, 0, 8);
            return BitConverter.ToUInt64(buffer, 0);
        }

        static public float ReadAndWriteSingle(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
        {
            data.BaseStream.Read(buffer, 0, 4);
            if (write)
                outputData.BaseStream.Write(buffer, 0, 4);
            return BitConverter.ToSingle(buffer, 0);
        }

        static public double ReadAndWriteDouble(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
        {
            data.BaseStream.Read(buffer, 0, 8);
            if (write)
                outputData.BaseStream.Write(buffer, 0, 8);
            return BitConverter.ToDouble(buffer, 0);
        }


        static public string ReadAndWriteString(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
        {
            int startIndex = (int)data.BaseStream.Position;
            string text = "";
            int letterCount = ReadInt16(buffer, data);
            if (write)
                outputData.BaseStream.Write(buffer, 0, 2);

            data.BaseStream.Read(buffer, 0, letterCount);
            if (write)
                outputData.BaseStream.Write(buffer, 0, letterCount);
            for (int i = 0; i < letterCount; i++)
            {
                byte nextByte = (byte)(buffer[i]);
                text += (char)nextByte;
            }

            int endIndex = (int)(data.BaseStream.Position);
            int alignedIndex = Align4(endIndex - startIndex);
            int newIndex = startIndex + alignedIndex;
            int bytesNeededToReachAlignment = newIndex - endIndex;
            data.BaseStream.Read(buffer, 0, bytesNeededToReachAlignment);
            if (write)
            {
                for (int i = 0; i < bytesNeededToReachAlignment; i++)
                    outputData.BaseStream.WriteByte(0x00);
            }
            return text;
        }

        static public string ReadAndWriteEncodedString(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
        {
            int startIndex = (int)data.BaseStream.Position;
            string text = "";
            int letterCount = ReadInt16(buffer, data);
            if (write)
                outputData.BaseStream.Write(buffer, 0, 2);

            data.BaseStream.Read(buffer, 0, letterCount);
            if (write)
                outputData.BaseStream.Write(buffer, 0, letterCount);
            for (int i = 0; i < letterCount; i++)
            {
                byte nextByte = (byte)((buffer[i] >> 4) ^ (buffer[i] << 4));
                text += (char)nextByte;
            }

            int endIndex = (int)(data.BaseStream.Position);
            int alignedIndex = Align4(endIndex - startIndex);
            int newIndex = startIndex + alignedIndex;
            int bytesNeededToReachAlignment = newIndex - endIndex;
            data.BaseStream.Read(buffer, 0, bytesNeededToReachAlignment);
            if (write)
            {
                for (int i = 0; i < bytesNeededToReachAlignment; i++)
                    outputData.BaseStream.WriteByte(0x00);
            }
            return text;
        }

        static public void writeByte(byte value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 1);
        }

        static public void writeBytes(byte[] value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(value, 0, value.Length);
        }

        static public void writeBytes(byte[] value, int offset, int length, StreamWriter outputData)
        {
            outputData.BaseStream.Write(value, offset, length);
        }

        static public void writeInt16(Int16 value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 2);
        }

        static public void writeBool(bool value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 1);
        }

        static public void writeUInt16(UInt16 value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 2);
        }

        static public void writeInt32(Int32 value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 4);
        }

        static public void writeInt64(Int64 value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 8);
        }

        static public void writeUInt32(UInt32 value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 4);
        }

        static public void writeUInt64(UInt64 value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 8);
        }

        static public UInt16 LowWord(UInt32 number)
        {
            return (UInt16)(number & 0x0000FFFF);
        }

        static public UInt16 HighWord(UInt32 number)
        {
            return (UInt16)(number & 0xFFFF0000);
        }

        static public void writeCompressedUInt32(UInt32 value, StreamWriter outputData)
        {
            byte[] byteArray = new byte[4];
            writeToByteArray(value, byteArray, 0);

            if (value > 0x7F)
            {
                if (value > 0x3FFF)
                {
                    writeByte((byte)(byteArray[3] | 0xC0), outputData);
                    writeByte(byteArray[2], outputData);
                    writeByte(byteArray[0], outputData);
                    writeByte(byteArray[1], outputData);
                }
                else
                {
                    writeByte((byte)(byteArray[1] | 0x80), outputData);
                    writeByte(byteArray[0], outputData);
                }
            }
            else
                writeByte(byteArray[0], outputData);
        }

        static public void writePackedUInt32(UInt32 value, StreamWriter outputData, UInt32 typeSize = 0)
        {
            value = value - typeSize;

            if (value <= 16383)
            {
                UInt16 shortPackedValue = Convert.ToUInt16(value);
                outputData.BaseStream.Write(BitConverter.GetBytes(shortPackedValue), 0, 2);
            }
            else
            {
                UInt32 packedValue = (value << 16) | ((value >> 16) | 0x8000);
                outputData.BaseStream.Write(BitConverter.GetBytes(packedValue), 0, 4);
            }
        }

        static public void writeSingle(Single value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 4);
        }

        static public void writeDouble(Double value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes(value), 0, 8);
        }

        static public void writeSerializedString(string value, StreamWriter outputData)
        {
            writeCompressedUInt32((UInt32)value.Length, outputData);

            if (value.Length > 0)
            {
                byte[] buffer = new byte[value.Length + 4];
                convertStringToByteArray(value, ref buffer, 0, value.Length);
                outputData.BaseStream.Write(buffer, 0, value.Length);
            }
        }

        static public void writeString(string value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes((short)value.Length), 0, 2);

            if (value.Length > 0)
            {
                byte[] buffer = new byte[value.Length + 4];
                convertStringToByteArray(value, ref buffer, 0, value.Length);
                outputData.BaseStream.Write(buffer, 0, value.Length);
            }
            Align(outputData);
        }

        static public void writeEncodedString(string value, StreamWriter outputData)
        {
            outputData.BaseStream.Write(BitConverter.GetBytes((short)value.Length), 0, 2);
            if (value.Length > 0)
            {
                byte[] buffer = new byte[value.Length + 4];
                convertStringToEncodedByteArray(value, ref buffer, 0, value.Length);
                outputData.BaseStream.Write(buffer, 0, value.Length);
            }
            Align(outputData);
        }

        static public void writeJson(StreamWriter outputStream, string key, string value, string tab = "", bool isFirst = false, bool lineBreak = true, int padAmount = 0)
        {
            string entryStarter = isFirst ? "" : ",";
            string newLine = lineBreak ? "\n" : "";

            string padding = "";
            //padding = padding.PadLeft(Math.Max(padAmount - (value.Length + 2), 0));
            padding = padding.PadLeft(Math.Max(padAmount, 0));

            string output = $"{entryStarter}{newLine}{tab}\"{key}\": {padding}\"{value}\"";
            outputStream.Write(output);
        }

        static public void writeJson(StreamWriter outputStream, string key, int value, string tab = "", bool isFirst = false, bool lineBreak = true, int padAmount = 0)
        {
            string entryStarter = isFirst ? "" : ",";
            string newLine = lineBreak ? "\n" : "";

            string paddedValue = value.ToString().PadLeft(padAmount);

            string output = $"{entryStarter}{newLine}{tab}\"{key}\": {paddedValue}";
            outputStream.Write(output);
        }

        static public void writeJson(StreamWriter outputStream, string key, uint value, string tab = "", bool isFirst = false, bool lineBreak = true, int padAmount = 0)
        {
            string entryStarter = isFirst ? "" : ",";
            string newLine = lineBreak ? "\n" : "";

            string paddedValue = value.ToString().PadLeft(padAmount);

            string output = $"{entryStarter}{newLine}{tab}\"{key}\": {paddedValue}";
            outputStream.Write(output);
        }

        static public void writeJson(StreamWriter outputStream, string key, Int64 value, string tab = "", bool isFirst = false, bool lineBreak = true, int padAmount = 0)
        {
            string entryStarter = isFirst ? "" : ",";
            string newLine = lineBreak ? "\n" : "";

            string paddedValue = value.ToString().PadLeft(padAmount);

            string output = $"{entryStarter}{newLine}{tab}\"{key}\": {paddedValue}";
            outputStream.Write(output);
        }

        static public void writeJson(StreamWriter outputStream, string key, Single value, string tab = "", bool isFirst = false, bool lineBreak = true, int padAmount = 0)
        {
            string entryStarter = isFirst ? "" : ",";
            string newLine = lineBreak ? "\n" : "";

            string paddedValue = value.ToString("0.0000").PadLeft(padAmount);

            string output = $"{entryStarter}{newLine}{tab}\"{key}\": {paddedValue}";
            outputStream.Write(output);
        }

        static public void writeJson(StreamWriter outputStream, string key, Double value, string tab = "", bool isFirst = false, bool lineBreak = true, int padAmount = 0)
        {
            string entryStarter = isFirst ? "" : ",";
            string newLine = lineBreak ? "\n" : "";

            string paddedValue = value.ToString("0.00000000").PadLeft(padAmount);

            string output = $"{entryStarter}{newLine}{tab}\"{key}\": {paddedValue}";
            outputStream.Write(output);
        }

        static public string removeWcidNameRedundancy(string className, string name)
        {
            string result = "";
            if (className != "" && name != "")
            {
                if (className.ToLower() != name.ToLower())
                    result = $"{name}({className})";
                else
                    result = name;
            }
            else if (className == "" && name != "")
                result = name;
            else if (className != "" && name == "")
                result = className;
            else
                result = "";
            return result;
        }

        static int getRandomNumberWithFavoredValue(int minInclusive, int maxInclusive, double favorValue, double favorStrength, double favorModifier = 0)
        {
            int numValues = (maxInclusive - minInclusive) + 1;
            float maxWeight = (numValues) * 1000;

            IntRange[] range = new IntRange[numValues];

            int value = minInclusive;
            for (int i = 0; i < numValues; i++)
            {
                range[i].Min = value;
                range[i].Max = value;
                range[i].Weight = maxWeight / (float)Math.Pow(1 + ((Math.Pow(favorStrength, 2) / numValues)), Math.Abs(favorValue - value));
                value++;
            }

            return RandomRange.Range(RandomUtil.Standard, range);
        }

        public static int getRandomNumberExclusive(int maxExclusive)
        {
            return getRandomNumber(0, maxExclusive - 1, eRandomFormula.equalDistribution, 0);
        }

        public static int getRandomNumberExclusive(int maxExclusive, eRandomFormula formula, double favorStrength, double favorModifier = 0)
        {
            return getRandomNumber(0, maxExclusive - 1, formula, 0, favorStrength, favorModifier);
        }

        public static int getRandomNumber(int maxInclusive)
        {
            return getRandomNumber(0, maxInclusive, eRandomFormula.equalDistribution, 0);
        }

        public static int getRandomNumber(int maxInclusive, eRandomFormula formula, double favorStrength, double favorModifier = 0)
        {
            return getRandomNumber(0, maxInclusive, formula, 0, favorStrength, favorModifier);
        }

        public static int getRandomNumberExclusive(int maxExclusive, eRandomFormula formula, double favorValue, double favorStrength, double favorModifier = 0)
        {
            return getRandomNumber(0, maxExclusive - 1, formula, favorValue, favorStrength, favorModifier);
        }

        public static int getRandomNumber(int minInclusive, int maxInclusive)
        {
            return getRandomNumber(minInclusive, maxInclusive, eRandomFormula.equalDistribution, 0);
        }

        public static List<int> getRandomNumbersNoRepeat(int amount, int minInclusive, int maxInclusive)
        {
            List<int> numbers = new List<int>();
            for(int i = 0; i < amount; i++)
            {
                numbers.Add(getRandomNumberNoRepeat(minInclusive, maxInclusive, numbers));
            }
            return numbers;
        }

        public static int getRandomNumberNoRepeat(int minInclusive, int maxInclusive, List<int> notThese, int maxTries = 10)
        {
            int potentialValue = getRandomNumber(minInclusive, maxInclusive, eRandomFormula.equalDistribution, 0);
            for (int i = 0; i < maxTries; i++)
            {
                potentialValue = getRandomNumber(minInclusive, maxInclusive, eRandomFormula.equalDistribution, 0);
                if (!notThese.Contains(potentialValue))
                    break;
            }
            return potentialValue;
        }

        public static int getRandomNumber(int minInclusive, int maxInclusive, eRandomFormula formula, double favorStrength, double favorModifier = 0)
        {
            return getRandomNumber(minInclusive, maxInclusive, formula, 0, favorStrength, favorModifier);
        }

        public static int getRandomNumber(int minInclusive, int maxInclusive, eRandomFormula formula, double favorValue, double favorStrength, double favorModifier = 0)
        {
            int numbersAmount = maxInclusive - minInclusive;
            switch (formula)
            {
                case eRandomFormula.favorSpecificValue:
                    {
                        favorValue = favorValue + (numbersAmount * favorModifier);
                        favorValue = Math.Min(favorValue, maxInclusive);
                        favorValue = Math.Max(favorValue, minInclusive);
                        return getRandomNumberWithFavoredValue(minInclusive, maxInclusive, favorValue, favorStrength);
                    }
                case eRandomFormula.favorLow:
                    {
                        favorValue = minInclusive + (numbersAmount * favorModifier);
                        favorValue = Math.Min(favorValue, maxInclusive);
                        favorValue = Math.Max(favorValue, minInclusive);
                        return getRandomNumberWithFavoredValue(minInclusive, maxInclusive, favorValue, favorStrength);
                    }
                case eRandomFormula.favorMid:
                    {
                        int midValue = (int)Math.Round(((double)(maxInclusive - minInclusive) / 2)) + minInclusive;
                        favorValue = midValue + (numbersAmount * favorModifier);
                        favorValue = Math.Min(favorValue, maxInclusive);
                        favorValue = Math.Max(favorValue, minInclusive);
                        return getRandomNumberWithFavoredValue(minInclusive, maxInclusive, favorValue, favorStrength);
                    }
                case eRandomFormula.favorHigh:
                    {
                        favorValue = maxInclusive - (numbersAmount * favorModifier);
                        favorValue = Math.Min(favorValue, maxInclusive);
                        favorValue = Math.Max(favorValue, minInclusive);
                        return getRandomNumberWithFavoredValue(minInclusive, maxInclusive, favorValue, favorStrength);
                    }
                default:
                case eRandomFormula.equalDistribution:
                    {
                        return RandomUtil.Standard.Next(minInclusive, maxInclusive + 1);
                    }
            }
        }

        public static double getRandomDouble(double maxInclusive)
        {
            return getRandomDouble(0, maxInclusive, eRandomFormula.equalDistribution, 0);
        }

        public static double getRandomDouble(double maxInclusive, eRandomFormula formula, double favorStrength, double favorModifier = 0)
        {
            return getRandomDouble(0, maxInclusive, formula, favorStrength, favorModifier);
        }

        public static double getRandomDouble(double minInclusive, double maxInclusive)
        {
            return getRandomDouble(minInclusive, maxInclusive, eRandomFormula.equalDistribution, 0);
        }

        public static double getRandomDouble(double minInclusive, double maxInclusive, eRandomFormula formula, double favorStrength, double favorModifier = 0)
        {
            double decimalPlaces = 1000;
            int minInt = (int)Math.Round(minInclusive * decimalPlaces);
            int maxInt = (int)Math.Round(maxInclusive * decimalPlaces);

            int randomInt = getRandomNumber(minInt, maxInt, formula, favorStrength, favorModifier);
            double returnValue = randomInt / decimalPlaces;

            returnValue = Math.Min(returnValue, maxInclusive);
            returnValue = Math.Max(returnValue, minInclusive);

            return returnValue;
        }

        public static void copyDirectory(string sourceDirName, string destDirName, bool copySubDirs, bool replaceFiles)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, replaceFiles);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    copyDirectory(subdir.FullName, temppath, copySubDirs, replaceFiles);
                }
            }
        }

        public static void writeToByteArray(int source, byte[] destination, int offset)
        {
            if (destination == null)
                throw new ArgumentException("Destination array cannot be null");

            // check if there is enough space for all the 4 bytes we will copy
            if (destination.Length < offset + sizeof(int))
                throw new ArgumentException("Not enough room in the destination array");

            for (int i = 0; i < sizeof(int); i++)
            {
                destination[offset + i] = (byte)(source >> (8 * i));
            }
        }

        public static void writeToByteArray(uint source, byte[] destination, int offset)
        {
            if (destination == null)
                throw new ArgumentException("Destination array cannot be null");

            // check if there is enough space for all the 4 bytes we will copy
            if (destination.Length < offset + sizeof(uint))
                throw new ArgumentException("Not enough room in the destination array");

            for (int i = 0; i < sizeof(int); i++)
            {
                destination[offset + i] = (byte)(source >> (8 * i));
            }
        }
    }
}