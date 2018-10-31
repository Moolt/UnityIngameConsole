using UnityEngine;

namespace IngameConsole
{
    public static class ConsoleIO
    {
        private static IConsoleUI _consoleUI;

        public static void InitializeIO(IConsoleUI consoleUI)
        {
            _consoleUI = consoleUI;
        }

        public static void WriteLine(string text)
        {
            Write("\n" + text);
        }

        public static void Write(string text)
        {
            _consoleUI.AppendToOutput(text);
        }

        public static void WriteLineItalic(string text)
        {
            WriteLine(string.Format("<i>{0}</i>", text));
        }

        public static void WriteItalic(string text)
        {
            Write(string.Format("<i>{0}</i>", text));
        }

        public static void WriteLineBold(string text)
        {
            WriteLine(string.Format("<b>{0}</b>", text));
        }

        public static void WriteBold(string text)
        {
            Write(string.Format("<b>{0}</b>", text));
        }

        public static void WriteError(string text)
        {
            OpenColor(Color.red);
            WriteLine(text);
            CloseColor();
        }

        public static void WriteInfo(string text)
        {
            OpenColor(new Color32(30, 98, 206, 255));
            WriteLine(text);
            CloseColor();
        }

        public static void OpenBold()
        {
            Write("<b>");
        }

        public static void CloseBold()
        {
            Write("</b>");
        }

        public static void OpenColor(Color color)
        {
            string hexCol = ColorUtility.ToHtmlStringRGB(color);
            Write(string.Format("<color=#{0}>", hexCol));
        }

        public static void CloseColor()
        {
            Write("</color>");
        }

        public static void NextLine()
        {
            Write("\n");
        }
    }
}