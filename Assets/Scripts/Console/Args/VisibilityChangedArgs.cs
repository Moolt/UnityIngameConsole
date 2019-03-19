using System;

namespace IngameConsole
{
    public class VisibilityChangedArgs : EventArgs
    {
        public VisibilityChangedArgs(bool newVisibility)
        {
            Visibility = newVisibility;
        }

        public bool Visibility { get; private set; }
    }
}
