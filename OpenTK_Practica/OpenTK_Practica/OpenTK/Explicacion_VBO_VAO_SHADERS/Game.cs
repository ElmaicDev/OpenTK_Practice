using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK_Practica.OpenTK.Explicacion_VBO_VAO_SHADERS.Shaders;

namespace OpenTK_Practica.OpenTK.Explicacion_VBO_VAO_SHADERS
{
    public class Game : GameWindow
    {

        Shader Shader; // este es el compilador que hicimos manualmente para vertex y frag


        // OpenGL es una libería 3D entonces todas las coordenados que especifiquemos deben estar en x, y, y z. 
        // OpenGL solo procesa coordenadas 3D (A 2D) cuando los 3 ejes varían entre -1 y 1. Todas las coordenadas dentro de este rango (llamado Normalized Device Coordinates - NDC) 
        // terminarán siendo visibles en la plantalla, lo que se salga de esta región, no.

        // Tus coordenadas NDC serán transformadas a coordenadas de pantalla
        // mediante la transformación del viewport usando los datos que
        // proporcionaste con GL.Viewport. Las coordenadas de pantalla
        // resultantes se transforman luego en fragmentos como
        // entradas para tu fragment shader
        float[] vertices = {
                            -0.5f, -0.5f, 0.0f, //Bottom-left vertex
                            0.5f, -0.5f, 0.0f, //Bottom-right vertex
                            0.0f,  0.5f, 0.0f  //Top vertex
                        };

        int VertexBufferObject;

        int VertexArrayObject;
        // La información de los vertices se debe enviar como input al primer proceso del pipeline (flujo) de los gráficos: Vertex_Shader. Para eso se debe crear memoria en la gpu donde se almacena la info de los vértices,
        // se debe configurar cómo OpenGl debe interpertar la memoria y cómo enviar datos a la tarjeta gráfica.

