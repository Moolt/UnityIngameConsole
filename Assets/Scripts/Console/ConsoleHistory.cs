using System.Collections.Generic;

namespace IngameConsole
{
    public class ConsoleHistory
    {
        private List<string> _history = new List<string>();
        private int _offset = 0;
        private int _maxCapacity = 10;

        public ConsoleHistory()
        {

        }

        public ConsoleHistory(int maxCapacity)
        {
            _maxCapacity = maxCapacity;
        }

        public void WriteToHistory(string command)
        {
            _history.Add(command);

            if(_history.Count > _maxCapacity)
            {
                _history.RemoveAt(0);
            }
        }

        public string ShiftBack()
        {
            var command = CurrentCommand;

            _offset++;
            CheckOffset();

            return command;
        }

        public string ShiftForward()
        {
            var command = CurrentCommand;

            _offset--;
            CheckOffset();

            return command;
        }

        public void Reset()
        {
            _offset = 0;
        }

        public void Clear()
        {
            _history.Clear();
        }

        private string CurrentCommand
        {
            get { return _history[CurrentIndex]; }
        }

        private int CurrentIndex
        {
            get { return _history.Count - 1 - _offset; }
        }

        private void CheckOffset()
        {
            if(_offset > _history.Count - 1)
            {
                _offset = 0;
            }

            if(_offset < 0)
            {
                _offset = _history.Count - 1;
            }
        }
    }
}