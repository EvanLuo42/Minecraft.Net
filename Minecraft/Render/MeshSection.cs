namespace Minecraft.Render;

public class MeshSection(Texture texture)
{
    public readonly List<Vertex> Vertices = [];
    public readonly List<uint> Indices = [];
    public readonly Texture Texture = texture;
}