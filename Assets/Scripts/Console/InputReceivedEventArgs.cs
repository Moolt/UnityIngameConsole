using System;

namespace IngameConsole
{
    public class InputReceivedEventArgs : EventArgs
    {
        private readonly string _input;

        public InputReceivedEventArgs(string input)
        {
            _input = input;
        }

        public string Input
        {
            get { return _input; }
        }
    }
}