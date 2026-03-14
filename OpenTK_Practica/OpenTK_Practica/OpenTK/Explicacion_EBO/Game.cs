using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK_Practica.OpenTK.Explicacion_VBO_VAO_SHADERS.Shaders;
using System.Runtime.InteropServices.Marshalling;

namespace OpenTK_Practica.OpenTK
{
    public class Game : GameWindow
    {

        Shader Shader;


        // como opengl solo funciona con triángulos, tocaría almacenar de esta forma para generar un rectángulo, haciendo bottom Right y bottomLeft dos veces en el arreglo.
        // float[] vertices = {
        //                    // first triangle
        //                    0.5f, 0.5f, 0.0f, // top right
        //                    0.5f, -0.5f, 0.0f, // bottom right
        //                    -0.5f, 0.5f, 0.0f, // top left
        //                    // second triangle
        //                    0.5f, -0.5f, 0.0f, // bottom right
        //                    -0.5f, -0.5f, 0.0f, // bottom left
        //                    -0.5f, 0.5f, 0.0f // top left
        //                };
        /// Una mejor solución para cuando se tengan miles de triángulos traslapados es guardar los vertices únicos y especificar en otro array cuál es el orden en el que queremos dibujar esos triángulos.

        float[] vertices = {
            0.5f, 0.5f, 0.0f, // top right
            0.5f, -0.5f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, // bottom left
            -0.5f, 0.5f, 0.0f // top left
        };

        // entero sin signo quita el bit reservado para el signo, por lo que duplica el rango de valores positivos
        uint[] indices = { // note that we start from 0!
            0, 1, 3, // first triangle
            1, 2, 3 // second triangle
        };

        ///  Cuando se usan índices se necesitan 4 vertices, en lugar de 6



        int EBO; // Element Buffer Object
        int VAO;
        int VBO;


        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title }) { }

        protected override void OnLoad()
        {
            // el flujo inicial es que en el onload definamos buffers y vertex arrays y en el onrender se dibujem llamando a los que ya están por medio de las vinculaciones.
            base.OnLoad();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1f);

            // Crear VBO
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Así generamos el buffer Object  //SIN EMBARGO ES IMPORTANTE TENER EN CUENTA QUE PARA PODER CREAR UN EBO SE DEBE TENER PRIMERO UN VAO

            EBO = GL.GenBuffer();

            // como en VBO, nosotros queremos enlazar EBO con GL.BufferData, y hacer las llamadas con bind y unbind.

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO); // se enlaza el element buffer object 
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StreamDraw);
            

        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit); // Limpia la ventana usando el color definido en OnLoad. Es la primer función que siempre se debe llamar en el renderizado.





            //Cualquier OpenGL context (Context.SwapBuffers) es conocido como doble búfer que son dos áreas en las que OpenGL dibuja, una es la que está siendo mostrada, y la otra es la que está siendo renderizada.
            // Cuando se llama el swap Buffers se reservan las dos áreas de dibujo para ser usadas.
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
