using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System;

[AttributeUsage(AttributeTargets.Method)]
public class ConsoleMethod : Attribute {
    private string command;
    private string descr;

    public ConsoleMethod(string command, string descr) {
        this.command = command;
        this.descr = descr;
    }

    public string Command {
        get { return command; }
    }

    public string Description {
        get { return descr; }
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ExecutableFromConsole : Attribute
{
}

[ExecutableFromConsole]
public class ConsoleLogic : MonoBehaviour {

    public InputField input;
    public Text outputText;

    private bool show = false;
    private Animator animator;
        
   void Awake() {
        animator = GetComponent<Animator>();
   }

    // Use this for initialization
    void Start () {
        SelectInput();
        OpenColor(Color.red);
        WriteInfo("Console has been initialized");
        WriteInfo("Write <b>help</b> for a list of all commands.");
        WriteInfo("Write <b>chelp command</b> to get further info on a specific command.");
        WriteInfo("Press <b>Tab</b> to close console window.");
        CloseColor();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Return)) {
            WriteItalic("> " + input.text);
            ExecuteLine(input.text);
            ClearInput();
            SelectInput();
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            show = !show;
            ApplyConsoleState();            
        }
	}

#region IO
    private void ApplyConsoleState() {
        animator.SetBool("Show", show);
        
        if (show) {
            ClearInput();
            SelectInput();
        }
        //GUI.GetNameOfFocusedControl();
    }

    private bool IsInputFieldSelected {
        get {
            return true;
        }
    }

    private void SelectInput() {        
        input.Select();
        input.ActivateInputField();        
    }

    private void ClearInput() {
        input.text = "";
    }

    private void WriteLine(string text) {
        Write("\n" + text);
    }

    private void Write(string text) {
        outputText.text += text;
    }

    private void WriteItalic(string text) {
        outputText.text += "<i>";
        WriteLine(text);
        outputText.text += "</i>";
    }

    private void WriteError(string text) {
        OpenColor(Color.red);
        WriteLine(text);
        CloseColor();
    }

    private void WriteInfo(string text) {
        OpenColor(new Color32(30, 98, 206, 255));
        WriteLine(text);
        CloseColor();
    }

    private void OpenBold() {
        outputText.text += "<b>";
    }

    private void CloseBold() {
        outputText.text += "</b>";
    }

    private void OpenColor(Color color) {
        string hexCol = ColorUtility.ToHtmlStringRGB(color);
        outputText.text += "<color=#" + hexCol + ">";
    }

    private void CloseColor() {
        outputText.text += "</color>";
    }

    private void NextLine() {
        outputText.text += "\n";
    }
    #endregion

#region Reflection

    private IEnumerable<Type> FindExecutableTypes() {
        var localAssembly = Assembly.GetAssembly(typeof(ConsoleLogic));
        var executableTypes = localAssembly.GetTypes().Where(_ => _.IsDefined(typeof(ExecutableFromConsole), false)).ToList();
        return executableTypes;
    }

    private bool TryFindExecutableInstanceOfType(Type type, out object instance)
    {
        instance = FindObjectOfType(type);
        return instance != null;
    }

    private void FindCmdMethod(string command, out MethodInfo minfo, out ConsoleMethod cmethod) {
        MethodInfo[] methods = CommandMethods;

        minfo = null;
        cmethod = null;

        foreach (MethodInfo method in methods) {
            ConsoleMethod[] attributes = method.GetCustomAttributes(typeof(ConsoleMethod), false) as ConsoleMethod[];
            if (attributes != null && attributes.Length > 0) {
                if (attributes[0].Command == command) {
                    minfo = method;
                    cmethod = attributes[0];
                }
            }
        }
    }

    public void ExecuteLine(string line) {
        
        string[] parameters = line.Split(' ');
        string command = parameters[0];        

        MethodInfo targetMethod;
        ConsoleMethod targetAttribute;

        FindCmdMethod(command, out targetMethod, out targetAttribute);

        if(targetMethod != null) {
            ParameterInfo[] methodParams = targetMethod.GetParameters();
            List<object> parameterValues = new List<object>();

            if(methodParams.Length == parameters.Length - 1) {
                for(int i = 0; i < methodParams.Length; i++) {
                    try {
                        object converted = Convert.ChangeType(parameters[i+1], methodParams[i].ParameterType);
                        parameterValues.Add(converted);
                    } catch {
                        WriteError("Parameter conversion error.");
                    }
                }

                object target;
                if(TryFindExecutableInstanceOfType(targetMethod.DeclaringType, out target))
                {
                    targetMethod.Invoke(target, parameterValues.ToArray());
                }
                else
                {
                    WriteError("No executable instance found in the scene.");
                }
            } else {
                WriteError((parameters.Length-1).ToString() + " parameters given but " + methodParams.Length + " expected.");
            }

        } else {
            WriteError("Command <b>" + command + "</b> not found.");
        }
    }

    private MethodInfo MethodByCmdName(string cmd) {
        return CommandMethods.Where(cm => GetAttribute(cm).Command == cmd).FirstOrDefault();
    }

    private MethodInfo[] CommandMethods {
        get {
            List<MethodInfo> methodInfos = new List<MethodInfo>();
            var types = FindExecutableTypes();
            types = types.Where(t => FindObjectOfType(t) != null).ToList();

            var methods = types.SelectMany(t => t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)).ToList();           
            MethodInfo[] filtered = methods.Where(m => m.GetCustomAttributes(typeof(ConsoleMethod), false).Length > 0).ToArray();
            return filtered;
        }
    }

    private ConsoleMethod GetAttribute(MethodInfo minfo) {
        ConsoleMethod[] attributes = minfo.GetCustomAttributes(typeof(ConsoleMethod), false) as ConsoleMethod[];
        if (attributes != null && attributes.Length > 0) {
            return attributes[0];
        }
        return null;
    }

    private string GetCommandName(MethodInfo minfo) {
        ConsoleMethod attribute = GetAttribute(minfo);
        return attribute != null ? attribute.Command : "";
    }

    private string GetCommandDescr(MethodInfo minfo) {
        ConsoleMethod attribute = GetAttribute(minfo);
        return attribute != null ? attribute.Description : "";
    }

    #endregion

#region Commands

    [ConsoleMethod("help", "Prints all available commands.")]
    private void HelpCmd() {
        WriteLine("");
        Write("<b>Available commands</b>: ");
        MethodInfo[] methods = CommandMethods;

        var commands = string.Join(", ", methods.Select(m => GetCommandName(m)).ToArray());
        Write(commands);
    }

    [ConsoleMethod("chelp", "Description of the given command.")]
    private void CHelp(string cmdName) {
        MethodInfo minfo = MethodByCmdName(cmdName);
        if(minfo != null) {
            WriteLine(GetCommandDescr(minfo));
        } else {
            WriteError("Invalid command <b>" + cmdName + "</b>.");
        }
    }

    [ConsoleMethod("quit", "Quits the game.")]
    private void QuitGame() {
        Application.Quit();
    }

    [ConsoleMethod("close", "Closes the developers console.")]
    private void CloseConsole() {
        show = false;
        ApplyConsoleState();        
    }

    #endregion
}