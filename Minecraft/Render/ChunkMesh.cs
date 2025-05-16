using OpenTK.Mathematics;

namespace Minecraft.Render;

public class ChunkMesh
{
    public readonly List<MeshSection> Sections = [];

    private readonly Dictionary<(Vector3, Vector2), uint> _vertexMap = new();

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

    public static ChunkMesh BuildIndexedMesh(Chunk chunk)
    {
        var mesh = new ChunkMesh();
        foreach (var face in CubeFace.All)
        {
            for (var x = 0; x < Chunk.SizeX; x++)
            for (var y = 0; y < Chunk.SizeX; y++)
            for (var z = 0; z < Chunk.SizeX; z++)
            {
                if (chunk[x, y, z].IsTransparent) continue;
                var block = chunk[x, y, z];

                var position = new Vector3i(x, y, z);
                var (nx, ny, nz) = position + face.Normal;
                if (!chunk[nx, ny, nz].IsTransparent) continue;
                var quad = face.Vertices.Select(v => position + v).ToArray();
                Vector2[] textures = [
                    new(0.0f, 0.0f),
                    new(1.0f, 0.0f),
                    new(1.0f, 1.0f),
                    new(0.0f, 1.0f)
                ];
                foreach (var (faceName, texture) in block.Textures)
                {
                    if (!face.Textures.Contains(faceName)) continue;
                    mesh.AddQuad(quad, textures, texture);
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