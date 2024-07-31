using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LogMuncher.Muncher;

internal class LineData(int Line, LogLevel Level, string Source, string Contents)
{
    public readonly int Line = Line;
    public readonly LogLevel Level = Level;
    public readonly string Source = Source;
    public readonly string Contents = Contents;
    public float Weight => GetWeight();
    public readonly List<Violation> violations = [];
    public const float BoringPenalty = 3f;

    public override string ToString()
    {
        StringBuilder builder = new($"  Line Number: {Line}\n  Source: {Source}\n  Severity: {Level}({Weight})\n  Entry:\n    {Contents}\n");

        if (violations.Count > 0)
        {
            builder.AppendLine("  Reasons:");

            foreach (var item in violations)
            {
                builder.AppendLine($"    {item.Description} [{item.Type.GetStringDesc(item.Value)}{item.Value}]");
            }
        }
        else
        {
            builder.AppendLine("  No specific rules matched");
        }

        return builder.ToString();
    }

    protected float GetWeight()
    {
        float value = Level.GetLogWeight();

        foreach (var item in violations)
        {
            switch (item.Type)
            {
                case CircumstanceType.Additive:
                    value += item.Value;
                    break;
                case CircumstanceType.Multiplicative:
                    value *= item.Value;
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(item.Type), (int)item.Type, typeof(CircumstanceType));
            }
        }

        if (violations.Count == 0)
            return value - BoringPenalty;

        return value;
    }
}
