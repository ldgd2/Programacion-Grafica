using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenTK_Hola_Mundo__Televisor_
{
    public class Game : GameWindow
    {
        List<Clase_televisor> televisores;
        int vbo_position, vbo_color, ibo_elements;
        int shaderProgram;
        int attribute_vpos, attribute_vcol;
        int uniform_mview;
        Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 3.0f);
        float cameraSpeed = 0.05f;
        float cameraSensitivity = 0.001f;
        float pitch = 0.0f;
        float yaw = -MathHelper.PiOver2;
        private Vector3 previousMousePosition;

        public Game() : base(800, 600, GraphicsMode.Default, "3D Television")
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(1.0f, 1.0f, 1.0f, 0.0f);

            televisores = new List<Clase_televisor>();

            // Generar Televisores
            televisores.Add(new Clase_televisor(new Vector3(0.15f, 0.10f, -1.0f), vbo_position, vbo_color, ibo_elements));
            televisores.Add(new Clase_televisor(new Vector3(3.1f, 2.0f, 1.50f), vbo_position, vbo_color, ibo_elements));
            televisores.Add(new Clase_televisor(new Vector3(3.1f, 0.0f, 1.0f), vbo_position, vbo_color, ibo_elements));
            televisores.Add(new Clase_televisor(new Vector3(0.5f, 5.0f, 1.0f), vbo_position, vbo_color, ibo_elements));
            foreach (var televisor in televisores)
            {
                televisor.InicializarBuffers();
            }
            // Compilar shaders
            int vertShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertShader, @"
                #version 330 core
                layout(location = 0) in vec3 vPos;
                layout(location = 1) in vec3 vCol;
                out vec3 color;
                uniform mat4 modelview;
                void main()
                {
                    color = vCol;
                    gl_Position = modelview * vec4(vPos, 1.0);
                }
            ");
            GL.CompileShader(vertShader);

            int fragShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragShader, @"
                #version 330 core
                in vec3 color;
                out vec4 FragColor;
                void main()
                {
                    FragColor = vec4(color, 1.0);
                }
            ");
            GL.CompileShader(fragShader);

            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertShader);
            GL.AttachShader(shaderProgram, fragShader);
            GL.LinkProgram(shaderProgram);

            attribute_vpos = GL.GetAttribLocation(shaderProgram, "vPos");
            attribute_vcol = GL.GetAttribLocation(shaderProgram, "vCol");
            uniform_mview = GL.GetUniformLocation(shaderProgram, "modelview");

            // Crear buffers
            vbo_position = GL.GenBuffer();
            vbo_color = GL.GenBuffer();
            ibo_elements = GL.GenBuffer();


        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            ProcessInput();

            Matrix4 view = Matrix4.CreateRotationX(pitch) * Matrix4.CreateRotationY(yaw) * Matrix4.CreateTranslation(-cameraPosition);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Width / (float)Height, 0.1f, 100f);
            Matrix4 modelview = view * projection;

            GL.UseProgram(shaderProgram);
            GL.UniformMatrix4(uniform_mview, false, ref modelview);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (var televisor in televisores)
            {
                televisor.Dibujar();
            }

            // Verificar errores de OpenGL
            var error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Console.WriteLine($"Error de OpenGL en OnRenderFrame: {error}");
            }

            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            GL.DeleteBuffers(1, ref vbo_position);
            GL.DeleteBuffers(1, ref vbo_color);
            GL.DeleteBuffers(1, ref ibo_elements);
            GL.DeleteProgram(shaderProgram);
            foreach (var televisor in televisores)
            {
                televisor.LimpiarBuffers();
            }
        }

        private void ProcessInput()
        {
            var keyboardState = OpenTK.Input.Keyboard.GetState();
            var currentMouseState = OpenTK.Input.Mouse.GetState();
            var currentMousePosition = new Vector3(currentMouseState.X, currentMouseState.Y, 0); // La coordenada Z puede representar cualquier otra cosa que necesites

            var mouseDelta = currentMousePosition - previousMousePosition;

            if (keyboardState.IsKeyDown(OpenTK.Input.Key.W))
            {
                cameraPosition += cameraSpeed * new Vector3((float)Math.Sin(yaw), 0, (float)Math.Cos(yaw));
            }
            if (keyboardState.IsKeyDown(OpenTK.Input.Key.S))
            {
                cameraPosition -= cameraSpeed * new Vector3((float)Math.Sin(yaw), 0, (float)Math.Cos(yaw));
            }
            if (keyboardState.IsKeyDown(OpenTK.Input.Key.D))
            {
                cameraPosition -= cameraSpeed * new Vector3((float)Math.Cos(yaw), 0, -(float)Math.Sin(yaw));
            }
            if (keyboardState.IsKeyDown(OpenTK.Input.Key.A))
            {
                cameraPosition += cameraSpeed * new Vector3((float)Math.Cos(yaw), 0, -(float)Math.Sin(yaw));
            }
            if (keyboardState.IsKeyDown(OpenTK.Input.Key.Space))
            {
                cameraPosition += cameraSpeed * Vector3.UnitY;
            }
            if (keyboardState.IsKeyDown(OpenTK.Input.Key.ControlLeft))
            {
                cameraPosition -= cameraSpeed * Vector3.UnitY;
            }

            // var mouseDelta = Mouse.GetCursorDelta();
            previousMousePosition = currentMousePosition;
            yaw += mouseDelta.X * cameraSensitivity;
            pitch -= mouseDelta.Y * cameraSensitivity;

            // Clamp pitch to avoid flipping upside down
            //  pitch = MathHelper.Clamp(pitch, -MathHelper.PiOver2 + 0.1f, MathHelper.PiOver2 - 0.1f);
        }
    }
}







