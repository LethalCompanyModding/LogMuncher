using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace LogMuncher.Muncher;

internal class Violation(Regex Regex, CircumstanceType Type, float Value, string Description)
{
    public readonly Regex Regex = Regex;
    public readonly CircumstanceType Type = Type;
    public readonly string Description = Description;
    public readonly float Value = Value;
}
