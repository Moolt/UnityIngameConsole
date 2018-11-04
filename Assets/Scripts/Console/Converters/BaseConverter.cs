using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace IngameConsole
{
    public abstract class BaseConverter<T> : IConverter<T>
    {
        public T AttemptConversion(params string[] rawParameters)
        {
            var writer = new FormattedWriter();

            var methods = GetType().MethodsWithAttribute<ConversionMethod>()
                .Where(m => m.GetParameters().Count() == rawParameters.Length);

            var targetMethod = methods.FirstOrDefault();

            if (targetMethod == null)
            {
                throw new Exception(string.Format("No conversion found for type \'{0}\' that takes {1} parameters.", typeof(T).Name, rawParameters.Count()));
            }

            var methodCount = methods.Count();
            if (methodCount > 1)
            {
                writer.WriteWarning(string.Format("Found {0} methods with {1} parameters. First one was chosen by default.", methodCount, rawParameters.Count()));
            }

            var parameterTypes = targetMethod.GetParameterTypes().ToList();
            IList<object> convertedParameters;

            if (!TryConvertParameters(rawParameters, parameterTypes, out convertedParameters))
            {
                throw new Exception("Conversion error.");
            }

            return (T)targetMethod.Invoke(this, convertedParameters.ToArray());
        }

        object IConverter.AttemptConversion(params string[] rawParameters)
        {
            return AttemptConversion(rawParameters);
        }

        private bool TryConvertParameters(IList<string> parameters, IList<Type> targetTypes, out IList<object> convertedParameters)
        {
            convertedParameters = new List<object>();

            if (parameters.Count() != targetTypes.Count()) return false;

            for (int i = 0; i < parameters.Count(); i++)
            {
                try
                {
                    var convertedParameter = Convert.ChangeType(parameters[i], targetTypes[i]);
                    convertedParameters.Add(convertedParameter);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
    }
}