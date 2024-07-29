using System.Text.RegularExpressions;

namespace LogMuncher.Muncher;

internal class WeightModifier(Regex Regex, CircumstanceType Type, float Value)
{
    public readonly Regex Regex = Regex;
    public readonly CircumstanceType Type = Type;
    public readonly float Value = Value;
}