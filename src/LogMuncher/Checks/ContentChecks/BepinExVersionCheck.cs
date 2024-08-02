using dev.mamallama.checkrunnerlib.Checks;
using System.Text.RegularExpressions;
using LogMuncher.Muncher;

namespace LogMuncher.Checks.ContentChecks;

internal class BepinExVersionCheck(string Target) : BaseViolationCheck(Target, SpecificViolation)
{
    private readonly static Violation SpecificViolation = new(
        new("""bepinex \(.+\) and might not work""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
        00000
    );
    protected override CheckStatus ViolationLevel => CheckStatus.Warning;
}
