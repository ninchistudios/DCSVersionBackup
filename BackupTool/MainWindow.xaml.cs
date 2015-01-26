using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BackupTool
{
     public partial class MainWindow : Window
    {
        //fields
        private const string backupFolder = @"e:\DCSBU";
        private string[] dcsFolders = new string[] { 
        @"d:\Users\admin\Saved Games\DCS", //dcsfolder   
        @"d:\users\admin\documents\Helios", //helios folder
        @"F:\DCS\program\DCS World", //dcs world folder
        @"d:\DCS_JSGME"};  //jsgme folder n
           
        public MainWindow()
        {
            InitializeComponent();

        }

        private void WriteToTextBlock(string text)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(
                new Action(() => this.outputTextblock.Text = text));
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
            catch (Exception ex)
            {
                ret = false;
            }
            return ret;
        }

         //============================================================
        private void ThreadCopyFolders()
        {
            //start time
            int result = Environment.TickCount & Int32.MaxValue;
            string textOut = "";
            try
            {
                string backupLocations;
                //check destination exists
                if (!Directory.Exists(backupFolder))
                {
                    throw new ApplicationException(backupFolder + " does not exist");
                }
                foreach (string fol in this.dcsFolders)
                {
                    //check each folder exists before copying
                    if (Directory.Exists(fol))
                    {
                        //get the name of the folder from the original path
                        string dirName = new DirectoryInfo(fol).Name;
                        //append the directory name to the backuplocation
                        backupLocations = backupFolder + "\\" + dirName;
                        //check for existing backup folder if so delete
                        if (Directory.Exists(backupLocations))
                        {
                            //If you have the specified directory open in File Explorer, 
                            //the Delete method may not be able to delete it
                            Directory.Delete(backupLocations,true);
                            textOut = backupLocations + " deleted\n";
                            WriteToTextBlock(textOut);
                        }
                        CopyDirectory(fol, backupLocations, true);
                        textOut += fol + " backed up\n";
                        WriteToTextBlock(textOut);
                    }
                    else
                    {
                        throw new ApplicationException(fol + " does not exist");
                    }
                }
                //finish time
                int result2 = Environment.TickCount & Int32.MaxValue;
                string timeTaken = ((result2 - result) / 1000).ToString();
                textOut += "Copy took " + timeTaken + " seconds";
                WriteToTextBlock(textOut);           
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

        private void Restore_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Backup_Click(object sender, RoutedEventArgs e)
        {
            // 2nd thread to run our copy so UI wont hang
            Thread trd = new Thread(new ThreadStart(ThreadCopyFolders));
            trd.IsBackground = true;
            trd.Start();
        }


        
    }
}
