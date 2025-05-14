using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Minecraft.Render;

public class ChunkRenderer
{
    private int _vao, _vbo, _ebo;
    private int _indexCount;

    public void Build(Chunk chunk)
    {
        var mesh = ChunkMesh.BuildIndexedMesh(chunk);

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();
        _indexCount = mesh.Indices.Count;
        
        GL.BindVertexArray(_vao);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, 
            mesh.Vertices.Count * Vector3.SizeInBytes, mesh.Vertices.ToArray(), BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
        GL.EnableVertexAttribArray(0);
        
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, 
            mesh.Indices.Count * sizeof(uint), mesh.Indices.ToArray(), BufferUsageHint.StaticDraw);
    }

    public void Render()
    {
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
    }
}