        // Se maneja esto en memoria usando los llamados Vertex Buffer Objects (VBO) Que puede almacenar un gran número de vertices en la memoria de la GPU. La ventaja de esos buffers, es que se puede mandar
        // Grandes lotes de información de una vez a la tarjeta gráfica, sin enviar la información de un solo vertice cada vez.



        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title }) { }

        protected override void OnLoad()
        {
            // SE EJECUTA UNA VEZ CUANDO LA PÁGINA SE ABRE POR PRIMERA VEZ. Cualquier coódigo de inicialización se ejecuta acá.
            base.OnLoad();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1f); // Decide el color de renderizado entre frames, es decir, cuando se borra un frame para darle paso a otro frame, es el color en medio de los dos frames.

            VertexBufferObject = GL.GenBuffer(); // Genera el ID del buffer que es el espacio en la memoria de la GPU.


            // OpenGL tiene muchos tipos de BUFFERS, el específico de vertices es BufferTarget.ArrayBuffer. OpenGl permite enlazarnos a distintos buffers a la misma vez mientras tengan un diferente tipo de buffer. 
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);


            // A partir de acá, las llamadas que hagamos al buffer objetivo (Target) serán para configurar el buffer enlazado. Entonces se llama a GL.bufferData que copia la información previa de los vertices dentro de la memoria del buffer
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            // BufferUsageHint especifica cómo queremos que la tarjeta gráfica maneje la información dada, Puede tomar 3 formas:
            // StaticDraw: the data will most likely not change at all or very rarely.
            // DynamicDraw: the data is likely to change a lot.
            // StreamDraw: the data will change every time it is drawn.

            // NO HAY NECESIDAD DE ELIMINAR LOS BUFFERS PORQUE CUANDO EL PROGFRAMA TERMINA TODOS LOS RECURSOS SON LIBERADOS. Pero si se quisieran eliminar se podría usar:
            //                                                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //                                                    GL.DeleteBuffer(VertexBufferObject);

            // Binding a buffer to 0 basically sets it to null, so any calls that modify a buffer without binding one first will result in a crash. 
            // This is easier to debug than accidentally modifying a buffer that we didn't want modified.

            // DESPUÉS DE CONFIGURAR LOS BUFFERS, SE SIGUE CON LOS SHADERS.
            // Ahora creamos el pipeline gráfico con un vertex shader y un fragment shader.
            // OpenGL moderno requiere al menos estos dos shaders para poder renderizar. Los shaders se deben escribir en GLSL (OpenGL Shading Language) y luego com´pilar el shader para que se puede usar en nuestra app.

            Shader = new Shader("shader.vert", "shader.frag"); //Son las rutas, el lenguaje del shader se parece a C.


            //// Vertex Attributes
            /// el vertex shader permite especificar cualquier input que queramos en forma de atributos vertex. Eso significa quedebemos especificar manualmente qué parte de la input data le corresponde al vertex attribute en el vertex shader. Esto significa
            /// que tenemos que especificar a OpenGL cómo interpretar la información de vertez antes de renderizarla.
            /// Esta información es almacenada en algo llamado Vertex Array Object (VAO). VAO contiene información sobre el formato de vertex, y de qué buffers leer la info. 

            //int VertexArrayObject = GL.GenVertexArray();    
            //GL.BindVertexArray(VertexArrayObject);


            // acá se inicializa y se enlaza el VAO, con esto podemos dar el formato y la info de los buffers. 


            // Ahora se deben linkear los atributos, el buffer luce como 
            //                 VERTEX1                                    VERTEX2                                          VERTEX3
            //            X            Y             Z           X            Y             Z                X            Y             Z
            // BYTE:     |0|    |4|         |8|          |12|         |16|         |20|           |24|            |28|        |32|     |36|    
            // STRIDE:   ---------------------------|
            // OFFSET:   0     

            //

            // Con lo anterior en mente, podemos decirle a OpenGL cómo debería interpretar la información por atributo usando GL.VertexAttributePointer
            // Parámetro 1: Especicifica cuál vertex attribute queremos configurar. (en el archivo .vert configuramos con layout (location = 0). 
            // Parámetro 2: Especifica el tamaño del atributo vertex. Como es vec3 en el archivo .vert se compone de 3 valores.
            // Parámetro 3: Especifica el tipo de dato. 
            // Parámetro 4: Especifica si queremos la información normalizada. Como estamos con float, esto no es tan relevante. Con int sí lo es.
            // Parámetro 5: Es conocido como stride y dice cuánto debo saltar de un byte a otro para llegar al siguiente vértice, o sea el espacio entre vértices consecutivos., como sabemos que el espacio dentro del buffer es consegutivo, podemos poner el stride como 0 y por eso el espacio son 3 floats que serían x,y,z
            // Parámetro 6: Es el offse donde la posición de la información dentro del bufer empieza, Como la posición está al inicio del buffer array, es 0.
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            // Cada vertez saca la info del VertexBufferObject (VBO) y el VBO determinado para sacar la info es el que esté recientemente enlazado con ArrayBuffer cuando se llama VertexAttribute.

            // Parámetro 1: Es el vertex Location que tiene el archivo .vert
            GL.EnableVertexAttribArray(0);


            Shader.Use();
            // HASTA ACÁ YA TENDRÍAMOS TODO COMPLETAMENTE CONFIGURADO PARA USAR LOS SHADERS Y DEMÁS. SOLO FALTA LLAMAR LAS FUNCIONES PARA DIBUJAR DE OPENGL.


            ////// --------------------------------------------------------------------------- CONTINUACIÓN DE VAO  ---------------------------------------------------------///////////////////
            /// Vertex Array Object (VAO) puede ser enlazado como un vertex buffer y cualquier llamado a vertex attribute subsecuente de ahí en adelante será almacenado en el VAO. 
            /// Esto da la ventaja de que cuando configuramos los punteros de vertex attributes, solo tiene que hacer las llamadas una sola vez y cuando quieras dibujar un objeto, enlazamos el VAO correspondiente.
            /// CORE OPENGL REQUIERE QUE USEMOS VAO PARA SABE QU´´E HACER CON LOS VERTEX INPUTS. Si se falla en definir un VAO no va a dibujar nada.
            /// 

            /// Un VAO almacena lo siguiente:
            /// Llama a GL.DisableVertexAttribArray o GL.EnableVertexAttribArray
            /// Configuraciones via GL.VertexAttribPointer
            /// Vertex Buffer Objects (VBO) asociados con vertex attributes por medio de GL.VertexAttribPointer


            /// El proceso para generar un VAO es parecido al de VBO

            VertexArrayObject = GL.GenVertexArray();

            /// Para usar el VAO solo se neceita enlazar la información usando GL.BindVertexArray. Desde este punto se debe configura o enlazar los correspondientes VBO's y attribute pointers y luego
            /// desenlazar el VAO para usarlo más tarde, y cuando queramos dibujar un objeto solo llamamos el VAO que queramos usar. 

           
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit); // Limpia la ventana usando el color definido en OnLoad. Es la primer función que siempre se debe llamar en el renderizado.


            //////////////
            ///
            ////////// ---------------------------------------- EN EL RENDER SE DEBE PONER LA LÓGICA DE DIBUJO 
             
            /// 1. Vincular, o enlazar VAO
            GL.BindVertexArray(VertexArrayObject);

            /// 2. Copiar los vertices en el buffer para que OpenGL lo use.
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // 3. Configurar los pointer Attributes
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // 4. Usar el programa, según los shaders (esto lo debo confirmar yo)
            Shader.Use(); // Acá se llama GL.UserProgram()

            // 5. Enlazar el vertex Array. 
            GL.BindVertexArray(VertexArrayObject);

            // 6. Ejecutar una función que se encargue de dibujar. 
            // Parámetro 1: Tipo primitivo de lo que queremos dibujar,
            // Parámetro 2: El índice inicial del vertex array que nos gustaría iniciar a dibujar
            // parámetro 3:  Cantidad de vértices que queremos dibujar.
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);


            //////////////



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
