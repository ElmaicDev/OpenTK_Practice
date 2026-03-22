# MÚLTIPLES TEXTURAS
En la carpeta de texturas, vimos que los shaders tienen un sampler2d que es precisamente un uniform. En dicho ejemplo, no se usó GL.Uniform* (como en la explicación de uniforms), esto es porque con GÑ.Uniform1 podemos asignar una valor de location al sampler para que podamos asignar múltiples exturas de una vez
en un solo fragment shader. Location de una textur a es comunmente conocida como texture unit. La unidad por defecto es 0. 

El propósito principal de texture unit es usar más de una textura en los shaders. Para asignar texture units a los samplers, podemos bindear m´ltiples textura de una mientras que se active la textura correspondiente, usando GL.BindTexture, y luego se activa la textura debida. 

```
GL.ActiveTexture(TextureUnit.Texture0) // activar la textura antes de bindearla
GL.BindTexture(TextureTarget.Texture2D, texture)

```


La unidad de textura Texture0 siempre está activa por defecto. OpenGL ofrece como mínimo 16 texture units Texture0 a Texture15

## Implementación en frag shader

```
uniform sampler2D texture1;
uniform sampler2D texture2;

void main()
{
    FragColor = mix(texture(texture1, TexCoord), texture(texture2, TexCoord), 0.2);
}

```

Él color final es la combinación de dos texturas. La función mix toma dos valores como input e interpola linealmente entre ellos basado en su tercer argumento. Por ejemplo, si el tecer valor es 0 retornará el color del primer input (texture1), pero si es 0.2 retornar+a el 80%
del color del primer input y el 20% del segundo input. Resultando en una mezcla de ambos colores. 