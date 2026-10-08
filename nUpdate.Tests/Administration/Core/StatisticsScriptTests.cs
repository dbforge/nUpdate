using nUpdate.Administration.Core.Statistics;

namespace nUpdate.Tests.Administration.Core;

public class StatisticsScriptTests
{
    [Fact]
    public void StatisticsScript_IsEmbeddedAndConfigIsRendered()
    {
        StatisticsScript.Script.ShouldStartWith("<?php");
        StatisticsScript.Script.ShouldContain("'v2', 'downloads'");
        StatisticsScript.Script.ShouldContain("PATH_INFO");
        StatisticsScript.ScriptFileName.ShouldBe("nupdate-statistics.php");
        StatisticsScript.ConfigFileName.ShouldBe("nupdate-statistics.config.php");
        StatisticsScript.LegacyScriptFileName.ShouldBe("statistics.php");
        StatisticsScript.Script.ShouldContain("/nupdate-statistics.config.php");

        var config = StatisticsScript.RenderConfig("db.host", "na'me", "us\\er", "p'w", "secret");
        config.ShouldStartWith("<?php");
        config.ShouldContain("$nupdateDbHost = 'db.host';");
        config.ShouldContain("$nupdateDbName = 'na\\'me';");
        config.ShouldContain("$nupdateDbUser = 'us\\\\er';");
        config.ShouldContain("$nupdateDbPassword = 'p\\'w';");
        config.ShouldContain("$nupdateAdminSecretHash = '" + nUpdate.Administration.Core.Security.SecretGenerator.HashSecret("secret") + "';");
        config.ShouldNotContain("secret'");
        Should.Throw<ArgumentNullException>(() => StatisticsScript.RenderConfig(null!, "n", "u", "p", "s"));
        Should.Throw<ArgumentNullException>(() => StatisticsScript.RenderConfig("h", null!, "u", "p", "s"));
        Should.Throw<ArgumentNullException>(() => StatisticsScript.RenderConfig("h", "n", null!, "p", "s"));
        Should.Throw<ArgumentNullException>(() => StatisticsScript.RenderConfig("h", "n", "u", null!, "s"));
        Should.Throw<ArgumentException>(() => StatisticsScript.RenderConfig("h", "n", "u", "p", ""));
    }
}
