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

    // configurar dados de vértice (e buffer(s)) e configurar atributos de vértice
    // --------------------------------------------------
    private float[] _vertices =
    {
        // positions           // normals
        -0.5f, -0.5f, -0.5f,   -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f,  0.5f,   -1.0f,  0.0f,  0.0f,
        -0.5f,  0.5f,  0.5f,   -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f, -0.5f,   -1.0f,  0.0f,  0.0f,
        -0.5f,  0.5f,  0.5f,   -1.0f,  0.0f,  0.0f,
        -0.5f,  0.5f, -0.5f,   -1.0f,  0.0f,  0.0f,
        
         0.5f, -0.5f,  0.5f,    1.0f,  0.0f,  0.0f,
         0.5f, -0.5f, -0.5f,    1.0f,  0.0f,  0.0f,
         0.5f,  0.5f, -0.5f,    1.0f,  0.0f,  0.0f,
         0.5f, -0.5f,  0.5f,    1.0f,  0.0f,  0.0f,
         0.5f,  0.5f, -0.5f,    1.0f,  0.0f,  0.0f,
         0.5f,  0.5f,  0.5f,    1.0f,  0.0f,  0.0f,
        
        -0.5f, -0.5f, -0.5f,    0.0f, -1.0f,  0.0f,
         0.5f, -0.5f, -0.5f,    0.0f, -1.0f,  0.0f,
         0.5f, -0.5f,  0.5f,    0.0f, -1.0f,  0.0f,
        -0.5f, -0.5f, -0.5f,    0.0f, -1.0f,  0.0f,
         0.5f, -0.5f,  0.5f,    0.0f, -1.0f,  0.0f,
        -0.5f, -0.5f,  0.5f,    0.0f, -1.0f,  0.0f,
        
        -0.5f,  0.5f,  0.5f,    0.0f,  1.0f,  0.0f,
         0.5f,  0.5f,  0.5f,    0.0f,  1.0f,  0.0f,
         0.5f,  0.5f, -0.5f,    0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f,  0.5f,    0.0f,  1.0f,  0.0f,
         0.5f,  0.5f, -0.5f,    0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f, -0.5f,    0.0f,  1.0f,  0.0f,
        
         0.5f, -0.5f, -0.5f,    0.0f,  0.0f, -1.0f,
        -0.5f, -0.5f, -0.5f,    0.0f,  0.0f, -1.0f,
        -0.5f,  0.5f, -0.5f,    0.0f,  0.0f, -1.0f,
         0.5f, -0.5f, -0.5f,    0.0f,  0.0f, -1.0f,
        -0.5f,  0.5f, -0.5f,    0.0f,  0.0f, -1.0f,
         0.5f,  0.5f, -0.5f,    0.0f,  0.0f, -1.0f,
        
        -0.5f, -0.5f,  0.5f,    0.0f,  0.0f,  1.0f,
         0.5f, -0.5f,  0.5f,    0.0f,  0.0f,  1.0f,
         0.5f,  0.5f,  0.5f,    0.0f,  0.0f,  1.0f,
        -0.5f, -0.5f,  0.5f,    0.0f,  0.0f,  1.0f,
         0.5f,  0.5f,  0.5f,    0.0f,  0.0f,  1.0f,
        -0.5f,  0.5f,  0.5f,    0.0f,  0.0f,  1.0f
    };

    private uint _cubeVAO, _lightCubeVAO;
    private uint _VBO;

    // lighting
    private Vector3 _lightPos = new Vector3(1.2f, 1.0f, 2.0f);

    public Game()
    {
        // construir e compilar nosso programa de shader
        // --------------------------------------------------
        _lightingShader = new Shader(
            "res/Shaders/basic_lighting/vertex.glsl",
            "res/Shaders/basic_lighting/fragment.glsl"
        );

        _lightCubeShader = new Shader(
            "res/Shaders/light_cube/vertex.glsl",
            "res/Shaders/light_cube/fragment.glsl"
        );

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
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), (void*)0);
        }
        _gl.EnableVertexAttribArray(0);

        // normal attribute
        unsafe
        {
            _gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), (void*)(3 * sizeof(float)));
        }
        _gl.EnableVertexAttribArray(1);

        // segundo, configure o VAO da luz (o VBO permanece o mesmo; os vértices são os mesmos para o objeto de luz, que também é um cubo 3D)
        _gl.GenVertexArrays(1, out _lightCubeVAO);
        _gl.BindVertexArray(_lightCubeVAO);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _VBO);

        unsafe
        {
            // observe que atualizamos o stride do atributo de posição da lâmpada para refletir os dados atualizados do buffer
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), (void*)0);
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
        _gl.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // certifique-se de ativar o shader ao definir uniforms ou desenhar objetos
        _lightingShader.Use();

        _lightingShader.setVec3("objectColor", 1.0f, 0.5f, 0.31f);
        _lightingShader.setVec3("lightColor",  1.0f, 1.0f, 1.0f);
        _lightingShader.setVec3("lightPos", _lightPos);
        _lightingShader.setVec3("viewPos", _camera.Position);

        // transformações de visualização/projeção
        Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(
            fieldOfView:       MathHelper.DegreesToRadians(_camera.Zoom), 
            aspectRatio:       (float)Screen.Width / (float)Screen.Height, 
            nearPlaneDistance: 0.1f, 
            farPlaneDistance:  100.0f
        );
        Matrix4x4 view = _camera.GetViewMatrix();
        _lightingShader.setMat4("projection", projection);
        _lightingShader.setMat4("view", view);

        // transformação do mundo
        Matrix4x4 model = Matrix4x4.Identity;
        _lightingShader.setMat4("model", model);

        // renderiza o cubo
        _gl.BindVertexArray(_cubeVAO);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);

        // também desenhe o objeto da lâmpada
        _lightCubeShader.Use();

        _lightCubeShader.setMat4("projection", projection);
        _lightCubeShader.setMat4("view", view);

        model = Matrix4x4.Identity;
        model *= Matrix4x4.CreateScale(new Vector3(0.2f)); // um cubo menor
        model *= Matrix4x4.CreateTranslation(_lightPos);
        _lightCubeShader.setMat4("model", model);

        _gl.BindVertexArray(_lightCubeVAO);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    public void Clear()
    {
        // opcional: desalocar todos os recursos assim que não forem mais necessários:
        // --------------------------------------------------
        _gl.DeleteVertexArrays(1, ref _cubeVAO);
        _gl.DeleteVertexArrays(1, ref _lightCubeVAO);
        _gl.DeleteBuffers(1, ref _VBO);
    }
}
