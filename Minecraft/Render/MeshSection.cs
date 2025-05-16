namespace Minecraft.Render;

public class MeshSection(Texture texture)
{
    public List<Vertex> Vertices = [];
    public List<uint> Indices = [];
    public Texture Texture = texture;
}