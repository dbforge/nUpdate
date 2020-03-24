// UpdateRolloutCondition.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;

namespace nUpdate
{
    [Serializable]
    public class UpdateRolloutCondition
    {
        public UpdateRolloutCondition(string key, string value, bool isNegative = false)
        {
            Key = key;
            Value = value;
            IsNegativeCondition = isNegative;
        }

        public bool IsNegativeCondition { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}