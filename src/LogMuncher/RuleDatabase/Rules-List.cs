using System.ComponentModel;

namespace LogMuncher.RuleDatabase;

internal static partial class Rules
{
    public static void Init()
    {
        RuleList.Add(1000,
            new(-15f, Muncher.CircumstanceType.Additive, "This warning is completely safe to ignore")
        );

        RuleList.Add(2000,
            new(15f, Muncher.CircumstanceType.Additive, "Expression contains an Exception")
        );

        RuleList.Add(2001,
            new(4f, Muncher.CircumstanceType.Additive, "Expression contains a null reference")
        );

        //BepinEx same plugin multiple times
        RuleList.Add(2002,
            new(5f, Muncher.CircumstanceType.Additive, "BepinEx is skipping a plugin because it already loaded it once")
        );

        RuleList.Add(2003,
            new(5f, Muncher.CircumstanceType.Additive, "A file failed to load")
        );
    }
}
