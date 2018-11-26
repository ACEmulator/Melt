using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melt
{
    /// <summary>
    /// I'm not quite sure what a "Stab" is, but this is what the client calls these.
    /// It is an object and a corresponding position. 
    /// Note that since these are referenced by either a landblock or a cellblock, the corresponding Landblock and Cell should come from the parent.
    /// </summary>
    public class cStab
    {
        public uint id;
        public sFrame frame;

        public cStab(byte[] buffer, StreamReader inputFile)
        {
            id = Utils.ReadUInt32(buffer, inputFile);
            frame = new sFrame(buffer, inputFile);

            //if (!validPortalDatEntries.isValidStabEntry(id)) //this is not complete, there's some entries that can be converted and aren't in the entry. particles? something else?
            //    id = 0x0200026B;//turbine box
        }

        public void writeToDat(StreamWriter outputFile)
        {
            Utils.writeUInt32(id, outputFile);
            frame.writeRaw(outputFile);
        }
    }

    public class cCBldPortal
    {
        public ushort Flags;

        // Not sure what these do. They are both calculated from the flags.
        public bool ExactMatch => (Flags & 1) != 0;
        public bool PortalSide => (Flags & 2) == 0;

        // Basically the cells that connect both sides of the portal
        public ushort OtherCellId;
        public ushort OtherPortalId;

        /// <summary>
        /// List of cells used in this classure. (Or possibly just those visible through it.)
        /// </summary>
        public List<ushort> visibleCells;

        public cCBldPortal(byte[] buffer, StreamReader inputFile, eDatFormat format)
        {
            Flags = Utils.ReadUInt16(buffer, inputFile);

            OtherCellId = Utils.ReadUInt16(buffer, inputFile);
            OtherPortalId = Utils.ReadUInt16(buffer, inputFile);

            visibleCells = new List<ushort>();
            ushort visibleCellCount = Utils.ReadUInt16(buffer, inputFile);
            for (var i = 0; i < visibleCellCount; i++)
                visibleCells.Add(Utils.ReadUInt16(buffer, inputFile));

            Utils.Align(inputFile);
        }

        public void writeToDat(StreamWriter outputFile)
        {
            Utils.writeUInt16(Flags, outputFile);

            Utils.writeUInt16(OtherCellId, outputFile);
            Utils.writeUInt16(OtherPortalId, outputFile);

            Utils.writeUInt16((ushort)visibleCells.Count, outputFile);
            foreach(ushort value in visibleCells)
            {
                Utils.writeUInt16(value, outputFile);
            }

            Utils.Align(outputFile);
        }
    }

    public class cBuildInfo
    {
        /// <summary>
        /// 0x01 or 0x02 model of the building
        /// </summary>
        public uint ModelId;

        /// <summary>
        /// specific @loc of the model
        /// </summary>
        public sFrame Frame;

        /// <summary>
        /// unsure what this is used for
        /// </summary>
        public uint NumLeaves;

        /// <summary>
        /// portals are things like doors, windows, etc.
        /// </summary>
        public List<cCBldPortal> Portals;

        public cBuildInfo(byte[] buffer, StreamReader inputFile, eDatFormat format)
        {
            ModelId = Utils.ReadUInt32(buffer, inputFile);

            Frame = new sFrame(buffer, inputFile);

            NumLeaves = Utils.ReadUInt32(buffer, inputFile);

            int portalsCount = Utils.ReadInt32(buffer, inputFile);
            Portals = new List<cCBldPortal>();
            for (int i = 0; i < portalsCount; i++)
            {
                cCBldPortal newPortal = new cCBldPortal(buffer, inputFile, format);
                Portals.Add(newPortal);
            }
        }

        public void writeToDat(StreamWriter outputFile)
        {
            Utils.writeUInt32(ModelId, outputFile);
            Frame.writeRaw(outputFile);
            Utils.writeUInt32(NumLeaves, outputFile);
            Utils.writeInt32(Portals.Count, outputFile);
            foreach (cCBldPortal portal in Portals)
            {
                portal.writeToDat(outputFile);
            }
        }
    }

    /// <summary>
    /// This reads the extra items in a landblock from the client_cell.dat. This is mostly buildings, but other static/non-interactive objects like tables, lamps, are also included.
    /// CLandBlockInfo is a file designated xxyyFFFE, where xxyy is the landblock.
    /// <para />
    /// The fileId is CELL + 0xFFFE. e.g. a cell of 1234, the file index would be 0x1234FFFE.
    /// </summary>
    /// <remarks>
    /// Very special thanks again to David Simpson for his early work on reading the cell.dat. Even bigger thanks for his documentation of it!
    /// </remarks>
    public class cLandblockInfo
    {
        public uint Id;

        /// <summary>
        /// number of EnvCells in the landblock. This should match up to the unique items in the building stab lists.
        /// </summary>
        public uint NumCells;

        /// <summary>
        /// list of model numbers. 0x01 and 0x01 types and their specific locations
        /// </summary>
        public List<cStab> Objects;

        /// <summary>
        /// As best as I can tell, this only affects whether there is a restriction table or not
        /// </summary>
        public ushort buildingFlags;

        /// <summary>
        /// Buildings and other classures with interior locations in the landblock
        /// </summary>
        public List<cBuildInfo> Buildings;

        /// <summary>
        /// The specicic landblock/cell controlled by a specific guid that controls access (e.g. housing barrier)
        /// </summary>
        public Dictionary<uint, uint> RestrictionTables;
        public ushort totalObjects;
        public ushort bucketSize;

        public cLandblockInfo(byte[] buffer, StreamReader inputFile, eDatFormat format)
        {
            Id = Utils.ReadUInt32(buffer, inputFile);

            NumCells = Utils.ReadUInt32(buffer, inputFile);

            Objects = new List<cStab>();
            int objectsCount = Utils.ReadInt32(buffer, inputFile);
            for (int i = 0; i < objectsCount; i++)
            {
                cStab newObject = new cStab(buffer, inputFile);
                Objects.Add(newObject);
            }

            ushort numBuildings = Utils.ReadUInt16(buffer, inputFile);
            buildingFlags = Utils.ReadUInt16(buffer, inputFile);

            Buildings = new List<cBuildInfo>();
            for (int i = 0; i < numBuildings; i++)
            {
                cBuildInfo newBuilding = new cBuildInfo(buffer, inputFile, format);
                Buildings.Add(newBuilding);
            }

            if (format == eDatFormat.retail)
                Utils.Align(inputFile);

            RestrictionTables = new Dictionary<uint, uint>();
            if ((buildingFlags & 1) == 1)
            {
                totalObjects = Utils.ReadUInt16(buffer, inputFile);
                bucketSize = Utils.ReadUInt16(buffer, inputFile);

                for (int i = 0; i < totalObjects; i++)
                    RestrictionTables.Add(Utils.ReadUInt32(buffer, inputFile), Utils.ReadUInt32(buffer, inputFile));
            }

            if (format == eDatFormat.retail)
                Utils.Align(inputFile);

            if (inputFile.BaseStream.Position != inputFile.BaseStream.Length)
                throw new Exception();
        }

        public void writeToDat(StreamWriter outputFile)
        {
            Utils.writeUInt32(Id, outputFile);
            Utils.writeUInt32(NumCells, outputFile);

            Utils.writeInt32(Objects.Count, outputFile);
            foreach (cStab stab in Objects)
            {
                stab.writeToDat(outputFile);
            }

            Utils.writeUInt16((ushort)Buildings.Count, outputFile);
            Utils.writeUInt16(buildingFlags, outputFile);

            foreach (cBuildInfo building in Buildings)
            {
                building.writeToDat(outputFile);
            }

            if ((buildingFlags & 1) == 1)
            {
                Utils.writeUInt16(totalObjects, outputFile);
                Utils.writeUInt16(bucketSize, outputFile);

                foreach(KeyValuePair<uint, uint> entry in RestrictionTables)
                {
                    Utils.writeUInt32(entry.Key, outputFile);
                    Utils.writeUInt32(entry.Value, outputFile);
                }
            }
        }
    }
}