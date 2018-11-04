using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace IngameConsole
{
    public static class ExtensionMethods
    {
        public static IEnumerable<MethodInfo> MethodsWithAttribute<T>(this Type type)
        {
            return type.GetMethods().Where(m => m.GetCustomAttributes(typeof(ConversionMethod), false).Any()).ToList();
        }

        public static IEnumerable<Type> GetParameterTypes(this MethodInfo minfo)
        {
            return minfo.GetParameters().Select(p => p.ParameterType).ToList();
        }

        public static IDictionary<string, T> GetStaticPropertiesOfType<T>(this Type type)
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static)
                    .Where(t => t.PropertyType == typeof(T))
                    .ToDictionary(p => p.Name.ToLower(), p => (T)p.GetValue(null, null));
        }
    }
}