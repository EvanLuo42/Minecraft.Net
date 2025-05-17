using Minecraft.Blocks;
using OpenTK.Mathematics;

namespace Minecraft.Render;

public class World
{
    private readonly Dictionary<Vector2i, Chunk> _chunks = new();
    private readonly Dictionary<Vector2i, ChunkRenderer> _renderers = new();
    private readonly IChunkGenerator _generator = new PerlinTerrainGenerator();

    public IEnumerable<ChunkRenderer> GetVisibleChunks(Vector3 camPos, int viewDist = 6)
    {
        var center = new Vector2i((int)MathF.Floor(camPos.X / 16), (int)MathF.Floor(camPos.Z / 16));

        for (var dx = -viewDist; dx <= viewDist; dx++)
        for (var dz = -viewDist; dz <= viewDist; dz++)
        {
            var pos = new Vector2i(center.X + dx, center.Y + dz);

            if (_chunks.TryGetValue(pos, out var chunk)) continue;
            chunk = new Chunk();
            _generator.Generate(pos, chunk);
            _chunks[pos] = chunk;
        }
        
        for (var dx = -viewDist; dx <= viewDist; dx++)
        for (var dz = -viewDist; dz <= viewDist; dz++)
        {
            var pos = new Vector2i(center.X + dx, center.Y + dz);
            var chunk = _chunks[pos];

            if (!_renderers.TryGetValue(pos, out var renderer))
            {
                renderer = new ChunkRenderer();
                renderer.Build(chunk, pos, this);
                _renderers[pos] = renderer;
            }

            yield return renderer;
        }
    }

    public Block GetBlock(int worldX, int worldY, int worldZ)
    {
        if (worldY is < 0 or >= Chunk.SizeY)
            return BlockRegistry.Instance["air"]!;

        var chunkX = worldX >= 0 ? worldX / Chunk.SizeX : (worldX - Chunk.SizeX + 1) / Chunk.SizeX;
        var chunkZ = worldZ >= 0 ? worldZ / Chunk.SizeZ : (worldZ - Chunk.SizeZ + 1) / Chunk.SizeZ;

        var localX = (worldX % Chunk.SizeX + Chunk.SizeX) % Chunk.SizeX;
        var localZ = (worldZ % Chunk.SizeZ + Chunk.SizeZ) % Chunk.SizeZ;

        var chunkPos = new Vector2i(chunkX, chunkZ);

        return _chunks.TryGetValue(chunkPos, out var chunk)
            ? chunk[localX, worldY, localZ]
            : new AirBlock();
    }
}