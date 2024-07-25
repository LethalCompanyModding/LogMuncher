using System.Runtime.ExceptionServices;
using System.Reflection;
using LogMuncher.Muncher;

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

    static void Main(string[] args)
    {

        //Become exception royalty
        //This works but its going to be more useful when Muncher becomes a plugin
        //AppDomain.CurrentDomain.FirstChanceException += FirstChanceHandler;

        //default filename
        string input = "LogOutput.log";
        string window = string.Empty;

        if (args.Length > 0)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-i":
                        if (args.Length >= i + 1)
                        {
                            input = args[i + 1];
                        }
                        break;
                    case "-o":
                        if (args.Length >= i + 1)
                        {
                            output = args[i + 1];
                            DoOutput = true;
                        }
                        break;
                    case "--con":
                        DoConsole = true;
                        break;
                    case "--help":
                    case "-h":
                        //Display help screen and exit
                        Console.WriteLine();
                        Console.WriteLine("Log Muncher by Robyn");
                        Console.WriteLine($"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
                        Console.WriteLine("\nValid switches:\n  -i <input file>\n  -o <output file>\n  --con Also output results to console\n");
                        Environment.Exit(0);
                        break;
                }
            }
        }

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
