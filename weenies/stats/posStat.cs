using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Melt
{
    public class singleConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Single singleValue = (Single)value;

            if (BitConverter.ToUInt32(BitConverter.GetBytes(singleValue), 0) == 0x80000000)
                writer.WriteValue($"-0 (0x{BitConverter.ToString(BitConverter.GetBytes(singleValue)).Replace("-", string.Empty)})");
            else
                writer.WriteValue($"{singleValue.ToString()} (0x{BitConverter.ToString(BitConverter.GetBytes(singleValue)).Replace("-", string.Empty)})");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException();

            string stringValue = serializer.Deserialize<string>(reader);

            string[] splitString = stringValue.Split('(',')');

            Single singleValue = 0;
            if (stringValue == "-0")
                return -0f;
            else if (splitString.Length == 3)
            {
                UInt32 a = UInt32.Parse(splitString[1].Replace("0x", string.Empty), System.Globalization.NumberStyles.HexNumber);
                byte[] b = BitConverter.GetBytes(a);
                Array.Reverse(b);
                singleValue = BitConverter.ToSingle(b, 0);
            }
            else
                singleValue = Single.Parse(stringValue);
            return singleValue;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Single);
        }
    }

    public class doubleConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Double doubleValue = (Double)value;

            if (BitConverter.ToUInt64(BitConverter.GetBytes(doubleValue), 0) == 0x8000000000000000)
                writer.WriteValue($"-0 (0x{BitConverter.ToString(BitConverter.GetBytes(doubleValue)).Replace("-", string.Empty)})");
            else
                writer.WriteValue($"{doubleValue.ToString()} (0x{BitConverter.ToString(BitConverter.GetBytes(doubleValue)).Replace("-", string.Empty)})");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException();

            string stringValue = serializer.Deserialize<string>(reader);

            string[] splitString = stringValue.Split('(', ')');

            Double doubleValue = 0;
            if (stringValue == "-0")
                return -0d;
            else if (splitString.Length == 3)
            {
                UInt64 a = UInt64.Parse(splitString[1].Replace("0x", string.Empty), System.Globalization.NumberStyles.HexNumber);
                byte[] b = BitConverter.GetBytes(a);
                Array.Reverse(b);
                doubleValue = BitConverter.ToDouble(b, 0);
            }
            else
                doubleValue = Double.Parse(stringValue);
            return doubleValue;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Double);
        }
    }

    public struct sAngles
    {
        //[JsonConverter(typeof(singleConverter))]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public Single w;

        //[JsonConverter(typeof(singleConverter))]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public Single x;

        //[JsonConverter(typeof(singleConverter))]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public Single y;

        //[JsonConverter(typeof(singleConverter))]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public Single z;

        public sAngles(byte[] buffer, StreamReader inputFile)
        {
            w = Utils.ReadSingle(buffer, inputFile);
            x = Utils.ReadSingle(buffer, inputFile);
            y = Utils.ReadSingle(buffer, inputFile);
            z = Utils.ReadSingle(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeSingle(w, outputStream);
            Utils.writeSingle(x, outputStream);
            Utils.writeSingle(y, outputStream);
            Utils.writeSingle(z, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";

            outputStream.Write($"{entryStarter}\n{tab}\"angles\": {{");
            Utils.writeJson(outputStream, "w", w, "", true, false, 5);
            Utils.writeJson(outputStream, "x", x, " ", false, false, 5);
            Utils.writeJson(outputStream, "y", y, " ", false, false, 5);
            Utils.writeJson(outputStream, "z", z, " ", false, false, 5);
            outputStream.Write($"}}");
        }
    }

    public struct sOrigin
    {
        //[JsonConverter(typeof(singleConverter))]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public Single x;
        //[JsonConverter(typeof(singleConverter))]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public Single y;
        //[JsonConverter(typeof(singleConverter))]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public Single z;

        public sOrigin(byte[] buffer, StreamReader inputFile)
        {
            x = Utils.ReadSingle(buffer, inputFile);
            y = Utils.ReadSingle(buffer, inputFile);
            z = Utils.ReadSingle(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeSingle(x, outputStream);
            Utils.writeSingle(y, outputStream);
            Utils.writeSingle(z, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";

            outputStream.Write($"{entryStarter}\n{tab}\"origin\": {{");
            Utils.writeJson(outputStream, "x", x, "", true, false, 5);
            Utils.writeJson(outputStream, "y", z, " ", false, false, 5);
            Utils.writeJson(outputStream, "z", z, " ", false, false, 5);
            outputStream.Write($"}}");
        }
    }

    public struct sFrame
    {
        public sOrigin origin;
        public sAngles angles;

        public sFrame(byte[] buffer, StreamReader inputFile)
        {
            origin = new sOrigin(buffer, inputFile);
            angles = new sAngles(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            origin.writeRaw(outputStream);
            angles.writeRaw(outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string subEntriesTab = $"{tab}\t";

            outputStream.Write($"{entryStarter}\n{tab}\"frame\": {{");
            origin.writeJson(outputStream, subEntriesTab, true);
            angles.writeJson(outputStream, subEntriesTab, false);
            outputStream.Write("\n{0}}}", tab);
        }
    }

    public struct sPosition
    {
        public UInt32 objcell_id;
        public sFrame frame;

        public sPosition(byte[] buffer, StreamReader inputFile)
        {
            objcell_id = Utils.ReadUInt32(buffer, inputFile);
            frame = new sFrame(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeUInt32(objcell_id, outputStream);
            frame.writeRaw(outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string subEntriesTab = $"{tab}\t";

            outputStream.Write($",\n{tab}\"mPosition\": {{");
            Utils.writeJson(outputStream, "objcell_id", objcell_id, subEntriesTab, true, true);
            frame.writeJson(outputStream, subEntriesTab, false);
            outputStream.Write($"\n{tab}}}");
        }
    }

    public struct sPosStat
    {
        public int key;
        public UInt32 objcell_id;
        public sFrame frame;

        public sPosStat(byte[] buffer, StreamReader inputFile)
        {
            key = Utils.ReadInt32(buffer, inputFile);
            objcell_id = Utils.ReadUInt32(buffer, inputFile);
            frame = new sFrame(buffer, inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32(key, outputStream);
            Utils.writeUInt32(objcell_id, outputStream);
            frame.writeRaw(outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string subEntriesTab = $"{tab}\t";
            string subSubEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, tab);
            Utils.writeJson(outputStream, "key", key, subEntriesTab, true, true);
            outputStream.Write($",\n{subEntriesTab}\"value\": {{");
            Utils.writeJson(outputStream, "objcell_id", objcell_id, subSubEntriesTab, true, true);
            frame.writeJson(outputStream, subSubEntriesTab, false);
            outputStream.Write($"\n{subEntriesTab}}}");
            outputStream.Write("\n{0}}}", tab);
        }
    }
}