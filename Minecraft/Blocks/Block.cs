using Minecraft.Render;

namespace Minecraft.Blocks;

public class Block
{
    public string Name { get; set; }
    public bool IsTransparent { get; set; }
    public readonly Dictionary<FaceName, Texture> Textures = new();

    protected Block(string name, bool isTransparent)
    {
        Name = name;
        IsTransparent = isTransparent;
    }
}