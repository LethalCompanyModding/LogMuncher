using dev.mamallama.checkrunnerlib.Checks;
using System.Text.RegularExpressions;
using LogMuncher.Muncher;

namespace LogMuncher.Checks.ContentChecks;

internal class NullRefMatcherCheck(string Target) : BaseViolationCheck(Target, SpecificViolation)
{
    private readonly static Violation SpecificViolation = new(
        new("""Object reference not set.*object""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
        2001
    );

    public override string CheckID => "NullRef Matcher";
    protected override CheckStatus ViolationLevel => CheckStatus.Warning;
}



/*

*/
