using System;
using System.IO;

namespace DirectorySync
{
    class EventFileWatcher
    {
        string strSourcePath, strDesPath;

        public EventFileWatcher(string SourcePath, string DestinationPath){
            strSourcePath = SourcePath;
            strDesPath = DestinationPath;
            SyncAllFilesFirstTime();
            }
        private void SyncAllFilesFirstTime()
        {
            //Get list of files from sourcepath
            string[] arrFiles = Directory.GetFiles(strSourcePath);

            foreach (string sourceFiles in arrFiles)
            {
                //get filename
                string strFileName = Path.GetFileName(sourceFiles);
                string strDesFilePath = string.Format(@"{0}\{1}", strDesPath, strFileName);
                //check whether the destination path contatins the same file
                File.Copy(sourceFiles, strDesFilePath, true);
            }
        }
        public void Watch()
        {
            using var watcher = new FileSystemWatcher(strSourcePath);

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.Filter = "*.*";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string strDesFilePath = string.Format(@"{0}\{1}", strDesPath, e.Name);
            File.Copy(e.FullPath, string.Format(@"{0}\{1}", strDesPath, e.Name), true);
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e) {

            string strDesFilePath = string.Format(@"{0}\{1}", strDesPath, e.Name);
            if (File.Exists(strDesFilePath))
            {
                File.Delete(strDesFilePath);
            }
            Console.WriteLine($"Deleted: {e.FullPath}"); }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            string strDesFilePath = string.Format(@"{0}\{1}", strDesPath, e.OldName);

            if (File.Exists(strDesFilePath))
            {
                File.Move(strDesFilePath, string.Format(@"{0}\{1}", strDesPath, e.Name));
            }
            else
            {
                File.Copy(e.FullPath, string.Format(@"{0}\{1}", strDesPath, e.Name), true);
            }

            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
        }

        private void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }
}
