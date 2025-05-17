namespace Minecraft.Blocks;

public static class BlockRegistration
{
    public static void RegisterBlocks()
    {
        var air = new AirBlock();
        BlockRegistry.Instance.Register(air.Name, air);
        
        var grass = new GrassBlock();
        BlockRegistry.Instance.Register(grass.Name, grass);
    }
}