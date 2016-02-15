using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BootAsAdmin
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var bootlist = ReadBootlist();

            if (bootlist == null)
            {
                Exit();
                return;
            }

            foreach (var kvp in bootlist)
            {
                try
                {
                    var applicationPath = kvp.Item1;
                    var applicationArgs = kvp.Item2;
                    var workingDir = kvp.Item3;

                    var startInfo = new ProcessStartInfo(applicationPath)
                    {
                        Verb = "runas",
                        UseShellExecute = true,
                        Arguments = applicationArgs,
                        WorkingDirectory = workingDir
                    };

                    var process = Process.Start(startInfo);

                    if (process != null)
                    {
                        Console.WriteLine($"Process {process.Id} started for {applicationPath}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}\n{ex.StackTrace}");
                }
            }

            if (args != null && args.Length == 1 && args[0] == "-noexit")
            {
                Exit();
            }
        }

        private static void Exit()
        {
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static IEnumerable<Tuple<string, string, string>> ReadBootlist()
        {
            var exePath = Assembly.GetExecutingAssembly().Location;

            var exeDir = Path.GetDirectoryName(exePath);

            if (exeDir == null)
            {
                return null;
            }

            var bootlistFilePath = Path.Combine(exeDir, "Bootlist.txt");

            if (File.Exists(bootlistFilePath))
            {
                return
                    File.ReadAllLines(bootlistFilePath)
                        .Where(line => !line.StartsWith("#"))
                        .Select(line => line.Split(';'))
                        .Select(ParseLine);
            }

            Console.WriteLine("Couldn't find Bootlist.txt. It should exist in same dir as BootStatoil.exe");
            return null;
        }

        private static Tuple<string, string, string> ParseLine(string[] lineElements)
        {
            var exePath = lineElements[0];
            var arguments = lineElements.Length >= 2 ? lineElements[1] : string.Empty;
            var workingDir = lineElements.Length == 3 ? lineElements[2] : string.Empty;

            return new Tuple<string, string, string>(exePath, arguments, workingDir);
        }
    }
}
