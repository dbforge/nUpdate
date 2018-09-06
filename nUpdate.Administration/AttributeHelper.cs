// Copyright © Dominic Beger 2018

using System;
using System.Linq;

namespace nUpdate.Administration
{
    internal static class AttributeHelper
    {
        // TODO: Check the attribute implementation. Could become obsolete.
        internal static bool Compare<TAttribute, TValue>(object o, Type type, Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            return o.GetAttributeValue(valueSelector).Equals(type.GetAttributeValue(valueSelector));
        }

        internal static TValue GetAttributeValue<TAttribute, TValue>(
            this Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            if (type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() is TAttribute att)
                return valueSelector(att);
            return default(TValue);
        }

        internal static TValue GetAttributeValue<TAttribute, TValue>(
            this object o,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            return o.GetType().GetAttributeValue(valueSelector);
        }
    }
}