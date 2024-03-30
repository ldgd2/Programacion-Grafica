using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OpenTK_Hola_Mundo__Televisor_
{
    public class tv : GameWindow
    {
        float[] vertdata;
        Vector3[] coldata;
        int[] indicedata;
        int vbo_position, vbo_color, ibo_elements;
        int shaderProgram;
        int attribute_vpos, attribute_vcol;
        int uniform_mview;

        float time = 0.0f;

        public tv() : base(800, 600, GraphicsMode.Default, "3D Television")
        {
            VSync = VSyncMode.On;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(0f, 0f, 0f, 0f);

            vertdata = new float[]
 {
    -0.55f, 0.25f, -0.0f,
    0.55f, 0.25f, -0.00f,
    0.55f, -0.25f, -0.0f,
    -0.55f, -0.25f, -0.0f,

    -0.6f, 0.4f, 0.1f,
    0.6f, 0.4f, 0.1f,
    0.6f, -0.4f, 0.1f,
    -0.6f, -0.4f, 0.1f,

    0.6f, 0.4f, 0.1f,
    0.6f, -0.4f, 0.1f,
    0.6f, -0.4f, -0.1f,
    0.6f, 0.4f, -0.1f,

    -0.6f, 0.4f, 0.1f,
    -0.6f, 0.4f, -0.1f,
    -0.6f, -0.4f, -0.1f,
    -0.6f, -0.4f, 0.1f,

    -0.6f, 0.4f, -0.1f,
    0.6f, 0.4f, -0.1f,
    0.6f, -0.4f, -0.1f,
    -0.6f, -0.4f, -0.1f,

    -0.6f, 0.4f, 0.1f,
    0.6f, 0.4f, 0.1f,
    0.6f, 0.4f, -0.1f,
    -0.6f, 0.4f, -0.1f,

    -0.6f, -0.4f, 0.1f,
    0.6f, -0.4f, 0.1f,
    0.6f, -0.4f, -0.1f,
    -0.6f, -0.4f, -0.1f,

    -0.2f, -0.5f, 0.2f,
    0.2f, -0.5f, 0.2f,
    0.2f, -0.6f, 0.2f,
    -0.2f, -0.6f, 0.2f,

    -0.2f, -0.5f, -0.2f,
    0.2f, -0.5f, -0.2f,
    0.2f, -0.6f, -0.2f,
    -0.2f, -0.6f, -0.2f,

    -0.2f, -0.5f, 0.2f,
    -0.2f, -0.5f, -0.2f,
    -0.2f, -0.6f, -0.2f,
    -0.2f, -0.6f, 0.2f,

    0.2f, -0.5f, 0.2f,
    0.2f, -0.5f, -0.2f,
    0.2f, -0.6f, -0.2f,
    0.2f, -0.6f, 0.2f,

    0.0f, 0.4f, 0.0f,
    0.05f, 0.4f, 0.0f,
    0.05f, -0.5f, 0.0f,
    0.0f, -0.5f, 0.0f,

    0.05f, 0.4f, 0.0f,
    0.05f, -0.5f, 0.0f,
    0.05f, -0.5f, -0.05f,
    0.05f, 0.4f, -0.05f,

    0.0f, 0.4f, 0.0f,
    0.0f, 0.4f, -0.05f,
    0.0f, -0.5f, -0.05f,
    0.0f, -0.5f, 0.0f,

    0.0f, 0.4f, -0.05f,
    0.05f, 0.4f, -0.05f,
    0.05f, -0.5f, -0.05f,
    0.0f, -0.5f, -0.05f
 };


          coldata = new Vector3[vertdata.Length / 3];
            for (int i = 0; i < coldata.Length; i++)
                coldata[i] = new Vector3(1.0f, 1.0f, 1.0f);

            indicedata = new int[]
            {
    0, 1, 2, 2, 3, 0,
    4, 5, 6, 6, 7, 4,
    8, 9, 10, 10, 11, 8,
    12, 13, 14, 14, 15, 12,
    16, 17, 18, 18, 19, 16,
    20, 21, 22, 22, 23, 20,
    24, 25, 26, 26, 27, 24,
    28, 29, 30, 30, 31, 28,
    32, 33, 34, 34, 35, 32,
    36, 37, 38, 38, 39, 36,
    40, 41, 42, 42, 43, 40,
    44, 45, 46, 46, 47, 44,
    48, 49, 50, 50, 51, 48,
    52, 53, 54, 54, 55, 52,
    56, 57, 58, 58, 59, 56
            };

           
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
            Console.WriteLine(GL.GetShaderInfoLog(vertShader));

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
            Console.WriteLine(GL.GetShaderInfoLog(fragShader));

            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertShader);
            GL.AttachShader(shaderProgram, fragShader);
            GL.LinkProgram(shaderProgram);
            Console.WriteLine(GL.GetProgramInfoLog(shaderProgram));

            attribute_vpos = GL.GetAttribLocation(shaderProgram, "vPos");
            attribute_vcol = GL.GetAttribLocation(shaderProgram, "vCol");
            uniform_mview = GL.GetUniformLocation(shaderProgram, "modelview");
            vbo_position = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData(BufferTarget.ArrayBuffer, vertdata.Length * sizeof(float), vertdata, BufferUsageHint.StaticDraw);

            vbo_color = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_color);
            GL.BufferData(BufferTarget.ArrayBuffer, coldata.Length * Vector3.SizeInBytes, coldata, BufferUsageHint.StaticDraw);

            ibo_elements = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indicedata.Length * sizeof(int)), indicedata, BufferUsageHint.StaticDraw);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            time += (float)e.Time;

            // Crear la matriz de transformación para la rotación
            float rotationAngle = time; // Ángulo de rotación
            Matrix4 rotationMatrix = Matrix4.CreateFromAxisAngle(Vector3.UnitY, rotationAngle);

            //  transformaciones de rotación, traslación y proyección
            Matrix4 modelview = rotationMatrix *
                           Matrix4.CreateTranslation(0.20f, -0.05f, -3.0f) *
                           Matrix4.CreatePerspectiveFieldOfView(1.3f, ClientSize.Width / (float)ClientSize.Height, 1.0f, 40.0f);

            GL.UseProgram(shaderProgram);
            GL.UniformMatrix4(uniform_mview, false, ref modelview);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.EnableVertexAttribArray(attribute_vpos);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexAttribArray(attribute_vcol);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_color);
            GL.VertexAttribPointer(attribute_vcol, 3, VertexAttribPointerType.Float, true, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
            GL.DrawElements(BeginMode.Triangles, indicedata.Length, DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(attribute_vpos);
            GL.DisableVertexAttribArray(attribute_vcol);

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
        }
    }
}

