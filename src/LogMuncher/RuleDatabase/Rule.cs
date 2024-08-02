using LogMuncher.Muncher;

namespace LogMuncher.RuleDatabase;
internal readonly struct Rule(float Value, CircumstanceType Type, string Description)
{
    public readonly float Value = Value;
    public readonly CircumstanceType Type = Type;
    public readonly string Description = Description;
}
