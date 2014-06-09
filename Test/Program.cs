using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesLib;
using System.Threading;
using System.Collections;
using System.Timers;
namespace iTunesWatcher
{
    class Program
    {
        static iTunesApp myTunes = new iTunesApp(); 
        static IITLibraryPlaylist myLib;
        static Queue<string> myQueue = new Queue<string>();
        static DateTime lastChecked;
        static DateTime startTime;
        static int tracksAdded = 0;
        
        static void Main(string[] args)
        {

            myLib = myTunes.LibraryPlaylist;
            startTime = lastChecked = DateTime.Now;
            FileSystemWatcher fsw = new FileSystemWatcher(@"G:\transfers");
     
            //Set up FileSystemWatcher
            fsw.IncludeSubdirectories = true;
            fsw.Changed += new FileSystemEventHandler(fswChanged);          
            fsw.EnableRaisingEvents = true;

            System.Timers.Timer myTime = new System.Timers.Timer(10000);
            myTime.Elapsed += CheckQueue;
            myTime.Enabled = true;

            string consoleInput = Console.ReadLine();
            while (consoleInput != "quit")
            {
                if(consoleInput.ToLower() == "clear")
                {
                    Console.Clear();
                }
                else if (consoleInput.ToLower() == "stats")
                {
                    Console.WriteLine("");
                    Console.WriteLine("-------Stats-------");
                    Console.WriteLine("Uptime: " + (DateTime.Now - startTime));
                    Console.WriteLine("Tracks Added: " + tracksAdded);
                }

                consoleInput = Console.ReadLine();
            }
        }

        static void fswChanged(object sender, FileSystemEventArgs e)
        {
                
                if(!myQueue.Contains(e.FullPath) && Path.GetExtension(e.FullPath) != "m3u")
                    myQueue.Enqueue(e.FullPath);
        }

        static void CheckQueue(object sender,  ElapsedEventArgs e)
        {

            foreach (string file in myQueue.ToList())
            {
                FileInfo f = new FileInfo(file);
                f.Refresh();
                if (f.LastAccessTime < lastChecked)
                {
                    myQueue.Dequeue();
                    Console.WriteLine(@"Trying to add " + file);
                    IITOperationStatus status = ProcessFile(file);
                    if (status != null && !status.InProgress && status.Tracks.Count > 0)
                    {
                        tracksAdded++;
                        Console.WriteLine(@"Added " + status.Tracks[1].Artist + " - " + status.Tracks[1].Name);
                    }
                    
                }
            }
            lastChecked = DateTime.Now;
        }

        static IITOperationStatus ProcessFile(string filepath)
        {
            return myLib.AddFile(filepath);  
        }
        
    }
}
