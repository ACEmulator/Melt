using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Melt
{
    class DMtoToDTexture
    {
        static public void convert()
        {
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/0500145c.png", 0x06006d6f, 21);//BarrenRock
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001459.png", 0x06006d40, 21);//Grassland
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001468.png", 0x06006d4b, 21);//Ice
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001456.png", 0x06006d06, 21);//LushGrass
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001467.png", 0x06006d4a, 21);//MarshSparseSwamp
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001462.png", 0x06006d46, 21);//MudRichDirt
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001463.png", 0x06006d56, 21);//ObsidianPlain
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001465.png", 0x06006d48, 21);//PackedDirt
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/0500145b.png", 0x06006d42, 21);//PatchyDirt
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001457.png", 0x06006d3c, 21);//PatchyGrassland
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/0500145d.png", 0x06006d43, 21);//sand-yellow
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/0500145f.png", 0x06006d44, 21);//sand-grey
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/0500145e.png", 0x06006d53, 21);//sand-rockStrewn
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/050014a7.png", 0x06006d51, 21);//SedimentaryRock
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/0500145a.png", 0x06006d41, 21);//SemiBarrenRock
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001464.png", 0x06006d47, 21);//Snow
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/0500146a.png", 0x06006d4d, 21);//WaterRunning
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001461.png", 0x06006d45, 21);//WaterStandingFresh
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/0500146c.png", 0x06006d4f, 21);//WaterShallowSea
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001469.png", 0x06006d4c, 21);//WaterShallowStillSea
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/0500146b.png", 0x06006d4e, 21);//WaterDeepSea
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001466.png", 0x06006d49, 21);//forestfloor
            //TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/0500146a.png", 0x06006d4d, 21);//FauxWaterRunning- same text as WaterRunning, ignore
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001827.png", 0x06006d55, 21);//SeaSlime
            //TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/0500145c.png", 0x06006d6f, 21);//Argila - same text as BarrenRock, ignore
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/0500181f.png", 0x06006d54, 21);//Volcano1
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001924.png", 0x06006d6a, 21);//Volcano2
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001900.png", 0x06006d50, 21);//BlueIce

            // in ToD this uses the same texture as PatchyGrassland, this is not the case in DM.
            //So we changed the unused? detail texture file 060037d2 and altered the header 05001c3a to direct to it
            //to do: replace this with a properly added new file to the portal.dat
            //TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001c3a.png", 0x060037d2, 21);//Moss
            //addendum: 
            //texture file 060037d2 is the waterfall particle effect
            //texture file 06006D58 is the buildings detail texture
            //texture file 06006D57 is the ground detail texture that doesnt appear it's being applid. Using this for now.
            //detail texture file 06006D57 and altered the header 05001c3a to direct to it
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001c3a.png", 0x06006d57, 21);//Moss


            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001c3b.png", 0x06006d3d, 21);//DarkMoss
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001c3c.png", 0x06006d3e, 21);//olthoi
            //TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/0500145c.png", 0x06006d6f, 21);//DesolateLands - same text as BarrenRock, ignore
            TextureConverter.toBin("Landscape Texture Conversion/DM/Textures/Upscaled + Detail Overlay/05001458.png", 0x06006d3f, 21);//Road

            TextureConverter.toBin("Landscape Texture Conversion/DM/Alpha Maps/Upscaled/05001440.png", 0x06006d6d, 244);
            TextureConverter.toBin("Landscape Texture Conversion/DM/Alpha Maps/Upscaled/05001441.png", 0x06006d61, 244);
            TextureConverter.toBin("Landscape Texture Conversion/DM/Alpha Maps/Upscaled/0500143e.png", 0x06006d6b, 244);
            TextureConverter.toBin("Landscape Texture Conversion/DM/Alpha Maps/Upscaled/0500143f.png", 0x06006d6c, 244);
            TextureConverter.toBin("Landscape Texture Conversion/DM/Alpha Maps/Upscaled/0500168c.png", 0x06006d36, 244);
            TextureConverter.toBin("Landscape Texture Conversion/DM/Alpha Maps/Upscaled/0500168d.png", 0x06006d37, 244);
            TextureConverter.toBin("Landscape Texture Conversion/DM/Alpha Maps/Upscaled/0500168e.png", 0x06006d30, 244);
            TextureConverter.toBin("Landscape Texture Conversion/DM/Alpha Maps/Upscaled/05001371.png", 0x06006d60, 244);
        }
    }
}