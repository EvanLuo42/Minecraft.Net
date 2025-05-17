namespace Minecraft.Blocks;

public class BlockRegistry
{
    private static BlockRegistry? _instance;
    public static BlockRegistry Instance => _instance ??= new BlockRegistry();

    private readonly Dictionary<string, Block?> _byName = new(StringComparer.OrdinalIgnoreCase);

    public void Register(string name, Block? block)
    {
        _byName[name] = block;
    }

    public Block? this[string name] => _byName[name];

    public bool TryGet(string name, out Block? block) => _byName.TryGetValue(name, out block);

    public IEnumerable<string> AllNames => _byName.Keys;

    public IEnumerable<Block?> AllBlocks => _byName.Values.Distinct();
}