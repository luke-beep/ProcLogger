using System.Diagnostics;
using System.Threading;

namespace ProcLogger;

internal class Program
{
    private Dictionary<string, int> processDict = new();

    private Process[] processes = new Process[Process.GetProcesses().Length];

    public Process[] Processes
    {
        get => processes;
        set => processes = value;
    }
    public Dictionary<string, int> ProcessDict
    {
        get => processDict;
        set => processDict = value;
    }

    private static async Task Main()
    {
        Dictionary<string, int> processDict = new();
        var processes = new Process[Process.GetProcesses().Length];
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Title = "Process Logger";
        await LogAllProcesses(processes, processDict);
    }



    private static async Task LogAllProcesses(Process[] Processes, Dictionary<string, int> ProcessDict)
    {
        
        Process.GetProcesses().CopyTo(Processes, 0);
        await PreviousProcesses(ProcessDict, Processes);
        await NewProcesses(ProcessDict, Processes);

        static async Task NewProcesses(Dictionary<string, int> processDict, Process[] processes)
        {
            try
            {
                while (true)
                {
                    var updatedProcessDict = new Dictionary<string, int>();
                    for (var i = 0; i < processes.Length; i++)
                    {
                        var process = processes[i];
                        if (!updatedProcessDict.ContainsKey(process.ProcessName))
                        {
                            updatedProcessDict.Add(process.ProcessName, process.Id);
                        }
                    }

                    // Display the newly created Processes
                    foreach (var kvp in updatedProcessDict)
                    {
                        var v = Process.GetProcessById(kvp.Value);
                        var handleCount = (ushort)(v.HandleCount);
                        if (!processDict.ContainsKey(kvp.Key))
                        {
                            processDict.Add(kvp.Key, kvp.Value);
                            if (handleCount > 0)
                            {
                                var ramUsage = (uint)(v.WorkingSet64 / 1000000);
                                var threads = (ushort)(v.Threads.Count);
                                if (kvp.Value > 0)
                                {
                                    Console.WriteLine(DateTime.Now + " | {0} | Process ID: {1} | Total RAM Usage: {2} mb | Total Threads: {3} | Total Handles: {4}", kvp.Key, kvp.Value, ramUsage, threads, handleCount);
                                }
                            }
                        }
                        await Task.Delay(10);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        static async Task PreviousProcesses(Dictionary<string, int> processDict, Process[] processes)
        {
            Console.WriteLine("\n");
            foreach (var process in processes)
            {
                if(process?.Id > 0)
                {
                    if (!processDict.ContainsKey(process.ProcessName))
                    {
                        processDict.Add(process.ProcessName, process.Id);
                    }
                }
            }


            using var sw = new StreamWriter(@"output.txt");
            foreach (var kvp in processDict)
            {
                if (kvp.Value > 0)
                {
                    var v = Process.GetProcessById(kvp.Value);
                    var handleCount = (ushort)(v.HandleCount);
                    if (handleCount > 0)
                    {
                        var ramUsage = (uint)(v.WorkingSet64 / 1000000);
                        var threads = (ushort)(v.Threads.Count);
                        await sw.WriteLineAsync($"{kvp.Key} | Process ID: {kvp.Value} | Total RAM Usage: {ramUsage} mb | Total Threads: {threads} | Total Handles: {handleCount}");
                        await Task.Run(() => Console.WriteLine(DateTime.Now + " | {0} | Process ID: {1} | Total RAM Usage: {2} mb | Total Threads: {3} | Total Handles: {4}", kvp.Key, kvp.Value, ramUsage, threads, handleCount));
                    }
                }
                Task.Delay(10).Wait();
            }


        }
    }
}