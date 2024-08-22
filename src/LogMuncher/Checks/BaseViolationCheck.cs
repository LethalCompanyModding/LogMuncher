using LogMuncher.Muncher;
using dev.mamallama.checkrunnerlib.Checks;
using dev.mamallama.checkrunnerlib.CheckRunners;

namespace LogMuncher.Checks;

internal abstract class BaseViolationCheck(string Target, Violation MyViolation) : BaseCheckRunner
{
    protected string Target = Target;
    public readonly Violation MyViolation = MyViolation;
    protected abstract CheckStatus ViolationLevel { get; }
    public override ICheckRunner[] MyChecks => [];
    public override string CheckID => "Violation Check";

    public sealed override void RunChecks()
    {
        var Matches = MyViolation.Regex.Match(Target);
        State = Matches.Success ? ViolationLevel : CheckStatus.Succeeded;
    }
}
