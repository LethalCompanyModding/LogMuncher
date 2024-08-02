using dev.mamallama.checkrunnerlib.Checks;
using System.Text.RegularExpressions;
using LogMuncher.Muncher;

namespace LogMuncher.Checks.ContentChecks;

internal class PlayerRagdollCheck(string Target) : BaseViolationCheck(Target, SpecificViolation)
{
    private readonly static Violation SpecificViolation = new(
        new("""tag\W PlayerRagdoll\d* .+defined""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
        00001
    );

    protected override CheckStatus ViolationLevel => CheckStatus.Warning;
}
