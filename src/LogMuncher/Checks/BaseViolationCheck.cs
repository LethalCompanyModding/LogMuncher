using LogMuncher.Muncher;
using System.Text.RegularExpressions;
using dev.mamallama.checkrunnerlib.Checks;

namespace LogMuncher.Checks;

internal abstract class BaseViolationCheck(string Target, Violation MyViolation) : BaseCheck
{
    protected string Target = Target;
    public readonly Violation MyViolation = MyViolation;
    protected abstract CheckStatus ViolationLevel { get; }

    public sealed override LineValidation RunCheck()
    {
        var Matches = MyViolation.Regex.Match(Target);

        if (Matches.Success)
        {
            return new LineValidation(CheckID, ViolationLevel, "", MyViolation.ErrorCode);
        }

        //Don't return a because if we match since it will be skipped
        return new LineValidation(CheckID, CheckStatus.Succeeded, "", MyViolation.ErrorCode);
    }
}
