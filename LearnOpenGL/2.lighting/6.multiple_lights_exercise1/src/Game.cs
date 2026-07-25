using System.Numerics;
using LearnSilkNET.Inputs;
using LearnSilkNET.Utilities;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace LearnSilkNET;

public class Game
{
    private GL _gl = Program.GL;

    private Shader _lightingShader;
    private Shader _lightCubeShader;

    private Camera _camera;

    private uint _diffuseMap;
    private uint _specularMap;

    // configurar dados de vértice (e buffer(s)) e configurar atributos de vértice
    // --------------------------------------------------
    private float[] _vertices =
    {
        // positions           // normals             // texture coords
        -0.5f, -0.5f, -0.5f,   -1.0f,  0.0f,  0.0f,   0.0f, 0.0f,
        -0.5f, -0.5f,  0.5f,   -1.0f,  0.0f,  0.0f,   1.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,   -1.0f,  0.0f,  0.0f,   1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,   -1.0f,  0.0f,  0.0f,   0.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,   -1.0f,  0.0f,  0.0f,   1.0f, 1.0f,
        -0.5f,  0.5f, -0.5f,   -1.0f,  0.0f,  0.0f,   0.0f, 1.0f,
        
         0.5f, -0.5f,  0.5f,    1.0f,  0.0f,  0.0f,   0.0f, 0.0f,
         0.5f, -0.5f, -0.5f,    1.0f,  0.0f,  0.0f,   1.0f, 0.0f,
         0.5f,  0.5f, -0.5f,    1.0f,  0.0f,  0.0f,   1.0f, 1.0f,
         0.5f, -0.5f,  0.5f,    1.0f,  0.0f,  0.0f,   0.0f, 0.0f,
         0.5f,  0.5f, -0.5f,    1.0f,  0.0f,  0.0f,   1.0f, 1.0f,
         0.5f,  0.5f,  0.5f,    1.0f,  0.0f,  0.0f,   0.0f, 1.0f,
        
        -0.5f, -0.5f, -0.5f,    0.0f, -1.0f,  0.0f,   0.0f, 0.0f,
         0.5f, -0.5f, -0.5f,    0.0f, -1.0f,  0.0f,   1.0f, 0.0f,
         0.5f, -0.5f,  0.5f,    0.0f, -1.0f,  0.0f,   1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,    0.0f, -1.0f,  0.0f,   0.0f, 0.0f,
         0.5f, -0.5f,  0.5f,    0.0f, -1.0f,  0.0f,   1.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,    0.0f, -1.0f,  0.0f,   0.0f, 1.0f,
        
        -0.5f,  0.5f,  0.5f,    0.0f,  1.0f,  0.0f,   0.0f, 0.0f,
         0.5f,  0.5f,  0.5f,    0.0f,  1.0f,  0.0f,   1.0f, 0.0f,
         0.5f,  0.5f, -0.5f,    0.0f,  1.0f,  0.0f,   1.0f, 1.0f,
        -0.5f,  0.5f,  0.5f,    0.0f,  1.0f,  0.0f,   0.0f, 0.0f,
         0.5f,  0.5f, -0.5f,    0.0f,  1.0f,  0.0f,   1.0f, 1.0f,
        -0.5f,  0.5f, -0.5f,    0.0f,  1.0f,  0.0f,   0.0f, 1.0f,
        
         0.5f, -0.5f, -0.5f,    0.0f,  0.0f, -1.0f,   0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f,    0.0f,  0.0f, -1.0f,   1.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,    0.0f,  0.0f, -1.0f,   1.0f, 1.0f,
         0.5f, -0.5f, -0.5f,    0.0f,  0.0f, -1.0f,   0.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,    0.0f,  0.0f, -1.0f,   1.0f, 1.0f,
         0.5f,  0.5f, -0.5f,    0.0f,  0.0f, -1.0f,   0.0f, 1.0f,
        
        -0.5f, -0.5f,  0.5f,    0.0f,  0.0f,  1.0f,   0.0f, 0.0f,
         0.5f, -0.5f,  0.5f,    0.0f,  0.0f,  1.0f,   1.0f, 0.0f,
         0.5f,  0.5f,  0.5f,    0.0f,  0.0f,  1.0f,   1.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,    0.0f,  0.0f,  1.0f,   0.0f, 0.0f,
         0.5f,  0.5f,  0.5f,    0.0f,  0.0f,  1.0f,   1.0f, 1.0f,
        -0.5f,  0.5f,  0.5f,    0.0f,  0.0f,  1.0f,   0.0f, 1.0f
    };

