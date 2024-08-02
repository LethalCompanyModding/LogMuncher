using LogMuncher.Muncher;
using System.Text.RegularExpressions;
using dev.mamallama.checkrunnerlib.Checks;
using dev.mamallama.checkrunnerlib.CheckRunners;

namespace LogMuncher.Checks;

internal abstract class BaseViolationCheck(string Target, Violation MyViolation) : BaseCheckRunner
{
    protected string Target = Target;
    public readonly Violation MyViolation = MyViolation;
    protected abstract CheckStatus ViolationLevel { get; }
    public override ICheckRunner[] MyChecks => [];

    public sealed override void RunChecks()
    {
        var Matches = MyViolation.Regex.Match(Target);

        if (Matches.Success)
        {
            State = ViolationLevel;
        }
        else
        {
            State = CheckStatus.Succeeded;
        }
    }
}
