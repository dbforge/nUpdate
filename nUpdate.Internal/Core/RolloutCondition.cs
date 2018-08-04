using System;

namespace nUpdate.Internal.Core
{
    [Serializable]
    public class RolloutCondition
    {
        public RolloutCondition()
        { }

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
