using System;
using System.Windows;
using Microsoft.Win32;

namespace BackupTool
{
    /// <summary>
    /// Get the settings from the user and set in registry keys
    /// </summary>
    public partial class Settings : Window
    {
        private string DCSBackupToolsubKey = "SOFTWARE\\DCSBackupTool\\Settings";
        private string EDPathsubKey = "SOFTWARE\\Eagle Dynamics\\DCS World";
        private RegistryKey baseRegistryKey = Registry.CurrentUser;
        private string userBackupPath;
        private string userHomePath;
        private string userSavedGames;
        private string usersDCSworldPath;

        //todo
        //@"d:\users\admin\documents\Helios", //helios folder
        //@"F:\DCS\program\DCS World", //dcs world folder
        //@"d:\DCS_JSGME"};  //jsgme folder n

        public Settings()
        {
            InitializeComponent();
            GetSettingsValues();
        }

        private void GetSettingsValues()
        {
            //get backup location
            this.userBackupPath = RegistryManipulator.ReadRegistry(this.baseRegistryKey, this.DCSBackupToolsubKey, "BackupPath");
            if (this.userBackupPath == null)
            {
                BackupLocationText.Text = "Enter a backup location";
            }
            else
            {
                BackupLocationText.Text = this.userBackupPath;
            }

            //get saved games
            this.userSavedGames = RegistryManipulator.ReadRegistry(baseRegistryKey, DCSBackupToolsubKey, "SavedGames");
            if (this.userSavedGames == null)
            {
                userHomePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string dcsSavedGames = userHomePath + "\\Saved Games\\DCS";
                this.userSavedGames = dcsSavedGames;
                savedGames.Text = dcsSavedGames;
            }
            else
            {
                savedGames.Text = this.userSavedGames;
            }

            //get dcsWorld /// redo this causing exception to do
            //my setting
            this.usersDCSworldPath = RegistryManipulator.ReadRegistry(this.baseRegistryKey, this.DCSBackupToolsubKey, "DCS World");

            if (this.usersDCSworldPath == null)
            {
                //get eagle dynamics setting
                this.usersDCSworldPath = RegistryManipulator.ReadRegistry(this.baseRegistryKey, this.EDPathsubKey, "Path");
            }

            if (this.usersDCSworldPath != null)
            {
                DCSWorldText.Text = this.usersDCSworldPath;
            }
            else
            {
                DCSWorldText.Text = "Can not find DCS. Enter path to DCS";
            }
        }

        private void BackupLocation_Button_Click(object sender, RoutedEventArgs e)
        {
            this.userBackupPath = GetPathFromUser();
            if(this.userBackupPath != "")
            { 
                BackupLocationText.Text = this.userBackupPath;
            }
        }

        private string GetPathFromUser()
        {
            //using System.Windows.Forms;
            string folderPath = "";
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result.ToString() == "OK")
            {
                folderPath = dialog.SelectedPath;
            }
            return folderPath;
        }

        private void Set_Click(object sender, RoutedEventArgs e)
        {
            RegistryManipulator.WriteRegistry(baseRegistryKey, DCSBackupToolsubKey, "BackupPath", this.userBackupPath);
            RegistryManipulator.WriteRegistry(baseRegistryKey, DCSBackupToolsubKey, "SavedGames", this.userSavedGames);
            RegistryManipulator.WriteRegistry(baseRegistryKey, DCSBackupToolsubKey, "DCS World", this.usersDCSworldPath);
            this.Close();
        }

        private void CloseSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SavedGames_Click(object sender, RoutedEventArgs e)
        {
            this.userSavedGames = GetPathFromUser();
            if (this.userSavedGames != "")
            {
                savedGames.Text = this.userSavedGames;
            }
        }

        private void DCSWorld_Click(object sender, RoutedEventArgs e)
        {
            this.usersDCSworldPath = GetPathFromUser();
            if (this.usersDCSworldPath != "")
            {
                DCSWorldText.Text = this.usersDCSworldPath;
            }
        }

    }

}
