using System;
using System.Configuration;
using WinSCP;
using System.IO;
using System.Timers;

namespace WinSCPPr
{
    public static class FileInfoExtensions
    {
        public static void Rename(this FileInfo fileInfo, string newName)
        {
            fileInfo.MoveTo(Path.Combine(fileInfo.Directory.FullName, newName));
        }

        public static void RenameWithoutExtension(this FileInfo fileInfo, string newNameWithoutExtension)
        {
            var name1 = Path.GetPathRoot(fileInfo.FullName);
            // fileInfo.MoveTo(Path.Combine(fileInfo.Directory.FullName, newNameWithoutExtension+fileInfo.Extension));
        }
    }

    class Example
    {
        private bool _runProcces = false;
        private static string log = "";
        public static FileInfo[] TestGetFiles(string path)
        {
            //string dirName = "C:\\Users\\Ma\\Documents\\1\\";
            var di = new DirectoryInfo($"{path}");
            return di.GetFiles();
        }

        public static void TestRenameFile(params FileInfo[] filepath)
        {
            //FileInfo file = new FileInfo(@"C:\Users\Ma\Documents\SlugaNarodu.txt");
            //file.Rename("SlugaNarodu2.txt");

            for (var i = 0; i < filepath.Length; i++)
            {
                string l = $"\norig name: {filepath[i].Name}" ;
                log += l;
                //Console.WriteLine(l);

                var fileName = Path.GetFileNameWithoutExtension(filepath[i].FullName);
                var pcName = Environment.MachineName;
                if (fileName.StartsWith(pcName + "_"))
                {
                    continue;
                }

                var time = DateTime.Now.ToString("yy-MM-dd_h_mm_ss_tt");
                string newNameWithExtension = $"{pcName}_{time}_{fileName}_{i}{filepath[i].Extension}";
                filepath[i].Rename(newNameWithExtension);
            }
        }

        public static int Send(SessionOptions sessionOptions, FileInfo file)
        {
            try
            {
                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);
                    session.MoveFile(file.FullName, "/sftptest/1231/*");
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                return 1;
            }
        }

        public static int Send(SessionOptions sessionOptions, string path)
        {
            try
            {
                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);


                    // Передать файлы
                    // session.PutFiles(@"C:\Users\Ma\Documents\SlugaNarodu.txt", "/sftptest/1231/*").Check();
                    session.PutFiles(@path + "\\*", "/sftptest/1231/*").Check();
                    //session.MoveFile(@path+"\\*", "/sftptest/1231/*");

                    //
                    // // Upload files
                    // TransferOptions transferOptions = new TransferOptions();
                    // transferOptions.TransferMode = TransferMode.Binary;
                    //
                    // TransferOperationResult transferResult;
                    // transferResult =
                    //     session.PutFiles(@"d:\toupload\*", "/home/user/", false, transferOptions);
                    //
                    // // Throw on any error
                    // transferResult.Check();
                    //
                    // // Print results
                    // foreach (TransferEventArgs transfer in transferResult.Transfers)
                    // {
                    //     Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                    // }
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                return 1;
            }
        }

        public static void DeleteFiles(FileInfo[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                files[i].Delete();
            }
        }

        public void Run()
        {
            Console.WriteLine("run");
            // int delay = 1;
            // int.TryParse(ConfigurationManager.AppSettings["Delay"], out delay );
            //
            // Timer timer = new Timer(delay*1000);
            // timer.Elapsed += OnTick;
            // Console.WriteLine("done01");
            // timer.Start();
            OnTick(null, null);
        }

        private void OnTick(object sender, ElapsedEventArgs e)
        {
            if (_runProcces == false)
            {
                _runProcces = true;
                try
                {
                    Console.WriteLine($"time {DateTime.Now:h:mm:ss tt zz}");
                    var path = ConfigurationManager.AppSettings["Folder"];
                    var files = TestGetFiles(path);
                    TestRenameFile(files);
                    SessionOptions sessionOptions = GetSessionOptions();
                    int error = Send(sessionOptions, path);
                    if (error == 0)
                    {
                        DeleteFiles(files);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"error send {exception.Message}");
                }
                finally
                {
                    _runProcces = false;
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
            else
            {
                Console.WriteLine("already started");
            }
        }

        private static SessionOptions GetSessionOptions()
        {
            string IP = ConfigurationManager.AppSettings["IP"];
            string UserName = ConfigurationManager.AppSettings["UserName"];
            string Password = ConfigurationManager.AppSettings["Password"];
            string PrivateKeyPath = ConfigurationManager.AppSettings["PrivateKeyPath"];
            string SshHostKeyFingerprint = ConfigurationManager.AppSettings["SshHostKeyFingerprint"];
            int Port = 22;
            int.TryParse(ConfigurationManager.AppSettings["Port"], out Port);

            Console.WriteLine($"IP:{IP}:{Port} => {UserName} / {new string('*', Password.Length)}");
            var result = new SessionOptions
            {
                PortNumber = Port,
                Protocol = Protocol.Sftp,
                HostName = IP,
                UserName = UserName,
                Password = Password,
                SshHostKeyFingerprint = SshHostKeyFingerprint,
                SshPrivateKeyPath = PrivateKeyPath,
            };
            return result;
        }
    }
}