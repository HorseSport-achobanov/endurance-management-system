namespace Not.Process;

public static class ProcessHelper
{
    public static async void MonitorParentProcess(int parentPid)
    {
        await Task.Run(async () =>
        {
            while (true)
            {
                if (!ProcessExists(parentPid))
                {
                    Console.WriteLine($"Parent process {parentPid} has exited. Shutting down child process...");
                    Environment.Exit(0);
                }

                await Task.Delay(2000);
            }
        });
    }

    static bool ProcessExists(int pid)
    {
        return System.Diagnostics.Process.GetProcessById(pid) != null;
    }
}
