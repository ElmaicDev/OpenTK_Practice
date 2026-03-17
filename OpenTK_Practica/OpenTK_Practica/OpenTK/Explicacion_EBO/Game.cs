using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK_Practica.Shaders;
using System.Diagnostics;

namespace OpenTK_Practica.OpenTK.Explicacion_EBO
{
    public class Game : GameWindow
    {

        Shader Shader;
        Stopwatch _timer = Stopwatch.StartNew(); // Esto es un cronómetro, por eso es que si está dentro de On Render no funciona, porque se reinicia en cada iteración.

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
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);


            // Así generamos el buffer Object  //SIN EMBARGO ES IMPORTANTE TENER EN CUENTA QUE PARA PODER CREAR UN EBO SE DEBE TENER PRIMERO UN VAO
            // Entonces el flujo sería ---> VBO ----> VAO ----> EBO
            EBO = GL.GenBuffer();

            // como en VBO, nosotros queremos enlazar EBO con GL.BufferData, y hacer las llamadas con bind y unbind.

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO); // se enlaza el element buffer object 
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StreamDraw);

            Shader.Use();

        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit); // Limpia la ventana usando el color definido en OnLoad. Es la primer función que siempre se debe llamar en el renderizado.

            Shader.Use();

            // 1- Obtenemos el tiempo en segundos con stopWatch. Luego variamos el rango de color de 0 - 1, usando seno y ese valor se almacena en greenValue
            
            double timeValue = _timer.Elapsed.TotalSeconds;

            float greenValue = (float)Math.Sin(timeValue) / 2.0f + 0.5f;

            // 2- Buscamos la ubicación del uniform dentro del shader, con GetUniformLocation, con el handle del shader en el que queremos buscar el objeto
            int vertexColorLocation = GL.GetUniformLocation(Shader.Handle, "ourColor");

            // Luego, seteamos el valor del uniform usando GL.Uniform4.  Encontrar el getUniformLocation no necesite el useProgram
            // pero agregar el color definitivamente lo necesita.
            //Diferentes uniforms que se pueden definir: 
            // 1- f: espera un float como valor.
            // 2- i: espera un int como valor
            // 3- ui: espera un uint como valor
            // 4- 3f:espera 3 floats como valor 
            // 5- fv: espera un vector/array float como valor.
            GL.Uniform4(vertexColorLocation, 0.0f, greenValue, 0.0f, 1.0f);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length);

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
