using System;
using System.IO;
using System.Linq;
using System.Timers;

namespace DirectorySync
{
    class SleepyFileWatcher
    {
        //class that takes source directory, destination directory and period (in seconds)
        //parameters for constructor

        //for each instance of running this application logger file is created
        //time stamped at the time this app was initialized

        string strSourcePath, strDesPath, logger;
        int period;
        StreamWriter sw;
        public SleepyFileWatcher(string SourcePath, string DestinationPath, string LogPath, int time_in_seconds)
        {
            strSourcePath = SourcePath;
            strDesPath = DestinationPath;
            if (!Directory.Exists(strSourcePath))
            {
                Console.WriteLine($"Can't find source directory {strSourcePath}");
                return;
            }
            period = time_in_seconds;
            logger = LogPath.AppendTimeStamp();
        }

        public void Watch()
        {
            //public method to run the appropriate procedures

            //--to initilize stream to write into log file information about all of the changes made
            //in destination directory (because source dir was changed)

            //-- scan and update file/folder information in the source directory
            //and apply changes into destination directory

            //-- then wait for a set period of time and do the above operations again

            // waiting is done through timer and events

            if (!Directory.Exists(strSourcePath))
            {
                Console.WriteLine($"Please provide existing source folder to mirror");
                return;
            }

            sw = File.AppendText(logger);
            ScanAndUpdateFiles();
            ScanAndUpdateFolders();
            sw.Flush();

            var aTimer = new System.Timers.Timer(1000*period);
            aTimer.Elapsed += new ElapsedEventHandler(DoWork);
            aTimer.Enabled = true;

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();

            sw.Dispose();
        }

        private void DoWork(object source, ElapsedEventArgs e)
        {
            //do methods when in the event after set period of time is passed
            ScanAndUpdateFiles();
            ScanAndUpdateFolders();
            sw.Flush();
        }
        private void ScanAndUpdateFiles()
        {
            //method to scan all of the files in the source directory
            //and miror changes to those files in the destination directory

            //if files have the same name, check date and if newer
            //replace that file in the destination direcotry

            string[] OgFiles = Directory.GetFiles(strSourcePath, "*", SearchOption.AllDirectories);
            for (int i = 0; i < OgFiles.Length; i++)
            {
                string strFileName = Path.GetFileName(OgFiles[i]);
                string strDirName = Path.GetDirectoryName(OgFiles[i]);
                string edit = strDirName.Replace(strSourcePath, "");
                string strDesFilePath = string.Format(@"{0}{1}\{2}", strDesPath, edit, strFileName);
                if (!File.Exists(strDesFilePath))
                {
                    string strDesFolderPath = string.Format(@"{0}{1}", strDesPath, edit);
                    if (!Directory.Exists(strDesFolderPath))
                    {
                        Directory.CreateDirectory(strDesFolderPath);
                    }

                    File.Copy(OgFiles[i], strDesFilePath);
                    string logger_create_file = $"Creating new file: {strDesFilePath} {DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ssfff")}";
                    sw.WriteLine(logger_create_file);
                    Console.WriteLine(logger_create_file);
                }
                else
                {
                    var source_time_stamp = File.GetCreationTime(OgFiles[i]);
                    var dist_time_stamp = File.GetCreationTime(strDesFilePath);
                    if (source_time_stamp > dist_time_stamp)
                    {
                        File.Copy(OgFiles[i], strDesFilePath, true);
                        string logger_update_file = $"Updating file: {strDesFilePath} {DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ssfff")}";
                        sw.WriteLine(logger_update_file);
                        Console.WriteLine(logger_update_file);
                    }
                }
                OgFiles[i] = strDesFilePath;
            }

            string[] DistFiles = Directory.GetFiles(strDesPath, "*", SearchOption.AllDirectories);

            var diff = DistFiles.Except(OgFiles);
            var dif_count = diff.Count();
            for (int j = 0; j < dif_count; j++)
            {
                File.Delete(diff.ElementAt(j));
                string logger_delete_file = $"Deleting file: {diff.ElementAt(j)} {DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ssfff")}";
                sw.WriteLine(logger_delete_file);
                Console.WriteLine(logger_delete_file);
            }
        }

        private void ScanAndUpdateFolders()
        {
            //method to scan all of the folders in the source directory
            //and miror changes to those folders in the destination directory
            //(for cases where there could be empty folders in source directory without files)

            string[] OgFolders = Directory.GetDirectories(strSourcePath, "*", SearchOption.AllDirectories);
            for (int i = 0; i < OgFolders.Length; i++)
            {
                string strFolderName = Path.GetDirectoryName(OgFolders[i]);
                string edit = OgFolders[i].Replace(strSourcePath, "");
                string strDesFolderPath = string.Format(@"{0}{1}", strDesPath, edit);
                OgFolders[i] = strDesFolderPath;
            }

            string[] DistFolders = Directory.GetDirectories(strDesPath, "*", SearchOption.AllDirectories);
            var diff = OgFolders.Except(DistFolders);
            var dif_count = diff.Count();
            for (int j = 0; j < dif_count; j++)
            {
                Directory.CreateDirectory(diff.ElementAt(j));
                string logger_create_folder = $"Creating new folder: {diff.ElementAt(j)} {DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ssfff")}";
                sw.WriteLine(logger_create_folder);
                Console.WriteLine(logger_create_folder);
            }

            DistFolders = Directory.GetDirectories(strDesPath, "*", SearchOption.AllDirectories);
            diff = DistFolders.Except(OgFolders);
            dif_count = diff.Count();
            for (int j = 0; j < dif_count; j++)
            {
                if (!Directory.Exists(diff.ElementAt(j)))
                {
                    continue;
                }
                Directory.Delete(diff.ElementAt(j), true);
                string logger_delete_folder = $"Deleting folder: {diff.ElementAt(j)} {DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ssfff")}";
                sw.WriteLine(logger_delete_folder);
                Console.WriteLine(logger_delete_folder);
            }
        }

    }

    public static class MyExtensions
    {
        //small extension to add time stamp to the name of the file
        public static string AppendTimeStamp(this string fileName)
        {
            return string.Concat(
                Path.GetFileNameWithoutExtension(fileName),
                DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Path.GetExtension(fileName)
                );
        }
    }
}
