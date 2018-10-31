using UnityEngine;
using UnityEngine.UI;

namespace IngameConsole
{
    [RequireComponent(typeof(Animator))]
    public class ConsoleUI : MonoBehaviour, IConsoleUI
    {
        [SerializeField]
        private InputField input;
        [SerializeField]
        private Text outputText;

        private Animator animator;
        private bool show = false;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public string Input
        {
            get { return input.text; }
        }

        public bool IsVisible
        {
            get { return show; }
            set
            {
                show = value;
                ApplyConsoleState();
            }
        }

        public void ToggleVisibility()
        {
            show = !show;
            ApplyConsoleState();
        }

        public void ClearInput()
        {
            input.text = "";
        }

        public void ClearOutput()
        {
            outputText.text = "";
        }

        public void SelectInput()
        {
            input.Select();
            input.ActivateInputField();
        }

        public void AppendToOutput(string text)
        {
            outputText.text += text;
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
    }
}