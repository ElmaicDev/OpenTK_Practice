#version 330 core
out vec4 FragColor;

void main()
{
    FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
}




As you can see, GLSL looks similar to C. Each shader begins with a declaration of its version.
Since OpenGL 3.3 and higher the version numbers of GLSL match the version of OpenGL (GLSL
version 420 corresponds to OpenGL version 4.2 for example). We also explicitly mention we’re
using core profile functionality.
Next we declare all the input vertex attributes in the vertex shader with the in keyword. Right
now we only care about position data so we only need a single vertex attribute. GLSL has a vector
datatype that contains 1 to 4 floats based on its postfix digit. Since each vertex has a 3D coordinate
we create a vec3 input variable with the name aPos. We also specifically set the location of the
input variable via layout (location = 0) and you’ll later see that why we’re going to need
that location.
Vector In graphics programming we use the mathematical concept of a vector quite often,
since it neatly represents positions/directions in any space and has useful mathematical
properties. A vector in GLSL has a maximum size of 4 and each of its values can
be retrieved via vec.x, vec.y, vec.z and vec.w respectively where each of them
represents a coordinate in space. Note that the vec.w component is not used as a position
in space (we’re dealing with 3D, not 4D) but is used for something called perspective
division. We’ll discuss vectors in much greater depth in a later chapter.
To set the output of the vertex shader we have to assign the position data to the predefined
gl_Position variable which is a vec4 behind the scenes. At the end of the main function,
whatever we set gl_Position to will be used as the output of the vertex shader. Since our input
is a vector of size 3 we have to cast this to a vector of size 4. We can do this by inserting the vec3
values inside th