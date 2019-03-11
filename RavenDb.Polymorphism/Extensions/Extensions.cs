using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace

namespace RavenDb.Polymorphism.Model
{
    public static class Extensions
    {
        public static string TypeStoreName(this Type type)
        {
            return typeof (Denizen).IsAssignableFrom(type)
                ? typeof (Denizen).Name
                : type.Name;
        }

        public static Guid IdFromKey<TBase>(this Type source, string key)
        {
            Guid result = Guid.Empty;
            if (key != null)
            {
                string keyValue = key
                    .Replace(String.Format("{0}/", source.Name.ToLower()), "")
                    .Replace(String.Format("{0}/", typeof (TBase).Name.ToLower()), "");
                result = new Guid(keyValue);
            }
            return result;
        }

        public static IEnumerable<string> SubClassNames(this Type type)
        {
            Assembly assembly = Assembly.GetAssembly(type);
            return assembly
                .GetTypes()
                .Where(type.IsAssignableFrom)
                .Select(i => i.Name)
                .ToList();
        }
    }
}