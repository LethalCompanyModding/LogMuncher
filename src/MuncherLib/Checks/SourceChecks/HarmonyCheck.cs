using dev.mamallama.checkrunnerlib.Checks;
using System.Text.RegularExpressions;
using MuncherLib.Muncher;

namespace MuncherLib.Checks.ContentChecks;

internal class HarmonySourceCheck(string Target) : BaseViolationCheck(Target, SpecificViolation)
{
    private readonly static Violation SpecificViolation = new(
        new("""harmonyX?""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
        35000
    );

    protected override CheckStatus ViolationLevel => CheckStatus.Warning;
}
