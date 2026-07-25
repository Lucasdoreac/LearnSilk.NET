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

    // adicione um novo conjunto de vértices para formar um segundo triângulo (um total de 6 vértices); a configuração dos atributos de vértice permanece a mesma (ainda um vetor de posição de 3 floats por vértice)
    private float[] _vertices =
    {
        // primeiro triangulo
        -0.9f,  -0.5f,  0.0f, // vertice inferior esquerdo
         0.0f,  -0.5f,  0.0f, // vertice inferior direito
        -0.45f,  0.5f,  0.0f, // vertice supeior

        // segundo triangulo
         0.0f,  -0.5f,  0.0f, // vertice inferior esquerdo
         0.9f,  -0.5f,  0.0f, // vertice inferior direito
         0.45f,  0.5f,  0.0f  // vertice supeior
    };

    private uint _vertexArrayObject;
    private uint _vertexBufferObject;

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

        _gl.GenVertexArrays(1, out _vertexArrayObject);
        _gl.GenBuffers(1, out _vertexBufferObject);

        // primeiro vincule o Vertex Array Object, depois vincule e configure o(s) buffer(s) de vértices e, em seguida, configure o(s) atributo(s) de vértice.
        _gl.BindVertexArray(_vertexArrayObject);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBufferObject);
        unsafe
        {
           fixed (float* buf = _vertices)
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(_vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
            }
        }

        unsafe
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);
        }
        _gl.EnableVertexAttribArray(0);

        // observe que isso é permitido; a chamada para glVertexAttribPointer registrou o VBO como o objeto de buffer de vértices vinculado ao atributo de vértice, portanto, podemos desvinculá-lo com segurança logo em seguida
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

        // Você pode desvincular o VAO posteriormente para que outras chamadas de VAO não modifiquem acidentalmente este VAO, mas isso raramente acontece. Modificar outros
        // VAOs exige uma chamada para glBindVertexArray de qualquer forma, então geralmente não desvinculamos VAOs (nem VBOs) quando não é diretamente necessário.
        _gl.BindVertexArray(0);
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

        // desenhar nosso primeiro triângulo

        _gl.UseProgram(_shaderProgram);

        _gl.BindVertexArray(_vertexArrayObject); // como temos apenas um único VAO, não há necessidade de vinculá-lo todas as vezes, mas faremos isso para manter as coisas um pouco mais organizadas
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 6); // defina a contagem como 6, já que estamos desenhando 6 vértices agora (2 triângulos); não 3!
        // _gl.BindVertexArray(0); // não é necessário desvinculá-lo toda vez
    }

    public void Clear()
    {
        // opcional: desalocar todos os recursos assim que não forem mais necessários:
        // --------------------------------------------------
        _gl.DeleteVertexArrays(1, ref _vertexArrayObject);
        _gl.DeleteBuffers(1, ref _vertexBufferObject);

        _gl.DeleteProgram(_shaderProgram);
    }
}
