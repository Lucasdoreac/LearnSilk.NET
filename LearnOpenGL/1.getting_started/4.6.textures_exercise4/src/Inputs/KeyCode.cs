// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace LearnSilkNET.Inputs;

/// <summary>
/// Códigos de tecla retornados por Event.keyCode. Eles correspondem diretamente a uma tecla física do teclado.
/// </summary>
public enum KeyCode
{
    /// <summary>
    /// Uma tecla Desconhecida.
    /// </summary>
    Unknown = -1,

    /// <summary>
    /// A tecla Espaço.
    /// </summary>
    Space = 32,

    /// <summary>
    /// A tecla Apóstrofo.
    /// </summary>
    Apostrophe = 39 /* ' */,

    /// <summary>
    /// A tecla Vírgula ','.
    /// </summary>
    Comma = 44 /* , */,

    /// <summary>
    /// A tecla Menos '-'.
    /// </summary>
    Minus = 45 /* - */,

    /// <summary>
    /// A tecla Ponto '.'.
    /// </summary>
    Period = 46 /* . */,

    /// <summary>
    /// A tecla Barra '/'.
    /// </summary>
    Slash = 47 /* / */,

    /// <summary>
    /// A tecla 0.
    /// </summary>
    Number0 = 48,

    /// <summary>
    /// A tecla 0; alias para <see cref="Number0"/>
    /// </summary>
    D0 = Number0,

    /// <summary>
    /// A tecla 1.
    /// </summary>
    Number1 = 49,

    /// <summary>
    /// A tecla 2.
    /// </summary>
    Number2 = 50,

    /// <summary>
    /// A tecla 3.
    /// </summary>
    Number3 = 51,

    /// <summary>
    /// A tecla 4.
    /// </summary>
    Number4 = 52,

    /// <summary>
    /// A tecla 5.
    /// </summary>
    Number5 = 53,

    /// <summary>
    /// A tecla 6.
    /// </summary>
    Number6 = 54,

    /// <summary>
    /// A tecla 7.
    /// </summary>
    Number7 = 55,

    /// <summary>
    /// A tecla 8.
    /// </summary>
    Number8 = 56,

    /// <summary>
    /// A tecla 9.
    /// </summary>
    Number9 = 57,

    /// <summary>
    /// A tecla Ponto e Vírgula ';'.
    /// </summary>
    Semicolon = 59 /* ; */,

    /// <summary>
    /// A tecla Igual '='.
    /// </summary>
    Equal = 61 /* = */,

    /// <summary>
    /// A tecla A.
    /// </summary>
    A = 65,

    /// <summary>
    /// A tecla B.
    /// </summary>
    B = 66,

    /// <summary>
    /// A tecla C.
    /// </summary>
    C = 67,

    /// <summary>
    /// A tecla D.
    /// </summary>
    D = 68,

    /// <summary>
    /// A tecla E.
    /// </summary>
    E = 69,

    /// <summary>
    /// A tecla F.
    /// </summary>
    F = 70,

    /// <summary>
    /// A tecla G.
    /// </summary>
    G = 71,

    /// <summary>
    /// A tecla H.
    /// </summary>
    H = 72,

    /// <summary>
    /// A tecla I.
    /// </summary>
    I = 73,

    /// <summary>
    /// A tecla J.
    /// </summary>
    J = 74,

    /// <summary>
    /// A tecla K.
    /// </summary>
    K = 75,

    /// <summary>
    /// A tecla L.
    /// </summary>
    L = 76,

    /// <summary>
    /// A tecla M.
    /// </summary>
    M = 77,

    /// <summary>
    /// A tecla N.
    /// </summary>
    N = 78,

    /// <summary>
    /// A tecla O.
    /// </summary>
    O = 79,

    /// <summary>
    /// A tecla P.
    /// </summary>
    P = 80,

    /// <summary>
    /// A tecla Q.
    /// </summary>
    Q = 81,

    /// <summary>
    /// A tecla R.
    /// </summary>
    R = 82,

    /// <summary>
    /// A tecla S.
    /// </summary>
    S = 83,

    /// <summary>
    /// A tecla T.
    /// </summary>
    T = 84,

    /// <summary>
    /// A tecla U.
    /// </summary>
    U = 85,

