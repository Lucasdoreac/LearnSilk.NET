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

    private Shader _ourShader;

    private uint _texture1, _texture2;

    // configurar dados de vértice (e buffer(s)) e configurar atributos de vértice
    // --------------------------------------------------
    private float[] _vertices =
    {
        // positions           // texture coords
        -0.5f, -0.5f, -0.5f,   0.0f, 0.0f,
        -0.5f, -0.5f,  0.5f,   1.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,   1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,   0.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,   1.0f, 1.0f,
        -0.5f,  0.5f, -0.5f,   0.0f, 1.0f,
        
         0.5f, -0.5f,  0.5f,   0.0f, 0.0f,
         0.5f, -0.5f, -0.5f,   1.0f, 0.0f,
         0.5f,  0.5f, -0.5f,   1.0f, 1.0f,
         0.5f, -0.5f,  0.5f,   0.0f, 0.0f,
         0.5f,  0.5f, -0.5f,   1.0f, 1.0f,
         0.5f,  0.5f,  0.5f,   0.0f, 1.0f,
        
        -0.5f, -0.5f, -0.5f,   0.0f, 0.0f,
         0.5f, -0.5f, -0.5f,   1.0f, 0.0f,
         0.5f, -0.5f,  0.5f,   1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,   0.0f, 0.0f,
         0.5f, -0.5f,  0.5f,   1.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,   0.0f, 1.0f,
        
        -0.5f,  0.5f,  0.5f,   0.0f, 0.0f,
         0.5f,  0.5f,  0.5f,   1.0f, 0.0f,
         0.5f,  0.5f, -0.5f,   1.0f, 1.0f,
        -0.5f,  0.5f,  0.5f,   0.0f, 0.0f,
         0.5f,  0.5f, -0.5f,   1.0f, 1.0f,
        -0.5f,  0.5f, -0.5f,   0.0f, 1.0f,
        
         0.5f, -0.5f, -0.5f,   0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f,   1.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,   1.0f, 1.0f,
         0.5f, -0.5f, -0.5f,   0.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,   1.0f, 1.0f,
         0.5f,  0.5f, -0.5f,   0.0f, 1.0f,
        
        -0.5f, -0.5f,  0.5f,   0.0f, 0.0f,
         0.5f, -0.5f,  0.5f,   1.0f, 0.0f,
         0.5f,  0.5f,  0.5f,   1.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,   0.0f, 0.0f,
         0.5f,  0.5f,  0.5f,   1.0f, 1.0f,
        -0.5f,  0.5f,  0.5f,   0.0f, 1.0f
    };

    private uint _vertexArrayObject;
    private uint _vertexBufferObject;

    // world space positions of our cubes
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

    public Game()
    {
        // construir e compilar nosso programa de shader
        // --------------------------------------------------
        _ourShader = new Shader( // você pode nomear seus arquivos de shader como quiser
            "res/Shaders/coordinate_systems/vertex.glsl",
            "res/Shaders/coordinate_systems/fragment.glsl"
        );
    }

    public void Init()
    {
        // configurar estado global do OpenGL
        // --------------------------------------------------
        _gl.Enable(EnableCap.DepthTest);

        // carregar e criar uma textura
        // --------------------------------------------------

        // texture 1
        // --------------------------------------------------
        _gl.GenTextures(1, out _texture1);
        _gl.BindTexture(TextureTarget.Texture2D, _texture1);

        // define os parâmetros de repetição da textura
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        // definir parâmetros de filtragem de textura
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        // carregar imagem, criar textura e gerar mipmaps
        int width, height;
        byte[] data;

        StbImage.stbi_set_flip_vertically_on_load(1); // instrui a stb_image.h a inverter as texturas carregadas no eixo Y.

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

        // texture 2
        // --------------------------------------------------
        _gl.GenTextures(1, out _texture2);
        _gl.BindTexture(TextureTarget.Texture2D, _texture2);

        // define os parâmetros de repetição da textura
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        // definir parâmetros de filtragem de textura
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        
        // carregar imagem, criar textura e gerar mipmaps
        using (FileStream stream = File.OpenRead("res/Textures//awesomeface.png"))
        {
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

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
                    // observe que o awesomeface.png possui transparência e, portanto, um canal alfa; certifique-se de informar ao OpenGL que o tipo de dado é GL_RGBA
                    _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)width, (uint)height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
                }
            }
            _gl.GenerateMipmap(TextureTarget.Texture2D);
        }
        else
        {
            Console.WriteLine("Falha ao carregar a textura");
        }

        // informar ao OpenGL, para cada sampler, a qual unidade de textura ele pertence (isso só precisa ser feito uma vez)
        // --------------------------------------------------
        _ourShader.Use();
        _ourShader.SetInt("texture1", 0);
        _ourShader.SetInt("texture2", 1);

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
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)0);
        }
        _gl.EnableVertexAttribArray(0);

        // texture coord attribute
        unsafe
        {
            _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
        }
        _gl.EnableVertexAttribArray(1);
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
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // limpe também o buffer de profundidade agora!

        // ativar shader
        _ourShader.Use();

        // criar transformações
        Matrix4x4 projection = Matrix4x4.Identity; // certifique-se de inicializar a matriz como a matriz identidade primeiro
        Matrix4x4 view = Matrix4x4.Identity;

        projection *= Matrix4x4.CreatePerspectiveFieldOfView(
            fieldOfView:       MathHelper.DegreesToRadians(45.0f),
            aspectRatio:       (float)Screen.Width / (float)Screen.Height,
            nearPlaneDistance: 0.1f,
            farPlaneDistance:  100.0f
        );
        view *= Matrix4x4.CreateTranslation(new Vector3(0.0f, 0.0f, -3.0f));

        // passa as matrizes de transformação para o shader
        _ourShader.setMat4("projection", projection); // nota: atualmente definimos a matriz de projeção a cada quadro, mas, como ela raramente muda, geralmente é uma boa prática defini-la apenas uma vez, fora do loop principal.
        _ourShader.setMat4("view", view);

        // vincular texturas às unidades de textura correspondentes
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _texture1);
        _gl.ActiveTexture(TextureUnit.Texture1);
        _gl.BindTexture(TextureTarget.Texture2D, _texture2);

        // renderizar caixa
        _gl.BindVertexArray(_vertexArrayObject);
        for (int i = 0; i < 10; i++)
        {
            // calcula a matriz de modelo para cada objeto e a passa para o shader antes de desenhar
            Matrix4x4 model = Matrix4x4.Identity;

            float angle = 20.0f * i;
            if (i % 3 == 0) // a cada 3 iterações (incluindo a primeira), definimos o ângulo usando a função de tempo da GLFW.
            {
                angle = Time.ElapsedTime * 25.0f;
            }
            model *= Matrix4x4.CreateFromAxisAngle(
                Vector3.Normalize(new Vector3(1.0f, 0.3f, 0.5f)),
                MathHelper.DegreesToRadians(angle)
            );

            model *= Matrix4x4.CreateTranslation(_cubePositions[i]);

            _ourShader.setMat4("model", model);

            _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
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
