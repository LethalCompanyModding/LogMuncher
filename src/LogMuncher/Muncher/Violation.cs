using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace LogMuncher.Muncher;

internal readonly struct Violation(Regex Regex, int ErrorCode)
{
    public readonly Regex Regex = Regex;
    public readonly int ErrorCode = ErrorCode;
}
