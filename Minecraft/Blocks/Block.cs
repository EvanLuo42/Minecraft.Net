namespace Minecraft.Blocks;

public class Block
{
    public string Name { get; set; }
    public bool IsTransparent { get; set; }

    protected Block(string name, bool isTransparent)
    {
        Name = name;
        IsTransparent = isTransparent;
    }
}