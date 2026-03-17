using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK_Practica.Shaders;
using System.Diagnostics;

namespace OpenTK_Practica.OpenTK.Explicacion_Mas_Atributos
{
    public class Game : GameWindow
    {

        Shader Shader;


        private readonly float[] vertices =
        {
            // positions        // colors
            0.5f, -0.5f, 0.0f,  1.0f, 0.0f, 0.0f,   // bottom right
            -0.5f, -0.5f, 0.0f,  0.0f, 1.0f, 0.0f,   // bottom left
            0.0f,  0.5f, 0.0f,  0.0f, 0.0f, 1.0f    // top 
        };

        int EBO; // Element Buffer Object
        int VAO; // Vertex Array Object
        int VBO; // Vertex Buffer Object 


        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title }) { }

        protected override void OnLoad()
        {
            // con esto puedo ver la cantidad máxima de los shaders que podemos crear.
            int nrAttrib = 0;
            GL.GetInteger(GetPName.MaxVertexAttribs, out nrAttrib);
            Debug.WriteLine("Máximo número de atributos soportado:" + nrAttrib);

            // el flujo inicial es que en el onload definamos buffers y vertex arrays y en el onrender se dibujem llamando a los que ya están por medio de las vinculaciones.
            base.OnLoad();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1f);

            Shader = new Shader("shader.vert", "shader.frag");
            // Crear VBO
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Crear VAO
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0); // Acá el offset es 0, porque la positicón comienza desde la primer ordenada del buffer
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float)); // acá eloffset es 3 sizeof float porque comienza cuando termina la z en el array de posición
            GL.EnableVertexAttribArray(1);

            Shader.Use();

        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit); // Limpia la ventana usando el color definido en OnLoad. Es la primer función que siempre se debe llamar en el renderizado.

            Shader.Use();

            GL.BindVertexArray(VAO); // Creo que esto es importante para reconocer cuál es el VAO que se quiere dibujar.
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length);


            SwapBuffers();

        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0, 0, e.Width, e.Height); // Se llama esta función cada que el framebuffer se redimensiona. 

        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Shader.Dispose();
        }

    }
}
