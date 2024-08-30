using System.Text.RegularExpressions;

namespace MuncherLib.Muncher;

internal readonly struct Violation(Regex Regex, int ErrorCode)
{
    public readonly Regex Regex = Regex;
    public readonly int ErrorCode = ErrorCode;
}
