using System.Numerics;
using MySilkProgram.Inputs;
using MySilkProgram.Utilities;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace MySilkProgram;

public class Game
{
    private IWindow _window = null!;

    private GL _gl = null!;
    public static GL GL = null!;

    private Shader lightingShader = null!;
    private Shader lightCubeShader = null!;

    private Camera _camera = null!;

    // configurar dados de vértice (e buffer(s)) e configurar atributos de vértice
    // ---------------------------------------------------------------------------
    private readonly float[] _vertices =
    {
        // positions          // colors          // tex        // normals
        -0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 0.0f,  0.0f, 0.0f,  -1.0f,  0.0f,  0.0f, // 0
        -0.5f, -0.5f,  0.5f,  0.0f, 1.0f, 0.0f,  1.0f, 0.0f,  -1.0f,  0.0f,  0.0f, // 1
        -0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 1.0f,  1.0f, 1.0f,  -1.0f,  0.0f,  0.0f, // 2
        -0.5f,  0.5f, -0.5f,  1.0f, 1.0f, 0.0f,  0.0f, 1.0f,  -1.0f,  0.0f,  0.0f, // 3
        
         0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 0.0f,  0.0f, 0.0f,   1.0f,  0.0f,  0.0f, // 4
         0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 0.0f,  1.0f, 0.0f,   1.0f,  0.0f,  0.0f, // 5
         0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 1.0f,  1.0f, 1.0f,   1.0f,  0.0f,  0.0f, // 6
         0.5f,  0.5f,  0.5f,  1.0f, 1.0f, 0.0f,  0.0f, 1.0f,   1.0f,  0.0f,  0.0f, // 7
        
        -0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 0.0f,  0.0f, 0.0f,   0.0f, -1.0f,  0.0f, // 8
         0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 0.0f,  1.0f, 0.0f,   0.0f, -1.0f,  0.0f, // 9
         0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 1.0f,  1.0f, 1.0f,   0.0f, -1.0f,  0.0f, // 10
        -0.5f, -0.5f,  0.5f,  1.0f, 1.0f, 0.0f,  0.0f, 1.0f,   0.0f, -1.0f,  0.0f, // 11
        
        -0.5f,  0.5f,  0.5f,  1.0f, 0.0f, 0.0f,  0.0f, 0.0f,   0.0f,  1.0f,  0.0f, // 12
         0.5f,  0.5f,  0.5f,  0.0f, 1.0f, 0.0f,  1.0f, 0.0f,   0.0f,  1.0f,  0.0f, // 13
         0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 1.0f,  1.0f, 1.0f,   0.0f,  1.0f,  0.0f, // 14
        -0.5f,  0.5f, -0.5f,  1.0f, 1.0f, 0.0f,  0.0f, 1.0f,   0.0f,  1.0f,  0.0f, // 15
        
         0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 0.0f,  0.0f, 0.0f,   0.0f,  0.0f, -1.0f, // 16
        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 0.0f,  1.0f, 0.0f,   0.0f,  0.0f, -1.0f, // 17
        -0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 1.0f,  1.0f, 1.0f,   0.0f,  0.0f, -1.0f, // 18
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f, 0.0f,  0.0f, 1.0f,   0.0f,  0.0f, -1.0f, // 19
        
        -0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 0.0f,  0.0f, 0.0f,   0.0f,  0.0f,  1.0f, // 20
         0.5f, -0.5f,  0.5f,  0.0f, 1.0f, 0.0f,  1.0f, 0.0f,   0.0f,  0.0f,  1.0f, // 21
         0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 1.0f,  1.0f, 1.0f,   0.0f,  0.0f,  1.0f, // 22
        -0.5f,  0.5f,  0.5f,  1.0f, 1.0f, 0.0f,  0.0f, 1.0f,   0.0f,  0.0f,  1.0f, // 23
    };

    private readonly uint[] _indices = // observe que começamos do 0!
    {
        0, 1, 2, // primeiro triangulo
        0, 2, 3, // segundo triangulo

        4, 5, 6,
        4, 6, 7,

        8, 9, 10,
        8, 10, 11,

        12, 13, 14,
        12, 14, 15,

        16, 17, 18,
        16, 18, 19,

        20, 21, 22,
        20, 22, 23
    };

    private uint cubeVAO, lightVAO;
    private uint VBO;
    private uint EBO;

    // lighting
    private Vector3 lightPos = new Vector3(1.2f, 1.0f, 2.0f);

