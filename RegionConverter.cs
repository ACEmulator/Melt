using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Melt
{
    class RegionConverter
    {
        public class RegionMisc
        {
            public uint Version { get; set; }
            public uint GameMapID { get; set; }
            public uint AutotestMapId { get; set; }
            public uint AutotestMapSize { get; set; }
            public uint ClearCellId { get; set; }
            public uint ClearMonsterId { get; set; }

            public static RegionMisc Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                RegionMisc obj = new RegionMisc();

                obj.Version = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.GameMapID = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.AutotestMapId = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.AutotestMapSize = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.ClearCellId = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.ClearMonsterId = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);

                return obj;
            }
        }

        public class TerrainTex
        {
            public uint TexGID { get; set; }
            public uint TexTiling { get; set; }
            public uint MaxVertBright { get; set; }
            public uint MinVertBright { get; set; }
            public uint MaxVertSaturate { get; set; }
            public uint MinVertSaturate { get; set; }
            public uint MaxVertHue { get; set; }
            public uint MinVertHue { get; set; }
            public uint DetailTexTiling { get; set; }
            public uint DetailTexGID { get; set; }

            public static TerrainTex Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                TerrainTex obj = new TerrainTex();

                obj.TexGID = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.TexTiling = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.MaxVertBright = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.MinVertBright = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.MaxVertSaturate = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.MinVertSaturate = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.MaxVertHue = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.MinVertHue = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.DetailTexTiling = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.DetailTexGID = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);

                return obj;
            }
            public void Write(StreamWriter outputData)
            {
                outputData.BaseStream.Write(BitConverter.GetBytes(TexGID), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(TexTiling), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(MaxVertBright), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(MinVertBright), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(MaxVertSaturate), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(MinVertSaturate), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(MaxVertHue), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(MinVertHue), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(DetailTexTiling), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(DetailTexGID), 0, 4);
            }
        }
        public class TMTerrainDesc
        {
            public uint terrainType { get; set; }
            public TerrainTex terrainTex { get; set; }

            public static TMTerrainDesc Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                TMTerrainDesc obj = new TMTerrainDesc();

                obj.terrainType = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.terrainTex = TerrainTex.Read(buffer, data, outputData, write);

                return obj;
            }

            public void Write(StreamWriter outputData)
            {
                outputData.BaseStream.Write(BitConverter.GetBytes(terrainType), 0, 4);
                terrainTex.Write(outputData);
            }
        }

        public class RoadAlphaMap
        {
            public uint RCode { get; set; }
            public uint RoadTexGID { get; set; }

            public static RoadAlphaMap Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                RoadAlphaMap obj = new RoadAlphaMap();
                obj.RCode = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.RoadTexGID = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                return obj;
            }
        }

        public class TerrainAlphaMap
        {
            public uint TCode { get; set; }
            public uint TexGID { get; set; }

            public static TerrainAlphaMap Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                TerrainAlphaMap obj = new TerrainAlphaMap();
                obj.TCode = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.TexGID = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                return obj;
            }
        }

        public class TexMerge
        {
            public uint BaseTexSize { get; set; }
            public List<TerrainAlphaMap> CornerTerrainMaps { get; set; }
            public List<TerrainAlphaMap> SideTerrainMaps { get; set; }
            public List<RoadAlphaMap> RoadMaps { get; set; }
            public List<TMTerrainDesc> TerrainDescription { get; set; }

            public static TexMerge Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                TexMerge obj = new TexMerge();

                obj.CornerTerrainMaps = new List<TerrainAlphaMap>();
                obj.SideTerrainMaps = new List<TerrainAlphaMap>();
                obj.RoadMaps = new List<RoadAlphaMap>();
                obj.TerrainDescription = new List<TMTerrainDesc>();

                //obj.BaseTexSize = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.BaseTexSize = Utils.ReadAndWriteUInt32(buffer, data, outputData, false);
                outputData.BaseStream.Write(BitConverter.GetBytes((uint)256), 0, 4);

                uint num_corner_terrain_maps = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                for (uint i = 0; i < num_corner_terrain_maps; i++)
                    obj.CornerTerrainMaps.Add(TerrainAlphaMap.Read(buffer, data, outputData, write));

                uint num_side_terrain_maps = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                for (uint i = 0; i < num_side_terrain_maps; i++)
                    obj.SideTerrainMaps.Add(TerrainAlphaMap.Read(buffer, data, outputData, write));

                uint num_road_maps = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                for (uint i = 0; i < num_road_maps; i++)
                    obj.RoadMaps.Add(RoadAlphaMap.Read(buffer, data, outputData, write));

                uint num_terrain_desc = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);

                for (uint i = 0; i < num_terrain_desc; i++)
                    obj.TerrainDescription.Add(TMTerrainDesc.Read(buffer, data, outputData, write));

                //TMTerrainDesc BarrenRock = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc Grassland = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc Ice = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc LushGrass = TMTerrainDesc.Read(buffer, data, outputData, false);

                //TMTerrainDesc MarshSparseSwamp = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc MudRichDirt = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc ObsidianPlain = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc PackedDirt = TMTerrainDesc.Read(buffer, data, outputData, false);

                //TMTerrainDesc PatchyDirt = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc PatchyGrassland = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc SandYellow = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc SandGrey = TMTerrainDesc.Read(buffer, data, outputData, false);

                //TMTerrainDesc SandRockStrewn = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc SedimentaryRock = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc SemiBarrenRock = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc Snow = TMTerrainDesc.Read(buffer, data, outputData, false);

                //TMTerrainDesc WaterRunning = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc WaterStandingFresh = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc WaterShallowSea = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc WaterShallowStillSea = TMTerrainDesc.Read(buffer, data, outputData, false);

                //TMTerrainDesc WaterDeepSea = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc Forestfloor = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc FauxWaterRunning = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc SeaSlime = TMTerrainDesc.Read(buffer, data, outputData, false);

                //TMTerrainDesc Argila = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc Volcano1 = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc Volcano2 = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc BlueIce = TMTerrainDesc.Read(buffer, data, outputData, false);

                //TMTerrainDesc Moss = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc DarkMoss = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc olthoi = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc DesolateLands = TMTerrainDesc.Read(buffer, data, outputData, false);
                //TMTerrainDesc Road = TMTerrainDesc.Read(buffer, data, outputData, false);

                //BarrenRock.terrainTex = Snow.terrainTex;
                //Grassland.terrainTex.TexGID = 0x0500146B;
                ////LushGrass.terrainTex = Snow.terrainTex;
                ////PatchyDirt.terrainTex = Snow.terrainTex;
                //PatchyGrassland.terrainTex = Snow.terrainTex;
                ////Forestfloor.terrainTex = Ice.terrainTex;
                //WaterRunning.terrainTex = BlueIce.terrainTex;
                //WaterStandingFresh.terrainTex = BlueIce.terrainTex;

                //BarrenRock.Write(outputData);
                //Grassland.Write(outputData);
                //Ice.Write(outputData);
                //LushGrass.Write(outputData);

                //MarshSparseSwamp.Write(outputData);
                //MudRichDirt.Write(outputData);
                //ObsidianPlain.Write(outputData);
                //PackedDirt.Write(outputData);

                //PatchyDirt.Write(outputData);
                //PatchyGrassland.Write(outputData);
                //SandYellow.Write(outputData);
                //SandGrey.Write(outputData);

                //SandRockStrewn.Write(outputData);
                //SedimentaryRock.Write(outputData);
                //SemiBarrenRock.Write(outputData);
                //Snow.Write(outputData);

                //WaterRunning.Write(outputData);
                //WaterStandingFresh.Write(outputData);
                //WaterShallowSea.Write(outputData);
                //WaterShallowStillSea.Write(outputData);

                //WaterDeepSea.Write(outputData);
                //Forestfloor.Write(outputData);
                //FauxWaterRunning.Write(outputData);
                //SeaSlime.Write(outputData);

                //Argila.Write(outputData);
                //Volcano1.Write(outputData);
                //Volcano2.Write(outputData);
                //BlueIce.Write(outputData);

                //Moss.Write(outputData);
                //DarkMoss.Write(outputData);
                //olthoi.Write(outputData);
                //DesolateLands.Write(outputData);
                //Road.Write(outputData);

                return obj;
            }
        }

        public class LandSurf
        {
            public uint HasPalShift { get; set; }
            public TexMerge texMerge { get; set; }

            public static LandSurf Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                LandSurf obj = new LandSurf();

                obj.HasPalShift = Utils.ReadAndWriteUInt32(buffer, data, outputData, write); // This is always 0

                if (obj.HasPalShift == 1)
                {
                    // PalShift.Read would go here, if it ever actually existed...which it doesn't.
                }
                else
                    obj.texMerge = TexMerge.Read(buffer, data, outputData, write);

                return obj;
            }
        }

        public class TerrainType
        {
            public string TerrainName { get; set; }
            public uint TerrainColor { get; set; }
            public List<uint> SceneTypes { get; set; }

            public static TerrainType Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                TerrainType obj = new TerrainType();

                obj.TerrainName = Utils.ReadAndWriteString(buffer, data, outputData, write);

                obj.TerrainColor = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);

                obj.SceneTypes = new List<uint>();
                uint num_stypes = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                for (uint i = 0; i < num_stypes; i++)
                    obj.SceneTypes.Add(Utils.ReadAndWriteUInt32(buffer, data, outputData, write));

                return obj;
            }

            public void Write(StreamWriter outputData)
            {
                byte[] buffer = new byte[1024];
                outputData.BaseStream.Write(BitConverter.GetBytes((short)TerrainName.Length), 0, 2);
                Utils.convertStringToByteArray(TerrainName, ref buffer, 0, TerrainName.Length);
                int startIndex = (int)outputData.BaseStream.Position;
                int endIndex = (int)outputData.BaseStream.Position + TerrainName.Length + 2;
                int alignedIndex = Utils.Align4(endIndex - startIndex);
                int newIndex = startIndex + alignedIndex;
                int bytesNeededToReachAlignment = newIndex - endIndex;
                outputData.BaseStream.Write(buffer, 0, TerrainName.Length + bytesNeededToReachAlignment);

                outputData.BaseStream.Write(BitConverter.GetBytes(TerrainColor), 0, 4);
                outputData.BaseStream.Write(BitConverter.GetBytes(SceneTypes.Count), 0, 4);
                foreach (uint stype in SceneTypes)
                    outputData.BaseStream.Write(BitConverter.GetBytes(stype), 0, 4);
            }
        }

        public class TerrainDesc
        {
            public List<TerrainType> TerrainTypes { get; set; }
            public LandSurf LandSurfaces { get; set; }

            public static TerrainDesc Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                TerrainDesc obj = new TerrainDesc();

                uint num_terrain_types = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);

                obj.TerrainTypes = new List<TerrainType>();

                for (uint i = 0; i < num_terrain_types; i++)
                    obj.TerrainTypes.Add(TerrainType.Read(buffer, data, outputData, write));

                //TerrainType BarrenRock = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType Grassland = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType Ice = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType LushGrass = TerrainType.Read(buffer, data, outputData, false);

                //TerrainType MarshSparseSwamp = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType MudRichDirt = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType ObsidianPlain = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType PackedDirt = TerrainType.Read(buffer, data, outputData, false);

                //TerrainType PatchyDirt = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType PatchyGrassland = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType SandYellow = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType SandGrey = TerrainType.Read(buffer, data, outputData, false);

                //TerrainType SandRockStrewn = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType SedimentaryRock = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType SemiBarrenRock = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType Snow = TerrainType.Read(buffer, data, outputData, false);

                //TerrainType WaterRunning = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType WaterStandingFresh = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType WaterShallowSea = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType WaterShallowStillSea = TerrainType.Read(buffer, data, outputData, false);

                //TerrainType WaterDeepSea = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType Forestfloor = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType FauxWaterRunning = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType SeaSlime = TerrainType.Read(buffer, data, outputData, false);

                //TerrainType Argila = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType Volcano1 = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType Volcano2 = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType BlueIce = TerrainType.Read(buffer, data, outputData, false);

                //TerrainType Moss = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType DarkMoss = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType olthoi = TerrainType.Read(buffer, data, outputData, false);
                //TerrainType DesolateLands = TerrainType.Read(buffer, data, outputData, false);

                ////BarrenRock = Snow;
                ////Grassland = Snow;
                ////LushGrass = Snow;
                ////PatchyDirt = Snow;
                ////PatchyGrassland = Snow;
                ////Forestfloor = Snow;

                //BarrenRock.Write(outputData);
                //Grassland.Write(outputData);
                //Ice.Write(outputData);
                //LushGrass.Write(outputData);

                //MarshSparseSwamp.Write(outputData);
                //MudRichDirt.Write(outputData);
                //ObsidianPlain.Write(outputData);
                //PackedDirt.Write(outputData);

                //PatchyDirt.Write(outputData);
                //PatchyGrassland.Write(outputData);
                //SandYellow.Write(outputData);
                //SandGrey.Write(outputData);

                //SandRockStrewn.Write(outputData);
                //SedimentaryRock.Write(outputData);
                //SemiBarrenRock.Write(outputData);
                //Snow.Write(outputData);

                //WaterRunning.Write(outputData);
                //WaterStandingFresh.Write(outputData);
                //WaterShallowSea.Write(outputData);
                //WaterShallowStillSea.Write(outputData);

                //WaterDeepSea.Write(outputData);
                //Forestfloor.Write(outputData);
                //FauxWaterRunning.Write(outputData);
                //SeaSlime.Write(outputData);

                //Argila.Write(outputData);
                //Volcano1.Write(outputData);
                //Volcano2.Write(outputData);
                //BlueIce.Write(outputData);

                //Moss.Write(outputData);
                //DarkMoss.Write(outputData);
                //olthoi.Write(outputData);
                //DesolateLands.Write(outputData);

                obj.LandSurfaces = LandSurf.Read(buffer, data, outputData, write);

                return obj;
            }
        }

        public class SceneType
        {
            public List<uint> Scenes { get; set; }

            public static SceneType Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                SceneType obj = new SceneType();

                // Not sure what this is...
                uint unknown = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);

                uint num_scenes = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.Scenes = new List<uint>();
                for (uint i = 0; i < num_scenes; i++)
                    obj.Scenes.Add(Utils.ReadAndWriteUInt32(buffer, data, outputData, write));

                return obj;
            }
        }
        public class SceneDesc
        {
            public List<SceneType> SceneTypes { get; set; }

            public static SceneDesc Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                SceneDesc obj = new SceneDesc();

                uint num_scene_types = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.SceneTypes = new List<SceneType>();
                for (uint i = 0; i < num_scene_types; i++)
                    obj.SceneTypes.Add(SceneType.Read(buffer, data, outputData, write));

                return obj;
            }
        }
        public class AmbientSoundDesc
        {
            public uint SType { get; set; }
            public float Volume { get; set; }
            public float BaseChance { get; set; }
            public float MinRate { get; set; }
            public float MaxRate { get; set; }

            public static AmbientSoundDesc Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                AmbientSoundDesc obj = new AmbientSoundDesc();
                obj.SType = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.Volume = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.BaseChance = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.MinRate = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.MaxRate = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                return obj;
            }
        }
        public class AmbientSTBDesc
        {
            public uint STBId { get; set; }
            public List<AmbientSoundDesc> AmbientSounds { get; set; }

            public static AmbientSTBDesc Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                AmbientSTBDesc obj = new AmbientSTBDesc();
                obj.STBId = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);

                uint num_ambient_sounds = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.AmbientSounds = new List<AmbientSoundDesc>();
                for (uint i = 0; i < num_ambient_sounds; i++)
                    obj.AmbientSounds.Add(AmbientSoundDesc.Read(buffer, data, outputData, write));

                return obj;
            }
        }
        public class SoundDesc
        {
            public List<AmbientSTBDesc> STBDesc { get; set; }

            public static SoundDesc Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                SoundDesc obj = new SoundDesc();

                uint num_stb_desc = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.STBDesc = new List<AmbientSTBDesc>();
                for (uint i = 0; i < num_stb_desc; i++)
                    obj.STBDesc.Add(AmbientSTBDesc.Read(buffer, data, outputData, write));

                return obj;
            }
        }

        public class SkyObjectReplace
        {
            public uint ObjectIndex { get; set; }
            public uint GFXObjId { get; set; }
            public float Rotate { get; set; }
            public float Transparent { get; set; }
            public float Luminosity { get; set; }
            public float MaxBright { get; set; }

            public static SkyObjectReplace Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                SkyObjectReplace obj = new SkyObjectReplace();
                obj.ObjectIndex = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.GFXObjId = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.Rotate = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.Transparent = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.Luminosity = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.MaxBright = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                return obj;
            }
        }
        public class SkyTimeOfDay
        {
            public float Begin { get; set; }
            public float DirBright { get; set; }
            public float DirHeading { get; set; }
            public float DirPitch { get; set; }
            public uint DirColor { get; set; }

            public float AmbBright { get; set; }
            public uint AmbColor { get; set; }

            public float MinWorldFog { get; set; }
            public float MaxWorldFog { get; set; }
            public uint WorldFogColor { get; set; }
            public uint WorldFog { get; set; }

            public List<SkyObjectReplace> SkyObjReplace { get; set; }

            public static SkyTimeOfDay Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                SkyTimeOfDay obj = new SkyTimeOfDay();
                obj.Begin = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.DirBright = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.DirHeading = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.DirPitch = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.DirColor = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);

                obj.AmbBright = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.AmbColor = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);

                obj.MinWorldFog = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.MaxWorldFog = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.WorldFogColor = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.WorldFog = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);

                uint num_sky_obj_replace = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.SkyObjReplace = new List<SkyObjectReplace>();
                for (uint i = 0; i < num_sky_obj_replace; i++)
                    obj.SkyObjReplace.Add(SkyObjectReplace.Read(buffer, data, outputData, write));

                return obj;
            }
        }
        public class SkyObject
        {
            public float BeginTime { get; set; }
            public float EndTime { get; set; }
            public float BeginAngle { get; set; }
            public float EndAngle { get; set; }
            public float TexVelocityX { get; set; }
            public float TexVelocityY { get; set; }
            public float TexVelocityZ { get; set; }
            public uint DefaultGFXObjectId { get; set; }
            public uint DefaultPESObjectId { get; set; }
            public uint Properties { get; set; }

            public static SkyObject Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                SkyObject obj = new SkyObject();
                obj.BeginTime = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.EndTime = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.BeginAngle = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.EndAngle = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.TexVelocityX = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.TexVelocityY = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.TexVelocityZ = 0;
                obj.DefaultGFXObjectId = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.DefaultPESObjectId = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.Properties = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                return obj;
            }
        }

        public class DayGroup
        {
            public float ChanceOfOccur { get; set; }
            public string DayName { get; set; }
            public List<SkyObject> SkyObjects { get; set; }
            public List<SkyTimeOfDay> SkyTime { get; set; }

            public static DayGroup Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                DayGroup obj = new DayGroup();
                obj.ChanceOfOccur = Utils.ReadAndWriteSingle(buffer, data, outputData, write);
                obj.DayName = Utils.ReadAndWriteString(buffer, data, outputData, write);

                uint num_sky_objects = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.SkyObjects = new List<SkyObject>();
                for (uint i = 0; i < num_sky_objects; i++)
                    obj.SkyObjects.Add(SkyObject.Read(buffer, data, outputData, write));

                uint num_sky_times = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.SkyTime = new List<SkyTimeOfDay>();
                for (uint i = 0; i < num_sky_times; i++)
                    obj.SkyTime.Add(SkyTimeOfDay.Read(buffer, data, outputData, write));

                return obj;
            }
        }
        public class SkyDesc
        {
            public UInt64 TickSize { get; set; }
            public UInt64 LightTickSize { get; set; }
            public List<DayGroup> DayGroups { get; set; }

            public static SkyDesc Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                SkyDesc obj = new SkyDesc();
                obj.TickSize = Utils.ReadAndWriteUInt64(buffer, data, outputData, write);
                obj.LightTickSize = Utils.ReadAndWriteUInt64(buffer, data, outputData, write);

                uint numDayGroups = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.DayGroups = new List<DayGroup>();
                for (uint i = 0; i < numDayGroups; i++)
                    obj.DayGroups.Add(DayGroup.Read(buffer, data, outputData, write));

                return obj;
            }
        }

        public class LandDefs
        {
            public List<float> LandHeightTable { get; set; }

            public static LandDefs Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                LandDefs obj = new LandDefs();
                obj.LandHeightTable = new List<float>();
                for (int i = 0; i < 256; i++)
                {
                    obj.LandHeightTable.Add(Utils.ReadAndWriteSingle(buffer, data, outputData, write));
                }
                return obj;
            }
        }

        public class Season
        {
            public uint StartDate { get; set; }
            public string Name { get; set; }

            public static Season Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                Season obj = new Season();
                obj.StartDate = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.Name = Utils.ReadAndWriteString(buffer, data, outputData, write);
                return obj;
            }
        }

        public class TimeOfDay
        {
            public uint Start { get; set; }
            public uint IsNight { get; set; }
            public string Name { get; set; }

            public static TimeOfDay Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                TimeOfDay obj = new TimeOfDay();
                obj.Start = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.IsNight = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.Name = Utils.ReadAndWriteString(buffer, data, outputData, write);
                return obj;
            }
        }

        public class GameTime
        {
            public UInt64 ZeroTimeOfYear { get; set; }
            public uint ZeroYear { get; set; } // Year "0" is really "P.Y. 10" in the calendar.
            public uint DayLength { get; set; }
            public uint DaysPerYear { get; set; } // 360. Likely for easier math so each month is same length
            public string YearSpec { get; set; } // "P.Y."
            public List<TimeOfDay> TimesOfDay { get; set; }
            public List<string> DaysOfTheWeek { get; set; }
            public List<Season> Seasons { get; set; }

            public static GameTime Read(byte[] buffer, StreamReader data, StreamWriter outputData, bool write = true)
            {
                GameTime obj = new GameTime();
                obj.ZeroTimeOfYear = Utils.ReadAndWriteUInt64(buffer, data, outputData, write);
                obj.ZeroYear = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.DayLength = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.DaysPerYear = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.YearSpec = Utils.ReadAndWriteString(buffer, data, outputData, write);

                uint numTimesOfDay = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.TimesOfDay = new List<TimeOfDay>();
                for (uint i = 0; i < numTimesOfDay; i++)
                {
                    obj.TimesOfDay.Add(TimeOfDay.Read(buffer, data, outputData, write));
                }

                uint numDaysOfTheWeek = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.DaysOfTheWeek = new List<string>();
                for (uint i = 0; i < numDaysOfTheWeek; i++)
                {
                    obj.DaysOfTheWeek.Add(Utils.ReadAndWriteString(buffer, data, outputData, write));
                }

                uint numSeasons = Utils.ReadAndWriteUInt32(buffer, data, outputData, write);
                obj.Seasons = new List<Season>();
                for (uint i = 0; i < numSeasons; i++)
                {
                    obj.Seasons.Add(Season.Read(buffer, data, outputData, write));
                }

                return obj;
            }
        }

        static public void convert(string filename)
        {
            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            if (inputFile == null)
            {
                Console.WriteLine("Unable to open {0}", filename);
                return;
            }
            StreamWriter outputFile = new StreamWriter(new FileStream("./Region/13000000 - World Info - Winter.bin", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open 13000000 - World Info - Winter.bin");
                return;
            }

            Console.WriteLine("Converting region file to winter...");

            byte[] buffer = new byte[1024];

            uint fileHeader;
            uint loaded;
            uint timeStamp;
            string regionName;
            uint partsMask;
            uint unknown1;
            uint unknown2;
            uint unknown3;
            uint unknown4;
            uint unknown5;
            uint unknown6;
            uint unknown7;

            LandDefs landDef;
            GameTime gameTime;
            uint next;

            SkyDesc skyInfo;
            SoundDesc soundInfo;
            SceneDesc sceneInfo;
            TerrainDesc terrainInfo;
            RegionMisc regionMisc;

            fileHeader = Utils.ReadAndWriteUInt32(buffer, inputFile, outputFile);
            if (fileHeader != 0x13000000)
            {
                Console.WriteLine("Invalid header, aborting.");
                return;
            }

            loaded = Utils.ReadAndWriteUInt32(buffer, inputFile, outputFile);
            timeStamp = Utils.ReadAndWriteUInt32(buffer, inputFile, outputFile);
            regionName = Utils.ReadAndWriteString(buffer, inputFile, outputFile);
            partsMask = Utils.ReadAndWriteUInt32(buffer, inputFile, outputFile);

            unknown1 = Utils.ReadAndWriteUInt32(buffer, inputFile, outputFile);
            unknown2 = Utils.ReadAndWriteUInt32(buffer, inputFile, outputFile);
            unknown3 = Utils.ReadAndWriteUInt32(buffer, inputFile, outputFile);
            unknown4 = Utils.ReadAndWriteUInt32(buffer, inputFile, outputFile);
            unknown5 = Utils.ReadAndWriteUInt32(buffer, inputFile, outputFile);
            unknown6 = Utils.ReadAndWriteUInt32(buffer, inputFile, outputFile);
            unknown7 = Utils.ReadAndWriteUInt32(buffer, inputFile, outputFile);

            landDef = LandDefs.Read(buffer, inputFile, outputFile);
            gameTime = GameTime.Read(buffer, inputFile, outputFile);

            next = Utils.ReadAndWriteUInt32(buffer, inputFile, outputFile);

            if ((next & 0x10) > 0)
                skyInfo = SkyDesc.Read(buffer, inputFile, outputFile);

            if ((next & 0x01) > 0)
                soundInfo = SoundDesc.Read(buffer, inputFile, outputFile);

            if ((next & 0x02) > 0)
                sceneInfo = SceneDesc.Read(buffer, inputFile, outputFile);

            terrainInfo = TerrainDesc.Read(buffer, inputFile, outputFile);

            if ((next & 0x0200) > 0)
                regionMisc = RegionMisc.Read(buffer, inputFile, outputFile);

            //StreamWriter outputFile2 = new StreamWriter(new FileStream("./13000000 - Terrain Textures -ToD.txt", FileMode.Create, FileAccess.Write));
            //if (outputFile2 == null)
            //{
            //    Console.WriteLine("Unable to open 13000000 - Terrain Textures - ToD.txt");
            //    return;
            //}

            //for (int i = 0; i < terrainInfo.LandSurfaces.texMerge.CornerTerrainMaps.Count; i++)
            //{
            //    TerrainAlphaMap cornerTerrainMap = terrainInfo.LandSurfaces.texMerge.CornerTerrainMaps[i];

            //    outputFile2.WriteLine("{0}", cornerTerrainMap.TexGID.ToString("x8"));
            //    outputFile2.Flush();
            //}

            //for (int i = 0; i < terrainInfo.LandSurfaces.texMerge.SideTerrainMaps.Count; i++)
            //{
            //    TerrainAlphaMap sideTerrainMap = terrainInfo.LandSurfaces.texMerge.SideTerrainMaps[i];

            //    outputFile2.WriteLine("{0}", sideTerrainMap.TexGID.ToString("x8"));
            //    outputFile2.Flush();
            //}

            //for (int i = 0; i < terrainInfo.LandSurfaces.texMerge.RoadMaps.Count; i++)
            //{
            //    RoadAlphaMap roadMap = terrainInfo.LandSurfaces.texMerge.RoadMaps[i];

            //    outputFile2.WriteLine("{0}", roadMap.RoadTexGID.ToString("x8"));
            //    outputFile2.Flush();
            //}

            //for (int i = 0; i < terrainInfo.LandSurfaces.texMerge.TerrainDescription.Count; i++)//ignore first entry as it's a repeat of the second
            //{

            //    TMTerrainDesc desc = terrainInfo.LandSurfaces.texMerge.TerrainDescription[i];
            //    string terrainName = "Unknown";
            //    uint terrainColor = 0;
            //    if (i < terrainInfo.TerrainTypes.Count)
            //    {
            //        terrainName = terrainInfo.TerrainTypes[(int)desc.terrainType].TerrainName;
            //        terrainColor = terrainInfo.TerrainTypes[(int)desc.terrainType].TerrainColor;
            //    }
            //    else if (i == 32)
            //    {
            //        terrainName = "Road";
            //        terrainColor = 0;
            //    }

            //    terrainName = terrainName.PadLeft(20);

            //    outputFile2.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}", desc.terrainType, terrainName, desc.terrainTex.TexGID.ToString("x8"), desc.terrainTex.TexTiling, desc.terrainTex.DetailTexGID.ToString("x8"),
            //        desc.terrainTex.DetailTexTiling, desc.terrainTex.MaxVertBright, desc.terrainTex.MaxVertHue, desc.terrainTex.MaxVertSaturate, desc.terrainTex.MinVertBright,
            //        desc.terrainTex.MinVertHue, desc.terrainTex.MinVertSaturate);

            //    //outputFile2.WriteLine("TextureConverter.toBin(\"Landscape Texture Conversion/DM/Textures/Upscaled/xxx.png\", 0x{0}, 21);//{1}",desc.terrainTex.TexGID.ToString("x8"),terrainName);

            //    //outputFile2.WriteLine("writedat client_portal.dat -f {0}={0}.bin", desc.terrainTex.TexGID.ToString("x8"));
            //    outputFile2.Flush();
            //}
            //outputFile2.Close();

            inputFile.Close();

            outputFile.Flush();
            outputFile.Close();
            Console.WriteLine("Done");
        }
    }
}