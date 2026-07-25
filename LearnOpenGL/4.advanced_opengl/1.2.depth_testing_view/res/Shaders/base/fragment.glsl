#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D texture1;

float near = 0.1f;
float far = 100.0f;

float LinearizeDepth(float depth)
{
    float z = depth * 2.0f - 1.0f; // voltar para a NDC

    return (2.0f * near * far) / (far + near - z * (far - near));
}

void main()
{    
    FragColor = texture(texture1, TexCoords);

    // FragColor = vec4(vec3(gl_FragCoord.z), 1.0);

    // float depth = LinearizeDepth(gl_FragCoord.z) / far; // divide por um valor grande para fins de demonstração
    // FragColor = vec4(vec3(depth), 1.0f);
}