    public Game()
    {
        WindowOptions options = WindowOptions.Default;
        options.Size = new Vector2D<int>(800, 600);
        options.Title = "LearnOpenGL with Silk.NET";
        options.IsVisible = false;

        _window = Window.Create(options);

        _window.Load += OnLoad;
        _window.Resize += OnResize;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
        _window.Closing += OnClosing;

        try
        {
            _window.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                "Falha ao criar a janela Silk.NET" + "\n" +
                ex + "\n" + 
                " -- --------------------------------------------------- -- "
            );
        }
    }

    private void OnLoad()
    {
        _window.Center();
        _window.IsVisible = true;

        _gl = _window.CreateOpenGL();
        GL = _gl;

        Input.Initialize(_window);

        _gl.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);

        // construir e compilar nosso programa de shader
        // ------------------------------------
        lightingShader = new Shader( // você pode nomear seus arquivos de shader como quiser
            "res/Shaders/colors/vertex.glsl",
            "res/Shaders/colors/fragment.glsl"
        );

        lightCubeShader = new Shader(
            "res/Shaders/light_cube/vertex.glsl",
            "res/Shaders/light_cube/fragment.glsl"
        );

        // primeiro, configure o VAO (e o VBO) do cubo
        _gl.GenVertexArrays(1, out cubeVAO);
        _gl.GenBuffers(1, out VBO);
        _gl.GenBuffers(1, out EBO);

        // primeiro vincule o Vertex Array Object, depois vincule e configure o(s) buffer(s) de vértices e, em seguida, configure o(s) atributo(s) de vértice.
        _gl.BindVertexArray(cubeVAO);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, VBO);
        unsafe
        {
            fixed (float* buf = _vertices)
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(_vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
            }
        }

        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, EBO);
        unsafe
        {
            fixed (uint* buf = _indices)
            {
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (uint)(_indices.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);
            }
        }

        // position attribute
        unsafe
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 11 * sizeof(float), (void*)0);
        }
        _gl.EnableVertexAttribArray(0);

        // normal attribute
        unsafe
        {
            _gl.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 11 * sizeof(float), (void*)(8 * sizeof(float)));
        }
        _gl.EnableVertexAttribArray(3);

        // segundo, configure o VAO da luz (o VBO permanece o mesmo; os vértices são os mesmos para o objeto de luz, que também é um cubo 3D)
        _gl.GenVertexArrays(1, out lightVAO);
        _gl.BindVertexArray(lightVAO);

        // precisamos apenas vincular o VBO; os dados do VBO do contêiner já contêm os dados.
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, VBO);
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, EBO);

        // position attribute
        unsafe
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 11 * sizeof(float), (void*)0);
        }
        _gl.EnableVertexAttribArray(0);

        // observe que isso é permitido; a chamada para glVertexAttribPointer registrou o VBO como o objeto de buffer de vértices vinculado ao atributo de vértice, portanto, podemos desvinculá-lo com segurança logo em seguida
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

        // lembre-se: NÃO desvincule o EBO enquanto um VAO estiver ativo, pois o objeto de buffer de elementos vinculado ESTÁ armazenado no VAO; mantenha o EBO vinculado.
        // _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

        // Você pode desvincular o VAO posteriormente para que outras chamadas de VAO não modifiquem acidentalmente este VAO, mas isso raramente acontece. Modificar outros VAOs exige uma chamada para glBindVertexArray de qualquer forma, então geralmente não desvinculamos VAOs (nem VBOs) quando não é diretamente necessário.
        _gl.BindVertexArray(0);

        // descomente esta chamada para desenhar polígonos em wireframe.
        // _gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);

        _gl.Enable(EnableCap.DepthTest);

        _camera = new Camera();

        Input.CursorLockMode = CursorLockMode.Raw;
    }

    private void OnResize(Vector2D<int> newSize)
    {
        // certifique-se de que a viewport corresponda às novas dimensões da janela; observe que largura e a altura será significativamente maior do que a especificada em telas retina.
        _gl.Viewport(0, 0, (uint)newSize.X, (uint)newSize.Y);
    }

    private void OnUpdate(double deltaTime)
    {
        Time.Update(deltaTime);
        Input.NewFrame();

        if (Input.GetKey(Key.Escape))
        {
            _window.Close();
        }

        _camera.ProcessKeyboad();
        _camera.ProcessMouseMovement();
        _camera.ProcessMouseScroll();
    }

    private void OnRender(double deltaTime)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        lightingShader.Use();

        // view/projection transformations

        Matrix4x4 projection = _camera.GetProjectionMatrix(_window);
        lightingShader.SetUniform("projection", projection);

        Matrix4x4 view = _camera.GetViewMatrix();
        lightingShader.SetUniform("view", view);

        // world transformation
        Matrix4x4 model = Matrix4x4.Identity;
        lightingShader.SetUniform("model", model);

        // não se esqueça de usar o programa de shader correspondente primeiro (para definir o uniform)
        lightingShader.SetUniform("objectColor", 1.0f, 0.5f, 0.31f);
        lightingShader.SetUniform("lightColor", 1.0f, 1.0f, 1.0f);
        lightingShader.SetUniform("lightPos", lightPos);
        lightingShader.SetUniform("viewPos", _camera.Position);

        // render the cube
        _gl.BindVertexArray(cubeVAO);
        unsafe
        {
            // renderiza o triângulo
            _gl.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, (void*)0);
        }    
        _gl.BindVertexArray(0);

        // também desenhe o objeto da lâmpada

        lightCubeShader.Use();

        lightCubeShader.SetUniform("projection", projection);

        lightCubeShader.SetUniform("view", view);

        model = Matrix4x4.Identity;
        model *= Matrix4x4.CreateScale(new Vector3(0.2f)); // um cubo menor
        model *= Matrix4x4.CreateTranslation(lightPos);
        lightCubeShader.SetUniform("model", model);

        _gl.BindVertexArray(lightVAO);
        unsafe
        {
            // renderiza o triângulo
            _gl.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, (void*)0);
        }    
        _gl.BindVertexArray(0);
    }

    private void OnClosing()
    {
        // opcional: desalocar todos os recursos assim que não forem mais necessários:
        // ---------------------------------------------------------------------------
        _gl.DeleteVertexArrays(1, ref cubeVAO);
        _gl.DeleteBuffers(1, ref VBO);
        _gl.DeleteBuffers(1, ref EBO);
        
        lightingShader.Dispose();
    }
}
