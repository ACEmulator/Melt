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
    public class cCellPortal
    {
        public ushort Bitfield;
        public ushort EnvironmentId;
        public ushort OtherCellId;
        public ushort OtherPortalId;

        //public bool ExactMatch => (Bitfield & 1) != 0;
        //public bool PortalSide => (Bitfield & 2) == 0;

        public cCellPortal(byte[] buffer, StreamReader inputFile)
        {
            Bitfield = Utils.ReadUInt16(buffer, inputFile);
            EnvironmentId = Utils.ReadUInt16(buffer, inputFile);
            OtherCellId = Utils.ReadUInt16(buffer, inputFile);
            OtherPortalId = Utils.ReadUInt16(buffer, inputFile);
        }

        public void writeToDat(StreamWriter outputFile)
        {
            Utils.writeUInt16(Bitfield, outputFile);
            Utils.writeUInt16(EnvironmentId, outputFile);
            Utils.writeUInt16(OtherCellId, outputFile);
            Utils.writeUInt16(OtherPortalId, outputFile);
        }
    }

    public class cEnvCell
    {
        public uint Id;
        public UInt32 Bitfield { get; private set; }
        // 0x08000000 surfaces (which contains degrade/quality info to reference the specific 0x06000000 graphics)
        public List<ushort> Textures;
        // the 0x0D000000 model of the pre-fab dungeon block
        public ushort EnvironmentId;
        public ushort StructId;
        public sFrame Position;
        public List<cCellPortal> Portals;
        public List<ushort> Cells;
        public List<cStab> Stabs;
        public uint RestrictionObj;

        public bool SeenOutside => (Bitfield & 1) != 0;

        public cEnvCell(byte[] buffer, StreamReader inputFile, eDatFormat format, bool translateTextureIds = true)
        {
            if (format == eDatFormat.ToD)
            {
                Id = Utils.ReadUInt32(buffer, inputFile);

                Bitfield = Utils.ReadUInt32(buffer, inputFile);

                Utils.ReadUInt32(buffer, inputFile); //repeat id
            }
            else if(format == eDatFormat.retail)
            {
                Bitfield = Utils.ReadUInt32(buffer, inputFile);
                Id = Utils.ReadUInt32(buffer, inputFile);
            }

            byte numSurfaces = Utils.ReadByte(buffer, inputFile);
            byte numPortals = Utils.ReadByte(buffer, inputFile);    // Note that "portal" in this context does not refer to the swirly pink/purple thing, its basically connecting cells
            ushort numCells = Utils.ReadUInt16(buffer, inputFile);  // I believe this is what cells can be seen from this one. So the engine knows what else it needs to load/draw.

            Textures = new List<ushort>();
            // Read what surfaces are used in this cell
            for (uint i = 0; i < numSurfaces; i++)
            {
                ushort texture = Utils.ReadUInt16(buffer, inputFile);

                if (translateTextureIds)
                {
                    if (format == eDatFormat.ToD)
                        Textures.Add(texture);
                    else
                        Textures.Add(validPortalDatEntries.translateTextureId(texture));
                }
                else
                    Textures.Add(texture);
            }

            if (format == eDatFormat.retail)
                Utils.Align(inputFile);

            EnvironmentId = Utils.ReadUInt16(buffer, inputFile);

            StructId = Utils.ReadUInt16(buffer, inputFile);

            Position = new sFrame(buffer, inputFile);

            Portals = new List<cCellPortal>();
            for (int i = 0; i < numPortals; i++)
            {
                cCellPortal newPortal = new cCellPortal(buffer, inputFile);
                Portals.Add(newPortal);
            }

            if(format == eDatFormat.retail)
                Utils.Align(inputFile);

            Cells = new List<ushort>();
            for (uint i = 0; i < numCells; i++)
                Cells.Add(Utils.ReadUInt16(buffer, inputFile));

            if (format == eDatFormat.retail)
                Utils.Align(inputFile);

            Stabs = new List<cStab>();
            if ((Bitfield & 2) != 0)
            {
                int numStabs = Utils.ReadInt32(buffer, inputFile);

                for (int i = 0; i < numStabs; i++)
                {
                    //cStab Size is 32
                    cStab newStab = new cStab(buffer, inputFile);
                    Stabs.Add(newStab);
                }
            }

            if (format == eDatFormat.retail)
                Utils.Align(inputFile);

            if ((Bitfield & 8) != 0)
                RestrictionObj = Utils.ReadUInt32(buffer, inputFile);

            if (inputFile.BaseStream.Position != inputFile.BaseStream.Length)
                throw new Exception();
        }

        public void writeToDat(StreamWriter outputFile)
        {
            Utils.writeUInt32(Id, outputFile);
            Utils.writeUInt32(Bitfield, outputFile);
            Utils.writeUInt32(Id, outputFile);

            Utils.writeByte((byte)Textures.Count, outputFile);
            Utils.writeByte((byte)Portals.Count, outputFile);
            Utils.writeUInt16((ushort)Cells.Count, outputFile);

            foreach (ushort texture in Textures)
            {
                Utils.writeUInt16(texture, outputFile);
            }

            Utils.writeUInt16(EnvironmentId, outputFile);
            Utils.writeUInt16(StructId, outputFile);
            Position.writeRaw(outputFile);

            foreach (cCellPortal cellPortal in Portals)
            {
                cellPortal.writeToDat(outputFile);
            }

            foreach (ushort light in Cells)
            {
                Utils.writeUInt16(light, outputFile);
            }

            if ((Bitfield & 2) != 0)
            {
                Utils.writeInt32(Stabs.Count, outputFile);
                foreach (cStab stab in Stabs)
                {
                    stab.writeToDat(outputFile);
                }
            }

            if ((Bitfield & 8) != 0)
                Utils.writeUInt32(RestrictionObj, outputFile);
        }
    }
}