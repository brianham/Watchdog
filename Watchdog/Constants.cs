namespace Watchdog
{
    public static class Constants
    {
        public static class Application
        {
            public static readonly string WatchdogProcessName = "ProcessWatchdog";
            public static readonly string WatchdogLock = "WatchdogLock";
            public static readonly string LogName = "Application";
        }

        public static class AppSettingsKeys
        {
            public static readonly string MonitorIntervalMs = "MonitorIntervalMs";
            public static readonly string StartupDelayMs = "StartupDelayMs";            
        }
    }
}
