// RolloutCondition.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;

namespace nUpdate
{
    [Serializable]
    public class RolloutCondition
    {
        public RolloutCondition()
        {
        }

        public RolloutCondition(string key, string value, bool isNegative = false)
        {
            Key = key;
            Value = value;
            IsNegativeCondition = isNegative;
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsNegativeCondition { get; set; }
    }
}