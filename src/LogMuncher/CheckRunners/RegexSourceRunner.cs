using LogMuncher.Checks;
using dev.mamallama.checkrunnerlib.Checks;

namespace LogMuncher.CheckRunners;

internal class RegexSourceRunner(string Target) : BaseRegexCheckRunner(Target)
{
    public override string CheckGroupID => "Source Matcher";

    protected override ICheck[] MyChecks => checks;
    private readonly ICheck[] checks = [

    ];
}
