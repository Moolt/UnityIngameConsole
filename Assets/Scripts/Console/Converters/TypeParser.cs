using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace IngameConsole
{
    public static class TypeParser
    {
        private static readonly Dictionary<Type, IConverter> _converters = new Dictionary<Type, IConverter>();

        static TypeParser()
        {
            var assembly = Assembly.GetAssembly(typeof(BaseConverter<>));
            var types = assembly.GetTypes();

            types.Where(t => t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(BaseConverter<>))
                .ToList()
                .ForEach(t => _converters.Add(t.BaseType.GetGenericArguments()[0], (IConverter)Activator.CreateInstance(t)));            
        }

        public static bool HasConversionFor(Type type)
        {
            return _converters.ContainsKey(type);
        }

        public static bool HasConversionFor<T>()
        {
            return HasConversionFor(typeof(T));
        }

        public static object Convert(Type type, params string[] parameters)
        {
            if (!HasConversionFor(type)) return null;
            return _converters[type].AttemptConversion(parameters);
        }

        public static object Convert<T>(params string[] parameters)
        {
            return Convert(typeof(T), parameters);
        }
    }
}