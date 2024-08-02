using dev.mamallama.checkrunnerlib.Checks;
using System.Text.RegularExpressions;
using LogMuncher.Muncher;

namespace LogMuncher.Checks.ContentChecks;

internal class BepinExMultipleLoadsCheck(string Target) : BaseViolationCheck(Target, SpecificViolation)
{
    private readonly static Violation SpecificViolation = new(
        new("""skipping.*version exists""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
        20001
    );
    protected override CheckStatus ViolationLevel => CheckStatus.Warning;
}
