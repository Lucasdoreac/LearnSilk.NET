using LearnSilkNET.Inputs;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace LearnSilkNET;

public class Game
{
    private GL _gl = Program.GL;

    private const string _vertexShaderSource = 
    @"
        #version 330 core
        layout (location = 0) in vec3 aPos;

        void main()
        {
            gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
        }
    ";

    private const string _fragmentShaderSource =
    @"
        #version 330 core
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
        } 
    ";

    private uint _shaderProgram;

    // configurar dados de vértice (e buffer(s)) e configurar atributos de vértice
    // --------------------------------------------------
    private float[] _firstTriangle =
    {
        -0.9f,  -0.5f,  0.0f, // vertice inferior esquerdo
         0.0f,  -0.5f,  0.0f, // vertice inferior direito
        -0.45f,  0.5f,  0.0f  // vertice supeior
    };

    private float[] _secondTriangle =
    {
         0.0f,  -0.5f,  0.0f, // vertice inferior esquerdo
         0.9f,  -0.5f,  0.0f, // vertice inferior direito
         0.45f,  0.5f,  0.0f  // vertice supeior
    };

    private uint[] _vertexArrayObject = new uint[2];
    private uint[] _vertexBufferObject = new uint[2];

    public Game()
    {
        
    }

    public void Init()
    {
        // construir e compilar nosso programa de shader
        // --------------------------------------------------

        // vertex shader
        uint vertexShader = _gl.CreateShader(ShaderType.VertexShader);
        _gl.ShaderSource(vertexShader, _vertexShaderSource);
        _gl.CompileShader(vertexShader);

        // verificar erros de compilação de shader
        int success;
        string infoLog;

        _gl.GetShader(vertexShader, ShaderParameterName.CompileStatus, out success);
        if (success == 0)
        {
            _gl.GetShaderInfoLog(vertexShader, out infoLog);
            Console.WriteLine("ERROR::SHADER::VERTEX::COMPILATION_FAILED\n" + infoLog);
        }

        // fragment shader
        uint fragmentShader = _gl.CreateShader(ShaderType.FragmentShader);
        _gl.ShaderSource(fragmentShader, _fragmentShaderSource);
        _gl.CompileShader(fragmentShader);

        // verificar erros de compilação de shader
        _gl.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out success);
        if (success == 0)
        {
            _gl.GetShaderInfoLog(fragmentShader, out infoLog);
            Console.WriteLine("ERROR::SHADER::FRAGMENT::COMPILATION_FAILED\n" + infoLog);
        }

        // link shaders
        _shaderProgram = _gl.CreateProgram();
        _gl.AttachShader(_shaderProgram, vertexShader);
        _gl.AttachShader(_shaderProgram, fragmentShader);
        _gl.LinkProgram(_shaderProgram);

        // verificar erros de vinculação
        _gl.GetProgram(_shaderProgram, ProgramPropertyARB.LinkStatus, out success);
        if (success == 0)
        {
            _gl.GetProgramInfoLog(_shaderProgram, out infoLog);
            Console.WriteLine("ERROR::SHADER::PROGRAM::LINKING_FAILED\n" + infoLog);
        }

        _gl.DeleteShader(vertexShader);
        _gl.DeleteShader(fragmentShader);

        _gl.GenVertexArrays(2, _vertexArrayObject); // também podemos gerar múltiplos VAOs ou buffers ao mesmo tempo
        _gl.GenBuffers(2, _vertexBufferObject);

        // configuração do primeiro triângulo
        // --------------------------------------------------
        _gl.BindVertexArray(_vertexArrayObject[0]);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBufferObject[0]);
        unsafe
        {
           fixed (float* buf = _firstTriangle)
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(_firstTriangle.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
            }
        }

        unsafe
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0); // Os atributos de vértice permanecem os mesmos
        }
        _gl.EnableVertexAttribArray(0);

        // _gl.BindVertexArray(0); // não é necessário desfazer a vinculação, pois vinculamos diretamente um VAO diferente nas próximas linhas

        // configuração do segundo triângulo
        // --------------------------------------------------
        _gl.BindVertexArray(_vertexArrayObject[1]); // observe que agora vinculamos a um VAO diferente

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBufferObject[1]); // e um VBO diferente
        unsafe
        {
           fixed (float* buf = _secondTriangle)
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(_secondTriangle.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
            }
        }

        unsafe
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, (void*)0); // como os dados dos vértices estão compactados, também podemos especificar 0 como o stride do atributo de vértice para deixar o OpenGL determiná-lo
        }
        _gl.EnableVertexAttribArray(0);

        // _gl.BindVertexArray(0); // também não é estritamente necessário, mas cuidado com chamadas que possam afetar VAOs enquanto este estiver vinculado (como vincular *element buffer objects* ou habilitar/desabilitar atributos de vértice)
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
    }

    public void Render()
    {
        _gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        _gl.Clear(ClearBufferMask.ColorBufferBit);

        _gl.UseProgram(_shaderProgram);

        // desenha o primeiro triângulo usando os dados do primeiro VAO
        _gl.BindVertexArray(_vertexArrayObject[0]);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 3);

        // então, desenhamos o segundo triângulo usando os dados do segundo VAO
        _gl.BindVertexArray(_vertexArrayObject[1]);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 3);
    }

    public void Clear()
    {
        // opcional: desalocar todos os recursos assim que não forem mais necessários:
        // --------------------------------------------------
        _gl.DeleteVertexArrays(2, _vertexArrayObject);
        _gl.DeleteBuffers(2, _vertexBufferObject);

        _gl.DeleteProgram(_shaderProgram);
    }
}
