﻿using System.IO;
using System;
using LogMuncher.Muncher;
using System.Collections.Generic;
using System.Diagnostics;
using LogMuncher.RuleDatabase;

[assembly: System.Resources.NeutralResourcesLanguage("en")]
namespace LogMuncher;

internal class Program
{
    /// <summary>
    /// <para>
    /// LogMuncher written by Robyn
    /// </para>
    /// </summary>
    /// <param name="i">File name to read input from, requires -o [output]</param>
    /// <param name="o">File name to output to, requires -i [input]</param>
    /// <param name="f">Folder name to read in</param>
    /// <param name="quiet">Suppress most output</param>
    /// <param name="source">(Optional) All logs not from this source are discarded. Can be provided multiple times</param>
    static void Main(FileInfo i, FileInfo o, DirectoryInfo f, bool quiet, string[] source)
    {

        source ??= [];

        Stopwatch timer = new();
        timer.Start();

        List<TheLogMuncher> Munchers = [];
        Console.WriteLine("Starting up");

        TheLogMuncher.quiet = quiet;

        Rules.Init();

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
                    var WRITER = new StreamWriter(File.Open(Path.Combine(OutputPath.FullName, $"{Path.GetFileNameWithoutExtension(item.Name)}.html"), FileMode.Create));
                    Munchers.Add(new(item, WRITER, source));
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

            o ??= new($"{Path.GetFileNameWithoutExtension(i.FullName)}.html");

            if (Path.GetExtension(o.Name) != ".html")
            {
                o = new($"{Path.GetFileNameWithoutExtension(o.FullName)}.html");
                Console.WriteLine("Changing filename to end in HTML, thank me later");
            }

            Munchers.Add(new(i, new StreamWriter(o.Open(FileMode.Create)), source));
        }

        foreach (var item in Munchers)
        {
            item.MunchLog();
            item.Dispose();
        }

        Console.WriteLine($"Task completed in {timer.ElapsedMilliseconds}ms");
    }
}
