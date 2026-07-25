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
    private Shader _shaderSingleColor;

    private Camera _camera;

    private uint _cubeTexture;
    private uint _floorTexture;

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
        // positions           // texture Coords (Observe que definimos esses valores como maiores que 1 (juntamente com GL_REPEAT como modo de repetição de textura). Isso fará com que a textura do chão se repita.)
        -5.0f, -0.5f, -5.0f,   0.0f, 0.0f,
         5.0f, -0.5f, -5.0f,   2.0f, 0.0f,
         5.0f, -0.5f,  5.0f,   2.0f, 2.0f,
        -5.0f, -0.5f, -5.0f,   0.0f, 0.0f,
         5.0f, -0.5f,  5.0f,   2.0f, 2.0f,
        -5.0f, -0.5f,  5.0f,   0.0f, 2.0f
    };

    private uint _cubeVAO, _planeVAO;
    private uint _cubeVBO, _planeVBO;

    public Game()
    {
        // construir e compilar nosso programa de shader
        // --------------------------------------------------
        _shader = new Shader(
            "res/Shaders/stencil_testing/vertex.glsl",
            "res/Shaders/stencil_testing/fragment.glsl"
        );

        _shaderSingleColor = new Shader(
            "res/Shaders/stencil_testing/vertex.glsl",
            "res/Shaders/stencil_single_color/fragment.glsl"
        );

        // carregar texturas (agora usamos uma função utilitária para manter o código mais organizado)
        // --------------------------------------------------
        _cubeTexture = LoadTexture("res/Textures/marble.jpg");
        _floorTexture = LoadTexture("res/Textures/metal.png");

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
        _gl.DepthFunc(DepthFunction.Less);

        _gl.Enable(EnableCap.StencilTest);
        _gl.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
        _gl.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);

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
        _gl.Clear(
            ClearBufferMask.ColorBufferBit | 
            ClearBufferMask.DepthBufferBit | 
            ClearBufferMask.StencilBufferBit
        ); // não se esqueça de limpar o stencil buffer!

        // set uniforms
        _shaderSingleColor.Use();

        Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(
            fieldOfView:       MathHelper.DegreesToRadians(_camera.Zoom), 
            aspectRatio:       (float)Screen.Width / (float)Screen.Height, 
            nearPlaneDistance: 0.1f, 
            farPlaneDistance:  100.0f
        );
        _shaderSingleColor.setMat4("projection", projection);

        Matrix4x4 view = _camera.GetViewMatrix();
        _shaderSingleColor.setMat4("view", view);

        Matrix4x4 model = Matrix4x4.Identity;
        _shaderSingleColor.setMat4("model", model);

        _shader.Use();
        _shader.setMat4("projection", projection);
        _shader.setMat4("view", view);

        // Desenhe o chão normalmente, mas não o grave no stencil buffer; só nos interessam os contêineres. Definimos a máscara como 0x00 para não gravar no stencil buffer.
        _gl.StencilMask(0x00);
        _gl.Disable(EnableCap.StencilTest);

        // floor
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _floorTexture);

        model = Matrix4x4.Identity;
        _shader.setMat4("model", model);

        _gl.BindVertexArray(_planeVAO);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 6);

        _gl.BindVertexArray(0);

        _gl.Enable(EnableCap.StencilTest);

        // 1ª passagem de renderização: desenha os objetos normalmente, escrevendo no stencil buffer
        // --------------------------------------------------
        _gl.StencilFunc(StencilFunction.Always, 1, 0xFF);
        _gl.StencilMask(0xFF);

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

        // 2ª passada de renderização: agora, desenhe versões dos objetos levemente redimensionadas, desta vez desativando a escrita no stencil buffer. 
        // Isso ocorre porque o stencil buffer já está preenchido com vários valores 1. As partes do buffer que contêm 1 não são desenhadas; 
        // assim, desenham-se apenas as diferenças de tamanho dos objetos, criando um efeito de bordas.
        // --------------------------------------------------
        _gl.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
        _gl.StencilMask(0x00);

        _gl.Disable(EnableCap.DepthTest);

        _shaderSingleColor.Use();

        float scale = 1.1f;

        // cubes
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _cubeTexture);

        _gl.BindVertexArray(_cubeVAO);

        model = Matrix4x4.Identity;
        model *= Matrix4x4.CreateScale(new Vector3(scale, scale, scale));
        model *= Matrix4x4.CreateTranslation(new Vector3(-1.0f, 0.0f, -1.0f));
        _shaderSingleColor.setMat4("model", model);

        _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);

        model = Matrix4x4.Identity;
        model *= Matrix4x4.CreateScale(new Vector3(scale, scale, scale));
        model *= Matrix4x4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f));
        _shaderSingleColor.setMat4("model", model);

        _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);

        _gl.BindVertexArray(0);

        _gl.StencilMask(0xFF);
        _gl.StencilFunc(StencilFunction.Notequal, 0, 0xFF);

        _gl.Enable(EnableCap.DepthTest);
    }

    public void Clear()
    {
        // opcional: desalocar todos os recursos assim que não forem mais necessários:
        // --------------------------------------------------
        _gl.DeleteVertexArrays(1, ref _cubeVAO);
        _gl.DeleteVertexArrays(1, ref _planeVAO);
        _gl.DeleteBuffers(1, ref _cubeVBO);
        _gl.DeleteBuffers(1, ref _planeVBO);
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
