using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace nUpdate.Core.Localization
{
    internal class LocalizationHelper
    {
        /// <summary>
        ///     Returns the localized values for the given enumeration objects.
        /// </summary>
        /// <param name="properties">The <see cref="LocalizationProperties"/>-instance to use for the localization.</param>
        /// <param name="objects">The objects for the localization.</param>
        /// <returns>Returns the found localizations.</returns>
        public static IEnumerable<string> GetLocalizedEnumerationValues(LocalizationProperties properties, Object[] objects)
        {
            foreach (var o in objects)
            {
                FieldInfo fieldInfo = o.GetType().GetField(o.ToString());
                DescriptionAttribute[] descriptionAttributes =
                    (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (descriptionAttributes.Length > 0)
                {
                    var resourceId = descriptionAttributes[0].Description;
                    yield return (string)properties.GetType().GetProperties().First(x => x.Name == resourceId).GetValue(properties, null);
                }
                else
                {
                    yield return o.ToString();
                }
            }
        }
    }
}
