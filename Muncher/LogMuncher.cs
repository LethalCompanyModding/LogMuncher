/*
Line munching algo for weighting a Unity log entry
*/

using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LogMuncher.Muncher;
internal class TheLogMuncher(FileInfo Input, TextWriter Output) : IDisposable
{

    protected TextReader Input = new StreamReader(Input.OpenRead());
    protected String FileName = Input.Name;
    protected TextWriter Output = Output;
    private int LastInHash = 0;

    public static bool quiet = false;

    //FAKE
    protected static readonly LineData def = new(-1, "", "", "", 0f);

    //For repeat log snipping
    protected readonly List<int> AllErrorHashes = [];

    protected static readonly List<WeightModifier> Modifiers =
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

    protected static readonly Regex LineBreaker = new("""\[(debug|info|warning|message|fatal|error)\s*:([\s\w]+)\](.*)""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1));

    protected const float DefaultLogWeight = 0.1f;

    public void MunchLog()
    {
        List<LineData> lines = [];

        int lineNo = 1;
        int addedLines = 0;

        try
        {
            string? line;
            while ((line = Input.ReadLine()) != null)
            {

                //Read context until next line
                while (Input.Peek() != '[' && Input.Peek() != -1)
                {
                    line += Input.ReadLine()?.ReplaceLineEndings("");
                    addedLines++;
                }

                var (value, data) = MunchLine(lineNo, line);

                if (value > 1f)
                {
                    lines.Add(data);
                }

                lineNo++;
                lineNo += addedLines;
                addedLines = 0;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("The file could not be read because:");
            Console.WriteLine($"  {e.GetType()}");
            Console.WriteLine($"  {e.Message}");

            throw;
        }

        Output.WriteLine("----------------------------------------------------------------------------------");
        Output.WriteLine($"Finished Sorting: {FileName}");
        Output.WriteLine($"Sorted {lineNo} total lines into {lines.Count} potential issues");
        Output.WriteLine("----------------------------------------------------------------------------------\n");

        lines.Sort((x, y) => y.Weight.CompareTo(x.Weight));

        foreach (var item in lines)
        {
            Output.WriteLine(item);
        }

        Output.Flush();
    }

    internal (float value, LineData data) MunchLine(int LineNo, string Contents)
    {
        //Break the line into pieces
        float value = 0;
        Match data = LineBreaker.Match(Contents);
        LogLevel level = string.Empty;
        string source = string.Empty;
        string contents = string.Empty;

        //If we match all 4 groups
        if (data.Success && data.Groups.Count == 4)
        {
            level = data.Groups[1].Captures[0].Value.Trim();
            source = data.Groups[2].Captures[0].Value.Trim();
            contents = data.Groups[3].Captures[0].Value.Trim();
        }

        if (level == string.Empty || source == string.Empty || contents == string.Empty)
        {
            WriteLine("Nothing to capture, skipping");
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
                    default:
                        throw new InvalidEnumArgumentException(nameof(item.Type), (int)item.Type, typeof(CircumstanceType));
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
            WriteLine("Skipping a repeat line");
            return (0f, def);
        }

        AllErrorHashes.Add(tempHash);

        return (value, new(LineNo, level, source, contents, value));
    }

    protected void WriteLine(object Message)
    {

        //SHUT THE HELL UP
        if (quiet)
            return;

        //Check if this is a repeat message
        int TempHash = Message.GetHashCode();

        if (TempHash == LastInHash)
            return;

        LastInHash = TempHash;
        Console.WriteLine(Message);
    }

    public void Dispose()
    {
        Input.Dispose();
        Output.Dispose();
    }
}
