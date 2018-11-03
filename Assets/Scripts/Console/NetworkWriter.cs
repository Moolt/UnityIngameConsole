namespace IngameConsole
{
    public class NetworkWriter : BaseWriter
    {
        private const string baseIdentifier = "#>>";
        private const string errorIdentifier = "x";
        private const string warningIdentifier = "!";

        public NetworkWriter(BaseConsoleIO consoleIO) : base(consoleIO) { }

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
            WriteFormattedLine(text, errorIdentifier);
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
            WriteFormattedLine(text, warningIdentifier);
        }

        public override void WriteLine(string text)
        {
            Write(text + "\n");
        }

        private void WriteFormattedLine(string text, string type)
        {
            WriteLine(string.Format("{0}{1}{2}", baseIdentifier, type, text));
        }
    }
}