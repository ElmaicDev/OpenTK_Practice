using System;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp; // librería que permite cargar imágenes a c#
using System.IO;

namespace OpenTK_Practica.Multiple_Texturas
{

    /// <summary>
    /// ESTA ES UNA CLASE PARA CARGAR Y CREAR TEXTURAS.
    /// </summary>
    public class Texture
    {
        public int Handle;

        public Texture(string imagePath)
        {
            // Esto generará una textura en blanco, vacía.
            Handle = GL.GenTexture();
            Use(); // el use  debe hacerse antes que teximage2D para indicarle a openGl con cual textura debería trabajar. 
            // stb_image carga desde el pixel de superior-izquierda, mientras que Opengl carga desde inferior-Izquierda, haciendo que se invierta la textura verticalmente.
            // Este comando corregirá esa inversión vertical.
            StbImage.stbi_set_flip_vertically_on_load(1); // indica que debe ir flipped para que se cargue bien.

            //Cargar la imagen
            ImageResult image = ImageResult.FromStream(File.OpenRead(imagePath), ColorComponents.RedGreenBlueAlpha);

            // Ahora se puede cargar la textura.
            /// Parámetros
            /// 1. Tipo de textura que será generado. Puedem ser en 1D, 2D y 3D pero es raro usar otra mas que 2D
            /// 2. El nivel de detalle. Si se tiene algo más que 0 se puede poner un mipmap por defecto como un nivel más bajo que el máximo.
            /// 3. El formato qu queremos que OpengGL almacenará los pixeles en la GPU.
            /// 4. Ancho de la imagen.
            /// 5. Alto de la imagen.
            /// 6. Borde de la imagen. Esto siempre debe ser 0, es un parámetro de versiones anciana de opengl.
            /// 7. Formato de los Bytes. ImageSharp siempre los pondrá en RGBa
            /// 8. Tipo de pixeles.
            /// 9. Arreglo de pixeles.
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);


            // Se pueden generar estos mipmaps pero no es necesario aquí, solo se puso por referencia.
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);


            // CON ESTO SE FINALIZA LA CREACIÓN DE LA TEXTURA. AHORA SE NECESITARÁ MODIFICAR LOS SHADERS Y LOS VERTICES PARA USAR LA TEXTURA.
            
        }

        public void Use(TextureUnit textureUnit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(textureUnit); // se debe activar la textura que se quiere bindear, porque ahora vamos a crear diferentes textura.
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}
