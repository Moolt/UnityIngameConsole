using System;

namespace IngameConsole
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ConsoleParameter : Attribute
    {
        private readonly string descr;

        public ConsoleParameter(string descr)
        {
            this.descr = descr;
        }

        public string Description
        {
            get { return descr; }
        }
    }
}