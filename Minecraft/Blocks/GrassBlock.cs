using Minecraft.Render;

namespace Minecraft.Blocks;

public class GrassBlock : Block
{
    public GrassBlock() : base("grass", false)
    {
        Textures.Add(FaceName.Top, Texture.LoadFromFile("green_concrete_powder.png"));
        Textures.Add(FaceName.Horizontal, Texture.LoadFromFile("grass_block_side.png"));
        Textures.Add(FaceName.Bottom, Texture.LoadFromFile("dirt.png"));
    }
}