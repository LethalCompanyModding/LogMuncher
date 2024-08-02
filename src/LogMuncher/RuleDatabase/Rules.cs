using System;
using System.Collections.Generic;

namespace LogMuncher.RuleDatabase;

internal static partial class Rules
{
    public static readonly Dictionary<int, Rule> RuleList = [];

    public static Rule GetRuleById(int ID)
    {
        if (RuleList.TryGetValue(ID, out Rule value))
        {
            return value;
        }

        throw new ArgumentOutOfRangeException(nameof(ID));
    }
}
