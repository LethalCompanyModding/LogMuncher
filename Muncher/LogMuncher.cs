/*
Line munching algo for weighting a Unity log entry
*/

using System;
using System.IO;
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
    protected static readonly LineData def = new(-1, "", "", "");

    //For repeat log snipping
    protected readonly List<int> AllErrorHashes = [];

    protected static readonly List<Violation> Modifiers =
    [
        //Additive Modifiers first

        //Do not care about BepinEx wrong version warnings
        new(
            new("""bepinex \(.+\) and might not work""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
            CircumstanceType.Additive,
            -15f,
            "[LCM1000] This warning is completely safe to ignore"
        ),

        //Matches all properly named exceptions
        new(
            new("""[\s\w]*Exception""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
            CircumstanceType.Additive,
            15f,
            "[LCM2000] Expression contains an Exception"
        ),

        //Elevate logs that talk about null refs without mentioning an exception
        new(
            new("""Object reference not set.*object""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
            CircumstanceType.Additive,
            4f,
            "[LCM2001] Expression contains a null reference"
        ),

        //Matches if bepin skips a plugin because its being loaded multiple times
        new(
            new("""skipping.*version exists""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
            CircumstanceType.Additive,
            5f,
            "[LCM2002] BepinEx is skipping a plugin because it already loaded it once"
        ),

        //Elevate logs that fail to open files due to sharing violations
        //TODO: maybe make this match on more general failed to open file statements
        new(
            new("""sharing violation""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
            CircumstanceType.Additive,
            5f,
            "[LCM2003] A file failed to load"
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

                var data = MunchLine(lineNo, line);

                if (data.Weight > 1f)
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

    internal LineData MunchLine(int LineNo, string Contents)
    {
        //Break the line into pieces
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

        LineData line = new(LineNo, level, source, contents);

        if (level == string.Empty || source == string.Empty || contents == string.Empty)
        {
            WriteLine("Nothing to capture, skipping");
            return def;
        }

        //Run all circumstance modifiers
        foreach (var item in Modifiers)
        {
            var Matches = item.Regex.Match(contents);

            if (Matches.Success)
            {
                line.violations.Add(item);
            }
        }

        var tempHash = contents.GetHashCode();

        if (AllErrorHashes.Contains(tempHash))
        {
            WriteLine("Skipping a repeat line");
            return def;
        }

        AllErrorHashes.Add(tempHash);

        return line;
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
