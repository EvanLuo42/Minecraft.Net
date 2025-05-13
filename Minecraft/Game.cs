using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Minecraft;

public class Game(int width, int height, string title) : GameWindow(GameWindowSettings.Default,
    new NativeWindowSettings { ClientSize = (width, height), Title = title })
{
    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private int _elementBufferObject;
    private Shader _shader = null!;
    private double _time;

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        _time += UpdateTime;

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        
        GL.Enable(EnableCap.DepthTest);

        float[] vertices =
        [
            -0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 0.0f,
            0.5f, -0.5f, 0.5f, 0.0f, 1.0f, 0.0f,
            0.5f, -0.5f, -0.5f, 0.0f, 0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, 0.0f, 1.0f, 1.0f,
            -0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f,
            0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f,
            0.5f, 0.5f, -0.5f, 0.0f, 0.0f, 1.0f,
            -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 1.0f,
        ];

        uint[] indices =
        [
            0, 1, 4,
            1, 4, 5,
            1, 5, 2,
            5, 6, 2,
            3, 2, 7,
            7, 2, 6,
            3, 4, 7,
            0, 3, 4,
            0, 1, 3,
            3, 1, 2,
            4, 5, 7,
            5, 6, 7
        ];

        _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

        _vertexBufferObject = GL.GenBuffer();
        _vertexArrayObject = GL.GenVertexArray();
        _elementBufferObject = GL.GenBuffer();

        GL.BindVertexArray(_vertexArrayObject);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices,
            BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _shader.Use();

        var deltaY = (float) Math.Sin(_time) * 0.25f;
        var model = Matrix4.Identity * Matrix4.CreateTranslation(0.0f, deltaY, 0.0f) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(20 * (float) _time));
        var view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
        var projection =
            Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Size.X / (float) Size.Y, 0.1f,
                100.0f);
        var modelLocation = GL.GetUniformLocation(_shader.Handle, "model");
        var projectionLocation = GL.GetUniformLocation(_shader.Handle, "projection");
        var viewLocation = GL.GetUniformLocation(_shader.Handle, "view");
        GL.UniformMatrix4(modelLocation, true, ref model);
        GL.UniformMatrix4(projectionLocation, true, ref projection);
        GL.UniformMatrix4(viewLocation, true, ref view);

        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);

        SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
    }
}