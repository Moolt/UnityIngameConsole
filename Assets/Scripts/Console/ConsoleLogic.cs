using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System;

namespace IngameConsole
{
    [ExecutableFromConsole]
    [RequireComponent(typeof(ConsoleUI))]
    public class ConsoleLogic : MonoBehaviour
    {
        [SerializeField]
        private KeyCode consoleToggleKey = KeyCode.Tab;

        private IConsoleUI _consoleUI;
        private ConsoleHistory _history;

        void Awake()
        {
            _history = new ConsoleHistory(maxCapacity: 10);
            _consoleUI = GetComponent(typeof(IConsoleUI)) as IConsoleUI;
            ConsoleIO.InitializeIO(_consoleUI);
        }

        void Start()
        {
            _consoleUI.SelectInput();
            ConsoleIO.OpenColor(Color.red);
            ConsoleIO.WriteInfo("Console has been initialized");
            ConsoleIO.WriteInfo("Write <b>help</b> for a list of all commands.");
            ConsoleIO.WriteInfo("Write <b>chelp command</b> to get further info on a specific command.");
            ConsoleIO.WriteInfo(string.Format("Press <b>{0}</b> to close console window.", consoleToggleKey.ToString()));
            ConsoleIO.CloseColor();
        }

        void Update()
        {
            if (Input.GetKeyDown(consoleToggleKey))
            {
                _consoleUI.ToggleVisibility();
            }

            if (_consoleUI.IsVisible)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    ConsoleIO.WriteLineItalic("> " + _consoleUI.Input);

                    try
                    {
                        ExecuteLine(_consoleUI.Input);
                    }
                    catch (Exception e)
                    {
                        if (e.Message != string.Empty)
                        {
                            ConsoleIO.WriteError(e.Message);
                        }
                    }
                    finally
                    {
                        _consoleUI.ClearInput();
                        _consoleUI.SelectInput();
                    }
                }

