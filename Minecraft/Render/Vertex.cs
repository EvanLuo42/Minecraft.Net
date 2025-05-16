using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace Minecraft.Render;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex(Vector3 position, Vector2 texCoord)
{
    public Vector3 Position = position;
    public Vector2 TexCoord = texCoord;

    public static readonly int SizeInBytes = Marshal.SizeOf<Vertex>();
}