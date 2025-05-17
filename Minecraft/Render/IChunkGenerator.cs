using Minecraft.Blocks;
using Minecraft.Utils;
using OpenTK.Mathematics;

namespace Minecraft.Render;

public interface IChunkGenerator
{
    void Generate(Vector2i chunkPos, Chunk chunk);
}

public class PerlinTerrainGenerator : IChunkGenerator
{
    private readonly FastNoiseLite _noise;

    private readonly Block _grass;

    public PerlinTerrainGenerator()
    {
        _noise = new FastNoiseLite();
        _noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        _noise.SetSeed(1337);
        _noise.SetFrequency(0.01f);

        _grass = BlockRegistry.Instance["grass"]!;
    }

    public void Generate(Vector2i chunkPos, Chunk chunk)
    {
        for (var x = 0; x < Chunk.SizeX; x++)
        for (var z = 0; z < Chunk.SizeZ; z++)
        {
            var worldX = chunkPos.X * Chunk.SizeX + x;
            var worldZ = chunkPos.Y * Chunk.SizeZ + z;
            
            var noiseValue = _noise.GetNoise(worldX, worldZ);
            var height = (int)(noiseValue * 6f + 8f);
            height = Math.Clamp(height, 1, Chunk.SizeY - 1);
            
            for (var y = 0; y <= height; y++)
            {
                chunk[x, y, z] = _grass;
            }
        }
    }
}