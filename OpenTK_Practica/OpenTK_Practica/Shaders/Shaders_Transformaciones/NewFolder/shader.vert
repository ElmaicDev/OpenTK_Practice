#version 330 core 

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTextCoord;

out vec2 texCoord; // output la coordenada de la textura

// Se añade un uniform para la matriz de transformación
uniform mat4 transform; 
// es de cuatro porque es una matriz de transofrmación 3D con coordenadas homogéneas

void main()
{
	texCoord = aTextCoord;
	gl_Position = transform * vec4(aPosition, 1.0); 
	// se multiplica la posición del vértice por la matriz de transformación para aplicar las transformaciones a los vértices
	// la multiplicación del vector por la matriz se hace en este orden porque vec4 se está tratando como un vector fila , y la matriz se multiplica a la derecha. Si se tratara como un vector columna, la multiplicación sería al revés (transform * vec4(aPosition, 1.0)).
}
