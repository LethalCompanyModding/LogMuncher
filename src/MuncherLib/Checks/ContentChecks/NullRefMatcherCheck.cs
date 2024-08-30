using dev.mamallama.checkrunnerlib.Checks;
using System.Text.RegularExpressions;
using MuncherLib.Muncher;

namespace MuncherLib.Checks.ContentChecks;

internal class NullRefMatcherCheck(string Target) : BaseViolationCheck(Target, SpecificViolation)
{
    private readonly static Violation SpecificViolation = new(
        new("""Object reference not set.*object""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
        20000
    );
    protected override CheckStatus ViolationLevel => CheckStatus.Warning;
}
