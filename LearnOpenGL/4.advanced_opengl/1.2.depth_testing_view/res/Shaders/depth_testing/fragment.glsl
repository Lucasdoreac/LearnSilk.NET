#version 330 core
out vec4 FragColor;

float near = 0.1; 
float far = 100.0; 
float LinearizeDepth(float depth) 
{
    float z = depth * 2.0 - 1.0; // voltar para a NDC
    return (2.0 * near * far) / (far + near - z * (far - near));	
}

void main()
{    
    float depth = LinearizeDepth(gl_FragCoord.z) / far; // divide por 'far' para obter a profundidade no intervalo [0, 1] para fins de visualização
    FragColor = vec4(vec3(depth), 1.0);
}
