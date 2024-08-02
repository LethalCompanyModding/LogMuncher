using System.Text;
using LogMuncher.Checks;
using dev.mamallama.checkrunnerlib.Checks;
using LogMuncher.RuleDatabase;

namespace LogMuncher.Muncher;

internal class LineData(int Line, LogLevel Level, string Source, string Contents, CheckValidation Validation)
{
    public readonly int Line = Line;
    public readonly LogLevel Level = Level;
    public readonly string Source = Source;
    public readonly string Contents = Contents;
    public float Weight
    {
        get
        {
            if (_weight == -100f)
            {
                _weight = GetWeight();
            }

            return _weight;
        }

        protected set => _weight = value;
    }
    private float _weight = -100f;
    public readonly CheckValidation Validation = Validation;
    public const float BoringPenalty = 3f;

    public override string ToString()
    {

        StringBuilder builder = new("- Line Number: $");
        builder.AppendLine(Line.ToString());
        builder.Append("- Source: ");
        builder.AppendLine(Source);
        builder.Append("- Severity: ");
        builder.AppendLine(Level);
        builder.Append('`');
        builder.Append(Weight);
        builder.Append('`');
        builder.AppendLine();

        //remove interpolation
        builder.AppendLine($"\n\n```\n{Contents}\n```\n\n");

        Validation.TraverseAndPrint(builder);

        return builder.ToString();
    }

    protected virtual float GetWeight()
    {
        float value = Level.GetLogWeight();

        //Todo match

        return value;
    }
}

internal static class TraverseLineData
{
    internal static void TraverseAndPrint(this CheckValidation head, StringBuilder sb)
    {
        //If we are the problem matcher, skip
        if (!(head.CheckName == "LCM Problem Matcher"))
        {
            //success here means this problem matcher didn't match
            if (!(head.Passed == CheckStatus.Succeeded))
            {
                if (head is LineValidation foot)
                {
                    var rule = Rules.GetRuleById(foot.MatchedRule);

                    sb.Append("- [LCM");
                    sb.Append(foot.MatchedRule);
                    sb.Append("] ");
                    sb.AppendLine(rule.Description);
                }
                else
                {
                    throw new System.Exception("FUCK");
                }
            }
        }
        else
        {
            //success here means no checks matched
            if (head.Passed == CheckStatus.Succeeded)
            {
                sb.AppendLine("- No specific rules matched");
                return;
            }
        }

        //Check for inner validations
        if (head.InnerValidations is not null)
        {
            foreach (var item in head.InnerValidations)
            {
                item.TraverseAndPrint(sb);
            }
        }
    }
}
