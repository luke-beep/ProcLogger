using System;
using System.Diagnostics;
using System.Management;

namespace ProcLogger;

/// <summary>
/// Process Logger
/// </summary>
/// <returns></returns>
internal class Program
{
    private static async Task Main(string[] args)
    {
        CIN(args);

        await LogAllProcesses(2);

        
    }

    public static void CIN(string[] args)
    {
        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }
    }

    private static async Task LogAllProcesses(int _case)
    {
        var processDict = new Dictionary<string, int>();
        var processes = new Process[Process.GetProcesses().Length];
        Process.GetProcesses().CopyTo(processes, 0);


        switch (_case)
        {
            case 0:
                await OrgProc(processDict, processes);
                break;
            case 1:
                processes = await NotOrgProc(processDict);
                break;
            case 2:
                await OrgProc(processDict, processes);
                processes = await NotOrgProc(processDict);
                break;

        }

        

        static async Task OrgProc(Dictionary<string, int> processDict, Process[] processes)
        {
            foreach (var process in processes)
            {
                if (!processDict.ContainsKey(process.ProcessName))
                {
                    processDict.Add(process.ProcessName, process.Id);
                }
            }

            foreach (var kvp in processDict)
            {
                await Task.Run(() => Console.WriteLine("B | " + DateTime.Now + " | {0} | Process ID: {1}", kvp.Key, kvp.Value));
                Task.Delay(10).Wait();

            }          
        }

        static async Task<Process[]> NotOrgProc(Dictionary<string, int> processDict)
        {
            Process[] processes;
            while (true)
            {
                var updatedProcessDict = new Dictionary<string, int>();

                processes = Process.GetProcesses();

                foreach (var process in processes)
                {
                    if (!updatedProcessDict.ContainsKey(process.ProcessName))
                    {
                        updatedProcessDict.Add(process.ProcessName, process.Id);
                    }
                }

                foreach (var kvp in updatedProcessDict)
                {
                    if (!updatedProcessDict.ContainsKey(kvp.Key))
                    {
                        Console.WriteLine("A | " + DateTime.Now + " | {0} | Process ID: {1}", kvp.Key, kvp.Value);
                    }

                }
                await Task.Delay(1000);

            }
        }
    }
}