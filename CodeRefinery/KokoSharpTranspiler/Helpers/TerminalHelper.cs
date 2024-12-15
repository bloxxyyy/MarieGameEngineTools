using System.Runtime.InteropServices;

namespace KokoSharpTranspiler.Helpers;
internal static class TerminalHelper
{
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern nint GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    static extern bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

    private static readonly int STD_OUTPUT_HANDLE = -11;
    private static readonly uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

    internal static string WriteUnderline(string text)
    {
        var handle = GetStdHandle(STD_OUTPUT_HANDLE);
        uint mode;
        GetConsoleMode(handle, out mode);
        mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
        SetConsoleMode(handle, mode);
        return $"\x1B[4m{text}\x1B[24m";
    }

    internal static void Log(string action, string logData, ConsoleColor color)
    {
        Console.Write(action + ": \"");
        Console.ForegroundColor = color;
        Console.Write(WriteUnderline(Path.GetFileName(logData)));
        Console.ResetColor();
        Console.WriteLine("\"");
    }

    internal static void Error(string action, string logData)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(action + ": \"");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(WriteUnderline(Path.GetFileName(logData)));
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\"");
        Console.ResetColor();
    }
}