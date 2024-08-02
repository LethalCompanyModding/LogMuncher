using dev.mamallama.checkrunnerlib.Checks;
using LogMuncher.Checks;
using LogMuncher.Checks.ContentChecks;

namespace LogMuncher.CheckRunners;

internal class RegexContentRunner(string Target) : BaseRegexCheckRunner(Target)
{
    public override string CheckGroupID => "Content Matcher";
    protected override ICheck[] MyChecks => checks;
    private readonly ICheck[] checks = [
            new BepinExMultipleLoadsCheck(Target),
            new BepinExVersionCheck(Target),
            new ExceptionMatcherCheck(Target),
            new NullRefMatcherCheck(Target),
            new SharingViolationCheck(Target),
        ];
}
