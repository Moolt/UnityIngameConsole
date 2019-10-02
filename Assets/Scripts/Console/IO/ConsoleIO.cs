using System;
using UnityEngine;
using UnityEngine.UI;
using UnityInput = UnityEngine.Input;

namespace IngameConsole
{
    [RequireComponent(typeof(Animator))]
    public class ConsoleIO : BaseConsoleIO<ConsoleWriter>
    {
        [SerializeField]
        private InputField _input;
        [SerializeField]
        private Text _outputText;
        [SerializeField]
        private KeyCode _consoleToggleKey = KeyCode.Tab;
        [SerializeField]
        private int _inputHistoryCapacity = 10;

        [Header("Colors")]
        [SerializeField]
        private Color _errorColor;
        [SerializeField]
        private Color _infoColor;
        [SerializeField]
        private Color _warningColor;

        private ConsoleHistory _history;
        private Animator _animator;
        private bool _isVisible = false;

        public event EventHandler<VisibilityChangedArgs> VisibilityChanged;

        protected override void Awake()
        {
            base.Awake();
            _history = new ConsoleHistory(maxCapacity: _inputHistoryCapacity);
            _animator = GetComponent<Animator>();
        }

        private void Update()
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
            get { return _input.text; }
            set { _input.text = value; _input.caretPosition = _input.text.Length; }
        }

        public override bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    ApplyConsoleState();

                    if (VisibilityChanged != null)
                    {
                        VisibilityChanged.Invoke(this, new VisibilityChangedArgs(_isVisible));
                    }
                }
            }
        }

        public override void ToggleVisibility()
        {
            IsVisible = !IsVisible;
        }

        public override void ClearInput()
        {
            _input.text = string.Empty;
        }

        public override void ClearOutput()
        {
            _outputText.text = string.Empty;
        }

        public override void SelectInput()
        {
            _input.Select();
            _input.ActivateInputField();
        }

        public override void AppendToOutput(string text)
        {
            _outputText.text += text;
        }

        public KeyCode ToggleKey
        {
            get { return _consoleToggleKey; }
        }

        protected override ConsoleWriter CreateWriter()
        {
            var colors = new OutputColors(_errorColor, _infoColor, _warningColor);
            return new ConsoleWriter(this, colors);
        }

        private void ApplyConsoleState()
        {
            _animator.SetBool("Show", _isVisible);

            if (_isVisible)
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