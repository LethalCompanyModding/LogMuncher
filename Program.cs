using System.Runtime.ExceptionServices;
using LogMuncher.Muncher;

[assembly: System.Resources.NeutralResourcesLanguageAttribute("en")]
namespace LogMuncher;

internal class Program
{

    static void FirstChanceHandler(object? source, FirstChanceExceptionEventArgs e)
    {

        if (lastEvent != null)
        {
            if (lastEvent == e.Exception)
            {
                RepeatLogger.WriteLogLine("Skipping a previously handled, unwinding exception");
                return;
            }
        }

        lastEvent = e.Exception;

        //RepeatLogger.WriteLogLine("FirstChanceException event raised in {0}: {1}",
        //AppDomain.CurrentDomain.FriendlyName, e.Exception.Message);
    }

    protected static Exception? lastEvent;

    protected static string output = "";
    protected static bool DoOutput = false;
    protected static StreamWriter? AppendFile;
    protected static bool DoConsole = false;

    public static void WriteData(object Data)
    {

        //Write to console
        if (DoConsole)
            RepeatLogger.WriteLogLine(Data);

        //Write to output file
        if (DoOutput)
        {
            AppendFile ??= new(output, true);
            AppendFile.AutoFlush = false;

            AppendFile.WriteLine(Data);
        }
    }

    /// <summary>
    /// LogMuncher written by Robyn
    /// </summary>
    /// <param name="i">File name to read input from</param>
    /// <param name="o">File name to output to</param>
    /// <param name="con">Flag to show output to the console too</param>
    static void Main(string o, bool con, string i = "LogOutput.log")
    {

        //Become exception royalty
        //This works but its going to be more useful when Muncher becomes a plugin
        //AppDomain.CurrentDomain.FirstChanceException += FirstChanceHandler;

        //default filename
        string input = i;
        string window = string.Empty;

        //Check if output is given
        if (o is not null)
        {
            output = o;
            DoOutput = true;
        }

        //check if we are writing to the console
        DoConsole = con;

        TheLogMuncher muncher = new();
        List<LineData> Lines = [];

        RepeatLogger.WriteLogLine($"Munching {input}");

        int lineNo = 1;
        int addedLines = 0;

        try
        {
            using StreamReader sr = new(input);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {

                //Read context until next line
                while (sr.Peek() != '[' && sr.Peek() != -1)
                {
                    line += sr.ReadLine()?.ReplaceLineEndings("");
                    addedLines++;
                }

                var (value, data) = muncher.MunchLine(lineNo, line);

                if (value > 1f)
                {
                    Lines.Add(data);
                }

                lineNo++;
                lineNo += addedLines;
                addedLines = 0;
            }
        }
        catch (Exception e) when (e is IOException || e is OutOfMemoryException)
        {
            RepeatLogger.WriteLogLine("The file could not be read because:");
            RepeatLogger.WriteLogLine($"  {e.Message}");
            return;
        }

        WriteData("\n----------------------------------------------------------------------------------");
        WriteData($"Sorted {lineNo} total lines into {Lines.Count} potential issues");
        WriteData("----------------------------------------------------------------------------------\n");

        Lines.Sort((x, y) => y.Weight.CompareTo(x.Weight));

        foreach (var item in Lines)
        {
            WriteData(item);
        }

        if (DoOutput)
        {
            AppendFile?.Flush();
            AppendFile?.Close();
        }
    }
}
