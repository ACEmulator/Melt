using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public struct sClothingTableObjectEffect
    {
        public uint index;
        public uint modelId;
        public List<sClothingTableTextureEffect> textureEffects;
    }

    public struct sClothingTableTextureEffect
    {
        public uint oldTexture;
        public uint newTexture;
    }

    public struct sClothingTableBaseEffect
    {
        public uint setupModel;
        public List<sClothingTableObjectEffect> objectEffects;
    }

    public struct sClothingTableSubPaletteEffect
    {
        public uint icon;
        public List<sClothingTableSubPalette> subPalettes;
    }

    public struct sClothingTableSubPalette
    {
        public List<sClothingTableSubPaletteRange> ranges;
        public uint paletteSet;
    }

    public struct sClothingTableSubPaletteRange
    {
        public uint offset;
        public uint numColors;
    }

    public struct sClothingTable
    {
        public uint id;
        public Dictionary<uint, sClothingTableBaseEffect> baseEffects;
        public Dictionary<uint, sClothingTableSubPaletteEffect> subPaletteEffects;
    }

    public class cClothingTableManager
    {
        public Dictionary<uint, sClothingTable> clothingTables = new Dictionary<uint, sClothingTable>();

        public void loadClothingTablesFromRaw(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Unable to open {0}", path);
                return;
            }

            string[] fileEntries = Directory.GetFiles(path, "*.bin", SearchOption.AllDirectories);

            Console.WriteLine("Reading clothing tables from raw files...");
            foreach (string filename in fileEntries)
            {
                StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
                if (reader == null)
                {
                    Console.WriteLine("Unable to open {0}", filename);
                    continue;
                }
                loadClothingTable(reader);
                reader.Close();
            }
            Console.WriteLine("Read {0} clothing tables.", clothingTables.Count);
        }

        void loadClothingTable(StreamReader inputFile)
        {
            byte[] buffer = new byte[1024];
            sClothingTable newTable = new sClothingTable();
            newTable.baseEffects = new Dictionary<uint, sClothingTableBaseEffect>();
            newTable.subPaletteEffects = new Dictionary<uint, sClothingTableSubPaletteEffect>();

            newTable.id = Utils.ReadUInt32(buffer, inputFile);

            UInt16 clothingEffectsCount = Utils.ReadUInt16(buffer, inputFile);
            UInt16 unknown1 = Utils.ReadUInt16(buffer, inputFile);
            for (uint i = 0; i < clothingEffectsCount; i++)
            {
                sClothingTableBaseEffect baseEffect = new sClothingTableBaseEffect();
                baseEffect.setupModel = Utils.ReadUInt32(buffer, inputFile);

                baseEffect.objectEffects = new List<sClothingTableObjectEffect>();
                int objectEffectsCount = Utils.ReadInt32(buffer, inputFile);
                for (int j = 0; j < objectEffectsCount; j++)
                {
                    sClothingTableObjectEffect objectEffect = new sClothingTableObjectEffect();
                    objectEffect.index = Utils.ReadUInt32(buffer, inputFile);
                    objectEffect.modelId = Utils.ReadUInt32(buffer, inputFile);

                    objectEffect.textureEffects = new List<sClothingTableTextureEffect>();
                    uint textureEffectsCount = Utils.ReadUInt32(buffer, inputFile);
                    for (uint k = 0; k < textureEffectsCount; k++)
                    {
                        sClothingTableTextureEffect textureEffect = new sClothingTableTextureEffect();
                        textureEffect.oldTexture = Utils.ReadUInt32(buffer, inputFile);
                        textureEffect.newTexture = Utils.ReadUInt32(buffer, inputFile);
                        objectEffect.textureEffects.Add(textureEffect);
                    }
                    baseEffect.objectEffects.Add(objectEffect);
                }
                newTable.baseEffects.Add(baseEffect.setupModel, baseEffect);
            }

            ushort palleteEffectCount = Utils.ReadUInt16(buffer, inputFile);
            for (uint i = 0; i < palleteEffectCount; i++)
            {
                Utils.Align(inputFile);
                sClothingTableSubPaletteEffect subPaletteEffect = new sClothingTableSubPaletteEffect();
                uint subPaletteId = Utils.ReadUInt32(buffer, inputFile);
                subPaletteEffect.icon = Utils.ReadUInt32(buffer, inputFile);

                subPaletteEffect.subPalettes = new List<sClothingTableSubPalette>();
                uint palettesCount = Utils.ReadUInt32(buffer, inputFile);
                for (uint j = 0; j < palettesCount; j++)
                {
                    sClothingTableSubPalette subPalette = new sClothingTableSubPalette();

                    subPalette.ranges = new List<sClothingTableSubPaletteRange>();
                    uint length = Utils.ReadUInt32(buffer, inputFile);
                    for (uint k = 0; k < length; k++)
                    {
                        sClothingTableSubPaletteRange range = new sClothingTableSubPaletteRange();
                        range.offset = Utils.ReadUInt32(buffer, inputFile);
                        range.numColors = Utils.ReadUInt32(buffer, inputFile);
                        subPalette.ranges.Add(range);
                    }
                    subPalette.paletteSet = Utils.ReadUInt32(buffer, inputFile);
                    subPaletteEffect.subPalettes.Add(subPalette);
                }
                newTable.subPaletteEffects.Add(subPaletteId, subPaletteEffect);
            }
            clothingTables.Add(newTable.id, newTable);
        }

        public uint getIcon(uint tableId, uint paletteEffectId)
        {
            sClothingTable table;
            if (clothingTables.TryGetValue(tableId, out table))
            {
                if (table.subPaletteEffects.ContainsKey(paletteEffectId))
                    return (table.subPaletteEffects[paletteEffectId].icon);
            }
            return 0;
        }
    }
}