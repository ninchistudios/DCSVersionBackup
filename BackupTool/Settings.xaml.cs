using System;
using System.Windows;
using Microsoft.Win32;

namespace DCSBackupTool
{
    public partial class Settings : Window
    {
        private RegistryKey baseRegistryKey = Registry.CurrentUser;
        private string dCSBackupToolSubKey = "SOFTWARE\\DCSBackupTool\\Settings";
        private string eDPathSubKey = "SOFTWARE\\Eagle Dynamics\\DCS World";
        private string usersBackupPath;
        private string usersHomePath;
        private string usersSavedGames;
        private string usersDCSworldPath;
        private string usersHeliosPath;
        private string usersJsgmePath;

        public Settings()
        {
            InitializeComponent();
            GetSettingsValues();
        }

        private void GetSettingsValues()
        {
            //get backup location
            this.usersBackupPath = RegistryManipulator.ReadRegistry(this.baseRegistryKey, this.dCSBackupToolSubKey, "BackupPath");
            if (this.usersBackupPath == null)
            {
                BackupLocationText.Text = "Select a backup location";
            }
            else
            {
                BackupLocationText.Text = this.usersBackupPath;
            }

            //get saved games
            this.usersSavedGames = RegistryManipulator.ReadRegistry(baseRegistryKey, dCSBackupToolSubKey, "SavedGames");
            if (this.usersSavedGames == null)
            {
                this.usersHomePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string dcsSavedGames = this.usersHomePath + "\\Saved Games\\DCS";
                this.usersSavedGames = dcsSavedGames;
                savedGamesText.Text = dcsSavedGames;
            }
            else
            {
                savedGamesText.Text = this.usersSavedGames;
            }

            //get dcsWorld location my setting first if in registry
            this.usersDCSworldPath = RegistryManipulator.ReadRegistry(this.baseRegistryKey, this.dCSBackupToolSubKey, "DCS World");
            if (this.usersDCSworldPath == null)
            {
                //get eagle dynamics setting
                this.usersDCSworldPath = RegistryManipulator.ReadRegistry(this.baseRegistryKey, this.eDPathSubKey, "Path");
            }
            if (this.usersDCSworldPath != null)
            {
                DCSWorldText.Text = this.usersDCSworldPath;
            }
            else
            {
                DCSWorldText.Text = "Can not find DCS. Enter path to DCS";
            }

            //get helios path
            this.usersHeliosPath = RegistryManipulator.ReadRegistry(this.baseRegistryKey, this.dCSBackupToolSubKey, "Helios");
            if (this.usersHeliosPath == null)
            {
                //usual helios path
                HeliosText.Text = "If installed choose location";
                HeliosText.ToolTip = "Usual path is " + this.usersHomePath + "\\Documents\\Helios";
            }
            else
            { 
                HeliosText.Text = this.usersHeliosPath;
            }
            //get jsgme path
            this.usersJsgmePath = RegistryManipulator.ReadRegistry(this.baseRegistryKey, this.dCSBackupToolSubKey, "Jsgme");
            if (this.usersJsgmePath == null)
            {
                JsgmeText.ToolTip = "Select path for JSGME folder if your using one";
            }
            else 
            {
                JsgmeText.Text = this.usersJsgmePath;
            }
        }

        private void BackupLocation_Button_Click(object sender, RoutedEventArgs e)
        {
            this.usersBackupPath = GetPathFromUser();
            if(this.usersBackupPath != "")
            { 
                BackupLocationText.Text = this.usersBackupPath;
            }
        }

        private void Helios_Click(object sender, RoutedEventArgs e)
        {
            this.usersHeliosPath = GetPathFromUser();
            if (this.usersHeliosPath != "")
            {
                HeliosText.Text = this.usersHeliosPath;
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

        private void SavedGames_Click(object sender, RoutedEventArgs e)
        {
            this.usersSavedGames = GetPathFromUser();
            if (this.usersSavedGames != "")
            {
                savedGamesText.Text = this.usersSavedGames;
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

        private void Jsgme_Click(object sender, RoutedEventArgs e)
        {
            this.usersJsgmePath = GetPathFromUser();
            if (this.usersJsgmePath != "")
            {
                JsgmeText.Text = this.usersJsgmePath;
            }
        }

        private void Set_Click(object sender, RoutedEventArgs e)
        {
            RegistryManipulator.WriteRegistry(baseRegistryKey, dCSBackupToolSubKey, "BackupPath", this.usersBackupPath);
            RegistryManipulator.WriteRegistry(baseRegistryKey, dCSBackupToolSubKey, "SavedGames", this.usersSavedGames);
            RegistryManipulator.WriteRegistry(baseRegistryKey, dCSBackupToolSubKey, "DCS World", this.usersDCSworldPath);
            RegistryManipulator.WriteRegistry(baseRegistryKey, dCSBackupToolSubKey, "Helios", this.usersHeliosPath);
            RegistryManipulator.WriteRegistry(baseRegistryKey, dCSBackupToolSubKey, "Jsgme", this.usersJsgmePath);
            this.Close();
        }

        private void CloseSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
