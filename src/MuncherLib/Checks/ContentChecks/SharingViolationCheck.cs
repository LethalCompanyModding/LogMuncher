using dev.mamallama.checkrunnerlib.Checks;
using System.Text.RegularExpressions;
using MuncherLib.Muncher;

namespace MuncherLib.Checks.ContentChecks;

internal class SharingViolationCheck(string Target) : BaseViolationCheck(Target, SpecificViolation)
{
    private readonly static Violation SpecificViolation = new(
        new("""sharing violation""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
        20002
    );
    protected override CheckStatus ViolationLevel => CheckStatus.Warning;
}
