using LearnSilkNET.Inputs;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace LearnSilkNET;

public class Game
{
    private GL _gl = Program.GL;

    private Shader _ourShader;

    // configurar dados de vértice (e buffer(s)) e configurar atributos de vértice
    // --------------------------------------------------
    private float[] _vertices =
    {
        // positions           // colors
        -0.5f, -0.5f,  0.0f,   1.0f, 0.0f, 0.0f,   // vertice inferior esquerdo
         0.5f, -0.5f,  0.0f,   0.0f, 1.0f, 0.0f,   // vertice inferior direito
         0.0f,  0.5f,  0.0f,   0.0f, 0.0f, 1.0f    // vertice superior
    };

    private uint _vertexArrayObject;
    private uint _vertexBufferObject;

    public Game()
    {
        // construir e compilar nosso programa de shader
        // --------------------------------------------------
        _ourShader = new Shader( // você pode nomear seus arquivos de shader como quiser
            "res/Shaders/shader/vertex.glsl",
            "res/Shaders/shader/fragment.glsl"
        );
    }

    public void Init()
    {
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

        // position attribute
        unsafe
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), (void*)0);
        }
        _gl.EnableVertexAttribArray(0);

         // color attribute
        unsafe
        {
            _gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), (void*)(3 * sizeof(float)));
        }
        _gl.EnableVertexAttribArray(1);

        // Você pode desvincular o VAO posteriormente para que outras chamadas de VAO não modifiquem acidentalmente este VAO, mas isso raramente acontece. Modificar outros VAOs exige uma chamada para glBindVertexArray de qualquer forma, então geralmente não desvinculamos VAOs (nem VBOs) quando não é diretamente necessário.
        // _gl.BindVertexArray(0);
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
        
        // renderiza o triângulo

        _ourShader.Use();

        _gl.BindVertexArray(_vertexArrayObject);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 3);
    }

    public void Clear()
    {
        // opcional: desalocar todos os recursos assim que não forem mais necessários:
        // --------------------------------------------------
        _gl.DeleteVertexArrays(1, ref _vertexArrayObject);
        _gl.DeleteBuffers(1, ref _vertexBufferObject);
    }
}
