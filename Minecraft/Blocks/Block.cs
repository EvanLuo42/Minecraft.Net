using Minecraft.Render;

namespace Minecraft.Blocks;

public abstract class Block(string name, bool isTransparent)
{
    public string Name { get; set; } = name;
    public bool IsTransparent { get; set; } = isTransparent;
    public readonly Dictionary<FaceName, Texture> Textures = new();
    
    public Texture? GetFaceTexture(FaceName face)
    {
        return Textures.TryGetValue(face, out var tex)
            ? tex
            : Textures.GetValueOrDefault(FaceName.All);
    }
    
    public Block Clone() => this;
}