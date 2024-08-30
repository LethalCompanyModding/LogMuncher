using System.IO;
using System;
using MuncherLib.Muncher;
using System.Collections.Generic;
using System.Diagnostics;
using MuncherLib.RuleDatabase;
using System.Threading.Tasks;

[assembly: System.Resources.NeutralResourcesLanguage("en")]
namespace LogMuncherApp;

internal class Program
{
    /// <summary>
    /// <para>
    /// LogMuncher written by Robyn
    /// </para>
    /// </summary>
    /// <param name="i">File name to read input from, requires -o [output]</param>
    /// <param name="f">Folder name to read in</param>
    /// <param name="quiet">Suppress most output</param>
    /// <param name="sources">(Optional) All logs not from this source are discarded. Can be provided multiple times</param>
    static async Task Main(FileInfo i, DirectoryInfo f, bool quiet, string[] sources)
    {

        sources ??= [];

        Stopwatch timer = new();
        timer.Start();

        Console.WriteLine("Starting up");

        Rules.Init();

        //Create task list
        List<Task> tasks = [];

        //Are we in Folder Mode
        if (f is not null)
        {
            if (f.Exists)
            {

                var dirs = f.GetDirectories("munched");

                //Prepare the output directory
                DirectoryInfo OutputPath = dirs.Length == 0
                ? f.CreateSubdirectory("munched") : dirs[0];

                var inputs = f.GetFiles("*.log");

                foreach (var item in inputs)
                {
                    tasks.Add(MunchIndividualLog(item, sources, quiet));
                }
            }
            else
            {
                Console.WriteLine($"  The input folder {f.Name} does not exist\n  Full: {f.FullName}");
                return;
            }
        }
        else
        {
            if (i is null)
            {
                Console.WriteLine("  Input file not specified");
                return;
            }

            if (!i.Exists)
            {
                Console.WriteLine($"  The input file {i.Name} could not be read\n  Full: {i.FullName}");
                return;
            }

            tasks.Add(MunchIndividualLog(i, sources, quiet));
        }

        await Task.WhenAll(tasks);

        Console.WriteLine($"Task completed in {timer.ElapsedMilliseconds}ms");
    }

    static async Task MunchIndividualLog(FileInfo item, string[] sources, bool Quiet)
    {

        var directory = Path.GetDirectoryName(item.FullName);
        var filename = Path.GetFileNameWithoutExtension(item.FullName);

        if (!item.Exists || directory is null)
        {
            Console.WriteLine($"Skipping file: {item.Name}, unable to read");
            return;
        }

        var InputReader = new StreamReader(item.OpenRead());

        var OutputWriter = new StreamWriter(File.Open(Path.Combine(directory, filename + ".html"), FileMode.Create));
        using var muncher = new LogMuncher(InputReader, OutputWriter, sources, Quiet, HTMLOutput: false);
        await muncher.MunchLog();
    }
}
