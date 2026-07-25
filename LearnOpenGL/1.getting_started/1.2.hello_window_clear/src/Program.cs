using LearnSilkNET.Inputs;
using Microsoft.VisualBasic;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace LearnSilkNET;

public class Program
{
    private static IWindow _window = null!;

    private static GL _gl = null!;
    public static GL GL = null!;

    private static void Main(string[] args)
    {
        // criação da janela Silk.NET
        // --------------------------------------------------
        WindowOptions options = WindowOptions.Default;

        options.Size = new Vector2D<int>(800, 600);
        options.Title = "Learn Silk.NET";
        options.IsVisible = false;

        _window = Window.Create(options);

        Run();
    }

    private static void Run()
    {
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
                "Falha ao criar a janela GLFW" + "\n" +
                ex
            );
        }
    }

    private static void OnLoad()
    {
        if (OperatingSystem.IsWindows())
        {
            // Usuarios de Linux relataram problemas com esta linha
            _window.Center();
        }
        _window.IsVisible = true;

        Input.Initialize(_window);

        _gl = _window.CreateOpenGL();
        GL = _gl;
    }

    // glfw: sempre que o tamanho da janela é alterado (pelo SO ou redimensionamento do usuário), esta função de callback é executada
    // --------------------------------------------------
    private static void OnResize(Vector2D<int> newSize)
    {
        // certifique-se de que a viewport corresponda às novas dimensões da janela; observe que a largura e a altura serão significativamente maiores do que as especificadas em telas Retina.
        _gl.Viewport(0, 0, (uint)newSize.X, (uint)newSize.Y);
    }

    private static void OnUpdate(double deltaTime)
    {
        Input.NewFrame();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _window.Close();
        }
    }

    private static void OnRender(double deltaTime)
    {
        _gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        _gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    private static void OnClosing()
    {
        
    }
}
