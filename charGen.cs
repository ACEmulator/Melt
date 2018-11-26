using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Melt
{
    class CharGen
    {
        public class HeritageGroupCG
        {
            public string Name;
            public uint IconImage;
            public uint SetupID; // Basic character model
            public uint EnvironmentSetupID; // This is the background environment during Character Creation
            public uint AttributeCredits;
            public uint SkillCredits;
            public List<int> PrimaryStartAreas = new List<int>();
            public List<int> SecondaryStartAreas = new List<int>();
            public List<SkillCG> Skills = new List<SkillCG>();
            public List<TemplateCG> Templates = new List<TemplateCG>();
            public byte Unknown;
            public Dictionary<int, SexCG> Genders = new Dictionary<int, SexCG>();

            public HeritageGroupCG(byte[] buffer, StreamReader inputFile)
            {
                Name = Utils.ReadSerializedString(buffer, inputFile);
                IconImage = Utils.ReadUInt32(buffer, inputFile);
                SetupID = Utils.ReadUInt32(buffer, inputFile);
                EnvironmentSetupID = Utils.ReadUInt32(buffer, inputFile);
                AttributeCredits = Utils.ReadUInt32(buffer, inputFile);
                SkillCredits = Utils.ReadUInt32(buffer, inputFile);

                uint count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    PrimaryStartAreas.Add(Utils.ReadInt32(buffer, inputFile));

                count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    SecondaryStartAreas.Add(Utils.ReadInt32(buffer, inputFile));

                count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    Skills.Add(new SkillCG(buffer, inputFile));

                count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    Templates.Add(new TemplateCG(buffer, inputFile));

                Unknown = Utils.ReadByte(buffer, inputFile);

                count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                {
                    int key = Utils.ReadInt32(buffer, inputFile);
                    SexCG value = new SexCG(buffer, inputFile);
                    Genders.Add(key, value);
                }
            }

            public void writeRaw(StreamWriter outputStream)
            {
                Utils.writeSerializedString(Name, outputStream);
                Utils.writeUInt32(IconImage, outputStream);
                Utils.writeUInt32(SetupID, outputStream);
                Utils.writeUInt32(EnvironmentSetupID, outputStream);
                Utils.writeUInt32(AttributeCredits, outputStream);
                Utils.writeUInt32(SkillCredits, outputStream);

                Utils.writeCompressedUInt32((uint)PrimaryStartAreas.Count, outputStream);
                foreach (uint area in PrimaryStartAreas)
                    Utils.writeUInt32(area, outputStream);

                Utils.writeCompressedUInt32((uint)SecondaryStartAreas.Count, outputStream);
                foreach (uint area in SecondaryStartAreas)
                    Utils.writeUInt32(area, outputStream);

                Utils.writeCompressedUInt32((uint)Skills.Count, outputStream);
                foreach (SkillCG skill in Skills)
                    skill.writeRaw(outputStream);

                Utils.writeCompressedUInt32((uint)Templates.Count, outputStream);
                foreach (TemplateCG template in Templates)
                    template.writeRaw(outputStream);

                Utils.writeByte(Unknown, outputStream);

                Utils.writeCompressedUInt32((uint)Genders.Count, outputStream);
                foreach(KeyValuePair<int, SexCG> entry in Genders)
                {
                    Utils.writeInt32(entry.Key, outputStream);
                    entry.Value.writeRaw(outputStream);
                }
            }
        }

        public class TemplateCG
        {
            public string Name;
            public uint IconImage;
            public uint Title;
            public uint Strength;
            public uint Endurance;
            public uint Coordination;
            public uint Quickness;
            public uint Focus;
            public uint Self;
            public List<uint> NormalSkillsList = new List<uint>();
            public List<uint> PrimarySkillsList = new List<uint>();

            public TemplateCG(byte[] buffer, StreamReader inputFile)
            {
                Name = Utils.ReadSerializedString(buffer, inputFile);
                IconImage = Utils.ReadUInt32(buffer, inputFile);
                Title = Utils.ReadUInt32(buffer, inputFile);

                Strength = Utils.ReadUInt32(buffer, inputFile);
                Endurance = Utils.ReadUInt32(buffer, inputFile);
                Coordination = Utils.ReadUInt32(buffer, inputFile);
                Quickness = Utils.ReadUInt32(buffer, inputFile);
                Focus = Utils.ReadUInt32(buffer, inputFile);
                Self = Utils.ReadUInt32(buffer, inputFile);

                uint count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    NormalSkillsList.Add(Utils.ReadUInt32(buffer, inputFile));

                count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    PrimarySkillsList.Add(Utils.ReadUInt32(buffer, inputFile));
            }

            public void writeRaw(StreamWriter outputStream)
            {
                Utils.writeSerializedString(Name, outputStream);
                Utils.writeUInt32(IconImage, outputStream);
                Utils.writeUInt32(Title, outputStream);

                Utils.writeUInt32(Strength, outputStream);
                Utils.writeUInt32(Endurance, outputStream);
                Utils.writeUInt32(Coordination, outputStream);
                Utils.writeUInt32(Quickness, outputStream);
                Utils.writeUInt32(Focus, outputStream);
                Utils.writeUInt32(Self, outputStream);

                Utils.writeCompressedUInt32((uint)NormalSkillsList.Count, outputStream);
                foreach (uint skill in NormalSkillsList)
                    Utils.writeUInt32(skill, outputStream);

                Utils.writeCompressedUInt32((uint)PrimarySkillsList.Count, outputStream);
                foreach (uint skill in PrimarySkillsList)
                    Utils.writeUInt32(skill, outputStream);
            }
        }

        public class SexCG
        {
            public string Name;
            public uint Scale;
            public uint SetupID;
            public uint SoundTable;
            public uint IconImage;
            public uint BasePalette;
            public uint SkinPalSet;
            public uint PhysicsTable;
            public uint MotionTable;
            public uint CombatTable;
            public sObjDesc BaseObjDesc;
            public List<uint> HairColorList = new List<uint>();
            public List<HairStyleCG> HairStyleList = new List<HairStyleCG>();
            public List<uint> EyeColorList = new List<uint>();
            public List<EyeStripCG> EyeStripList = new List<EyeStripCG>();
            public List<FaceStripCG> NoseStripList  = new List<FaceStripCG>();
            public List<FaceStripCG> MouthStripList = new List<FaceStripCG>();
            public List<GearCG> HeadgearList  = new List<GearCG>();
            public List<GearCG> ShirtList = new List<GearCG>();
            public List<GearCG> PantsList = new List<GearCG>();
            public List<GearCG> FootwearList = new List<GearCG>();
            public List<uint> ClothingColorsList = new List<uint>();

            public SexCG(byte[] buffer, StreamReader inputFile)
            {
                Name = Utils.ReadSerializedString(buffer, inputFile);
                Scale = Utils.ReadUInt32(buffer, inputFile);
                SetupID = Utils.ReadUInt32(buffer, inputFile);
                SoundTable = Utils.ReadUInt32(buffer, inputFile);
                IconImage = Utils.ReadUInt32(buffer, inputFile);
                BasePalette = Utils.ReadUInt32(buffer, inputFile);
                SkinPalSet = Utils.ReadUInt32(buffer, inputFile);
                PhysicsTable = Utils.ReadUInt32(buffer, inputFile);
                MotionTable = Utils.ReadUInt32(buffer, inputFile);
                CombatTable = Utils.ReadUInt32(buffer, inputFile);

                Utils.Align(inputFile);
                BaseObjDesc = new sObjDesc(buffer, inputFile);

                uint count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    HairColorList.Add(Utils.ReadUInt32(buffer, inputFile));

                count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    HairStyleList.Add(new HairStyleCG(buffer, inputFile));

                count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    EyeColorList.Add(Utils.ReadUInt32(buffer, inputFile));

                count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    EyeStripList.Add(new EyeStripCG(buffer, inputFile));

                count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    NoseStripList.Add(new FaceStripCG(buffer, inputFile));

                count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    MouthStripList.Add(new FaceStripCG(buffer, inputFile));

                count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    HeadgearList.Add(new GearCG(buffer, inputFile));

                count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    ShirtList.Add(new GearCG(buffer, inputFile));

                count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    PantsList.Add(new GearCG(buffer, inputFile));

                count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    FootwearList.Add(new GearCG(buffer, inputFile));

                count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                    ClothingColorsList.Add(Utils.ReadUInt32(buffer, inputFile));
            }

            public void writeRaw(StreamWriter outputStream)
            {
                Utils.writeSerializedString(Name, outputStream);
                Utils.writeUInt32(Scale, outputStream);
                Utils.writeUInt32(SetupID, outputStream);
                Utils.writeUInt32(SoundTable, outputStream);
                Utils.writeUInt32(IconImage, outputStream);
                Utils.writeUInt32(BasePalette, outputStream);
                Utils.writeUInt32(SkinPalSet, outputStream);
                Utils.writeUInt32(PhysicsTable, outputStream);
                Utils.writeUInt32(MotionTable, outputStream);
                Utils.writeUInt32(CombatTable, outputStream);

                Utils.Align(outputStream);
                BaseObjDesc.writeRaw(outputStream);

                Utils.writeCompressedUInt32((uint)HairColorList.Count, outputStream);
                foreach (uint hairColor in HairColorList)
                    Utils.writeUInt32(hairColor, outputStream);

                Utils.writeCompressedUInt32((uint)HairStyleList.Count, outputStream);
                foreach (HairStyleCG hairStyle in HairStyleList)
                    hairStyle.writeRaw(outputStream);

                Utils.writeCompressedUInt32((uint)EyeColorList.Count, outputStream);
                foreach (uint eyeColor in EyeColorList)
                    Utils.writeUInt32(eyeColor, outputStream);

                Utils.writeCompressedUInt32((uint)EyeStripList.Count, outputStream);
                foreach (EyeStripCG eyeStyle in EyeStripList)
                    eyeStyle.writeRaw(outputStream);

                Utils.writeCompressedUInt32((uint)NoseStripList.Count, outputStream);
                foreach (FaceStripCG noseStrip in NoseStripList)
                    noseStrip.writeRaw(outputStream);

                Utils.writeCompressedUInt32((uint)MouthStripList.Count, outputStream);
                foreach (FaceStripCG mouthStrip in MouthStripList)
                    mouthStrip.writeRaw(outputStream);

                Utils.writeCompressedUInt32((uint)HeadgearList.Count, outputStream);
                foreach (GearCG headGear in HeadgearList)
                    headGear.writeRaw(outputStream);

                Utils.writeCompressedUInt32((uint)ShirtList.Count, outputStream);
                foreach (GearCG shirts in ShirtList)
                    shirts.writeRaw(outputStream);

                Utils.writeCompressedUInt32((uint)PantsList.Count, outputStream);
                foreach (GearCG pants in PantsList)
                    pants.writeRaw(outputStream);

                Utils.writeCompressedUInt32((uint)FootwearList.Count, outputStream);
                foreach (GearCG footwear in FootwearList)
                    footwear.writeRaw(outputStream);

                Utils.writeCompressedUInt32((uint)ClothingColorsList.Count, outputStream);
                foreach (uint color in ClothingColorsList)
                    Utils.writeUInt32(color, outputStream);
            }
        }

        public class GearCG
        {
            public string Name;
            public uint ClothingTable;
            public uint WeenieDefault;

            public GearCG(byte[] buffer, StreamReader inputFile)
            {
                Name = Utils.ReadSerializedString(buffer, inputFile);
                ClothingTable = Utils.ReadUInt32(buffer, inputFile);
                WeenieDefault = Utils.ReadUInt32(buffer, inputFile);
            }

            public void writeRaw(StreamWriter outputStream)
            {
                Utils.writeSerializedString(Name, outputStream);
                Utils.writeUInt32(ClothingTable, outputStream);
                Utils.writeUInt32(WeenieDefault, outputStream);
            }
        }

        public class FaceStripCG
        {
            public uint IconImage;
            public sObjDesc ObjDesc;

            public FaceStripCG(byte[] buffer, StreamReader inputFile)
            {
                IconImage = Utils.ReadUInt32(buffer, inputFile);
                Utils.Align(inputFile);
                ObjDesc = new sObjDesc(buffer, inputFile);
            }

            public void writeRaw(StreamWriter outputStream)
            {
                Utils.writeUInt32(IconImage, outputStream);
                Utils.Align(outputStream);
                ObjDesc.writeRaw(outputStream);
            }
        }

        public class EyeStripCG
        {
            public uint IconImage;
            public uint IconImageBald;
            public sObjDesc ObjDesc;
            public sObjDesc ObjDescBald;

            public EyeStripCG(byte[] buffer, StreamReader inputFile)
            {
                IconImage = Utils.ReadUInt32(buffer, inputFile);
                IconImageBald = Utils.ReadUInt32(buffer, inputFile);
                Utils.Align(inputFile);
                ObjDesc = new sObjDesc(buffer, inputFile);
                Utils.Align(inputFile);
                ObjDescBald = new sObjDesc(buffer, inputFile);
            }

            public void writeRaw(StreamWriter outputStream)
            {
                Utils.writeUInt32(IconImage, outputStream);
                Utils.writeUInt32(IconImageBald, outputStream);
                Utils.Align(outputStream);
                ObjDesc.writeRaw(outputStream);
                Utils.Align(outputStream);
                ObjDescBald.writeRaw(outputStream);
            }
        }

        public class HairStyleCG
        {
            public uint IconImage;
            public bool Bald;
            public uint AlternateSetup;
            public sObjDesc ObjDesc;

            public HairStyleCG(byte[] buffer, StreamReader inputFile)
            {
                IconImage = Utils.ReadUInt32(buffer, inputFile);
                Bald = Utils.ReadBool(buffer, inputFile);
                AlternateSetup = Utils.ReadUInt32(buffer, inputFile);
                Utils.Align(inputFile);
                ObjDesc = new sObjDesc(buffer, inputFile);
            }

            public void writeRaw(StreamWriter outputStream)
            {
                Utils.writeUInt32(IconImage, outputStream);
                Utils.writeBool(Bald, outputStream);
                Utils.writeUInt32(AlternateSetup, outputStream);
                Utils.Align(outputStream);
                ObjDesc.writeRaw(outputStream);
            }
        }

        public class SkillCG
        {
            public uint SkillNum;
            public uint NormalCost;
            public uint PrimaryCost;

            public SkillCG(uint SkillNum, uint NormalCost, uint PrimaryCost)
            {
                this.SkillNum = SkillNum;
                this.NormalCost = NormalCost;
                this.PrimaryCost = PrimaryCost;
            }

            public SkillCG(byte[] buffer, StreamReader inputFile)
            {
                SkillNum = Utils.ReadUInt32(buffer, inputFile);
                NormalCost = Utils.ReadUInt32(buffer, inputFile);
                PrimaryCost = Utils.ReadUInt32(buffer, inputFile);
            }

            public void writeRaw(StreamWriter outputStream)
            {
                Utils.writeUInt32(SkillNum, outputStream);
                Utils.writeUInt32(NormalCost, outputStream);
                Utils.writeUInt32(PrimaryCost, outputStream);
            }
        }

        public class StarterArea
        {
            public string Name;
            public List<sPosition> Locations = new List<sPosition>();

            public StarterArea(byte[] buffer, StreamReader inputFile)
            {
                Name = Utils.ReadSerializedString(buffer, inputFile);

                uint count = Utils.ReadCompressedUInt32(buffer, inputFile);
                for (int i = 0; i < count; i++)
                {
                    Locations.Add(new sPosition(buffer, inputFile));
                }
            }

            public void writeRaw(StreamWriter outputStream)
            {
                Utils.writeSerializedString(Name, outputStream);

                Utils.writeCompressedUInt32((uint)Locations.Count, outputStream);
                foreach(sPosition position in Locations)
                    position.writeRaw(outputStream);
            }
        }

        public uint FileId;
        public uint FileId2;
        public List<StarterArea> StarterAreas = new List<StarterArea>();
        public byte Unknown;
        public Dictionary<uint, HeritageGroupCG> HeritageGroups = new Dictionary<uint, HeritageGroupCG>();

        public CharGen(string filename)
        {
            Console.WriteLine("Reading charGen from {0}...", filename);

            StreamReader inputFile = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));

            byte[] buffer = new byte[1024];

            FileId = Utils.ReadUInt32(buffer, inputFile);
            FileId2 = Utils.ReadUInt32(buffer, inputFile);

            if (FileId != 0x0E000002)
            {
                Console.WriteLine("Invalid header, aborting.");
                return;
            }

            uint count = Utils.ReadCompressedUInt32(buffer, inputFile);
            for (int i = 0; i < count; i++)
                StarterAreas.Add(new StarterArea(buffer, inputFile));

            Unknown = Utils.ReadByte(buffer, inputFile);

            count = Utils.ReadCompressedUInt32(buffer, inputFile);
            for (int i = 0; i < count; i++)
            {
                uint key = Utils.ReadUInt32(buffer, inputFile);
                HeritageGroupCG value = new HeritageGroupCG(buffer, inputFile);
                HeritageGroups.Add(key, value);
            }

            inputFile.Close();
            Console.WriteLine("Done");
        }

        public void save(string filename)
        {
            Console.WriteLine("Saving charGen to {0}...", filename);

            StreamWriter outputFile = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write));

            Utils.writeUInt32(FileId, outputFile);
            Utils.writeUInt32(FileId2, outputFile);

            Utils.writeCompressedUInt32((uint)StarterAreas.Count, outputFile);
            foreach (StarterArea starterArea in StarterAreas)
                starterArea.writeRaw(outputFile);

            Utils.writeByte(Unknown, outputFile);

            Utils.writeCompressedUInt32((uint)HeritageGroups.Count, outputFile);
            foreach (KeyValuePair<uint, HeritageGroupCG> entry in HeritageGroups)
            {
                Utils.writeUInt32(entry.Key, outputFile);
                entry.Value.writeRaw(outputFile);
            }

            outputFile.Close();
            Console.WriteLine("Done");
        }

        public void modify()
        {
            foreach(KeyValuePair<uint, HeritageGroupCG> entry in HeritageGroups)
            {
                HeritageGroupCG heritage = entry.Value;

                heritage.SkillCredits = 50;

                switch (heritage.Name)
                {
                    case "Aluvian":
                        heritage.Skills = new List<SkillCG>();
                        heritage.Skills.Add(new SkillCG((uint)eSkills.Dagger, 0, 4));
                        heritage.Skills.Add(new SkillCG((uint)eSkills.PersonalAppraisal, 0, 2));
                        break;
                    case "Gharu'ndim":
                        heritage.Skills = new List<SkillCG>();
                        heritage.Skills.Add(new SkillCG((uint)eSkills.Staff, 0, 4));
                        heritage.Skills.Add(new SkillCG((uint)eSkills.ItemAppraisal, 0, 2));
                        break;
                    case "Sho":
                        heritage.Skills = new List<SkillCG>();
                        heritage.Skills.Add(new SkillCG((uint)eSkills.UnarmedCombat, 0, 6));
                        break;
                }
                foreach (TemplateCG template in heritage.Templates)
                {
                    switch(template.Name)
                    {
                        case "Bow Hunter":
                            template.PrimarySkillsList = new List<uint>();
                            template.PrimarySkillsList.Add((uint)eSkills.ArcaneLore);
                            template.PrimarySkillsList.Add((uint)eSkills.Bow);
                            template.PrimarySkillsList.Add((uint)eSkills.MeleeDefense);
                            template.NormalSkillsList = new List<uint>();
                            template.NormalSkillsList.Add((uint)eSkills.ItemEnchantment);
                            break;
                        case "Life Caster":
                            template.PrimarySkillsList = new List<uint>();
                            template.PrimarySkillsList.Add((uint)eSkills.LifeMagic);
                            template.NormalSkillsList = new List<uint>();
                            template.NormalSkillsList.Add((uint)eSkills.CreatureEnchantment);
                            template.NormalSkillsList.Add((uint)eSkills.ManaConversion);
                            template.NormalSkillsList.Add((uint)eSkills.WarMagic);
                            break;
                        case "War Mage":
                            template.PrimarySkillsList = new List<uint>();
                            template.PrimarySkillsList.Add((uint)eSkills.WarMagic);
                            template.PrimarySkillsList.Add((uint)eSkills.ManaConversion);
                            template.NormalSkillsList = new List<uint>();
                            template.NormalSkillsList.Add((uint)eSkills.ArcaneLore);
                            template.NormalSkillsList.Add((uint)eSkills.Healing);
                            break;
                        case "Wayfarer":
                            template.PrimarySkillsList = new List<uint>();
                            template.PrimarySkillsList.Add((uint)eSkills.ArcaneLore);
                            template.PrimarySkillsList.Add((uint)eSkills.Dagger);
                            if(heritage.Name == "Aluvian")
                                template.PrimarySkillsList.Add((uint)eSkills.Lockpick);
                            template.NormalSkillsList = new List<uint>();
                            template.NormalSkillsList.Add((uint)eSkills.Crossbow);
                            template.NormalSkillsList.Add((uint)eSkills.Healing);
                            template.NormalSkillsList.Add((uint)eSkills.ItemEnchantment);
                            if (heritage.Name != "Aluvian")
                                template.NormalSkillsList.Add((uint)eSkills.Lockpick);
                            template.NormalSkillsList.Add((uint)eSkills.MeleeDefense);
                            break;
                        case "Soldier":
                            template.PrimarySkillsList = new List<uint>();
                            template.PrimarySkillsList.Add((uint)eSkills.ArcaneLore);
                            template.PrimarySkillsList.Add((uint)eSkills.Axe);
                            template.PrimarySkillsList.Add((uint)eSkills.MeleeDefense);
                            template.NormalSkillsList = new List<uint>();
                            template.NormalSkillsList.Add((uint)eSkills.Crossbow);
                            template.NormalSkillsList.Add((uint)eSkills.Healing);
                            break;
                        case "Swashbuckler":
                            template.PrimarySkillsList = new List<uint>();
                            template.PrimarySkillsList.Add((uint)eSkills.ArcaneLore);
                            template.PrimarySkillsList.Add((uint)eSkills.MeleeDefense);
                            template.PrimarySkillsList.Add((uint)eSkills.Sword);
                            template.NormalSkillsList = new List<uint>();
                            template.NormalSkillsList.Add((uint)eSkills.ItemEnchantment);
                            break;
                    }
                }
            }
        }
    }
}