using System.Numerics;
using LearnSilkNET.Inputs;
using LearnSilkNET.Utilities;

namespace LearnSilkNET;

// Uma classe de câmera abstrata que processa a entrada e calcula os ângulos de Euler, vetores e matrizes correspondentes para uso no OpenGL
public class Camera
{
    // Valores padrão da câmera
    private const float YAW         = -90.0f;
    private const float PITCH       = 0.0f;
    private const float SPEED       = 2.5f;
    private const float SENSITIVITY = 0.1f;
    private const float ZOOM        = 45.0f;

    // Atributos da câmera
    public Vector3 Position;
    public Vector3 Front;
    public Vector3 Up;

    // Ângulos de Euler
    public float Yaw;
    public float Pitch;

    // opções de câmera
    public float MovementSpeed;
    public float MouseSensitivity;
    public float Zoom;

    private bool _firstMouse = true;
    private Vector2 _lastPos;

    // constructor with vectors
    public Camera(Vector3? position = null, Vector3? up = null, float yaw = YAW, float pitch = PITCH)
    {
        Position = position ?? new Vector3(0.0f, 0.0f, 0.0f);
        Up = up ?? new Vector3(0.0f, 1.0f, 0.0f);
        Yaw = yaw;
        Pitch = pitch;

        Front = new Vector3(0.0f, 0.0f, -1.0f);
        MovementSpeed = SPEED;
        MouseSensitivity = SENSITIVITY;
        Zoom = ZOOM;

        UpdateCameraVectors();
    }

    // construtor com valores escalares
    public Camera(float posX, float posY, float posZ, float upX, float upY, float upZ, float yaw, float pitch)
    {
        Position = new Vector3(posX, posY, posZ);
        Up = new Vector3(upX, upY, upZ);
        Yaw = yaw;
        Pitch = pitch;

        Front = new Vector3(0.0f, 0.0f, -1.0f);
        MovementSpeed = SPEED;
        MouseSensitivity = SENSITIVITY;
        Zoom = ZOOM;

        UpdateCameraVectors();
    }

    public void Update()
    {
        ProcessKeyboard();
        ProcessMouseMovement();
        ProcessMouseScroll();
    }

    // retorna a matriz de visualização calculada usando ângulos de Euler e a matriz LookAt
    public Matrix4x4 GetViewMatrix()
    {
        return Matrix4x4.CreateLookAt(
            cameraPosition: Position, 
            cameraTarget:   Position + Front, 
            cameraUpVector: Up
        );
    }

    // processa a entrada recebida de qualquer sistema de entrada do tipo teclado. Aceita um parâmetro de entrada na forma de um ENUM definido pela câmera (para abstraí-lo de sistemas de janelas)
    private void ProcessKeyboard()
    {
        float velocity = MovementSpeed * Time.DeltaTime;

        Vector3 front = Front;
        Vector3 right = Vector3.Normalize(Vector3.Cross(Front, Up));
        Vector3 up = Up;

        if (Input.GetKey(KeyCode.W))
        {
            Position += velocity * front;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Position -= velocity * front;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Position -= velocity * right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Position += velocity * right;
        }

        // garanta que o usuário permaneça no nível do solo
        Position.Y = 0.0f; // <-- esta linha única mantém o usuário no nível do solo (plano xz)
    }

    // processa a entrada recebida de um sistema de entrada de mouse. Espera o valor de deslocamento nas direções x e y.
    private void ProcessMouseMovement(bool constrainPitch = true)
    {
        if (_firstMouse)
        {
            _lastPos = Input.MousePositon;
            _firstMouse = false;
        }

        float xoffset = Input.MousePositon.X - _lastPos.X;
        float yoffset = _lastPos.Y - Input.MousePositon.Y; // invertido, já que as coordenadas y vão de baixo para cima
        _lastPos = Input.MousePositon;

        xoffset *= MouseSensitivity;
        yoffset *= MouseSensitivity;

        Yaw += xoffset;
        Pitch += yoffset;

        // certifique-se de que a tela não seja invertida quando o pitch estiver fora dos limites
        if (constrainPitch)
        {
            if (Pitch > 89.0f)
            {
                Pitch = 89.0f;
            }
            if (Pitch < -89.0f)
            {
                Pitch = -89.0f;
            }
        }

        // atualiza os vetores Front, Right e Up usando os ângulos de Euler atualizados
        UpdateCameraVectors();
    }

    // processa a entrada recebida de um evento de roda de rolagem do mouse. Requer entrada apenas no eixo vertical da roda.
    private void ProcessMouseScroll()
    {
        Zoom -= Input.MouseScrollDelta.Y;

        if (Zoom < 1.0f)
        {
            Zoom = 1.0f;
        }
        if (Zoom > 45.0f)
        {
            Zoom = 45.0f;
        }
    }

    // calcula o vetor frontal a partir dos ângulos de Euler (atualizados) da câmera
    private void UpdateCameraVectors()
    {
        // calcula o novo vetor Front
        Vector3 front;
        front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
        front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
        front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));

        Front = Vector3.Normalize(front);
    }
}
