using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Watchdog
{
    public class ProcessMonitor
    {
        private static int heartbeatIntervalMs;
        private static int startupDelayMs;

        static void Main(string[] args)
        {
            try
            {
                // Get config values    
                heartbeatIntervalMs = Convert.ToInt32(ConfigurationManager.AppSettings[Constants.AppSettingsKeys.MonitorIntervalMs]);
                startupDelayMs = Convert.ToInt32(ConfigurationManager.AppSettings[Constants.AppSettingsKeys.StartupDelayMs]);

                // Ensure single watchdog process
                Process[] watchdogProcesses = Process.GetProcessesByName(Constants.Application.WatchdogProcessName);
                if (watchdogProcesses.Length > 1) Process.GetCurrentProcess().Kill();

                // Log
                LogInfoEvent($"Process watchdog has started.", 2000);

                // Startup delay
                if (startupDelayMs > 0) Thread.Sleep(startupDelayMs);

                // Spawn 'monitor' thread
                Thread watchdogThread = new Thread(new ThreadStart(Monitor));
                watchdogThread.Start();
            }
            catch (Exception e)
            {
                LogErrorEvent($"The process watchdog has caught an unhandled exception and is exiting: \n{e.Message}\n{e.InnerException}\n{e.StackTrace}", 1004);
                throw e;
            }
        }

        private static void Monitor()
        {
            while (true)
            {
                foreach (string path in Properties.Settings.Default.Applications)
                {
                    try
                    {
                        // Get process name
                        var processName = Path.GetFileNameWithoutExtension(path);

                        // Get process status and respond accordingly
                        switch (GetProcessStatus(processName))
                        {
                            case Enums.ProcessStatus.NotRunning:
                                {
                                    StartProcess(path);
                                    break;
                                }

                            case Enums.ProcessStatus.Crashed:
                            case Enums.ProcessStatus.NotResponding:
                                {
                                    KillProcesses(processName);                                        
                                    break;
                                }

                            default:
                                {
                                    // No issues, do nothing
                                    break;
                                }
                        }
                    }
                    catch (Exception e)
                    {
                        LogErrorEvent($"The process watchdog has caught an unhandled exception and is exiting: \n{e.Message}\n{e.InnerException}\n{e.StackTrace}", 1004);
                        throw e;
                    }
                }

                Thread.Sleep(heartbeatIntervalMs);
            }
        }

        private static Enums.ProcessStatus GetProcessStatus(string name)
        {
            Process[] processList = Process.GetProcessesByName(name);
            if (processList == null || processList.Length == 0) return Enums.ProcessStatus.NotRunning;
            else if (!processList[0].Responding) return Enums.ProcessStatus.NotResponding;
            else return Enums.ProcessStatus.Running;
        }

        private static void StartProcess(string path)
        {
            // Ensure application path exists
            if (!File.Exists(path)) LogErrorEvent($"Unable to start application: {path}", 1002);

            // Spin up new process
            Process process = new Process();
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.FileName = path;
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
            process.EnableRaisingEvents = true;

            // Log when process exited
            process.Exited += (object sender, EventArgs e) =>
            {
                LogErrorEvent($"The following application exited with an exit code of: {process.ExitCode}, path: {path}", 1003);
            };

            // Log
            LogInfoEvent($"Starting process: {path}", 2000);

            // Start process
            process.Start();
        }

        private static void KillProcesses(string name)
        {
            Process[] processList = Process.GetProcessesByName(name);
            foreach (Process process in processList) process.Kill();
        }

        private static void LogErrorEvent(string eventDescription, int eventCode)
        {
            try
            {
                if (!EventLog.SourceExists(Constants.Application.WatchdogProcessName))
                {
                    EventLog.CreateEventSource(Constants.Application.WatchdogProcessName, Constants.Application.LogName);
                }

                EventLog.WriteEntry(Constants.Application.WatchdogProcessName, eventDescription, EventLogEntryType.Error, eventCode);
            }
            catch (Exception)
            {
            }
        }

        private static void LogInfoEvent(string eventDescription, int eventCode)
        {
            try
            {
                if (!EventLog.SourceExists(Constants.Application.WatchdogProcessName))
                {
                    EventLog.CreateEventSource(Constants.Application.WatchdogProcessName, Constants.Application.LogName);
                }

                EventLog.WriteEntry(Constants.Application.WatchdogProcessName, eventDescription, EventLogEntryType.Information, eventCode);
            }
            catch (Exception)
            {
            }
        }
    }
}
