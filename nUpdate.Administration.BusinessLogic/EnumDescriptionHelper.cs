// EnumDescriptionHelper.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.ComponentModel;
using System.Linq;

namespace nUpdate.Administration.BusinessLogic
{
    public static class EnumDescriptionHelper
    {
        public static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes &&
                attributes.Any())
                return attributes.First().Description;
            return value.ToString();
        }
    }
}