using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Text;

namespace VireiContador.Infra.Extensions
{
    public static class DynamicExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static dynamic ToDynamicObject(this IDictionary<string, object> source)
        {
            ICollection<KeyValuePair<string, object>> someObject = new ExpandoObject();
            someObject.AddRange(source);
            return someObject;
        }
    }
}