    private uint _cubeVAO, _lightCubeVAO;
    private uint _VBO;

    // posiciona todos os contêineres
    private Vector3[] _cubePositions = 
    {
        new Vector3( 0.0f,  0.0f,  0.0f),
        new Vector3( 2.0f,  5.0f, -15.0f),
        new Vector3(-1.5f, -2.2f, -2.5f),
        new Vector3(-3.8f, -2.0f, -12.3f),
        new Vector3( 2.4f, -0.4f, -3.5f),
        new Vector3(-1.7f,  3.0f, -7.5f),
        new Vector3( 1.3f, -2.0f, -2.5f),
        new Vector3( 1.5f,  2.0f, -2.5f),
        new Vector3( 1.5f,  0.2f, -1.5f),
        new Vector3(-1.3f,  1.0f, -1.5f)
    };

    // posições das luzes pontuais
    private Vector3[] _pointLightPositions = 
    {
        new Vector3( 0.7f,  0.2f,  2.0f),
        new Vector3( 2.3f, -3.3f, -4.0f),
        new Vector3(-4.0f,  2.0f, -12.0f),
        new Vector3( 0.0f,  0.0f, -3.0f)
    };

    public Game()
    {
        // construir e compilar nosso programa de shader
        // --------------------------------------------------
        _lightingShader = new Shader(
            "res/Shaders/multiple_lights/vertex.glsl",
            "res/Shaders/multiple_lights/fragment.glsl"
        );

        _lightCubeShader = new Shader(
            "res/Shaders/light_cube/vertex.glsl",
            "res/Shaders/light_cube/fragment.glsl"
        );

        // carregar texturas (agora usamos uma função utilitária para manter o código mais organizado)
        // --------------------------------------------------
        _diffuseMap = LoadTexture("res/Textures/container2.png");
        _specularMap = LoadTexture("res/Textures/container2_specular.png");

        // configuração do shader
        // --------------------------------------------------
        _lightingShader.Use();
        _lightingShader.SetInt("material.diffuse", 0);
        _lightingShader.SetInt("material.specular", 1);

        // camera
        _camera = new Camera(new Vector3(0.0f, 0.0f, 3.0f));

        Input.CursorLockMode = CursorLockMode.Raw;
    }

    public void Init()
    {
        // configurar estado global do OpenGL
        // --------------------------------------------------
        _gl.Enable(EnableCap.DepthTest);        

        // primeiro, configure o VAO (e o VBO) do cubo
        _gl.GenVertexArrays(1, out _cubeVAO);
        _gl.GenBuffers(1, out _VBO);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _VBO);
        unsafe
        {
           fixed (float* buf = _vertices)
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(_vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
            }
        }
        
        _gl.BindVertexArray(_cubeVAO);

