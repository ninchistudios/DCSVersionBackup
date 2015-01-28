using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Threading;
using System.IO;
using Microsoft.Win32;

namespace DCSBackupTool
{
     public partial class MainWindow : Window
    {
        private string dCSBackupToolSubKey = "SOFTWARE\\DCSBackupTool\\Settings";
        private RegistryKey baseRegistryKey = Registry.CurrentUser;
           
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Backup_Click(object sender, RoutedEventArgs e)
        {
            Thread trd = new Thread(new ThreadStart(ThreadBackupFolders));
            trd.IsBackground = true;
            trd.Start();
        }

        private void ThreadBackupFolders()
        {
            //start time
            string backupLocations = null;
            int result = Environment.TickCount & Int32.MaxValue;
            StringBuilder textOut = new StringBuilder();
            textOut.Append("Starting Backup\n");
            SetProgressBar(true);

            try
            {
                string backupPath = RegistryManipulator.ReadRegistry(this.baseRegistryKey, 
                        this.dCSBackupToolSubKey, "BackupPath");

                if (!Directory.Exists(backupPath))
                {
                    SetProgressBar(false);
                    throw new ApplicationException("Backup location " + backupPath + " does not exist");
                }

                List<string> foldersToBackup = new List<string>(); 
                foldersToBackup.Add(RegistryManipulator.ReadRegistry(baseRegistryKey, dCSBackupToolSubKey, "SavedGames"));
                foldersToBackup.Add(RegistryManipulator.ReadRegistry(this.baseRegistryKey, this.dCSBackupToolSubKey, "DCS World"));
                foldersToBackup.Add(RegistryManipulator.ReadRegistry(this.baseRegistryKey, this.dCSBackupToolSubKey, "Helios"));
                foldersToBackup.Add(RegistryManipulator.ReadRegistry(this.baseRegistryKey, this.dCSBackupToolSubKey, "Jsgme"));

                foreach (string fol in foldersToBackup)
                {
                    if (fol != null)
                    {
                        //check each folder exists before copying
                        if (Directory.Exists(fol))
                        {
                            //get the name of the folder from the original path
                            string dirName = new DirectoryInfo(fol).Name;
                            //append the directory name to the backuplocation
                            backupLocations = backupPath + "\\" + dirName;
                            //check for existing backup folder if so delete
                            if (Directory.Exists(backupLocations))
                            {
                                //If you have the specified directory open in File Explorer 
                                //the Delete method may not be able to delete it
                                Directory.Delete(backupLocations, true);
                                textOut.Append(backupLocations + " deleted\n");
                                WriteToTextBlock(textOut.ToString());
                            }
                            CopyDirectory(fol, backupLocations, true);
                            textOut.Append(fol + " backed up\n");
                            WriteToTextBlock(textOut.ToString());
                        }
                        else
                        {
                            SetProgressBar(false);
                            throw new ApplicationException(fol + " does not exist");
                        }
                    }
                }

                //finish time
                SetProgressBar(false);
                int result2 = Environment.TickCount & Int32.MaxValue;
                string timeTaken = ((result2 - result) / 1000).ToString();
                textOut.Append("Copy took " + timeTaken + " seconds");
                WriteToTextBlock(textOut.ToString());           
            }
            catch (Exception oError)
            {
                WriteToTextBlock(oError.Message);
            }     
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            Settings mySet = new Settings();
            mySet.Show();
        }

        private void WriteToTextBlock(string text)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(
                new Action(() => this.outputTextblock.Text = text));
        }

         private void SetProgressBar(bool b)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(
               new Action(() => this.pbStatus.IsIndeterminate = b));
        }

        private static bool CopyDirectory(string SourcePath, string DestinationPath, bool overwriteexisting)
        {
            bool ret = true;
            try
            {
                SourcePath = SourcePath.EndsWith(@"\") ? SourcePath : SourcePath + @"\";
                DestinationPath = DestinationPath.EndsWith(@"\") ? DestinationPath : DestinationPath + @"\";

                if (Directory.Exists(SourcePath))
                {
                    if (Directory.Exists(DestinationPath) == false)
                        Directory.CreateDirectory(DestinationPath);

                    foreach (string fls in Directory.GetFiles(SourcePath))
                    {
                        FileInfo flinfo = new FileInfo(fls);
                        flinfo.CopyTo(DestinationPath + flinfo.Name, overwriteexisting);
                    }
                    foreach (string drs in Directory.GetDirectories(SourcePath))
                    {
                        DirectoryInfo drinfo = new DirectoryInfo(drs);
                        if (CopyDirectory(drs, DestinationPath + drinfo.Name, overwriteexisting) == false)
                            ret = false;
                    }
                }
                else
                {
                    ret = false;
                }
            }
            catch
            {
                ret = false;
            }
            return ret;
        }
    }
}
