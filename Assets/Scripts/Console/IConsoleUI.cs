namespace IngameConsole
{
    public interface IConsoleUI
    {
        /// <summary>
        /// The current input text.
        /// Readonly.
        /// </summary>
        string Input { get; }        

        /// <summary>
        /// Returns the visible state of the console.
        /// Can be used to show or hide the console.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Sets console to visible if invisible and vice versa.
        /// </summary>
        void ToggleVisibility();

        /// <summary>
        /// Clears current text from input field.
        /// </summary>
        void ClearInput();

        /// <summary>
        /// Clears all output lines from the console window.
        /// </summary>
        void ClearOutput();

        /// <summary>
        /// Sets selection to input field.
        /// </summary>
        void SelectInput();

        /// <summary>
        /// Adds the given text to the output window.
        /// Use ConsoleIO instead if you want to write to the console.
        /// </summary>        
        void AppendToOutput(string text);                
    }
}