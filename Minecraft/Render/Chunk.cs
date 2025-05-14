using Minecraft.Blocks;

namespace Minecraft.Render;

public class Chunk
{
    public const int SizeX = 16;
    public const int SizeY = 16;
    public const int SizeZ = 16;

    private readonly Block[,,] _blocks = new Block[SizeX, SizeY, SizeZ];

    public Chunk()
    {
        for (var x = 0; x < SizeX; x++)
        for (var y = 0; y < SizeX; y++)
        for (var z = 0; z < SizeX; z++)
        {
            _blocks[x, y, z] = new AirBlock();
        }
    }

    public Block this[int x, int y, int z]
    {
        get => InBounds(x, y, z) ? _blocks[x, y, z] : new AirBlock();
        set
        {
            if (InBounds(x, y, z)) _blocks[x, y, z] = value;
        }
    }
    
    private static bool InBounds(int x, int y, int z) => 
        x >= 0 && y >= 0 && z >= 0 && x < SizeX && y < SizeY && z < SizeZ;
}