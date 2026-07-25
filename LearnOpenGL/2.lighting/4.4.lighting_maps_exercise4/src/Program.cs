using LearnSilkNET.Inputs;
using LearnSilkNET.Utilities;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace LearnSilkNET;

public class Program
{
    private static IWindow _window = null!;

    private static GL _gl = null!;
    public static GL GL = null!;

    private static Game _game = null!;

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

    public static void Close()
    {
        _window.Close();
    }

    private static void OnLoad()
    {
        if (OperatingSystem.IsWindows()) 
        {
            // Usuarios de Linux relataram problemas com esta linha
            _window.Center(); 
        }
        _window.IsVisible = true;

        Screen.Initialize(_window);
        Input.Initialize(_window);

        _gl = _window.CreateOpenGL();
        GL = _gl;

        _game = new Game();
        _game.Init();
    }

    // glfw: sempre que o tamanho da janela é alterado (pelo SO ou redimensionamento do usuário), esta função de callback é executada
    // --------------------------------------------------
    private static void OnResize(Vector2D<int> newSize)
    {
        _game.Resize(newSize);
    }

    private static void OnUpdate(double deltaTime)
    {
        Time.Update(deltaTime);
        Input.NewFrame();

        _game.Update();
    }

    private static void OnRender(double deltaTime)
    {
        _game.Render();
    }

    private static void OnClosing()
    {
        _game.Clear();
    }
}
