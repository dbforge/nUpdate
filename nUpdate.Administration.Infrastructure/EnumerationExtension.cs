// EnumerationExtension.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

namespace nUpdate.Administration.Infrastructure
{
    public class EnumerationExtension : MarkupExtension
    {
        private Type _enumType;


        public EnumerationExtension(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType));

            EnumType = enumType;
        }

        public Type EnumType
        {
            get => _enumType;
            private set
            {
                if (_enumType == value)
                    return;

                var enumType = Nullable.GetUnderlyingType(value) ?? value;

                if (enumType.IsEnum == false)
                    throw new ArgumentException("Type must be an Enum.");

                _enumType = value;
            }
        }

        private string GetDescription(object enumValue)
        {
            return EnumType
                .GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault() is DescriptionAttribute descriptionAttribute
                ? descriptionAttribute.Description
                : enumValue.ToString();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(EnumType);

            return (
                from object enumValue in enumValues
                select new EnumerationMember
                {
                    Value = enumValue,
                    Description = GetDescription(enumValue)
                }).ToArray();
        }

        public class EnumerationMember
        {
            public string Description { get; set; }
            public object Value { get; set; }
        }
    }
}