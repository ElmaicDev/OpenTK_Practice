#version 330 core
out vec4 FragColor;
in vec2 texCoord;

uniform sampler2D texture0; // Sampler2D es una representación de la textura en shaders.

void main()
{
    FragColor = texture(texture0, texCoord);
}




