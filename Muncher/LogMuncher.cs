/*
Line munching algo for weighting a Unity log entry
*/
using System.Text.RegularExpressions;

namespace LogMuncher.Muncher;

internal class TheLogMuncher()
{

    //FAKE
    protected static readonly LineData def = new(-1, "", "", "", 0f);

    //For repeat log snipping
    protected List<int> AllErrorHashes = [];

    protected List<WeightModifier> Modifiers =
    [
        //Additive Modifiers first

        //Matches all properly named exceptions
        new(
            new("""[\s\w]*Exception""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
            CircumstanceType.Additive,
            15f
        ),

        //Matches if bepin skips a plugin because its being loaded multiple times
        new(
            new("""skipping.*version exists""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
            CircumstanceType.Additive,
            5f
        ),

        //Do not care about BepinEx wrong version warnings
        new(
            new("""bepinex \(.+\) and might not work""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
            CircumstanceType.Additive,
            -15f
        ),

        //Elevate logs that talk about null refs without mentioning an exception
        new(
            new("""Object reference not set.*object""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
            CircumstanceType.Additive,
            4f
        ),

        //Elevate logs that fail to open files due to sharing violations
        //TODO: maybe make this match on more general failed to open file statements
        new(
            new("""sharing violation""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
            CircumstanceType.Additive,
            5f
        ),
    ];

    protected Regex LineBreaker = new("""\[(debug|info|warning|message|fatal|error)\s*:([\s\w]+)\](.*)""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1));

    protected float DefaultLogWeight = 0.1f;

    public (float value, LineData data) MunchLine(int LineNo, string Contents)
    {
        //Break the line into pieces
        float value = 0;
        Match data = LineBreaker.Match(Contents);
        LogLevel level = string.Empty;
        string source = string.Empty;
        string contents = string.Empty;

        if (data.Success)
        {
            //Should always be four if we match all groups
            if (data.Groups.Count == 4)
            {
                level = data.Groups[1].Captures[0].Value.Trim();
                source = data.Groups[2].Captures[0].Value.Trim();
                contents = data.Groups[3].Captures[0].Value.Trim();
            }
        }

        if (level == string.Empty || source == string.Empty || contents == string.Empty)
        {
            RepeatLogger.WriteLogLine("Nothing to capture, skipping");
            return (0f, def);
        }

        //Get the base value of this log
        value = level.GetLogWeight();
        bool modsRun = false;

        //Run all circumstance modifiers
        foreach (var item in Modifiers)
        {
            var Matches = item.Regex.Match(contents);

            if (Matches.Success)
            {
                modsRun = true;

                switch (item.Type)
                {
                    case CircumstanceType.Additive:
                        value += item.Value;
                        break;
                    case CircumstanceType.Multiplicative:
                        value *= item.Value;
                        break;
                }
            }
        }

        if (!modsRun)
        {
            //No mods were run at all for this line so its boring
            value -= 3f;
        }

        var tempHash = contents.GetHashCode();

        if (AllErrorHashes.Contains(tempHash))
        {
            RepeatLogger.WriteLogLine("Skipping a repeat line");
            return (0f, def);
        }

        AllErrorHashes.Add(tempHash);

        return (value, new(LineNo, level, source, contents, value));
    }
}