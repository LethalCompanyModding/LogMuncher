namespace MuncherLib.Muncher;

public enum CircumstanceType
{
    Additive,
    Multiplicative,
}

internal static class CircumstanceType_Ext
{
    public static string GetStringDesc(this CircumstanceType self, float Value)
    {
        return self switch
        {
            CircumstanceType.Additive => Value < 0 ? "" : "+",
            CircumstanceType.Multiplicative => "*",
            _ => throw new System.Exception("Unreachable"),
        };
    }
}
