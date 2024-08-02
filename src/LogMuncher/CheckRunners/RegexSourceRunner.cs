using LogMuncher.Checks;
using dev.mamallama.checkrunnerlib.CheckRunners;
using LogMuncher.Checks.ContentChecks;

namespace LogMuncher.CheckRunners;

internal class RegexSourceRunner(string Target) : BaseRegexCheckRunner(Target)
{
    public override string CheckID => "Source Matcher";

    public override ICheckRunner[] MyChecks => checks;
    private readonly ICheckRunner[] checks = [
        new HarmonySourceCheck(Target),
    ];
}
