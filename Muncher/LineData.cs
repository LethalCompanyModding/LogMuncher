using System.Diagnostics;

namespace LogMuncher.Muncher;

public class LineData(int Line, LogLevel Level, string Source, string Contents, float Weight)
{
    public readonly int Line = Line;
    public readonly LogLevel Level = Level;
    public readonly string Source = Source;
    public readonly string Contents = Contents;
    public readonly float Weight = Weight;

    public override string ToString()
    {
        return $"[#{Line}]({Weight:F2})  <{Source} | {Level}> {Contents}";
    }
}