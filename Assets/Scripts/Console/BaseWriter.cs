using UnityEngine;

namespace IngameConsole
{
    public abstract class BaseWriter
    {
        protected readonly BaseConsoleIO _consoleIO;

        public BaseWriter() { }

        public BaseWriter(BaseConsoleIO consoleIO)
        {
            _consoleIO = consoleIO;
        }

        public virtual void WriteLine(string text) { }

        public virtual void Write(string text) { }

        public virtual void WriteLineItalic(string text) { }

        public virtual void WriteItalic(string text) { }

        public virtual void WriteLineBold(string text) { }

        public virtual void WriteBold(string text) { }

        public virtual void WriteError(string text) { }

        public virtual void WriteInfo(string text) { }

        public virtual void WriteWarning(string text) { }

        public virtual void OpenBold() { }

        public virtual void CloseBold() { }

        public virtual void OpenColor(Color color) { }

        public virtual void CloseColor() { }

        public virtual void NextLine() { }
    }
}