    /// <summary>
    /// A tecla V.
    /// </summary>
    V = 86,

    /// <summary>
    /// A tecla W.
    /// </summary>
    W = 87,

    /// <summary>
    /// A tecla X.
    /// </summary>
    X = 88,

    /// <summary>
    /// A tecla Y.
    /// </summary>
    Y = 89,

    /// <summary>
    /// A tecla Z.
    /// </summary>
    Z = 90,

    /// <summary>
    /// A tecla Colchete Esquerdo (Colchete de Abertura) '['.
    /// </summary>
    LeftBracket = 91 /* [ */,

    /// <summary>
    /// A tecla Barra Invertida '\'.
    /// </summary>
    BackSlash = 92 /* \ */,

    /// <summary>
    /// A tecla Colchete Direito (Colchete de Fechamento) ']'.
    /// </summary>
    RightBracket = 93 /* ] */,

    /// <summary>
    /// A tecla Acento Grave '`'.
    /// </summary>
    GraveAccent = 96 /* ` */,

    /// <summary>
    /// Tecla 1 de layout de teclado não americano.
    /// </summary>
    World1 = 161 /* non-US #1 */,

    /// <summary>
    /// Tecla 2 de layout de teclado não americano.
    /// </summary>
    World2 = 162 /* non-US #2 */,

    /// <summary>
    /// A tecla Escape.
    /// </summary>
    Escape = 256,

    /// <summary>
    /// A tecla Enter.
    /// </summary>
    Enter = 257,

    /// <summary>
    /// A tecla Tab.
    /// </summary>
    Tab = 258,

    /// <summary>
    /// A tecla Backspace.
    /// </summary>
    Backspace = 259,

    /// <summary>
    /// A tecla Insert.
    /// </summary>
    Insert = 260,

    /// <summary>
    /// A tecla Delete.
    /// </summary>
    Delete = 261,

    /// <summary>
    /// A tecla Seta para a Direita.
    /// </summary>
    RightArrow = 262,

    /// <summary>
    /// A tecla Seta para a Esquerda.
    /// </summary>
    LeftArrow = 263,

    /// <summary>
    /// A tecla Seta para Baixo.
    /// </summary>
    DownArrow = 264,

    /// <summary>
    /// A tecla Seta para Cima.
    /// </summary>
    UpArrow = 265,

    /// <summary>
    /// A tecla Page Up.
    /// </summary>
    PageUp = 266,

    /// <summary>
    /// A tecla Page Down.
    /// </summary>
    PageDown = 267,

    /// <summary>
    /// A tecla Home.
    /// </summary>
    Home = 268,

    /// <summary>
    /// A tecla End.
    /// </summary>
    End = 269,

    /// <summary>
    /// A tecla Caps Lock.
    /// </summary>
    CapsLock = 280,

    /// <summary>
    /// A tecla Scroll Lock.
    /// </summary>
    ScrollLock = 281,

    /// <summary>
    /// A tecla Num Lock.
    /// </summary>
    NumLock = 282,

    /// <summary>
    /// A tecla Print Screen.
    /// </summary>
    PrintScreen = 283,

    /// <summary>
    /// A tecla Pausa.
    /// </summary>
    Pause = 284,

    /// <summary>
    /// A tecla F1.
    /// </summary>
    F1 = 290,

    /// <summary>
    /// A tecla F2.
    /// </summary>
    F2 = 291,

    /// <summary>
    /// A tecla F3.
    /// </summary>
    F3 = 292,

    /// <summary>
    /// A tecla F4.
    /// </summary>
    F4 = 293,

    /// <summary>
    /// A tecla F5.
    /// </summary>
    F5 = 294,

    /// <summary>
    /// A tecla F6.
    /// </summary>
    F6 = 295,

    /// <summary>
    /// A tecla F7.
    /// </summary>
    F7 = 296,

    /// <summary>
    /// A tecla F8.
    /// </summary>
    F8 = 297,

    /// <summary>
    /// A tecla F9.
    /// </summary>
    F9 = 298,

    /// <summary>
    /// A tecla F10.
    /// </summary>
    F10 = 299,

    /// <summary>
    /// A tecla F11.
    /// </summary>
    F11 = 300,

    /// <summary>
    /// A tecla F12.
    /// </summary>
    F12 = 301,

