using dev.mamallama.checkrunnerlib.CheckRunners;
using MuncherLib.Checks.ContentChecks;

namespace MuncherLib.CheckRunners;

internal class RegexContentRunner(string Target) : BaseRegexCheckRunner(Target)
{
    public override string CheckID => "Content Matcher";
    public override ICheckRunner[] MyChecks => checks;
    private readonly ICheckRunner[] checks = [
            new BepinExMultipleLoadsCheck(Target),
            new BepinExVersionCheck(Target),
            new ExceptionMatcherCheck(Target),
            new NullRefMatcherCheck(Target),
            new PlayerRagdollCheck(Target),
            new SharingViolationCheck(Target),
        ];
}
