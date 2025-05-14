using OpenTK.Mathematics;

namespace Minecraft;

public class Camera(Vector3 position, float aspectRatio)
{
    private Vector3 _front = -Vector3.UnitZ;

    private float _pitch;
    
    private float _yaw = -MathHelper.PiOver2;
    
    private float _fov = MathHelper.PiOver2;
    
    public Vector3 Position { get; set; } = position;
    
    public float AspectRatio { get; set; } = aspectRatio;

    public Vector3 Front => _front;

    public Vector3 Up { get; private set; } = Vector3.UnitY;

    public Vector3 Right { get; private set; } = Vector3.UnitX;
    
    public float Pitch
    {
        get => MathHelper.RadiansToDegrees(_pitch);
        set
        {
            var angle = MathHelper.Clamp(value, -89f, 89f);
            _pitch = MathHelper.DegreesToRadians(angle);
            UpdateVectors();
        }
    }
    
    public float Yaw
    {
        get => MathHelper.RadiansToDegrees(_yaw);
        set
        {
            _yaw = MathHelper.DegreesToRadians(value);
            UpdateVectors();
        }
    }
    
    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set
        {
            var angle = MathHelper.Clamp(value, 1f, 90f);
            _fov = MathHelper.DegreesToRadians(angle);
        }
    }
    
    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Position + _front, Up);
    }
    
    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
    }
    
    private void UpdateVectors()
    {
        _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
        _front.Y = MathF.Sin(_pitch);
        _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);
        
        _front = Vector3.Normalize(_front);
        
        Right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(Right, _front));
    }
}