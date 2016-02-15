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

            foreach (var entry in bootlist)
            {
                try
                {
                    var applicationPath = entry.Item1;
                    var applicationArguments = entry.Item2;
                    var workingDirectory = entry.Item3;

                    Console.WriteLine($"Attempting to start {applicationPath} with args: {applicationArguments}, in working directory: {workingDirectory}");

                    StartProcess(applicationPath, applicationArguments, workingDirectory);
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                }
            }

            if (args != null && args.Length == 1 && args[0] == "-noexit")
            {
                Exit();
            }
        }

        private static void StartProcess(string applicationPath, string applicationArguments, string workingDirectory)
        {
            var startInfo = new ProcessStartInfo(applicationPath)
            {
                Verb = "runas",
                UseShellExecute = true,
                Arguments = applicationArguments,
                WorkingDirectory = workingDirectory
            };

            var process = Process.Start(startInfo);

            if (process != null)
            {
                Console.WriteLine($"Process {process.Id} started for {applicationPath}");
            }
        }

        private static IEnumerable<Tuple<string, string, string>> ReadBootlist()
        {
            try
            {
                var assemblyLocation = Assembly.GetExecutingAssembly().Location;

                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

                if (assemblyDirectory == null)
                {
                    return null;
                }

                var bootlistFilePath = Path.Combine(assemblyDirectory, "Bootlist.txt");

                if (File.Exists(bootlistFilePath))
                {
                    return
                        File.ReadAllLines(bootlistFilePath)
                            .Where(line => !line.StartsWith("#") && !string.IsNullOrEmpty(line) && !string.IsNullOrWhiteSpace(line))
                            .Select(line => line.Split(';'))
                            .Select(ParseLine);
                }

                Console.WriteLine("Couldn't find Bootlist.txt. It should exist in same dir as BootAsAdmin.exe");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong when attempting to read Bootlist.txt. Ensure it exists and has the correct format: exe-path;exe-arguments;working-dir");
                WriteException(ex);
                return null;
            }
        }

        private static Tuple<string, string, string> ParseLine(string[] lineElements)
        {
            var exePath = lineElements[0];
            var exeArguments = lineElements.Length >= 2 ? lineElements[1] : string.Empty;
            var workingDirectory = lineElements.Length == 3 ? lineElements[2] : string.Empty;

            return new Tuple<string, string, string>(exePath, exeArguments, workingDirectory);
        }

        private static void Exit()
        {
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
        
        private static void WriteException(Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
