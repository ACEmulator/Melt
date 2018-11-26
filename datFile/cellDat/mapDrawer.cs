using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melt
{
    public class cMapDrawer
    {
        int landSize = 2041;

        // The following constants change how the lighting works.  It is easy to wash out
        // the bright whites of the snow, so be careful.

        // Incresing ColorCorrection makes the base color more prominant.
        double ColorCorrection = 70;

        // Increasing LightCorrection increases the contrast between steep and flat slopes.
        double LightCorrection = 2.25;

        // Increasing AmbientLight makes everyting brighter.
        double AmbientLight = 64;

        // This vector reprsents a light coming from the northwest corner of the map.
        // Pretend the sun is on the horizon at the northwest corner.
        double[] lightVector = {-1.0, -1.0, 0.0};

        // These color values deserve the most comment in this file.  I edited my cell.dat
        // to create strips of each land type.  A road passes over the end of each strip.
        // I then took screenshots of each of the strips.  There were four strips in each
        // screenshot.  I then cut out a piece of each of the strips and made a seperate
        // image of each one.  Using the histogram feature in Paint Shop Pro, I found the
        // average red, green, and blue values for each land type.  These are the numbers
        // found below.  The fourth number in each group below is the average luminance
        // of a control patch in each screenshot, in this case the road.  The control is
        // needed since the screenshots were taken at slightly different times of the day
        // and thus the general brightness of the scene is different.
        //
        // The last entry is the color for the roads.
        byte[,] landColor = {
              {84, 67, 37, 110},
              {56, 66, 21, 110},
              {147, 154, 167, 110},
              {51, 69, 10, 110},
              {71, 37, 7, 113},
              {54, 34, 23, 113},
              {39, 35, 43, 113},
              {89, 65, 34, 113},
              {57, 41, 9, 113},
              {44, 77, 2, 113},
              {144, 99, 50, 113},
              {132, 132, 97, 113},
              {138, 93, 53, 114},
              {111, 68, 41, 114},
              {75, 85, 59, 114},
              {208, 219, 233, 114},
              {62, 108, 131, 130},
              {20, 79, 56, 130},
              {31, 80, 100, 130},
              {44, 76, 94, 130},
              {43, 59, 83, 130},
              {34, 47, 6, 130},
              {62, 108, 131, 130},
              {30, 38, 26, 130},
              {100, 79, 43, 130},
              {45, 33, 33, 130},
              {72, 72, 70, 130},
              {197, 227, 242, 130},
              {100, 79, 43, 130},
              {100, 79, 43, 130},
              {100, 79, 43, 130},
              {100, 79, 43, 130},
              {138, 130, 112, 130}
            };

        const int Luminance = 100;

        byte[,] landColorTrue = {
            {102, 88, 72, Luminance}, // 0 - BarrenRock - 0x06006d6f
            {89, 94, 47, Luminance}, // 1 - Grassland - 0x06006d40
            {175, 179, 178, Luminance}, // 2 - Ice - 0x06006d4b
            {95, 94, 36, Luminance}, // 3 - LushGrass - 0x06006d06
            {67, 47, 20, Luminance}, // 4 - MarshSparseSwamp - 0x06006d4a
            {56, 39, 21, Luminance}, // 5 - MudRichDirt - 0x06006d46
            {23, 17, 27, Luminance}, // 6 - ObsidianPlain - 0x06006d56
            {112, 84, 50, Luminance}, // 7 - PackedDirt - 0x06006d48
            {98, 80, 58, Luminance}, // 8 - PatchyDirt - 0x06006d42
            {70, 72, 36, Luminance}, // 9 - PatchyGrassland - 0x06006d3c
            {215, 155, 103, Luminance}, // 10 - sand-yellow - 0x06006d43
            {148, 129, 107, Luminance}, // 11 - sand-grey - 0x06006d44
            {183, 144, 109, Luminance}, // 12 - sand-rockStrewn - 0x06006d53
            {151, 121, 87, Luminance}, // 13 - SedimentaryRock - 0x06006d51
            {88, 82, 55, Luminance}, // 14 - SemiBarrenRock - 0x06006d41
            {191, 196, 201, Luminance}, // 15 - Snow - 0x06006d47
            {35, 76, 110, Luminance}, // 16 - WaterRunning - 0x06006d4d
            {21, 68, 80, Luminance}, // 17 - WaterStandingFresh - 0x06006d45
            {36, 41, 68, Luminance}, // 18 - WaterShallowSea - 0x06006d4f
            {31, 63, 57, Luminance}, // 19 - WaterShallowStillSea - 0x06006d4c
            {31, 35, 62, Luminance}, // 20 - WaterDeepSea - 0x06006d4e
            {90, 95, 41, Luminance}, // 21 - forestfloor - 0x06006d49
            {35, 76, 110, Luminance}, // 22 - FauxWaterRunning - 0x06006d4d
            {70, 90, 66, Luminance}, // 23 - SeaSlime - 0x06006d55
            {102, 88, 72, Luminance}, // 24 - Argila - 0x06006d6f
            {28, 19, 23, Luminance}, // 25 - Volcano1 - 0x06006d54
            {103, 103, 103, Luminance}, // 26 - Volcano2 - 0x06006d6a
            {127, 164, 163, Luminance}, // 27 - BlueIce - 0x06006d50
            {70, 72, 36, Luminance}, // 28 - Moss - 0x06006d3c
            {65, 54, 22, Luminance}, // 29 - DarkMoss - 0x06006d3d
            {70, 57, 56, Luminance}, // 30 - olthoi - 0x06006d3e
            {102, 88, 72, Luminance}, // 31 - DesolateLands - 0x06006d6f
            {112, 116, 105, Luminance}, // 32 - roads - 0x06006d3f
        };

        struct landData
        {
            public ushort type;
            public byte z;
            public bool used;
        }

        landData[,] land;
        byte [,,] topo;

        public cMapDrawer(cCellDat cellDat)
        {
            land = new landData[landSize, landSize];
            topo = new byte[landSize, landSize, 3];

            foreach(KeyValuePair<int, Dictionary<int, cCellLandblock>> entryX in cellDat.surfaceLandblocks)
            {
                foreach (KeyValuePair<int, cCellLandblock> entryY in entryX.Value)
                {
                    int landBlockX = entryX.Key * 8;
                    int landBlockY = entryY.Key * 8;

                    for(int x = 0; x < 9; x++)
                    {
                        for (int y = 0; y < 9; y++)
                        {
                            landData newLandData = new landData();
                            int terrainIndex = (x * 9) + y;
                            newLandData.type = entryY.Value.Terrain[terrainIndex];
                            newLandData.z = entryY.Value.Height[terrainIndex];
                            newLandData.used = true;

                            int finalX = landBlockX + x;
                            int finalY = landSize - (landBlockY + y) - 1;

                            land[finalX, finalY] = newLandData;
                        }
                    }
                }
            }
        }

        public void draw(bool useTrueColor = false)
        {
            Console.WriteLine("Drawing map to file...");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            Bitmap bmp = new Bitmap(landSize, landSize);
            int x, y;
            int i;
            ushort type;
            double color, light;
            double[] v = new double[3];

            for (y = 0; y < landSize; y++)
            {
                for (x = 0; x < landSize; x++)
                {
                    if (land[y, x].used)
                    {
                        // Calculate normal by using surrounding z values, if they exist
                        v[0] = 0.0;
                        v[1] = 0.0;
                        v[2] = 0.0;
                        if ((x < landSize - 1) && (y < landSize - 1))
                        {
                            if (land[y, x + 1].used && land[y + 1, x].used)
                            {
                                v[0] -= land[y, x + 1].z - land[y, x].z;
                                v[1] -= land[y + 1, x].z - land[y, x].z;
                                v[2] += 12.0;
                            }
                        }
                        if ((x > 0) && (y < landSize - 1))
                        {
                            if (land[y, x - 1].used && land[y + 1, x].used)
                            {
                                v[0] += land[y, x - 1].z - land[y, x].z;
                                v[1] -= land[y + 1, x].z - land[y, x].z;
                                v[2] += 12.0;
                            }
                        }
                        if ((x > 0) && (y > 0))
                        {
                            if (land[y, x - 1].used && land[y - 1, x].used)
                            {
                                v[0] += land[y, x - 1].z - land[y, x].z;
                                v[1] += land[y - 1, x].z - land[y, x].z;
                                v[2] += 12.0;
                            }
                        }
                        if ((x < landSize - 1) && (y > 0))
                        {
                            if (land[y, x + 1].used && land[y - 1, x].used)
                            {
                                v[0] -= land[y, x + 1].z - land[y, x].z;
                                v[1] += land[y - 1, x].z - land[y, x].z;
                                v[2] += 12.0;
                            }
                        }

                        // Check for road bit(s)
                        if ((land[y, x].type & 0x0003) != 0)
                            type = 32;
                        else
                            type = (ushort)((land[y, x].type & 0x00FF) >> 2);

                        // Calculate lighting scalar
                        light = (((lightVector[0] * v[0] + lightVector[1] * v[1] + lightVector[2] * v[2]) /
                            Math.Sqrt((lightVector[0] * lightVector[0] + lightVector[1] * lightVector[1] + lightVector[2] * lightVector[2]) *
                            (v[0] * v[0] + v[1] * v[1] + v[2] * v[2]))) * 128.0 + 128.0) * LightCorrection + AmbientLight;

                        // Apply lighting scalar to base colors
                        for (i = 0; i < 3; i++)
                        {
                            if (!useTrueColor)
                                color = (landColor[type, i] * ColorCorrection / landColor[type, 3]) * light / 256.0;
                            else
                                color = (landColorTrue[type, i] * ColorCorrection / landColorTrue[type, 3]) * light / 256.0;
                            if (color > 255.0)
                                topo[y, x, i] = 255;
                            else if (color < 0.0)
                                topo[y, x, i] = 0;
                            else
                                topo[y, x, i] = (byte)color;
                        }
                    }
                    else
                    {
                        // If data is not present for a point on the map, the resultant pixel is green
                        topo[y, x, 0] = 0;
                        topo[y, x, 1] = 0xFF;
                        topo[y, x, 2] = 0;
                    }
                    bmp.SetPixel(y, x, Color.FromArgb(topo[y, x, 0], topo[y, x, 1], topo[y, x, 2]));
                }
            }
            bmp.Save($"map{(useTrueColor ? "-true" : "")}.png", ImageFormat.Png);

            timer.Stop();
            Console.WriteLine("Map drawn in {0}.", timer.ElapsedMilliseconds / 1000f);
        }
    }
}