// Importez les espaces de noms nécessaires
using System.Threading;
using System.Windows;
using System;
using System.Globalization;  // Assurez-vous que cette ligne est présente
using System.Windows.Controls;
using System.ComponentModel;
using PROGRAMMATION_SYST_ME.Ressources;
using PROGRAMMATION_SYST_ME.ViewModel;
using PROGRAMMATION_SYST_ME.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public enum errorCode
{
    SUCCESS = 0, // Allow code to continue
    NORMAL_EXIT = 1, // Normal exit code
    INPUT_ERROR = 2, // User entered wrong input
    SOURCE_ERROR = 3, // Source File/Folder is inaccessible to the user
    BUSINESS_SOFT_LAUNCHED = 4 // Error if business software is launched while user starts backups
};

namespace PROGRAMMATION_SYST_ME.View
{
    public partial class MainWindow : Window
    {
        public errorCode error { set; get; }
        public readonly UserInteractionViewModel userInteract = new();
        private double _infoSaveWidth;
        private UpdateWorkJobView updateWind;
        public MainWindow()
        {
            Process proc = Process.GetCurrentProcess();
            int count = Process.GetProcesses().Where(p =>
                p.ProcessName == proc.ProcessName).Count();
            if (count > 1)
            {
                App.Current.Shutdown();
            }
            InitializeComponent();
            foreach (BackupJobDataModel job in userInteract.BackupJobsData)
            {
                BackupList.Items.Add("");
            }
            EN.IsChecked = true;
            if (userInteract.LogFile.ExtLog == "json")
                json.IsChecked = true;
            else
                xml.IsChecked = true;
            UpdateUI();
        }
        public double InfoSaveWidth
        {
            get { return _infoSaveWidth; }
            set
            {
                if (_infoSaveWidth != value)
                {
                    _infoSaveWidth = value;
                    OnPropertyChanged(nameof(InfoSaveWidth));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            string language = ((RadioButton)sender).Tag.ToString();

            ChangeLanguage(language);

            UpdateUI();
        }

        private static void ChangeLanguage(string language)
        {
            CultureInfo newCulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;
        }

        public void UpdateUI()
        {
            welcomeTextBlock.Text = LocalizedStrings.WelcomeMessage;
            info_save.Text = LocalizedStrings.BackupInformation;
            Execut.Content = LocalizedStrings.Execut;
            delete.Content = LocalizedStrings.Delete;
            update.Content = LocalizedStrings.Update;
            create.Content = LocalizedStrings.Create;
            language.Text = LocalizedStrings.Language;
            description.Text = LocalizedStrings.Description;
            InfoSaveWidth = Thread.CurrentThread.CurrentUICulture.Name == "fr-FR" ? 700 : 489;
            var i = 0;
            foreach (BackupJobDataModel job in userInteract.BackupJobsData)
            {
                var jobString = (job.Id + 1).ToString();
                var spacing = "    -    ";
                jobString += spacing;
                jobString += job.Name;
                jobString += spacing;
                jobString += job.Type == 0 ? LocalizedStrings.ComplSave : LocalizedStrings.DifSave;
                BackupList.Items[i] = jobString;
                i++;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            OpenUpdateWorkJobWindow(-1);
            updateWind.IsAdd = true;
        }
        private void SetBackupInfoForUpdateWin(int id)
        {
            if (id == -1)
                updateWind.Id = userInteract.BackupJobsData.Count;
            else
            {
                updateWind.Id = userInteract.BackupJobsData[id].Id;
                updateWind.SaveName = userInteract.BackupJobsData[id].Name;
                updateWind.Source = userInteract.BackupJobsData[id].Source;
                updateWind.Dest = userInteract.BackupJobsData[id].Destination;
                updateWind.Type = userInteract.BackupJobsData[id].Type;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (BackupList.SelectedItems.Count != 0)
            {
                if (MessageBox.Show("Are you sure that you want to delete ?",
                        "Delete confirmation",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    for (int i = BackupList.SelectedItems.Count - 1; i >= 0; i--)
                    {
                        var index = BackupList.SelectedItems[i].ToString()[0] - '0' - 1;
                        if (BackupList.SelectedItems[i].ToString()[0] - '0' - 1 == userInteract.BackupJobsData.Count)
                        {
                            index--;
                        }
                        error = userInteract.DeleteJobVM(index);
                        BackupList.Items.RemoveAt(index);
                    }
                }
            }
            UpdateUI();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (BackupList.SelectedIndex != -1)
            {
                OpenUpdateWorkJobWindow(BackupList.SelectedIndex);
                updateWind.IsAdd = false;
            } // Update UI of main window
        }
        private void OpenUpdateWorkJobWindow(int id)
        {
            if (!IsOpen(updateWind))
                updateWind = new UpdateWorkJobView(this);
            updateWind.Show();
            updateWind.Activate();
            SetBackupInfoForUpdateWin(id);
            updateWind.UpdateUI();
        }

        private void Execut_Click(object sender, RoutedEventArgs e)
        {
            List<int> jobsToExec = new ();
            foreach (var item in BackupList.SelectedItems)
            {
                jobsToExec.Add((item.ToString()[0] - '0') - 1);
            }
            error = userInteract.ExecuteJob(jobsToExec);
        }
        private void RadioExt_Checked(object sender, RoutedEventArgs e)
        {
            string ext = ((RadioButton)sender).Tag.ToString();
            userInteract.ChangeExtensionLog(ext);
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            updateWind?.Close();
        }
        public static bool IsOpen(Window window)
        {
            return Application.Current.Windows.Cast<Window>().Any(x => x == window);
        }
    }
}
