using System.Text;
using LogMuncher.Checks;
using dev.mamallama.checkrunnerlib.Checks;
using LogMuncher.RuleDatabase;
using LogMuncher.CheckRunners;
using dev.mamallama.checkrunnerlib.CheckRunners;

namespace LogMuncher.Muncher;

internal class LineData(int Line, LogLevel Level, string Source, string Contents, AllChecksRunner Runner)
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
    public readonly AllChecksRunner Runner = Runner;
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

        StringBuilder matcher = new();

        foreach (var item in Runner.MyChecks)
        {
            item.TraverseAndPrint(matcher);
        }

        //Append Problem Matcher Header
        if (matcher.Length > 0)
        {
            builder.Append("### ");
            builder.AppendLine(Runner.CheckID);
            builder.Append(matcher);
        }

        return builder.ToString();
    }

    protected virtual float GetWeight()
    {
        float value = Level.GetLogWeight();

        return value + Runner.TraverseAndCount();
    }
}

internal static class TraverseLineData
{
    internal static void TraverseAndPrint(this ICheckRunner head, StringBuilder sb)
    {

        //branch
        if (head.MyChecks.Length > 0)
        {
            foreach (var item in head.MyChecks)
            {
                item.TraverseAndPrint(sb);
            }
        }
        else
        {
            //output leaf
            if (head is BaseViolationCheck check)
            {

                //if this rule actually matched
                if (check.State == CheckStatus.Succeeded)
                    return;

                //cache the rule
                var rule = Rules.GetRuleById(check.MyViolation.ErrorCode);

                const string MatcherAddress = "https://lethalcompanymodding.github.io/Thunderstore/www/Tools/Log-Muncher.html#lcm-";

                //Output Violation Name and link
                sb.Append("- ");
                sb.Append("""<a href= '""");
                sb.Append(MatcherAddress);
                sb.Append(check.MyViolation.ErrorCode.ToString("D4"));
                sb.Append("' >");

                sb.Append("[LCM");
                sb.Append(check.MyViolation.ErrorCode.ToString("D4"));
                sb.Append("] ");
                sb.Append("""</a>""");

                //Violation description
                sb.Append(rule.Description);
                sb.Append(" `");
                sb.Append(rule.Type.GetStringDesc(rule.Value));
                sb.Append(rule.Value);
                sb.AppendLine("`");
            }
        }
    }

    internal static float TraverseAndCount(this ICheckRunner head)
    {

        if (head is null)
            return 0;

        float value = 0;

        //branch
        if (head.MyChecks.Length > 0)
        {
            foreach (var item in head.MyChecks)
            {
                value += item.TraverseAndCount();
            }

        }
        else
        {
            //output leaf
            if (head is BaseViolationCheck check)
            {
                //if this rule actually matched
                if (check.State == CheckStatus.Succeeded)
                    return 0;

                return Rules.GetRuleById(check.MyViolation.ErrorCode).Value;
            }
        }

        return value;
    }
}
