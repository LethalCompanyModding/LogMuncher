using dev.mamallama.checkrunnerlib.Checks;

namespace LogMuncher.Checks;

internal class LineValidation : CheckValidation
{

    public readonly int MatchedRule;

    public LineValidation(string CheckName, CheckStatus Passed, string Because, int MatchedRule) : base(CheckName, Passed, Because)
    {
        this.MatchedRule = MatchedRule;
    }

    public LineValidation(string CheckName, CheckStatus Passed, string Because, int MatchedRule, LineValidation[] InnerValidations) : base(CheckName, Passed, Because, InnerValidations)
    {
        this.MatchedRule = MatchedRule;
    }
}
