using System.ComponentModel;

namespace LogMuncher.RuleDatabase;

internal static partial class Rules
{
    public static void Init()
    {

        //Ignorable non-issues 0000s

        RuleList.Add(00000,
            new(-15f, Muncher.CircumstanceType.Additive, "This warning is completely safe to ignore")
        );

        RuleList.Add(00001,
            new(-15f, Muncher.CircumstanceType.Additive, "This error is completely safe to ignore")
        );

        //Slight Issues 1000s

        //Moderate Errors 2000s

        RuleList.Add(20000,
            new(4f, Muncher.CircumstanceType.Additive, "Expression contains a null reference")
        );

        //BepinEx same plugin multiple times
        RuleList.Add(20001,
            new(6f, Muncher.CircumstanceType.Additive, "BepinEx is skipping a plugin because it already loaded it once")
        );

        RuleList.Add(20002,
            new(5f, Muncher.CircumstanceType.Additive, "A file failed to load due to a sharing violation")
        );

        //Serious errors 3000s
        RuleList.Add(35000,
            new(10f, Muncher.CircumstanceType.Additive, "Expression originates from HarmonyX, this is usually a bad sign")
        );

        //Critical errors 4000s

        RuleList.Add(40000,
            new(15f, Muncher.CircumstanceType.Additive, "Expression contains an Exception")
        );
    }
}
