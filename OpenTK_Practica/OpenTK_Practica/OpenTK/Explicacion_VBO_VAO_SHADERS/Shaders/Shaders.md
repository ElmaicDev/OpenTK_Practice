# SHADERS 
Los shaders siempre empiezan con una declaración de la versión seguido de una lista de inputs y output, uniforms (que más adelantes
se explica qué es un uniform). Cada shader tiene un entrypoint en su función *main*
donde procesamos cualquier variable de entrada y devolvemos las variables de salida.
Típicamente los shaders son mostrados como a continuación: 

```
#version version_number
cin type in_variable_name;
in type in_variable_name;
out type out_variable_name;
  
uniform type uniform_name;
  
void main()
{
  // process input(s) and do some weird graphics stuff
  ...
  // output processed stuff to output variable
  out_variable_name = weird_stuff_we_processed;
}
```

Cada input variable en un shader es un atributo, y hay un máximo 
permitidos limitado por el hardware. OpenGL garantiza que siempre tendrás disponibles al menos 16 atributos de vértice, y cada uno puede tener hasta 4 componentes.

Los 4 componentes — ¿qué significa?
Cada atributo puede ser como máximo un vec4, es decir, 4 valores flotantes

Se puede saber la cantidad máxima con GL_MAX_VERTEX_ATTRIBS._



# TYPES
Tiene la mayoría de tipos que C: int, float, double, uint y bool. Vectores y matrices.

# VECTORS
En GLSL un vector es un contenedor de 1, 2, 3 o 4 componentes de los tipos básicos mencionados anteriormente.

- vecn: vector de n floats
- bvecn: vector de n booleanos
- ivecn: vector de n integers 
- uvecn: vector de n integers sin signo
- dvecn: vector de n doubles

Generalmente los floats son suficientes para la mayoría de los casos.

Los componentes de un vector pueden accederse através de vect.x o .y o .z.
y.w para acceder a los primeros, segundos, terceros y cuarto componente respectivamente. Y permite rgba para los colores o stpq para las texturas.

El vector permite una selección llamada swizzling, para sintax como:
- vec2 someVec;
- vec4 differentVec = someVec.xyxx;
- vec3 anotherVec = differentVec.zyw;
- vec4 otherVec = someVec.xxxx + anotherVec.yxzy

Se puede usar cualquier combinación de 4 letras para crear un nuevo vector (del mismo tipo) 
mientras el vector original tenga esos componentes. Por ejemplo no se puede acceder .z de un vec2.
Y se pueden pasar vectores como argumentos de diferentes llamadas de constructor:

- vec2 vect = vec2(0.5, 0.7);
- vec4 result = vec4(vect, 0.0, 0.0);
- vec4 otherResult = vec4(result.xyz, 1.0);

# Ins and Outs 

Los shaders son pequeños programas por sí solos. Cada shader puede especificar inputs y outputs con las keywords,
y cuando un input y un output coincidan, se pasarán al otro componente.

El vertex shader recibe sus datos directamente desde los vértices que defines en el CPU. Para que sepa cómo leer esos datos, cada variable in necesita un número de location, así:
glsllayout (location = 0) in vec3 aPosition;
Ese location = 0 es el que usas desde C# para enlazar el atributo con el VBO.

el fragment shader requiere un vec4 color output variable, desde que necesita para generar un color final. Si se falla con 
los colores, se renderiza en blanco o negro.

Si queremos pasar datos de un shader a otro, declaramos un out en el primero y un in en el segundo. Cuando el tipo y el nombre coinciden en ambos lados, OpenGL los enlaza automáticamente al momento de linkear el programa.