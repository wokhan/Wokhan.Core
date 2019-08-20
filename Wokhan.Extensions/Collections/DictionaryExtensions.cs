using System;
using System.Collections;

namespace Wokhan.Collections.Extensions
{
    public static class DictionaryExtensions
    {

        public static void AddIfValued(this IDictionary propertyCollection, string propertyName, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                propertyCollection[propertyName] = value;
            }
        }

        public static void AddIfValued(this IDictionary propertyCollection, string propertyName, bool value)
        {
            if (value)
            {
                propertyCollection[propertyName] = value;
            }
        }
    }
}
