using dev.mamallama.checkrunnerlib.Checks;
using System.Text.RegularExpressions;
using LogMuncher.Muncher;

namespace LogMuncher.Checks.ContentChecks;

internal class SharingViolationCheck(string Target) : BaseViolationCheck(Target, SpecificViolation)
{
    private readonly static Violation SpecificViolation = new(
        new("""sharing violation""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
        2002
    );

    public override string CheckID => "Sharing Violation";
    protected override CheckStatus ViolationLevel => CheckStatus.Warning;
}
