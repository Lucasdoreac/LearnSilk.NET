using System.Numerics;
using MySilkProgram.Inputs;
using MySilkProgram.Utilities;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using StbImageSharp;

namespace MySilkProgram;

public class Game
{
    private IWindow _window = null!;

    private GL _gl = null!;
    public static GL GL = null!;

    private Shader _shader = null!;

    private Camera _camera = null!;

    private uint cubeTexture, floorTexture;

    // configurar dados de vértice (e buffer(s)) e configurar atributos de vértice
    // ---------------------------------------------------------------------------
    private readonly float[] _cubeVertices =
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

    private readonly uint[] _cubeIndices = // observe que começamos do 0!
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

    private readonly float[] _planeVertices =
    {
        // positions          // texture Coords (Observe que definimos esses valores como maiores que 1 (juntamente com GL_REPEAT como modo de repetição de textura). Isso fará com que a textura do chão se repita.)
         5.0f, -0.5f,  5.0f,  2.0f, 0.0f,
        -5.0f, -0.5f,  5.0f,  0.0f, 0.0f,
        -5.0f, -0.5f, -5.0f,  0.0f, 2.0f,

         5.0f, -0.5f,  5.0f,  2.0f, 0.0f,
        -5.0f, -0.5f, -5.0f,  0.0f, 2.0f,
         5.0f, -0.5f, -5.0f,  2.0f, 2.0f
    };

    private uint cubeVAO, planeVAO;
    private uint cubeVBO, planeVBO;
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

        Input.Initialize(_window);

        _gl = _window.CreateOpenGL();
        GL = _gl;

        _gl.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        
        // configurar o estado global do OpenGL
        // -----------------------------
        _gl.Enable(EnableCap.DepthTest);
        _gl.DepthFunc(DepthFunction.Less); // sempre passa no teste de profundidade (mesmo efeito que glDisable(GL_DEPTH_TEST))

        // construir e compilar nosso programa de shader
        // ------------------------------------
        _shader = new Shader( // você pode nomear seus arquivos de shader como quiser
            "res/Shaders/base/vertex.glsl",
            "res/Shaders/base/fragment.glsl"
        );

        // carregar texturas
        // -------------
        cubeTexture = LoadTexture("res/Textures/marble.jpg");
        floorTexture = LoadTexture("res/Textures/metal.png");

        // configuração do shader
        // --------------------
        _shader.Use();
        _shader.SetUniform("texture1", 0);

        // cube VAO
        _gl.GenVertexArrays(1, out cubeVAO);
        _gl.GenBuffers(1, out cubeVBO);
        _gl.GenBuffers(1, out EBO);

        _gl.BindVertexArray(cubeVAO);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, cubeVBO);
        unsafe
        {
            fixed (float* buf = _cubeVertices)
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(_cubeVertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
            }
        }

        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, EBO);
        unsafe
        {
            fixed (uint* buf = _cubeIndices)
            {
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (uint)(_cubeIndices.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);
            }
        }

        // position attribute
        _gl.EnableVertexAttribArray(0);
        unsafe
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 11 * sizeof(float), (void*)0);
        }

        // texture attribute
        _gl.EnableVertexAttribArray(1);
        unsafe
        {
            _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 11 * sizeof(float), (void*)(6 * sizeof(float)));
        }

        _gl.BindVertexArray(0);

        // plane VAO
        _gl.GenVertexArrays(1, out planeVAO);
        _gl.GenBuffers(1, out planeVBO);

        _gl.BindVertexArray(planeVAO);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, planeVBO);
        unsafe
        {
            fixed (float* buf = _planeVertices)
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(_planeVertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
            }
        }
        
        // position attribute
        _gl.EnableVertexAttribArray(0);
        unsafe
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)0);
        }

        // texture attribute
        _gl.EnableVertexAttribArray(1);
        unsafe
        {
            _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
        }

        _gl.BindVertexArray(0);

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

        _shader.Use();

        // view/projection transformations

        Matrix4x4 projection = _camera.GetProjectionMatrix(_window);
        _shader.SetUniform("projection", projection);

        Matrix4x4 view = _camera.GetViewMatrix();
        _shader.SetUniform("view", view);

        // world transformation
        Matrix4x4 model = Matrix4x4.Identity;
        _shader.SetUniform("model", model);

        // cubes
        _gl.BindVertexArray(cubeVAO);

        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, cubeTexture);
        
        model = Matrix4x4.Identity;
        model *= Matrix4x4.CreateTranslation(new Vector3(-1.0f, 0.0f, -1.0f));
        _shader.SetUniform("model", model);

        unsafe
        {
            _gl.DrawElements(PrimitiveType.Triangles, (uint)_cubeIndices.Length, DrawElementsType.UnsignedInt, (void*)0);
        }

        model = Matrix4x4.Identity;
        model *= Matrix4x4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f));
        _shader.SetUniform("model", model);

        unsafe
        {
            _gl.DrawElements(PrimitiveType.Triangles, (uint)_cubeIndices.Length, DrawElementsType.UnsignedInt, (void*)0);
        }

        // floor
        _gl.BindVertexArray(planeVAO);

        _gl.BindTexture(TextureTarget.Texture2D, floorTexture);

        model = Matrix4x4.Identity;
        _shader.SetUniform("model", model);

        _gl.DrawArrays(PrimitiveType.Triangles, 0, 6);

        _gl.BindVertexArray(0);
    }

    private void OnClosing()
    {
        // opcional: desalocar todos os recursos assim que não forem mais necessários:
        // ---------------------------------------------------------------------------
        _gl.DeleteVertexArrays(1, ref cubeVAO);
        _gl.DeleteVertexArrays(1, ref planeVAO);
        _gl.DeleteBuffers(1, ref cubeVBO);
        _gl.DeleteBuffers(1, ref planeVBO);
        _gl.DeleteBuffers(1, ref EBO);
        
        _shader.Dispose();
    }

    private uint LoadTexture(string path)
    {
        uint textureID;
        _gl.GenTextures(1, out textureID);

        int width, height;
        byte[] data;

        ImageResult image;

        using (Stream stream = File.OpenRead(path))
        {
            image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            width  = image.Width;
            height = image.Height;
            data   = image.Data;
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
