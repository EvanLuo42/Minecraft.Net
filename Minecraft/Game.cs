using Minecraft.Blocks;
using Minecraft.Render;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Minecraft;

public class Game(int width, int height, string title) : GameWindow(GameWindowSettings.Default,
    new NativeWindowSettings { ClientSize = (width, height), Title = title, NumberOfSamples = 4 })
{
    private Shader _shader = null!;
    private double _time;
    private World _world = null!;
    private Camera _camera = null!;
    private bool _firstMove = true;
    private bool _pause;

    private Vector2 _lastPos;

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        _time += UpdateTime;

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            _pause = !_pause;
        }

        if (_pause)
        {
            CursorState = CursorState.Normal;
            return;
        }

        CursorState = CursorState.Grabbed;

        const float cameraSpeed = 4.5f;
        const float sensitivity = 0.2f;

        if (KeyboardState.IsKeyDown(Keys.W))
        {
            _camera.Position += _camera.Front * cameraSpeed * (float)e.Time;
        }

        if (KeyboardState.IsKeyDown(Keys.S))
        {
            _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time;
        }

        if (KeyboardState.IsKeyDown(Keys.A))
        {
            _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time;
        }

        if (KeyboardState.IsKeyDown(Keys.D))
        {
            _camera.Position += _camera.Right * cameraSpeed * (float)e.Time;
        }

        if (KeyboardState.IsKeyDown(Keys.Space))
        {
            _camera.Position += _camera.Up * cameraSpeed * (float)e.Time;
        }

        if (KeyboardState.IsKeyDown(Keys.LeftShift))
        {
            _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time;
        }

        if (_firstMove)
        {
            _lastPos = new Vector2(MouseState.X, MouseState.Y);
            _firstMove = false;
        }
        else
        {
            var deltaX = MouseState.X - _lastPos.X;
            var deltaY = MouseState.Y - _lastPos.Y;
            _lastPos = new Vector2(MouseState.X, MouseState.Y);

            _camera.Yaw += deltaX * sensitivity;
            _camera.Pitch -= deltaY * sensitivity;
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        _camera.Fov -= e.OffsetY;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.StencilTest);
        GL.ClearColor(Color4.SkyBlue);
        GL.Enable(EnableCap.Multisample);

        BlockRegistration.RegisterBlocks();
        _world = new World();

        _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

        _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        _shader.Use();
        
        _shader.SetMatrix4("view", _camera.GetViewMatrix());
        _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
        
        foreach (var chunk in _world.GetVisibleChunks(_camera.Position))
        {
            chunk.Render(_shader);
        }

        SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
        _camera.AspectRatio = e.Width / (float)e.Height;
    }
}