        // position attribute
        unsafe
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)0);
        }
        _gl.EnableVertexAttribArray(0);

        // normal attribute
        unsafe
        {
            _gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(3 * sizeof(float)));
        }
        _gl.EnableVertexAttribArray(1);

        // texture coord attribute
        unsafe
        {
            _gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(6 * sizeof(float)));
        }
        _gl.EnableVertexAttribArray(2);

        // segundo, configure o VAO da luz (o VBO permanece o mesmo; os vértices são os mesmos para o objeto de luz, que também é um cubo 3D)
        _gl.GenVertexArrays(1, out _lightCubeVAO);
        _gl.BindVertexArray(_lightCubeVAO);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _VBO);

        unsafe
        {
            // observe que atualizamos o stride do atributo de posição da lâmpada para refletir os dados atualizados do buffer
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)0);
        }
        _gl.EnableVertexAttribArray(0);
    }

    // glfw: sempre que o tamanho da janela é alterado (pelo SO ou redimensionamento do usuário), esta função de callback é executada
    // --------------------------------------------------
    public void Resize(Vector2D<int> newSize)
    {
        // certifique-se de que a viewport corresponda às novas dimensões da janela; observe que a largura e a altura serão significativamente maiores do que as especificadas em telas Retina.
        _gl.Viewport(0, 0, (uint)newSize.X, (uint)newSize.Y);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Program.Close();
        }

        // --------------------------------------------------
        
        _camera.Update();
    }

    public void Render()
    {        
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // certifique-se de ativar o shader ao definir uniforms ou desenhar objetos
        _lightingShader.Use();

        _lightingShader.SetVec3("viewPos", _camera.Position);

        /*
        // ==================================================
        //       DESERT
        // ==================================================

        _gl.ClearColor(0.75f, 0.52f, 0.3f, 1.0f);

        Vector3[] pointLightColors =
        {
            new Vector3(1.0f, 0.6f, 0.0f),
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(1.0f, 1.0f, 0.0f),
            new Vector3(0.2f, 0.2f, 1.0f)
        };

        // luz direcional
        _lightingShader.SetVec3("dirLight.direction", -0.2f, -1.0f, -0.3f);
        _lightingShader.SetVec3("dirLight.ambient", 0.3f, 0.24f, 0.14f);
        _lightingShader.SetVec3("dirLight.diffuse", 0.7f, 0.42f, 0.26f);
        _lightingShader.SetVec3("dirLight.specular", 0.5f, 0.5f, 0.5f);

        // luz pontual 1
        _lightingShader.SetVec3("pointLights[0].position", _pointLightPositions[0]);
        _lightingShader.SetVec3("pointLights[0].ambient", pointLightColors[0] * 0.1f);
        _lightingShader.SetVec3("pointLights[0].diffuse", pointLightColors[0]);
        _lightingShader.SetVec3("pointLights[0].specular", pointLightColors[0]);
        _lightingShader.SetFloat("pointLights[0].constant", 1.0f);
        _lightingShader.SetFloat("pointLights[0].linear", 0.09f);
        _lightingShader.SetFloat("pointLights[0].quadratic", 0.032f);

        // luz pontual 2
        _lightingShader.SetVec3("pointLights[1].position", _pointLightPositions[1]);
        _lightingShader.SetVec3("pointLights[1].ambient",pointLightColors[1] * 0.1f);
        _lightingShader.SetVec3("pointLights[1].diffuse", pointLightColors[1]);
        _lightingShader.SetVec3("pointLights[1].specular", pointLightColors[1]);
        _lightingShader.SetFloat("pointLights[1].constant", 1.0f);
        _lightingShader.SetFloat("pointLights[1].linear", 0.09f);
        _lightingShader.SetFloat("pointLights[1].quadratic", 0.032f);

        // luz pontual 3
        _lightingShader.SetVec3("pointLights[2].position", _pointLightPositions[2]);
        _lightingShader.SetVec3("pointLights[2].ambient", pointLightColors[2] * 0.1f);
        _lightingShader.SetVec3("pointLights[2].diffuse", pointLightColors[2]);
        _lightingShader.SetVec3("pointLights[2].specular", pointLightColors[2]);
        _lightingShader.SetFloat("pointLights[2].constant", 1.0f);
        _lightingShader.SetFloat("pointLights[2].linear", 0.09f);
        _lightingShader.SetFloat("pointLights[2].quadratic", 0.032f);

        // luz pontual 4
        _lightingShader.SetVec3("pointLights[3].position", _pointLightPositions[3]);
        _lightingShader.SetVec3("pointLights[3].ambient", pointLightColors[3] * 0.1f);
        _lightingShader.SetVec3("pointLights[3].diffuse", pointLightColors[3]);
        _lightingShader.SetVec3("pointLights[3].specular", pointLightColors[3]);
        _lightingShader.SetFloat("pointLights[3].constant", 1.0f);
        _lightingShader.SetFloat("pointLights[3].linear", 0.09f);
        _lightingShader.SetFloat("pointLights[3].quadratic", 0.032f);

        // spotLight
        _lightingShader.SetVec3("spotLight.position", _camera.Position);
        _lightingShader.SetVec3("spotLight.direction", _camera.Front);
        _lightingShader.SetVec3("spotLight.ambient", 0.0f, 0.0f, 0.0f);
        _lightingShader.SetVec3("spotLight.diffuse", 0.8f, 0.8f, 0.0f); 
        _lightingShader.SetVec3("spotLight.specular", 0.8f, 0.8f, 0.0f);
        _lightingShader.SetFloat("spotLight.constant", 1.0f);
        _lightingShader.SetFloat("spotLight.linear", 0.09f);
        _lightingShader.SetFloat("spotLight.quadratic", 0.032f);
        _lightingShader.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
        _lightingShader.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(13.0f))); 
        //*/

        /*
        // ==================================================
        //       FACTORY
        // ==================================================

        _gl.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);

        Vector3[] pointLightColors =
        {
            new Vector3(0.2f, 0.2f, 0.6f),
            new Vector3(0.3f, 0.3f, 0.7f),
            new Vector3(0.0f, 0.0f, 0.3f),
            new Vector3(0.4f, 0.4f, 0.4f)
        };

        // luz direcional
        _lightingShader.SetVec3("dirLight.direction", -0.2f, -1.0f, -0.3f);	
        _lightingShader.SetVec3("dirLight.ambient", 0.05f, 0.05f, 0.1f);
        _lightingShader.SetVec3("dirLight.diffuse", 0.2f, 0.2f, 0.7f); 
        _lightingShader.SetVec3("dirLight.specular", 0.7f, 0.7f, 0.7f);

        // luz pontual 1
        _lightingShader.SetVec3("pointLights[0].position", _pointLightPositions[0]);
        _lightingShader.SetVec3("pointLights[0].ambient", pointLightColors[0] * 0.1f);
        _lightingShader.SetVec3("pointLights[0].diffuse", pointLightColors[0]);
        _lightingShader.SetVec3("pointLights[0].specular", pointLightColors[0]);
        _lightingShader.SetFloat("pointLights[0].constant", 1.0f);
        _lightingShader.SetFloat("pointLights[0].linear", 0.09f);
        _lightingShader.SetFloat("pointLights[0].quadratic", 0.032f);

        // luz pontual 2
        _lightingShader.SetVec3("pointLights[1].position", _pointLightPositions[1]);
        _lightingShader.SetVec3("pointLights[1].ambient",pointLightColors[1] * 0.1f);
        _lightingShader.SetVec3("pointLights[1].diffuse", pointLightColors[1]);
        _lightingShader.SetVec3("pointLights[1].specular", pointLightColors[1]);
        _lightingShader.SetFloat("pointLights[1].constant", 1.0f);
        _lightingShader.SetFloat("pointLights[1].linear", 0.09f);
        _lightingShader.SetFloat("pointLights[1].quadratic", 0.032f);

        // luz pontual 3
        _lightingShader.SetVec3("pointLights[2].position", _pointLightPositions[2]);
        _lightingShader.SetVec3("pointLights[2].ambient", pointLightColors[2] * 0.1f);
        _lightingShader.SetVec3("pointLights[2].diffuse", pointLightColors[2]);
        _lightingShader.SetVec3("pointLights[2].specular", pointLightColors[2]);
        _lightingShader.SetFloat("pointLights[2].constant", 1.0f);
        _lightingShader.SetFloat("pointLights[2].linear", 0.09f);
        _lightingShader.SetFloat("pointLights[2].quadratic", 0.032f);

        // luz pontual 4
        _lightingShader.SetVec3("pointLights[3].position", _pointLightPositions[3]);
        _lightingShader.SetVec3("pointLights[3].ambient", pointLightColors[3] * 0.1f);
        _lightingShader.SetVec3("pointLights[3].diffuse", pointLightColors[3]);
        _lightingShader.SetVec3("pointLights[3].specular", pointLightColors[3]);
        _lightingShader.SetFloat("pointLights[3].constant", 1.0f);
        _lightingShader.SetFloat("pointLights[3].linear", 0.09f);
        _lightingShader.SetFloat("pointLights[3].quadratic", 0.032f);

        // spotLight
        _lightingShader.SetVec3("spotLight.position", _camera.Position);
        _lightingShader.SetVec3("spotLight.direction", _camera.Front);
        _lightingShader.SetVec3("spotLight.ambient", 0.0f, 0.0f, 0.0f);
        _lightingShader.SetVec3("spotLight.diffuse", 1.0f, 1.0f, 1.0f); 
        _lightingShader.SetVec3("spotLight.specular", 1.0f, 1.0f, 1.0f);
        _lightingShader.SetFloat("spotLight.constant", 1.0f);
        _lightingShader.SetFloat("spotLight.linear", 0.009f);
        _lightingShader.SetFloat("spotLight.quadratic", 0.0032f);
        _lightingShader.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(10.0f)));
        _lightingShader.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
        //*/

        /*
        // ==================================================
        //       HORROR
        // ==================================================

        _gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

        Vector3[] pointLightColors =
        {
            new Vector3(0.1f, 0.1f, 0.1f),
            new Vector3(0.1f, 0.1f, 0.1f),
            new Vector3(0.1f, 0.1f, 0.1f),
            new Vector3(0.3f, 0.1f, 0.1f)
        };

        // luz direcional
        _lightingShader.SetVec3("dirLight.direction", -0.2f, -1.0f, -0.3f);	
        _lightingShader.SetVec3("dirLight.ambient", 0.0f, 0.0f, 0.0f);
        _lightingShader.SetVec3("dirLight.diffuse", 0.05f, 0.05f, 0.05f); 
        _lightingShader.SetVec3("dirLight.specular", 0.2f, 0.2f, 0.2f);

        // luz pontual 1
        _lightingShader.SetVec3("pointLights[0].position", _pointLightPositions[0]);
        _lightingShader.SetVec3("pointLights[0].ambient", pointLightColors[0] * 0.1f);
        _lightingShader.SetVec3("pointLights[0].diffuse", pointLightColors[0]);
        _lightingShader.SetVec3("pointLights[0].specular", pointLightColors[0]);
        _lightingShader.SetFloat("pointLights[0].constant", 1.0f);
        _lightingShader.SetFloat("pointLights[0].linear", 0.14f);
        _lightingShader.SetFloat("pointLights[0].quadratic", 0.07f);

        // luz pontual 2
        _lightingShader.SetVec3("pointLights[1].position", _pointLightPositions[1]);
        _lightingShader.SetVec3("pointLights[1].ambient",pointLightColors[1] * 0.1f);
        _lightingShader.SetVec3("pointLights[1].diffuse", pointLightColors[1]);
        _lightingShader.SetVec3("pointLights[1].specular", pointLightColors[1]);
        _lightingShader.SetFloat("pointLights[1].constant", 1.0f);
        _lightingShader.SetFloat("pointLights[1].linear", 0.14f);
        _lightingShader.SetFloat("pointLights[1].quadratic", 0.07f);

        // luz pontual 3
        _lightingShader.SetVec3("pointLights[2].position", _pointLightPositions[2]);
        _lightingShader.SetVec3("pointLights[2].ambient", pointLightColors[2] * 0.1f);
        _lightingShader.SetVec3("pointLights[2].diffuse", pointLightColors[2]);
        _lightingShader.SetVec3("pointLights[2].specular", pointLightColors[2]);
        _lightingShader.SetFloat("pointLights[2].constant", 1.0f);
        _lightingShader.SetFloat("pointLights[2].linear", 0.14f);
        _lightingShader.SetFloat("pointLights[2].quadratic", 0.07f);

        // luz pontual 4
        _lightingShader.SetVec3("pointLights[3].position", _pointLightPositions[3]);
        _lightingShader.SetVec3("pointLights[3].ambient", pointLightColors[3] * 0.1f);
        _lightingShader.SetVec3("pointLights[3].diffuse", pointLightColors[3]);
        _lightingShader.SetVec3("pointLights[3].specular", pointLightColors[3]);
        _lightingShader.SetFloat("pointLights[3].constant", 1.0f);
        _lightingShader.SetFloat("pointLights[3].linear", 0.14f);
        _lightingShader.SetFloat("pointLights[3].quadratic", 0.07f);

        // spotLight
        _lightingShader.SetVec3("spotLight.position", _camera.Position);
        _lightingShader.SetVec3("spotLight.direction", _camera.Front);
        _lightingShader.SetVec3("spotLight.ambient", 0.0f, 0.0f, 0.0f);
        _lightingShader.SetVec3("spotLight.diffuse", 1.0f, 1.0f, 1.0f); 
        _lightingShader.SetVec3("spotLight.specular", 1.0f, 1.0f, 1.0f);
        _lightingShader.SetFloat("spotLight.constant", 1.0f);
        _lightingShader.SetFloat("spotLight.linear", 0.09f);
        _lightingShader.SetFloat("spotLight.quadratic", 0.032f);
        _lightingShader.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(10.0f)));
        _lightingShader.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(15.0f)));
        //*/

        //*
        // ==================================================
        //       BIOCHEMICAL LAB
        // ==================================================

        _gl.ClearColor(0.9f, 0.9f, 0.9f, 1.0f);

        Vector3[] pointLightColors =
        {
            new Vector3(0.4f, 0.7f, 0.1f),
            new Vector3(0.4f, 0.7f, 0.1f),
            new Vector3(0.4f, 0.7f, 0.1f),
            new Vector3(0.4f, 0.7f, 0.1f)
        };

        // luz direcional
        _lightingShader.SetVec3("dirLight.direction", -0.2f, -1.0f, -0.3f);	
        _lightingShader.SetVec3("dirLight.ambient", 0.5f, 0.5f, 0.5f);
        _lightingShader.SetVec3("dirLight.diffuse", 1.0f, 1.0f, 1.0f);
        _lightingShader.SetVec3("dirLight.specular", 1.0f, 1.0f, 1.0f);

        // luz pontual 1
        _lightingShader.SetVec3("pointLights[0].position", _pointLightPositions[0]);
        _lightingShader.SetVec3("pointLights[0].ambient", pointLightColors[0] * 0.1f);
        _lightingShader.SetVec3("pointLights[0].diffuse", pointLightColors[0]);
        _lightingShader.SetVec3("pointLights[0].specular", pointLightColors[0]);
        _lightingShader.SetFloat("pointLights[0].constant", 1.0f);
        _lightingShader.SetFloat("pointLights[0].linear", 0.07f);
        _lightingShader.SetFloat("pointLights[0].quadratic", 0.017f);

        // luz pontual 2
        _lightingShader.SetVec3("pointLights[1].position", _pointLightPositions[1]);
        _lightingShader.SetVec3("pointLights[1].ambient",pointLightColors[1] * 0.1f);
        _lightingShader.SetVec3("pointLights[1].diffuse", pointLightColors[1]);
        _lightingShader.SetVec3("pointLights[1].specular", pointLightColors[1]);
        _lightingShader.SetFloat("pointLights[1].constant", 1.0f);
        _lightingShader.SetFloat("pointLights[1].linear", 0.07f);
        _lightingShader.SetFloat("pointLights[1].quadratic", 0.017f);

        // luz pontual 3
        _lightingShader.SetVec3("pointLights[2].position", _pointLightPositions[2]);
        _lightingShader.SetVec3("pointLights[2].ambient", pointLightColors[2] * 0.1f);
        _lightingShader.SetVec3("pointLights[2].diffuse", pointLightColors[2]);
        _lightingShader.SetVec3("pointLights[2].specular", pointLightColors[2]);
        _lightingShader.SetFloat("pointLights[2].constant", 1.0f);
        _lightingShader.SetFloat("pointLights[2].linear", 0.07f);
        _lightingShader.SetFloat("pointLights[2].quadratic", 0.017f);

        // luz pontual 4
        _lightingShader.SetVec3("pointLights[3].position", _pointLightPositions[3]);
        _lightingShader.SetVec3("pointLights[3].ambient", pointLightColors[3] * 0.1f);
        _lightingShader.SetVec3("pointLights[3].diffuse", pointLightColors[3]);
        _lightingShader.SetVec3("pointLights[3].specular", pointLightColors[3]);
        _lightingShader.SetFloat("pointLights[3].constant", 1.0f);
        _lightingShader.SetFloat("pointLights[3].linear", 0.07f);
        _lightingShader.SetFloat("pointLights[3].quadratic", 0.017f);

        // spotLight
        _lightingShader.SetVec3("spotLight.position", _camera.Position);
        _lightingShader.SetVec3("spotLight.direction", _camera.Front);
        _lightingShader.SetVec3("spotLight.ambient", 0.0f, 0.0f, 0.0f);
        _lightingShader.SetVec3("spotLight.diffuse", 0.0f, 1.0f, 0.0f); 
        _lightingShader.SetVec3("spotLight.specular", 0.0f, 1.0f, 0.0f); 
        _lightingShader.SetFloat("spotLight.constant", 1.0f);
        _lightingShader.SetFloat("spotLight.linear", 0.07f);
        _lightingShader.SetFloat("spotLight.quadratic", 0.017f);
        _lightingShader.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(7.0f)));
        _lightingShader.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(10.0f)));
        //*/

        // propriedades do material
        _lightingShader.SetFloat("material.shininess", 32.0f);

        // transformações de visualização/projeção
        Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(
            fieldOfView:       MathHelper.DegreesToRadians(_camera.Zoom), 
            aspectRatio:       (float)Screen.Width / (float)Screen.Height, 
            nearPlaneDistance: 0.1f, 
            farPlaneDistance:  100.0f
        );
        Matrix4x4 view = _camera.GetViewMatrix();
        _lightingShader.SetMat4("projection", projection);
        _lightingShader.SetMat4("view", view);

        // transformação do mundo
        Matrix4x4 model = Matrix4x4.Identity;
        _lightingShader.SetMat4("model", model);

        // vincular mapa de difusão
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _diffuseMap);

        // vincular mapa especular
        _gl.ActiveTexture(TextureUnit.Texture1);
        _gl.BindTexture(TextureTarget.Texture2D, _specularMap);

        // renderizar contêineres
        _gl.BindVertexArray(_cubeVAO);
        for (int i = 0; i < 10; i++)
        {
            // calcula a matriz de modelo para cada objeto e a passa para o shader antes de desenhar
            model = Matrix4x4.Identity;

            float angle = 20.0f * i;
            model *= Matrix4x4.CreateFromAxisAngle(
                Vector3.Normalize(new Vector3(1.0f, 0.3f, 0.5f)),
                MathHelper.DegreesToRadians(angle)
            );

            model *= Matrix4x4.CreateTranslation(_cubePositions[i]);

            _lightingShader.SetMat4("model", model);

            _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        // também desenhe o(s) objeto(s) de luminária
        _lightCubeShader.Use();

        _lightCubeShader.SetMat4("projection", projection);
        _lightCubeShader.SetMat4("view", view);

        // agora desenhamos tantas lâmpadas quantas forem as nossas luzes pontuais.
        _gl.BindVertexArray(_lightCubeVAO);
        for (int i = 0; i < 4; i++)
        {
            model = Matrix4x4.Identity;
            model *= Matrix4x4.CreateScale(new Vector3(0.2f)); // um cubo menor
            model *= Matrix4x4.CreateTranslation(_pointLightPositions[i]);
            _lightCubeShader.SetMat4("model", model);

            _lightCubeShader.SetVec3("ourColor", pointLightColors[i]);

            _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
    }

    public void Clear()
    {
        // opcional: desalocar todos os recursos assim que não forem mais necessários:
        // --------------------------------------------------
        _gl.DeleteVertexArrays(1, ref _cubeVAO);
        _gl.DeleteVertexArrays(1, ref _lightCubeVAO);
        _gl.DeleteBuffers(1, ref _VBO);
    }

    // função utilitária para carregar uma textura 2D a partir de um arquivo
    // --------------------------------------------------
    private uint LoadTexture(string path)
    {
        uint textureID;
        _gl.GenTextures(1, out textureID);

        int width, height;
        byte[] data;

        ImageResult image;

        using (FileStream stream = File.OpenRead(path))
        {
            image = ImageResult.FromStream(stream, ColorComponents.Default);

            width = image.Width;
            height = image.Height;
            data = image.Data;
        }

        if (data != null)
        {
            InternalFormat internalFormat = InternalFormat.Rgb;
            PixelFormat pixelFormat = PixelFormat.Rgb;

            if (image.Comp == ColorComponents.Grey)
            {
                internalFormat = InternalFormat.Red;
                pixelFormat = PixelFormat.Red;
            }
            else if (image.Comp == ColorComponents.RedGreenBlue)
            {
                internalFormat = InternalFormat.Rgb;
                pixelFormat = PixelFormat.Rgb;
            }
            else if (image.Comp == ColorComponents.RedGreenBlueAlpha)
            {
                internalFormat = InternalFormat.Rgba;
                pixelFormat = PixelFormat.Rgba;
            }

            _gl.BindTexture(TextureTarget.Texture2D, textureID);
            unsafe
            {
                fixed (byte* ptr = data)
                {
                    _gl.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, (uint)width, (uint)height, 0, pixelFormat, PixelType.UnsignedByte, ptr);
                }
            }
            _gl.GenerateMipmap(TextureTarget.Texture2D);

            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
        else
        {
            Console.WriteLine("Falha ao carregar a textura no caminho: " + path);
        }

        return textureID;
    }
}
