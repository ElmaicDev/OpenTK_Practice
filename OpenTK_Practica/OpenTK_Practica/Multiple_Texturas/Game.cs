using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK_Practica.Shaders;
using OpenTK_Practica.Texturas;
using System.Diagnostics;

namespace OpenTK_Practica.OpenTK.Multiple_Texturas
{
    public class Game : GameWindow
    {

        Shader Shader;

        // las texturas se añaden a cada vertices. 
        float[] vertices =
        {
            //Position          Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        };

        uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        int VBO;
        int VAO;
        int EBO;
        Texture Texture;
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
            Shader.Use();

            // Crear VAO
            VAO = GL.GenVertexArray();  // al parecer siempre es mejor crear el vao primero
            GL.BindVertexArray(VAO);

            // Crear VBO
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            

            // Debemos modificar el VAO porque ya tenemos son otra estruxtura para las coordenadas de la textura.
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0); // Acá el offset es 0, porque la positicón comienza desde la primer ordenada del buffer
            GL.EnableVertexAttribArray(0);


            // Crear EBO para indicar cuáles son los vertices con  los que se conformará la  estructura
            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);


            int textCoordLocation = Shader.GetAttribLocation("aTextCoord");
            GL.VertexAttribPointer(textCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float)); //esto extrae la coordenada de vertices.
            GL.EnableVertexAttribArray(textCoordLocation);


            // Ahora solo se modifican los shaders.

            Texture = new Texture("./wooden.jpg");



            // TEXTURAS -- VER MARKDOWN DE TEXTURAS.
            /// Parámetros 
            /// 1. Especifica la tectura objetivo. En este caso una textura 2D (imagen)
            /// 2. Especifica cua; option queremos de la imagen y los ejes, que sería S y T.
            /// 3. Por último nos pide el modo de wrapping y la ponemos en repeat.
            /// NOTA: Si escogemos TextureWrapMode.ClampToBorder, también debemos especificar el color del borde dentro del texparameter, por ejemplo: 
            /// float[] borderColor = { 1.0f, 1.0f, 0.0f, 1.0f };        ----Importante
            ///GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor); -----Importante

            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            //// Este código sirve para maximizar o minimizar una textura, magnify and minify  Ver las recomendaciones del md de texturas para saber porqué nearest o linear
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //// MIPMAPS --VER MARKDOWN DE TEXTURAS.  
            //// Es un error común poner un mipmap filtering como un filtro de magnificación porque mipmap solo es usado para objetos que son minimizados.
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);


        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit); // Limpia la ventana usando el color definido en OnLoad. Es la primer función que siempre se debe llamar en el renderizado.


            Shader.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            Texture.Use(); //Bindea la textura.
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);



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