                HandleHistory();
            }
        }

        private void HandleHistory()
        {
            var historyRequested = false;
            var command = string.Empty;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                command = _history.ShiftBack();
                historyRequested = true;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                command = _history.ShiftForward();
                historyRequested = true;
            }

            if (historyRequested && command != string.Empty)
            {
                _consoleUI.Input = command;
            }
        }

        #region Reflection

        private IEnumerable<Type> FindExecutableTypes()
        {
            var localAssembly = Assembly.GetAssembly(typeof(ConsoleLogic));
            var executableTypes = localAssembly.GetTypes().Where(_ => _.IsDefined(typeof(ExecutableFromConsole), false)).ToList();
            return executableTypes;
        }

        private bool TryFindExecutableInstanceOfType(Type type, out object instance)
        {
            var instances = FindObjectsOfType(type);
            instance = instances.FirstOrDefault();

            //Check for ambiguity
            if (instances.Count() > 1)
            {
                var gameObjectName = (instance as UnityEngine.Object).name;
                ConsoleIO.WriteWarning(string.Format("More than one instance found for type <b>{0}</b>. Choosing <b>{1}</b> for execution.", type.ToString(), gameObjectName));
            }

            return instance != null;
        }

        private void FindCmdMethod(string command, out MethodInfo minfo, out ConsoleMethod cmethod)
        {
            MethodInfo[] methods = CommandMethods;

            minfo = null;
            cmethod = null;

            foreach (MethodInfo method in methods)
            {
                ConsoleMethod[] attributes = method.GetCustomAttributes(typeof(ConsoleMethod), false) as ConsoleMethod[];
                if (attributes != null && attributes.Length > 0)
                {
                    if (attributes[0].Command == command)
                    {
                        minfo = method;
                        cmethod = attributes[0];
                    }
                }
            }
        }

        public void ExecuteLine(string line)
        {
            string[] parameters = line.Split(' ');
            string command = parameters[0];

            MethodInfo targetMethod;
            ConsoleMethod targetAttribute;

            FindCmdMethod(command, out targetMethod, out targetAttribute);

            if (targetMethod != null)
            {
                ParameterInfo[] methodParams = targetMethod.GetParameters();
                List<object> parameterValues = new List<object>();

                //Check if the amount of parameters is as expected
                if (methodParams.Length == parameters.Length - 1)
                {
                    //Convert all parameters to prepare for the invocation of the target method
                    for (int i = 0; i < methodParams.Length; i++)
                    {
                        try
                        {
                            var parameterType = methodParams[i].ParameterType;
                            var parameterValue = parameters[i + 1];
                            object converted = null;

                            //Try to find a GameObject with the given string name if the parameter is of type GameObject
                            if (parameterType == typeof(GameObject))
                            {
                                converted = GameObject.Find(parameterValue.ToString());

                                if (converted == null)
                                {
                                    ConsoleIO.WriteError(string.Format("GameObject with name <b>{0}</b> not found.", parameterValue.ToString()));
                                    return;
                                }
                            }
                            //Try to convert the string to the target type
                            else
                            {
                                converted = Convert.ChangeType(parameterValue, parameterType);
                            }

                            parameterValues.Add(converted);
                        }
                        catch
                        {
                            throw new Exception("Parameter conversion error.");
                        }
                    }

                    object target;
                    if (TryFindExecutableInstanceOfType(targetMethod.DeclaringType, out target))
                    {
                        targetMethod.Invoke(target, parameterValues.ToArray());
                        _history.WriteToHistory(line);
                    }
                    else
                    {
                        throw new Exception("No executable instance found in the scene.");
                    }
                    return;
                }
                else
                {
                    ConsoleIO.WriteError((parameters.Length - 1).ToString() + " parameters given but " + methodParams.Length + " expected.");
                    ConsoleIO.WriteError("Usage: " + GetUsageInformation(command, targetMethod));
                    throw new Exception(string.Empty);
                }
            }
            else
            {
                throw new Exception("Command <b>" + command + "</b> not found.");
            }
        }

        private MethodInfo MethodByCmdName(string cmd)
        {
            return CommandMethods.Where(cm => GetAttribute(cm).Command == cmd).FirstOrDefault();
        }

        private MethodInfo[] CommandMethods
        {
            get
            {
                var types = FindExecutableTypes();
                types = types.Where(t => FindObjectOfType(t) != null).ToList();

                var methods = types.SelectMany(t => t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)).ToList();
                MethodInfo[] filtered = methods.Where(m => m.GetCustomAttributes(typeof(ConsoleMethod), false).Length > 0).ToArray();
                return filtered;
            }
        }

        private ConsoleMethod GetAttribute(MethodInfo minfo)
        {
            ConsoleMethod[] attributes = minfo.GetCustomAttributes(typeof(ConsoleMethod), false) as ConsoleMethod[];
            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0];
            }
            return null;
        }

        private string GetCommandName(MethodInfo minfo)
        {
            ConsoleMethod attribute = GetAttribute(minfo);
            return attribute != null ? attribute.Command : string.Empty;
        }

        private string GetCommandDescr(MethodInfo minfo)
        {
            ConsoleMethod attribute = GetAttribute(minfo);
            return attribute != null ? attribute.Description : string.Empty;
        }

        private string GetUsageInformation(string cmdName, MethodInfo minfo)
        {
            var description = string.Format("<b>{0}</b>", cmdName); ;
            Action<string> AddParameterDescr = (string line) => { description += " " + line; };
            var parameters = minfo.GetParameters();

            foreach (ParameterInfo pinfo in parameters)
            {
                var parameterAttribute = pinfo.GetCustomAttributes(typeof(ConsoleParameter), false).FirstOrDefault() as ConsoleParameter;

                if (parameterAttribute != null)
                {
                    AddParameterDescr(string.Format("[{0}]", parameterAttribute.Description));
                }
                else
                {
                    AddParameterDescr(string.Format("[{0}]", pinfo.Name));
                }
            }

            return description;
        }

        #endregion

        #region Commands

        [ConsoleMethod("help", "Prints all available commands.")]
        private void HelpCmd()
        {
            ConsoleIO.WriteLine("");
            ConsoleIO.Write("<b>Available commands</b>: ");
            MethodInfo[] methods = CommandMethods;

            var commands = string.Join(", ", methods.Select(m => GetCommandName(m)).ToArray());
            ConsoleIO.Write(commands);
        }

        [ConsoleMethod("chelp", "Description of the given command.")]
        private void CHelp(string cmdName)
        {
            MethodInfo minfo = MethodByCmdName(cmdName);
            if (minfo != null)
            {
                ConsoleIO.WriteLine(GetCommandDescr(minfo));
                ConsoleIO.WriteLine("Usage: " + GetUsageInformation(cmdName, minfo));
            }
            else
            {
                ConsoleIO.WriteError("Invalid command <b>" + cmdName + "</b>.");
            }
        }

        [ConsoleMethod("quit", "Quits the game.")]
        private void QuitGame()
        {
            Application.Quit();
        }

        [ConsoleMethod("close", "Closes the developers console.")]
        private void CloseConsole()
        {
            _consoleUI.IsVisible = false;
        }

        [ConsoleMethod("clear", "Clears the console window from text.")]
        private void ClearConsole()
        {
            _consoleUI.ClearOutput();
        }

        #endregion
    }
}