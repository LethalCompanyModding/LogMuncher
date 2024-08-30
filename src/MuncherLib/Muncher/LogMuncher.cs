/*
Line munching algo for weighting a Unity log entry
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using Markdig;
using MuncherLib.CheckRunners;
using System.Linq;
using System.Threading.Tasks;

namespace MuncherLib.Muncher;
public class LogMuncher(StreamReader Input, StreamWriter Output, string[] sources = null!, bool Quiet = true, bool HTMLOutput = true) : IDisposable
{
    public const char RETURN_CHAR = '\u2028';
    protected TextReader Input = Input;
    protected TextWriter Output = Output;
    protected string[] sources = sources ?? [];
    private int LastInHash = 0;
    public bool Quiet = Quiet;
    public bool HTMLOutput = HTMLOutput;

    //FAKE
    protected static readonly LineData def = new(-1, "", "", "", null!);

    //For repeat log snipping
    protected readonly List<int> AllErrorHashes = [];

    protected static readonly Regex LineBreaker = new("""\[(debug|info|warning|message|fatal|error)\s*:([\s\w]+)\](.*)""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1));

    protected const float DefaultLogWeight = 0.1f;

    public async Task<List<LineData>> MunchLog(bool raw = false)
    {
        List<LineData> lines = [];
        StringBuilder buffer = new();

        if (HTMLOutput)
        {

            buffer.AppendLine("""<link rel="stylesheet" href="https://raw.githack.com/hyrious/github-markdown-css/main/dist/dark.css">""");
            buffer.AppendLine("""
        <style type="text/css">
        html, body {
        margin: 0 !important;
        padding: 0 !important;
        }
        .markdown-body {
        margin: 0 !important;
        padding: 1em;
        }
        .markdown-body pre > code {
        white-space: pre-wrap;
        word-break: break-word;
        }
        </style>
        """);
            buffer.AppendLine("""<div class="markdown-body">""");
            buffer.AppendLine("");
        }

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
                    line += RETURN_CHAR + Input.ReadLine();
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

        buffer.AppendLine("# LogMuncher Report");
        buffer.AppendLine($"Sorted {lineNo} total lines into {lines.Count} potential issues\n");

        lines.Sort((x, y) => y.Weight.CompareTo(x.Weight));

        if (raw)
            return lines;

        foreach (var item in lines)
        {
            buffer.AppendLine("## Issue");
            buffer.AppendLine(item.ToString());
        }

        if (HTMLOutput)
        {
            buffer.AppendLine("""</div>""");
            Markdown.ToHtml(buffer.ToString(), Output);
        }
        else
            Output.Write(buffer.ToString());

        Output.Flush();
        return null!;
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

        if (sources.Length > 0 && !sources.Contains(source))
        {
            WriteLine("Skipping source not in source list");
            return def;
        }

        if (level == string.Empty || source == string.Empty || contents == string.Empty)
        {
            //WriteLine("Nothing to capture, skipping");
            return def;
        }

        var tempHash = contents.GetHashCode();

        if (AllErrorHashes.Contains(tempHash))
        {
            //WriteLine("Skipping a repeat line");
            return def;
        }

        AllErrorHashes.Add(tempHash);

        //Run all checks for the line
        var Runner = new AllChecksRunner(source, contents);
        Runner.RunChecks();

        return new(LineNo, level, source, contents, Runner);
    }

    protected void WriteLine(object Message)
    {

        //SHUT THE HELL UP
        if (Quiet)
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
