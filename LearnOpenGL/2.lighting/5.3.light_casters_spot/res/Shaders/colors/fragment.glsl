#version 330 core
out vec4 FragColor;
  
uniform vec3 objectColor;
uniform vec3 lightColor;

in vec2 TexCoords;
in vec3 FragPos;
in vec3 Normal;

uniform vec3 lightPos;
uniform vec3 viewPos;

struct Light
{
    vec3 position;
    vec3 direction;
    float cutOff;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadractic;
};

uniform Light light;

struct Material
{
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
};

uniform Material material;

void main()
{
    vec3 lightDir = normalize(light.position - FragPos);

    // verifica se a iluminação está dentro do cone do holofote
    float theta = dot(lightDir, normalize(-light.direction));

    if (theta > light.cutOff) // Lembre-se de que estamos trabalhando com ângulos na forma de cossenos, em vez de graus; por isso, utiliza-se '>'.
    {
        // realizar cálculos de iluminação

        // ambient
        vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));

        // diffuse
        vec3 norm = normalize(Normal);
        // vec3 lightDir = normalize(light.position - FragPos);
        float diff = max(dot(norm, lightDir), 0.0f);
        vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));

        // specular
        vec3 viewDir = normalize(viewPos - FragPos);
        vec3 reflextDir = reflect(-lightDir, norm);
        float spec = pow(max(dot(viewDir, reflextDir), 0.0f), material.shininess);
        vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));

        float distance = length(light.position - FragPos);
        float attenuation = 1.0f / (light.constant + light.linear * distance + light.quadractic * (distance * distance));

        ambient *= attenuation;
        diffuse *= attenuation;
        specular *= attenuation;

        vec3 result = ambient + diffuse + specular;
        FragColor = vec4(result, 1.0f);
    }
    else // caso contrário, use luz ambiente para que a cena não fique totalmente escura fora do foco de luz.
    {
        FragColor  = vec4(light.ambient * vec3(texture(material.diffuse, TexCoords)), 1.0f);
    }
}
