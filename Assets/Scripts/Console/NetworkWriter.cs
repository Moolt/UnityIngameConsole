using UnityEngine;

namespace IngameConsole
{
    public class NetworkWriter : BaseConsoleWriter
    {
        public override void CloseBold()
        {
            Write("'");
        }

        public override void NextLine()
        {
            Write("\n");
        }

        public override void OpenBold()
        {
            Write("'");
        }

        public override void Write(string text)
        {
            text = text.Replace("<b>", "'").Replace("</b>", "'");
            _consoleIO.AppendToOutput(text);
        }

        public override void WriteBold(string text)
        {
            Write(string.Format("'{0}'", text));
        }

        public override void WriteError(string text)
        {
            WriteLine(text);
        }

        public override void WriteInfo(string text)
        {
            WriteLine(text);
        }

        public override void WriteItalic(string text)
        {
            Write(string.Format("'{0}'", text));
        }

        public override void WriteLineBold(string text)
        {
            WriteLine(text);
        }

        public override void WriteLineItalic(string text)
        {
            WriteLine(text);
        }

        public override void WriteWarning(string text)
        {
            WriteLine(text);
        }

        public override void WriteLine(string text)
        {
            Write(text + "\n");
        }
    }
}