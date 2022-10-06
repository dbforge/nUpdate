﻿// LocalizationHelper.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace nUpdate.Localization
{
    internal class LocalizationHelper
    {
        internal static CultureInfo[] IntegratedCultures => new[]
        {
            new CultureInfo("de-AT"), new CultureInfo("de-CH"), new CultureInfo("de-DE"), new CultureInfo("zh-CN"),
            new CultureInfo("it-IT"), new CultureInfo("en"), new CultureInfo("es-ES")
        };

        internal static LocalizationProperties GetLocalizationProperties(CultureInfo cultureInfo,
            Dictionary<CultureInfo, string> localizationFilePaths)
        {
            var resourceName = $"nUpdate.Localization.{cultureInfo.Name}.json";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                    return Serializer.Deserialize<LocalizationProperties>(stream);

                string localizationFilePath;
                localizationFilePaths.TryGetValue(cultureInfo, out localizationFilePath);
                if (localizationFilePath == null)
                    throw new Exception("The path of the localization file is not valid.");
                return Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(localizationFilePath,
                    Encoding.UTF8));
            }
        }

        /// <summary>
        ///     Returns the localized values for the given enumeration objects.
        /// </summary>
        /// <param name="properties">The <see cref="LocalizationProperties" />-instance to use for the localization.</param>
        /// <param name="objects">The objects for the localization.</param>
        /// <returns>Returns the found localizations.</returns>
        internal static IEnumerable<string> GetLocalizedEnumerationValues(LocalizationProperties properties,
            object[] objects)
        {
            foreach (var o in objects)
            {
                var fieldInfo = o.GetType().GetField(o.ToString());
                var descriptionAttributes =
                    (DescriptionAttribute[]) fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (descriptionAttributes.Length > 0)
                {
                    var resourceId = descriptionAttributes[0].Description;
                    yield return
                        (string)
                        properties.GetType()
                            .GetProperties()
                            .First(x => x.Name == resourceId)
                            .GetValue(properties, null);
                }
                else
                {
                    yield return o.ToString();
                }
            }
        }

        internal static bool IsIntegratedCulture(CultureInfo cultureInfo,
            Dictionary<CultureInfo, string> localizationFilePaths)
        {
            localizationFilePaths.TryGetValue(cultureInfo, out var localizationFilePath);
            return IntegratedCultures.Contains(cultureInfo) || localizationFilePath != null;
        }
    }
}