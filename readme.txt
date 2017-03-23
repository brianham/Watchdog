************************************************************************
Configuration
************************************************************************
1) Add write permissions to following registry key for user account 
HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\EventLog

2) Set Process Watchdog as Windows Shell replacement for user account
HKEY_CURRENT_USER\Software\Microsoft\Windows NT\CurrentVersion\Winlogon

************************************************************************
Notes
************************************************************************
Supress crashing application error window
https://blogs.msdn.microsoft.com/oldnewthing/20040727-00/?p=38323
http://stackoverflow.com/questions/3561545/how-to-terminate-a-program-when-it-crashes-which-should-just-fail-a-unit-test/3637710#3637710