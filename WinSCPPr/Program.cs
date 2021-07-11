using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Timers;
using WinSCP;

namespace WinSCPPr
{
    internal class Program
    {
        private void OnTick(object source, ElapsedEventArgs e)
                 {
                     Console.WriteLine();
                 }
        public static void Main(string[] args)
        {
            Example e = new Example();
            e.Run();
            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "CMD.EXE";
            psi.Arguments = "/K yourmainprocess.exe";
            p.StartInfo = psi;
            p.Start();
            p.WaitForExit();
            
            //
            //  var path = ConfigurationManager.AppSettings["Folder"];
            // var files = Example.TestGetFiles(path);
            // Example.TestRenameFile(files);
            // SessionOptions sessionOptions = GetSessionOptions();
            // int error = Example.Send(sessionOptions, path);
            // if (error == 0)
            // {
            //    Example.DeleteFiles(files);
            // }
        }

       
    }
}