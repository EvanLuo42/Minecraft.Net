using Minecraft.Blocks;
using OpenTK.Mathematics;

namespace Minecraft.Render;

public class ChunkMesh
{
    public readonly List<Vector3> Vertices = [];
    public readonly List<uint> Indices = [];

    private readonly Dictionary<Vector3, uint> _vertexMap = new();

    private void AddQuad(Vector3[] quad)
    {
        var index = new uint[4];

        for (var i = 0; i < 4; i++)
        {
            if (_vertexMap.TryGetValue(quad[i], out var existing))
            {
                index[i] = existing;
            }
            else
            {
                var newIndex = (uint)Vertices.Count;
                Vertices.Add(quad[i]);
                _vertexMap[quad[i]] = newIndex;
                index[i] = newIndex;
            }
        }
        
        Indices.Add(index[0]);
        Indices.Add(index[1]);
        Indices.Add(index[2]);
        Indices.Add(index[2]);
        Indices.Add(index[3]);
        Indices.Add(index[0]);
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

                var position = new Vector3i(x, y, z);
                var (nx, ny, nz) = position + face.Normal;

                if (!chunk[nx, ny, nz].IsTransparent) continue;
                var quad = face.Vertices.Select(v => position + v).ToArray();
                mesh.AddQuad(quad);
            }
        }

        return mesh;
    }
}

public struct CubeFace
{
    public Vector3i Normal;
    public Vector3[] Vertices;

    public static readonly CubeFace[] All =
    [
        new()
        {
            Normal = new Vector3i(0, 0, 1),
            Vertices = [new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(0, 1, 1)]
        },
        new()
        {
            Normal = new Vector3i(0, 0, -1),
            Vertices = [new Vector3(1, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0)]
        },
        new()
        {
            Normal = new Vector3i(1, 0, 0),
            Vertices = [new Vector3(1, 0, 1), new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1)]
        },
        new()
        {
            Normal = new Vector3i(-1, 0, 0),
            Vertices = [new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(0, 1, 0)]
        },
        new()
        {
            Normal = new Vector3i(0, 1, 0),
            Vertices = [new Vector3(0, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 0), new Vector3(0, 1, 0)]
        },
        new()
        {
            Normal = new Vector3i(0, -1, 0),
            Vertices = [new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(0, 0, 1)]
        }
    ];
}