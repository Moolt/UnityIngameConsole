using System.Linq;
using UnityEngine;

namespace IngameConsole
{
    public class FormattedWriter : BaseWriter
    {
        private static BaseWriter _writer;

        public static void Initialize()
        {
            var ios = Object.FindObjectsOfType<BaseConsoleIO>();
            _writer = ios.Select(io => io.Writer).FirstOrDefault();
        }

        public FormattedWriter() { }

        public override void WriteLine(string text) { _writer.WriteLine(text); }

        public override void Write(string text) { _writer.Write(text); }

        public override void WriteLineItalic(string text) { _writer.WriteLineItalic(text); }

        public override void WriteItalic(string text) { _writer.WriteItalic(text); }

        public override void WriteLineBold(string text) { _writer.WriteLineBold(text); }

        public override void WriteBold(string text) { _writer.WriteBold(text); }

        public override void WriteError(string text) { _writer.WriteError(text); }

        public override void WriteInfo(string text) { _writer.WriteInfo(text); }

        public override void WriteWarning(string text) { _writer.WriteWarning(text); }

        public override void OpenBold() { _writer.OpenBold(); }

        public override void CloseBold() { _writer.CloseBold(); }

        public override void OpenColor(Color color) { _writer.OpenColor(color); }

        public override void CloseColor() { _writer.CloseColor(); }

        public override void NextLine() { _writer.NextLine(); }
    }
}