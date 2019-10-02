using System;
using UnityEngine;

namespace IngameConsole
{
    public abstract class BaseConsoleIO : MonoBehaviour
    {
        public delegate void InputReceivedEventHandler(object sender, InputReceivedEventArgs e);

        public event InputReceivedEventHandler InputReceived;

        /// <summary>
        /// Helper class to format input text
        /// Needs to be initialized in the Awake method
        /// </summary>
        public abstract BaseWriter Writer { get; }

        /// <summary>
        /// Informs the ConsoleLogic about input changes
        /// </summary>
        protected void RaiseInputReceived()
        {
            InputReceived(this, new InputReceivedEventArgs(Input));
        }

        /// <summary>
        /// The current input text.
        /// Readonly.
        /// </summary>
        public abstract string Input { get; set; }

        /// <summary>
        /// Adds the given text to the output window.
        /// Use ConsoleIO instead if you want to write to the console.
        /// </summary>

        public abstract void AppendToOutput(string text);

        /// <summary>
        /// Clears current text from input field.
        /// </summary>
        public virtual void ClearInput() { }

        /// <summary>
        /// Clears all output lines from the console window.
        /// </summary>
        public virtual void ClearOutput() { }

        /// <summary>
        /// Sets selection to input field.
        /// </summary>
        public virtual void SelectInput() { }

        /// <summary>
        /// Returns the visible state of the console.
        /// Can be used to show or hide the console.
        /// If your implementation does not depend on any UI, 
        /// true should be returned by default
        /// </summary>
        public virtual bool IsVisible { get { return true; } set { } }

        /// <summary>
        /// Sets console to visible if invisible and vice versa.
        /// </summary>
        public virtual void ToggleVisibility() { }
    }

    public abstract class BaseConsoleIO<T> : BaseConsoleIO where T : BaseWriter
    {
        private T _writer;

        protected virtual void Awake()
        {
            _writer = CreateWriter();
        }

        protected virtual T CreateWriter()
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { this });
        }

        public override BaseWriter Writer
        {
            get
            {
                return _writer;
            }
        }
    }
}