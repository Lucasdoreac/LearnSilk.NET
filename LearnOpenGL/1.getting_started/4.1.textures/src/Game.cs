using LearnSilkNET.Inputs;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace LearnSilkNET;

public class Game
{
    private GL _gl = Program.GL;

    private Shader _ourShader = null!;

    private uint _texture;

    // configurar dados de vértice (e buffer(s)) e configurar atributos de vértice
    // --------------------------------------------------
    private float[] _vertices =
    {
        // positions           // colors           // texture coords
        -0.5f, -0.5f,  0.0f,   1.0f, 0.0f, 0.0f,   0.0f, 0.0f,   // inferior esquerdo
         0.5f, -0.5f,  0.0f,   0.0f, 1.0f, 0.0f,   1.0f, 0.0f,   // inferior direito
         0.5f,  0.5f,  0.0f,   0.0f, 0.0f, 1.0f,   1.0f, 1.0f,   // superior direito
        -0.5f,  0.5f,  0.0f,   1.0f, 1.0f, 0.0f,   0.0f, 1.0f    // superior esquerdo
    };

    private uint[] _indices =
    {
        0, 1, 2, // primeiro triangulo
        0, 2, 3  // segundo triangulo
    };

    private uint _vertexArrayObject;
    private uint _vertexBufferObject;
    private uint _elementBufferObject;

    public Game()
    {
        
    }

    public void Init()
    {
        // construir e compilar nosso programa de shader
        // --------------------------------------------------
        _ourShader = new Shader( // você pode nomear seus arquivos de shader como quiser
            "res/Shaders/texture/vertex.glsl",
            "res/Shaders/texture/fragment.glsl"
        );

        // carregar e criar uma textura
        // --------------------------------------------------
        _gl.GenTextures(1, out _texture);
        _gl.BindTexture(TextureTarget.Texture2D, _texture); // todas as operações GL_TEXTURE_2D subsequentes agora afetam este objeto de textura

        // define os parâmetros de repetição da textura
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat); // define o modo de repetição da textura como GL_REPEAT (método padrão)
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        // definir parâmetros de filtragem de textura
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        // carregar imagem, criar textura e gerar mipmaps
        int width, height;
        byte[] data;

        using (FileStream stream = File.OpenRead("res/Textures/container.jpg"))
        {
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlue);

            width = image.Width;
            height = image.Height;
            data = image.Data;
        }

        if (data != null)
        {
            unsafe
            {
                fixed (byte* ptr = data)
                {
                    _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)width, (uint)height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, ptr);
                }
            }
            _gl.GenerateMipmap(TextureTarget.Texture2D);
        }
        else
        {
            Console.WriteLine("Falha ao carregar a textura");
        }

        _gl.GenVertexArrays(1, out _vertexArrayObject);
        _gl.GenBuffers(1, out _vertexBufferObject);
        _gl.GenBuffers(1, out _elementBufferObject);

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

        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _elementBufferObject);
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
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)0);
        }
        _gl.EnableVertexAttribArray(0);

         // color attribute
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
        
        // renderizar contêiner

        _ourShader.Use();

        // vincular textura
        _gl.BindTexture(TextureTarget.Texture2D, _texture);

        _gl.BindVertexArray(_vertexArrayObject);
        unsafe
        {
            _gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*)0);
        }
    }

    public void Clear()
    {
        // opcional: desalocar todos os recursos assim que não forem mais necessários:
        // --------------------------------------------------
        _gl.DeleteVertexArrays(1, ref _vertexArrayObject);
        _gl.DeleteBuffers(1, ref _vertexBufferObject);
    }
}
