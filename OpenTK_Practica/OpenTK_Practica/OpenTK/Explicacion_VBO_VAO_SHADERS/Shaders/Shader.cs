using OpenTK.Graphics.OpenGL4;
using System.IO;
using System.Diagnostics;

namespace OpenTK_Practica.OpenTK.Explicacion_VBO_VAO_SHADERS.Shaders
{
    public class Shader : IDisposable
    {
        int Handle; // representa la ubicación final del shader después de que termina siendo compilado. 
        private bool disposedValue = false;
         
        /// <summary>
        /// Esta clase sirve para compilar en tiempo de ejecución los shaders que están definidos en el lenguaje de GL. 
        /// </summary>
        /// <param name="vertexPath"></param>
        /// <param name="fragmentPath"></param>
        public Shader(string vertexPath, string fragmentPath)
        {
            int VertexShader;
            int FragmentShader;

            string VertexShaderSource = File.ReadAllText(vertexPath); // Vertex Shader es el que se puede modificar de manera programática. Acá es el archivo .vert Al parecer los shaders los busca en debug.
            string FragmentShaderSource = File.ReadAllText(fragmentPath); // acá es el archivo .frag

            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            FragmentShader = GL.CreateShader (ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);


            GL.CompileShader(VertexShader);

            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int success);
            if(success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(VertexShader);
                Debug.WriteLine(infoLog);
            }

            GL.CompileShader(FragmentShader);
            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int success1);
            if(success1 == 0)
            {
                string infolog = GL.GetShaderInfoLog(FragmentShader);
                Debug.WriteLine(infolog);
            }


            // Hasta este momento los shaders han sido creados y compilados, pero para usarlos los tenemos que relaciones en un programa que pueda correr en la GPU. Y esta unión de los dos shaders individuales los llamamos SHADER.

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success2);
            
            if(success2 == 0)
            {
                string infolog = GL.GetProgramInfoLog(Handle);
                Debug.WriteLine(infolog);
            }
            // Esto es lo que se necesita para hacer el shader usable dentro del programa. Ahora después de salir del constructor se debe hacer una limpieza. Ya que se creó el programa vertex y fragment son inútiles- 
            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);

        }


        public void Use()
        {
            GL.UseProgram(Handle);
            // Por último, se debe limpiar el handle antes de que la clase muera.  Esto se hace implementando IDisposable
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);
                disposedValue = true;
            }
        }

        ~Shader()
        {
            if(disposedValue == false)
            {
                Debug.WriteLine("Filtración de recursos en la GPU! ¿OLVIDASTE LLAMAR DISPOSE()? ");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Permite encontrar la ubicación de un atributo, de manera que no es necesario hardcodear con la ubicación de un atributo en .vert
        /// </summary>
        /// <param name="attribName"></param>
        /// <returns></returns>
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }
    }
}




