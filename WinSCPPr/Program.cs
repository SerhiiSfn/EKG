using System;
using System.Configuration;
using System.IO;
using WinSCP;

namespace EKG01
{
  internal class Program
  {
    private static string log = "";

    public static void Main(string[] args)
    {
      try
      {
        Console.WriteLine($"time {DateTime.Now:h:mm:ss tt zz}");
        string path = ConfigurationManager.AppSettings["Folder"];

        var di = new DirectoryInfo($"{path}");
        FileInfo[] files = di.GetFiles();

        // TestRenameFile(files);

        for (var i = 0; i < files.Length; i++)
        {
          string l = $"\norig name: {files[i].Name}";
          log += l;

          string fileName = Path.GetFileNameWithoutExtension(files[i].FullName);
          string pcName = Environment.MachineName;
          if (fileName.StartsWith(pcName + "_"))
          {
            continue;
          }

          string time = DateTime.Now.ToString("yy-MM-dd_h_mm_ss_tt");
          string newNameWithExtension = $"{pcName}_{time}_{fileName}_{i}{files[i].Extension}";
          files[i].MoveTo(Path.Combine(files[i].Directory.FullName, newNameWithExtension));
        }


        string IP = ConfigurationManager.AppSettings["IP"];
        string UserName = ConfigurationManager.AppSettings["UserName"];
        string Password = ConfigurationManager.AppSettings["Password"];
        string PrivateKeyPath = ConfigurationManager.AppSettings["PrivateKeyPath"];
        string SshHostKeyFingerprint = ConfigurationManager.AppSettings["SshHostKeyFingerprint"];
        int Port = 22;
        int.TryParse(ConfigurationManager.AppSettings["Port"], out Port);

        Console.WriteLine($"IP:{IP}:{Port} => {UserName} / {new string('*', Password.Length)}");
        SessionOptions sessionOptions = new SessionOptions
        {
          PortNumber = Port,
          Protocol = Protocol.Sftp,
          HostName = IP,
          UserName = UserName,
          Password = Password,
          SshHostKeyFingerprint = SshHostKeyFingerprint,
          SshPrivateKeyPath = PrivateKeyPath,
        };
        // return result;

        //SessionOptions sessionOptions = result;
        bool sendSuccessful = false;

        try
        {
          Session session = new Session();

          // Connect
          session.Open(sessionOptions);
          session.PutFiles(@path + "\\*", "/sftptest/1231/*").Check();

          sendSuccessful = true;
        }
        catch (Exception e)
        {
          Console.WriteLine("Error: {0}", e);

          sendSuccessful = false;
        }

        if (sendSuccessful == true)
        {
          for (int i = 0; i < files.Length; i++)
          {
            files[i].Delete();
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      finally
      {
        Console.WriteLine("Done");
        string fileName = $@"C:\Temp\Log{DateTime.Now.ToString("yy-MM-dd_h_mm_ss_tt")}.txt";

        try
        {
          // Check if file already exists. If yes, delete it.     
          if (File.Exists(fileName))
          {
            File.Delete(fileName);
          }
          // Create a new file     
          using (StreamWriter sw = File.CreateText(fileName))
          {
            sw.WriteLine("New file created: {0}", DateTime.Now.ToString());
            sw.WriteLine(log);
          }
          // Write file contents on console.     
          using (StreamReader sr = File.OpenText(fileName))
          {
            string s = "";
            while ((s = sr.ReadLine()) != null)
            {
              Console.WriteLine(s);
            }
          }
        }
        catch (Exception Ex)
        {
          Console.WriteLine(Ex.ToString());
        }
      }
    }
  }
}