using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using static System.GC;

namespace Minecraft;

public sealed class Shader : IDisposable
{
    public readonly int Handle;

    private bool _disposed;
    private readonly Dictionary<string, int> _uniformLocations = new();

    public Shader(string vertexPath, string fragmentPath)
    {
        var vertexShaderSource = File.ReadAllText(vertexPath);
        var fragmentShaderSource = File.ReadAllText(fragmentPath);
        
        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        
        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        
        GL.CompileShader(vertexShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out var vertexSuccess);
        if (vertexSuccess == 0)
        {
            var infoLog = GL.GetShaderInfoLog(vertexShader);
            throw new ShaderCompileException(infoLog);
        }
        
        GL.CompileShader(fragmentShader);
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out var fragmentSuccess);
        if (fragmentSuccess == 0)
        {
            var infoLog = GL.GetShaderInfoLog(fragmentShader);
            throw new ShaderCompileException(infoLog);
        }

        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);
        
        GL.LinkProgram(Handle);
        
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out var programSuccess);
        if (programSuccess == 0)
        {
            var infoLog = GL.GetProgramInfoLog(Handle);
            throw new ShaderProgramException(infoLog);
        }
        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
        
        GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
        for (var i = 0; i < numberOfUniforms; i++)
        {
            var key = GL.GetActiveUniform(Handle, i, out _, out _);
            
            var location = GL.GetUniformLocation(Handle, key);
            
            _uniformLocations.Add(key, location);
        }
    }
    
    public void Use()
    {
        GL.UseProgram(Handle);
    }
    
    public void SetMatrix4(string name, Matrix4 data)
    {
        GL.UseProgram(Handle);
        GL.UniformMatrix4(_uniformLocations[name], true, ref data);
    }

    ~Shader()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        GL.DeleteProgram(Handle);
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        SuppressFinalize(this);
    }
}

[Serializable]
public class ShaderCompileException : Exception
{
    public ShaderCompileException(string message) : base(message) {}
    public ShaderCompileException(string message, Exception inner) : base(message, inner) {}
}

[Serializable]
public class ShaderProgramException : Exception
{
    public ShaderProgramException(string message) : base(message) {}
    public ShaderProgramException(string message, Exception inner) : base(message, inner) {}
}
