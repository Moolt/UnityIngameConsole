using System;

namespace IngameConsole
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleMethod : Attribute
    {
        private readonly string command;
        private readonly string descr;

        public ConsoleMethod(string command, string descr)
        {
            this.command = command;
            this.descr = descr;
        }

        public ConsoleMethod(string command) : this(command, "No description available.")
        {
        }

        public string Command
        {
            get { return command; }
        }

        public string Description
        {
            get { return descr; }
        }
    }
}