using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Minecraft.Render;

public class ChunkRenderer
{
    private record RenderSection(int Vao, int IndexCount, Texture Texture);
    private readonly List<RenderSection> _sections = [];
    private Vector3 _position;
    
    public void Build(Chunk chunk, Vector2i chunkPos, World world)
    {
        _position = new Vector3(chunkPos.X * Chunk.SizeX, 0, chunkPos.Y * Chunk.SizeZ);
        var mesh = ChunkMesh.BuildIndexedMesh(chunk, world, chunkPos);

        foreach (var section in mesh.Sections)
        {
            var vao = GL.GenVertexArray();
            var vbo = GL.GenBuffer();
            var ebo = GL.GenBuffer();
        
            GL.BindVertexArray(vao);
        
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, 
                section.Vertices.Count * Vertex.SizeInBytes, section.Vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vector3.SizeInBytes);
            GL.EnableVertexAttribArray(1);
        
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, 
                section.Indices.Count * sizeof(uint), section.Indices.ToArray(), BufferUsageHint.StaticDraw);
            
            _sections.Add(new RenderSection(vao, section.Indices.Count, section.Texture));
            Console.WriteLine($"[Build] Section {section.Texture.Handle}: V={section.Vertices.Count}, I={section.Indices.Count}");
        }
    }

    public void Render(Shader shader)
    {
        var model = Matrix4.CreateTranslation(_position);
        shader.SetMatrix4("model", model);
        
        foreach (var section in _sections)
        {
            section.Texture.Use(TextureUnit.Texture0);
            GL.BindVertexArray(section.Vao);
            GL.DrawElements(PrimitiveType.Triangles, section.IndexCount, DrawElementsType.UnsignedInt, 0);
        }
    }
}