    /// <summary>
    /// A tecla F13.
    /// </summary>
    F13 = 302,

    /// <summary>
    /// A tecla F14.
    /// </summary>
    F14 = 303,

    /// <summary>
    /// A tecla F15.
    /// </summary>
    F15 = 304,

    /// <summary>
    /// A tecla F16.
    /// </summary>
    F16 = 305,

    /// <summary>
    /// A tecla F17.
    /// </summary>
    F17 = 306,

    /// <summary>
    /// A tecla F18.
    /// </summary>
    F18 = 307,

    /// <summary>
    /// A tecla F19.
    /// </summary>
    F19 = 308,

    /// <summary>
    /// A tecla F20.
    /// </summary>
    F20 = 309,

    /// <summary>
    /// A tecla F21.
    /// </summary>
    F21 = 310,

    /// <summary>
    /// A tecla F22.
    /// </summary>
    F22 = 311,

    /// <summary>
    /// A tecla F23.
    /// </summary>
    F23 = 312,

    /// <summary>
    /// A tecla F24.
    /// </summary>
    F24 = 313,

    /// <summary>
    /// A tecla F25.
    /// </summary>
    F25 = 314,

    /// <summary>
    /// A tecla 0 no teclado numérico.
    /// </summary>
    Keypad0 = 320,

    /// <summary>
    /// A tecla 1 no teclado numérico.
    /// </summary>
    Keypad1 = 321,

    /// <summary>
    /// A tecla 2 no teclado numérico.
    /// </summary>
    Keypad2 = 322,

    /// <summary>
    /// A tecla 3 no teclado numérico.
    /// </summary>
    Keypad3 = 323,

    /// <summary>
    /// A tecla 4 no teclado numérico.
    /// </summary>
    Keypad4 = 324,

    /// <summary>
    /// A tecla 5 no teclado numérico.
    /// </summary>
    Keypad5 = 325,

    /// <summary>
    /// A tecla 6 no teclado numérico.
    /// </summary>
    Keypad6 = 326,

    /// <summary>
    /// A tecla 7 no teclado numérico.
    /// </summary>
    Keypad7 = 327,

    /// <summary>
    /// A tecla 8 no teclado numérico.
    /// </summary>
    Keypad8 = 328,

    /// <summary>
    /// A tecla 9 no teclado numérico.
    /// </summary>
    Keypad9 = 329,

    /// <summary>
    /// A tecla Ponto Decimal no teclado numérico '.'.
    /// </summary>
    KeypadDecimal = 330, /* . */

    /// <summary>
    /// A tecla Divisão no teclado numérico '/'.
    /// </summary>
    KeypadDivide = 331, /* / */

    /// <summary>
    /// A tecla Multiplicação no teclado numérico '*'.
    /// </summary>
    KeypadMultiply = 332, /* * */

    /// <summary>
    /// A tecla Subtração no teclado numérico '-'.
    /// </summary>
    KeypadSubtract = 333, /* - */

    /// <summary>
    /// A tecla Adição no teclado numérico '+'.
    /// </summary>
    KeypadAdd = 334, /* + */

    /// <summary>
    /// A tecla Enter do teclado numérico.
    /// </summary>
    KeypadEnter = 335,

    /// <summary>
    /// A tecla Igual no teclado numérico.
    /// </summary>
    KeypadEqual = 336,

    /// <summary>
    /// A tecla Shift esquerda.
    /// </summary>
    ShiftLeft = 340,

    /// <summary>
    /// A tecla Control esquerda.
    /// </summary>
    ControlLeft = 341,

    /// <summary>
    /// A tecla Alt esquerda.
    /// </summary>
    AltLeft = 342,

    /// <summary>
    /// A tecla Super esquerda.
    /// </summary>
    SuperLeft = 343,

    /// <summary>
    /// A tecla Shift direita.
    /// </summary>
    ShiftRight = 344,

    /// <summary>
    /// A tecla Control direita.
    /// </summary>
    ControlRight = 345,

    /// <summary>
    /// A tecla Alt direita.
    /// </summary>
    AltRight = 346,

    /// <summary>
    /// A tecla Super direita.
    /// </summary>
    SuperRight = 347,

    /// <summary>
    /// A tecla de Menu.
    /// </summary>
    Menu = 348
}
