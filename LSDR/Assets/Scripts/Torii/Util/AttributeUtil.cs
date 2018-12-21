using System;
using System.Linq;
using System.Reflection;

namespace Torii.Util
{
    public static class AttributeUtil
    {
        /// <summary>
        /// Get an attribute value from a Type.
        /// 
        /// Usage:
        /// <code>string attrValue = GetAttributeValue(typeof(SomeClass), (SomeAttribute attr) => attr.Value);</code>
        /// </summary>
        /// <typeparam name="TAttribute">The Type of the attribute.</typeparam>
        /// <typeparam name="TValue">The Type of the value we want from the attribute.</typeparam>
        /// <param name="type">The Type of the class the attribute is on.</param>
        /// <param name="valueSelector">Which attribute value we want to select.</param>
        /// <returns>The value from the attribute, or the default value if the attribute wasn't found.</returns>
        public static TValue GetAttributeValue<TAttribute, TValue>(Type type, Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }

        /// <summary>
        /// Get an attribute value from a property.
        /// 
        /// Usage:
        /// <code>string attrValue = GetAttributeValue(aProperty, (SomeAttribute attr) => attr.Value);</code>
        /// </summary>
        /// <typeparam name="TAttribute">The Type of the attribute.</typeparam>
        /// <typeparam name="TValue">The Type of the value we want from the attribute.</typeparam>
        /// <param name="property">The PropertyInfo of the property we're interested in.</param>
        /// <param name="valueSelector">Which attribute value we want to select.</param>
        /// <returns>The value from the attribute, or the default value if the attribute wasn't found.</returns>
        public static TValue GetAttributeValue<TAttribute, TValue>(PropertyInfo property, Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var att = property.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }

        /// <summary>
        /// Find out if a Type has a particular attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="type">The Type the attribute might be on.</param>
        /// <returns>True if the Type had the attribute, false otherwise.</returns>
        public static bool HasAttribute<T>(Type type)
        {
            return type.GetCustomAttributes(typeof(T), true).Length > 0;
        }

        /// <summary>
        /// Find out if a property has a particular attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="property">The property the attribute might be on.</param>
        /// <returns>True if the property had the attribute, false otherwise.</returns>
        public static bool HasAttribute<T>(PropertyInfo property)
        {
            return property.GetCustomAttributes(typeof(T), true).Length > 0;
        }
    }
}