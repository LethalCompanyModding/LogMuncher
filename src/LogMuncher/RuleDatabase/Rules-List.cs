using System.ComponentModel;

namespace LogMuncher.RuleDatabase;

internal static partial class Rules
{
    public static void Init()
    {

        //Ignorable non-issues 0000s

        RuleList.Add(0000,
            new(-15f, Muncher.CircumstanceType.Additive, "This warning is completely safe to ignore")
        );

        //Slight Issues 1000s

        //Moderate Errors 2000s

        RuleList.Add(2000,
            new(4f, Muncher.CircumstanceType.Additive, "Expression contains a null reference")
        );

        //BepinEx same plugin multiple times
        RuleList.Add(2001,
            new(5f, Muncher.CircumstanceType.Additive, "BepinEx is skipping a plugin because it already loaded it once")
        );

        RuleList.Add(2002,
            new(5f, Muncher.CircumstanceType.Additive, "A file failed to load due to a sharing violation")
        );

        //Serious errors 3000s

        //Critical errors 4000s

        RuleList.Add(4000,
            new(15f, Muncher.CircumstanceType.Additive, "Expression contains an Exception")
        );
    }
}
