using System;
using System.Collections.Generic;

namespace MuncherLib.RuleDatabase;

public static partial class Rules
{
    internal static readonly Dictionary<int, Rule> RuleList = [];

    public static Rule GetRuleById(int ID)
    {
        if (RuleList.TryGetValue(ID, out Rule value))
        {
            return value;
        }

        throw new ArgumentOutOfRangeException(nameof(ID));
    }
}
