using System.ComponentModel;

namespace nUpdate.Administration.Models.Http
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
