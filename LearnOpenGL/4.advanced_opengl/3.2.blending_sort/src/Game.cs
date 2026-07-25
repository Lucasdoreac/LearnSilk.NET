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

    private Shader _shader;

    private Camera _camera;

    private uint _cubeTexture;
    private uint _floorTexture;
    private uint _transparentTexture;

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

    private float[] _planeVertices =
    {
        // positions           // texture Coords
        -5.0f, -0.5f, -5.0f,   0.0f, 0.0f,
         5.0f, -0.5f, -5.0f,   2.0f, 0.0f,
         5.0f, -0.5f,  5.0f,   2.0f, 2.0f,
        -5.0f, -0.5f, -5.0f,   0.0f, 0.0f,
         5.0f, -0.5f,  5.0f,   2.0f, 2.0f,
        -5.0f, -0.5f,  5.0f,   0.0f, 2.0f
    };

    private float[] _transparentVertices =
    {
        // positions           // texture Coords
         0.0f, -0.5f,  0.0f,   0.0f, 1.0f,
         1.0f, -0.5f,  0.0f,   1.0f, 1.0f,
         1.0f,  0.5f,  0.0f,   1.0f, 0.0f,
         0.0f, -0.5f,  0.0f,   0.0f, 1.0f,
         1.0f,  0.5f,  0.0f,   1.0f, 0.0f,
         0.0f,  0.5f,  0.0f,   0.0f, 0.0f
    };

    private uint _cubeVAO, _planeVAO, _transparentVAO;
    private uint _cubeVBO, _planeVBO, _transparentVBO;

    private Vector3[] _vegetation =
    {
        new Vector3(-1.5f, 0.0f, -0.48f),
        new Vector3( 1.5f, 0.0f, 0.51f),
        new Vector3( 0.0f, 0.0f, 0.7f),
        new Vector3(-0.3f, 0.0f, -2.3f),
        new Vector3 (0.5f, 0.0f, -0.6f)
    };

    public Game()
    {
        // construir e compilar nosso programa de shader
        // --------------------------------------------------
        _shader = new Shader(
            "res/Shaders/blending/vertex.glsl",
            "res/Shaders/blending/fragment.glsl"
        );

        // carregar texturas (agora usamos uma função utilitária para manter o código mais organizado)
        // --------------------------------------------------
        _cubeTexture = LoadTexture("res/Textures/marble.jpg");
        _floorTexture = LoadTexture("res/Textures/metal.png");
        _transparentTexture = LoadTexture("res/Textures/window.png");

        // configuração do shader
        // --------------------------------------------------
        _shader.Use();
        _shader.SetInt("texture1", 0);

        // camera
        _camera = new Camera(new Vector3(0.0f, 0.0f, 3.0f));

        Input.CursorLockMode = CursorLockMode.Raw;
    }

    public void Init()
    {
        // configurar estado global do OpenGL
        // --------------------------------------------------
        _gl.Enable(EnableCap.DepthTest);

        _gl.Enable(EnableCap.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        // cube VAO
        _gl.GenVertexArrays(1, out _cubeVAO);
        _gl.GenBuffers(1, out _cubeVBO);
        
        _gl.BindVertexArray(_cubeVAO);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _cubeVBO);
        unsafe
        {
           fixed (float* buf = _vertices)
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(_vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
            }
        }

        // position attribute
        _gl.EnableVertexAttribArray(0);
        unsafe
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)0);
        }

        // texture coord attribute
        _gl.EnableVertexAttribArray(1);
        unsafe
        {
            _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
        }
        
        _gl.BindVertexArray(0);

        // plane VAO
        _gl.GenVertexArrays(1, out _planeVAO);
        _gl.GenBuffers(1, out _planeVBO);

        _gl.BindVertexArray(_planeVAO);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _planeVBO);
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

        // texture coord attribute
        _gl.EnableVertexAttribArray(1);
        unsafe
        {
            _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
        }

        // transparent VAO
        _gl.GenVertexArrays(1, out _transparentVAO);
        _gl.GenBuffers(1, out _transparentVBO);

        _gl.BindVertexArray(_transparentVAO);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _transparentVBO);
        unsafe
        {
           fixed (float* buf = _transparentVertices)
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(_transparentVertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
            }
        }

        // position attribute
        _gl.EnableVertexAttribArray(0);
        unsafe
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)0);
        }

        // texture coord attribute
        _gl.EnableVertexAttribArray(1);
        unsafe
        {
            _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
        }
        
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

        // --------------------------------------------------
        
        _camera.Update();
    }

    public void Render()
    {
        _gl.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // draw objects
        _shader.Use();

        Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(
            fieldOfView:       MathHelper.DegreesToRadians(_camera.Zoom), 
            aspectRatio:       (float)Screen.Width / (float)Screen.Height, 
            nearPlaneDistance: 0.1f, 
            farPlaneDistance:  100.0f
        );
        _shader.setMat4("projection", projection);

        Matrix4x4 view = _camera.GetViewMatrix();
        _shader.setMat4("view", view);

        Matrix4x4 model = Matrix4x4.Identity;
        _shader.setMat4("model", model);

        // cubes
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _cubeTexture);

        _gl.BindVertexArray(_cubeVAO);

        model = Matrix4x4.Identity;
        model *= Matrix4x4.CreateTranslation(new Vector3(-1.0f, 0.0f, -1.0f));
        _shader.setMat4("model", model);

        _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);

        model = Matrix4x4.Identity;
        model *= Matrix4x4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f));
        _shader.setMat4("model", model);

        _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);

        // floor
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _floorTexture);

        model = Matrix4x4.Identity;
        _shader.setMat4("model", model);

        _gl.BindVertexArray(_planeVAO);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 6);

        // windows (do mais distante para o mais próximo)
        // --------------------------------------------------
        // o blending só produz o resultado correto se as superfícies transparentes
        // forem desenhadas de trás para frente: uma janela próxima desenhada antes
        // grava no depth buffer e descarta as janelas mais distantes atrás dela.
        Array.Sort(_vegetation, (a, b) =>
            Vector3.DistanceSquared(_camera.Position, b)
                .CompareTo(Vector3.DistanceSquared(_camera.Position, a))
        );

        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _transparentTexture);

        _gl.BindVertexArray(_transparentVAO);
        for (int i = 0; i < _vegetation.Length; i++)
        {
            model = Matrix4x4.Identity;
            model *= Matrix4x4.CreateTranslation(_vegetation[i]);
            _shader.setMat4("model", model);

            _gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }
    }

    public void Clear()
    {
        // opcional: desalocar todos os recursos assim que não forem mais necessários:
        // --------------------------------------------------
        _gl.DeleteVertexArrays(1, ref _cubeVAO);
        _gl.DeleteVertexArrays(1, ref _planeVAO);
        _gl.DeleteVertexArrays(1, ref _transparentVAO);
        _gl.DeleteBuffers(1, ref _cubeVBO);
        _gl.DeleteBuffers(1, ref _planeVBO);
        _gl.DeleteBuffers(1, ref _transparentVBO);
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

            // texturas com alpha usam ClampToEdge: com Repeat, a filtragem linear
            // busca o lado oposto da imagem e deixa uma borda semitransparente no topo da janela
            TextureWrapMode wrapMode = pixelFormat == PixelFormat.Rgba
                ? TextureWrapMode.ClampToEdge
                : TextureWrapMode.Repeat;

            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);
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
