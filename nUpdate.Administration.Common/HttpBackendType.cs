using System.ComponentModel;

namespace nUpdate.Administration.Common
{
    public enum HttpBackendType
    {
        [Description("administration.php")]
        Php,
        [Description("index.js")]
        NodeJs,
        Custom
    }
}
