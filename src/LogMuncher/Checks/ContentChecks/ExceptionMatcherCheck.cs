using dev.mamallama.checkrunnerlib.Checks;
using System.Text.RegularExpressions;
using LogMuncher.Muncher;

namespace LogMuncher.Checks.ContentChecks;

internal class ExceptionMatcherCheck(string Target) : BaseViolationCheck(Target, SpecificViolation)
{
    private readonly static Violation SpecificViolation = new(
        new("""[\s\w]*Exception""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
        2000
    );

    public override string CheckID => "Exception Matcher";
    protected override CheckStatus ViolationLevel => CheckStatus.Warning;
}



/*

*/
