using UnityEngine;
using UnityEngine.UI;
using UnityInput = UnityEngine.Input;

namespace IngameConsole
{
    [RequireComponent(typeof(Animator))]
    public class ConsoleIO : BaseConsoleIO
    {
        [SerializeField]
        private InputField input;
        [SerializeField]
        private Text outputText;
        [SerializeField]
        private KeyCode _consoleToggleKey = KeyCode.Tab;
        [SerializeField]
        private int _inputHistoryCapacity = 10;

        private ConsoleHistory _history;
        private Animator animator;
        private bool show = false;

        void Awake()
        {
            _history = new ConsoleHistory(maxCapacity: _inputHistoryCapacity);
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            //Open or close console
            if (UnityInput.GetKeyDown(_consoleToggleKey))
            {
                ToggleVisibility();
            }

            if (IsVisible)
            {
                //Command has been entered
                if (UnityInput.GetKeyDown(KeyCode.Return))
                {
                    _history.WriteToHistory(Input);
                    RaiseInputReceived();
                }

                HandleHistory();
            }
        }

        public override string Input
        {
            get { return input.text; }
            set { input.text = value; input.caretPosition = input.text.Length; }
        }

        public override bool IsVisible
        {
            get { return show; }
            set
            {
                show = value;
                ApplyConsoleState();
            }
        }

        public override void ToggleVisibility()
        {
            show = !show;
            ApplyConsoleState();
        }

        public override void ClearInput()
        {
            input.text = string.Empty;
        }

        public override void ClearOutput()
        {
            outputText.text = string.Empty;
        }

        public override void SelectInput()
        {
            input.Select();
            input.ActivateInputField();
        }

        public override void AppendToOutput(string text)
        {
            outputText.text += text;
        }

        public KeyCode ToggleKey
        {
            get { return _consoleToggleKey; }
        }

        private void ApplyConsoleState()
        {
            animator.SetBool("Show", show);

            if (show)
            {
                ClearInput();
                SelectInput();
            }
        }

        private void HandleHistory()
        {
            var historyRequested = false;
            var command = string.Empty;

            if (UnityInput.GetKeyDown(KeyCode.UpArrow))
            {
                command = _history.ShiftBack();
                historyRequested = true;
            }

            if (UnityInput.GetKeyDown(KeyCode.DownArrow))
            {
                command = _history.ShiftForward();
                historyRequested = true;
            }

            if (historyRequested && command != string.Empty)
            {
                Input = command;
            }
        }
    }
}