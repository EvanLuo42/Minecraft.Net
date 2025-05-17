using OpenTK.Mathematics;

namespace Minecraft.Render;

public class ChunkMesh
{
    public readonly List<MeshSection> Sections = [];

    private readonly Dictionary<(Vector3, Vector2), uint> _vertexMap = new();
    
    private static readonly Vector2[] DefaultUv =
    [
        new(0f, 0f),
        new(1f, 0f),
        new(1f, 1f),
        new(0f, 1f)
    ];

    private MeshSection GetOrCreateSection(Texture texture)
    {
        var section = Sections.FirstOrDefault(s => s.Texture.Handle == texture.Handle);
        if (section != null) return section;
        section = new MeshSection(texture);
        Sections.Add(section);
        return section;
    }

    private void AddQuad(Vector3[] quad, Vector2[] texCoords, Texture texture)
    {
        var section = GetOrCreateSection(texture);
        var baseIndex = (uint)section.Vertices.Count;

        for (var i = 0; i < 4; i++)
            section.Vertices.Add(new Vertex(quad[i], texCoords[i]));

        section.Indices.Add(baseIndex + 0);
        section.Indices.Add(baseIndex + 1);
        section.Indices.Add(baseIndex + 2);
        section.Indices.Add(baseIndex + 2);
        section.Indices.Add(baseIndex + 3);
        section.Indices.Add(baseIndex + 0);
    }

    public static ChunkMesh BuildIndexedMesh(Chunk chunk, World world, Vector2i chunkPos)
    {
        var mesh = new ChunkMesh();
        foreach (var face in CubeFace.All)
        {
            for (var x = 0; x < Chunk.SizeX; x++)
            for (var y = 0; y < Chunk.SizeY; y++)
            for (var z = 0; z < Chunk.SizeZ; z++)
            {
                if (chunk[x, y, z].IsTransparent) continue;
                var block = chunk[x, y, z];
                
                var position = new Vector3i(x, y, z);
                var worldPos = new Vector3(chunkPos.X * Chunk.SizeX + x, y, chunkPos.Y * Chunk.SizeZ + z);
                var (nx, ny, nz) = worldPos + face.Normal;
                if (!world.GetBlock((int)nx, (int)ny, (int)nz).IsTransparent) continue;
                var quad = new Vector3[4];
                for (var i = 0; i < 4; i++)
                    quad[i] = position + face.Vertices[i];
                foreach (var (faceName, texture) in block.Textures)
                {
                    if (!face.Textures.Contains(faceName)) continue;
                    mesh.AddQuad(quad, DefaultUv, texture);
                }
            }
        }

        return mesh;
    }
}

public struct CubeFace
{
    public Vector3i Normal;
    public Vector3[] Vertices;
    public FaceName[] Textures;

    public static readonly CubeFace[] All =
    [
        new()
        {
            Normal = new Vector3i(0, 0, 1),
            Vertices = [new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(0, 1, 1)],
            Textures = [FaceName.All, FaceName.Horizontal, FaceName.Front]
        },
        new()
        {
            Normal = new Vector3i(0, 0, -1),
            Vertices = [new Vector3(1, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0)],
            Textures = [FaceName.All, FaceName.Horizontal, FaceName.Back]
        },
        new()
        {
            Normal = new Vector3i(1, 0, 0),
            Vertices = [new Vector3(1, 0, 1), new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1)],
            Textures = [FaceName.All, FaceName.Horizontal, FaceName.Right]
        },
        new()
        {
            Normal = new Vector3i(-1, 0, 0),
            Vertices = [new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(0, 1, 0)],
            Textures = [FaceName.All, FaceName.Horizontal, FaceName.Left]
        },
        new()
        {
            Normal = new Vector3i(0, 1, 0),
            Vertices = [new Vector3(0, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 0), new Vector3(0, 1, 0)],
            Textures = [FaceName.All, FaceName.Vertical, FaceName.Top]
        },
        new()
        {
            Normal = new Vector3i(0, -1, 0),
            Vertices = [new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(0, 0, 1)],
            Textures = [FaceName.All, FaceName.Vertical, FaceName.Bottom]
        }
    ];
}