using dev.mamallama.checkrunnerlib.Checks;
using System.Text.RegularExpressions;
using MuncherLib.Muncher;

namespace MuncherLib.Checks.ContentChecks;

internal class ExceptionMatcherCheck(string Target) : BaseViolationCheck(Target, SpecificViolation)
{
    private readonly static Violation SpecificViolation = new(
        new("""[\s\w]*Exception""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
        40000
    );
    protected override CheckStatus ViolationLevel => CheckStatus.Warning;
}
