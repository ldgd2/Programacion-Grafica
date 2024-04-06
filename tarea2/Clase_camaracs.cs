using System;
using OpenTK;
using OpenTK.Input;

namespace OpenTK_Hola_Mundo__Televisor_
{
    public class Camera
    {
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Orientation { get; set; } = new Vector3((float)Math.PI, 0f, 0f);
        public float MoveSpeed { get; set; } = 0.2f;
        public float MouseSensitivity { get; set; } = 0.0025f;

        public Matrix4 GetViewMatrix()
        {
            Vector3 lookat = new Vector3();

            lookat.X = (float)(Math.Sin(Orientation.X) * Math.Cos(Orientation.Y));
            lookat.Y = (float)Math.Sin(Orientation.Y);
            lookat.Z = (float)(Math.Cos(Orientation.X) * Math.Cos(Orientation.Y));

            return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);
        }

        public void Move(float x, float y, float z)
        {
            Vector3 offset = new Vector3();

            Vector3 forward = new Vector3((float)Math.Sin(Orientation.X), 0, (float)Math.Cos(Orientation.X));
            Vector3 right = new Vector3(-forward.Z, 0, forward.X);

            offset += x * right;
            offset += y * forward;
            offset.Y += z;

            offset.NormalizeFast();
            offset *= MoveSpeed;

            Position += offset;
        }

        public void AddRotation(float x, float y)
        {
            x *= MouseSensitivity;
            y *= MouseSensitivity;

            float newX = (Orientation.X + x) % ((float)Math.PI * 2.0f);
            float newY = MathHelper.Clamp(Orientation.Y + y, -(float)Math.PI / 2.0f + 0.1f, (float)Math.PI / 2.0f - 0.1f);

            Orientation = new Vector3(newX, newY, Orientation.Z);
        }
    }
}







