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
using System.Windows.Markup;

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
        private readonly UserInteractionViewModel userInteract = new();
        private double _infoSaveWidth;
        public MainWindow()
        {
            InitializeComponent();
            foreach (BackupJobDataModel job in userInteract.BackupJobsData)
            {
                var jobString = (job.Id + 1).ToString();
                jobString += job.Name;
                jobString += job.Source;
                jobString += job.Destination;
                jobString += job.Type == 0 ? LocalizedStrings.ComplSave : LocalizedStrings.DifSave;
                BackupList.Items.Add(jobString);
            }
            EN.IsChecked = true;
            if (userInteract.LogFile.ExtLog == "json")
                json.IsChecked = true;
            else
                xml.IsChecked = true;
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

        private void ChangeLanguage(string language)
        {
            CultureInfo newCulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;
        }

        private void UpdateUI()
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
                jobString += job.Name;
                jobString += job.Source;
                jobString += job.Destination;
                jobString += job.Type == 0 ? LocalizedStrings.ComplSave : LocalizedStrings.DifSave;
                BackupList.Items[i] = jobString;
                i++;
            }
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {

        }

        private void delete_Click(object sender, RoutedEventArgs e)
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
                        error = userInteract.DeleteJobVM(BackupList.SelectedItems[i].ToString()[0] - '0' - 1);
                        BackupList.Items.RemoveAt(BackupList.SelectedItems[i].ToString()[0] - '0' - 1);
                    }
                }
            }
            UpdateUI();
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Execut_Click(object sender, RoutedEventArgs e)
        {
            List<int> jobsToExec = new List<int>();
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
    }
}
