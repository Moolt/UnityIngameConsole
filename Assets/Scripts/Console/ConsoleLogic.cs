using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System;

namespace IngameConsole
{
    [ExecutableFromConsole]
    [RequireComponent(typeof(BaseConsoleIO))]
    public class ConsoleLogic : MonoBehaviour
    {
        private BaseConsoleIO _consoleIO;
        private BaseWriter _consoleWriter;

        void Awake()
        {
            //Connect with IO
            _consoleIO = GetComponent(typeof(BaseConsoleIO)) as BaseConsoleIO;
            _consoleIO.InputReceived += OnInputReceived;
        }

        private void Start()
        {
            //Initialize writer
            FormattedWriter.Initialize();
            _consoleWriter = new FormattedWriter();

            //Init IO
            _consoleIO.SelectInput();
            ShowInitializationMessage();
        }

        #region Intialization message

        private void ShowInitializationMessage()
        {
            _consoleWriter.WriteInfo("Console has been initialized");
            _consoleWriter.WriteInfo("Write <b>help</b> for a list of all commands.");
            _consoleWriter.WriteInfo("Write <b>chelp command</b> to get further info on a specific command.");

            if (_consoleIO is ConsoleIO)
            {
                _consoleWriter.WriteInfo(string.Format("Press <b>{0}</b> to close console window.", (_consoleIO as ConsoleIO).ToggleKey.ToString()));
            }
        }

        #endregion

        #region Handle input

        private void OnInputReceived(object sender, InputReceivedEventArgs args)
        {
            _consoleWriter.WriteLineItalic("> " + args.Input);

            try
            {
                ExecuteLine(_consoleIO.Input);
            }
            catch (Exception e)
            {
                if (e.Message != string.Empty)
                {
                    _consoleWriter.WriteError(e.Message);
                }
            }
            finally
            {
                _consoleIO.ClearInput();
                _consoleIO.SelectInput();
            }
        }

        private IEnumerable<string> SplitParameters(string input)
        {
            //Implementation of the escape sequence \"
            //Any occurence of \" will be replaced by a substitution string
            //This string will be replaced with a quote after separation is complete
            string quoteSubstitution = ">##>";
            input = input.Replace(@"\""", quoteSubstitution);

            //There has to be an even number of quotes (opening and closing)
            if (input.Count(c => c == '"') % 2 != 0)
            {
                throw new Exception("Invalid string format.");
            }

            var splitsByQuote = input.Split('"');
            var parameters = new List<string>();

            for (int i = 0; i < splitsByQuote.Count(); i++)
            {
                //Splitting by quotes will result in list with a pattern of
                //alternating (empty string or string) and (quoted string)
                //Empty strings can be ignored
                if (i % 2 == 0)
                {
                    //Normal parameters
                    var splitsBySpace = splitsByQuote[i].Trim(' ').Split(' ').Where(s => s != string.Empty);
                    parameters.AddRange(splitsBySpace);
                }
                else
                {
                    //Quoted parameters
                    parameters.Add(splitsByQuote[i]);
                }
            }

            parameters = parameters.Select(s => s.Replace(quoteSubstitution, @"""")).ToList();
            return parameters;
        }

        #endregion

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
                _consoleWriter.WriteWarning(string.Format("More than one instance found for type <b>{0}</b>. Choosing <b>{1}</b> for execution.", type.ToString(), gameObjectName));
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
            string[] parameters = SplitParameters(line).ToArray();

            if (parameters.Count() == 0)
            {
                throw new Exception("Empty input.");
            }
            
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

                            //Custom conversion
                            if (TypeParser.HasConversionFor(parameterType))
                            {
                                converted = TypeParser.Convert(parameterType, parameterValue.Split(','));
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
                    }
                    else
                    {
                        throw new Exception("No executable instance found in the scene.");
                    }
                    return;
                }
                else
                {
                    _consoleWriter.WriteError((parameters.Length - 1).ToString() + " parameters given but " + methodParams.Length + " expected.");
                    _consoleWriter.WriteError("Usage: " + GetUsageInformation(command, targetMethod));
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
            _consoleWriter.WriteLine("");
            _consoleWriter.Write("<b>Available commands</b>: ");
            MethodInfo[] methods = CommandMethods;

            var commands = string.Join(", ", methods.Select(m => GetCommandName(m)).ToArray());
            _consoleWriter.Write(commands);
        }

        [ConsoleMethod("chelp", "Description of the given command.")]
        private void CHelp(string cmdName)
        {
            MethodInfo minfo = MethodByCmdName(cmdName);
            if (minfo != null)
            {
                _consoleWriter.WriteLine(GetCommandDescr(minfo));
                _consoleWriter.WriteLine("Usage: " + GetUsageInformation(cmdName, minfo));
            }
            else
            {
                _consoleWriter.WriteError("Invalid command <b>" + cmdName + "</b>.");
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
            _consoleIO.IsVisible = false;
        }

        [ConsoleMethod("clear", "Clears the console window from text.")]
        private void ClearConsole()
        {
            _consoleIO.ClearOutput();
        }

        #endregion
    }
}