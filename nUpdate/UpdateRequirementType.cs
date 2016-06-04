// Author: Dominic Beger (Trade/ProgTrade)

using System.ComponentModel;

namespace nUpdate
{
    public enum UpdateRequirementType
    {
        [Description("DotNetFrameworkText")]
        DotNetFramework,
        // ReSharper disable once InconsistentNaming
        [Description("OperatingSystemText")]
        OSVersion,
    }
}