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
using System.Windows.Threading;

namespace PROGRAMMATION_SYST_ME.View
{
    public partial class MainWindow : Window
    {
        public ErrorCode Error { set; get; }
        public readonly MainWindowViewModel userInteract = new();
        private UpdateWorkJobWindow updateWind;
        public Save SaveWin {  set; get; }
        public MainWindow()
        {
            Process currentProcess = Process.GetCurrentProcess();
            int count = Process.GetProcessesByName(currentProcess.ProcessName).Length;

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
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;
            string language = radioButton.Tag.ToString();

            ChangeLanguage(language);

            if (IsOpen(updateWind))
                updateWind.ChangeLang();

            UpdateUI();
        }

        private static void ChangeLanguage(string language)
        {
            // JB: C'est très bien d'avoir changé la langue de cette façon! :) 
            CultureInfo newCulture = new(language);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;
        }

        public void UpdateUI()
        {
            welcomeTextBlock.Text = LocalizedStrings.WelcomeMessage;
            InfoSaveTextBlock.Text = LocalizedStrings.BackupInformation;
            ExecuteButton.Content = LocalizedStrings.Execut;
            DeleteButton.Content = LocalizedStrings.Delete;
            UpdateButton.Content = LocalizedStrings.Update;
            CreateButton.Content = LocalizedStrings.Create;
            LanguageTextBlock.Text = LocalizedStrings.Language;
            DescriptionTextBlock.Text = LocalizedStrings.Description;

            var i = 0;
            foreach (BackupJobDataModel job in userInteract.BackupJobsData)
            {
                var jobId = (job.Id + 1).ToString();
                var spacing = "    -    ";
                var backupType = job.Type == 0 ? LocalizedStrings.ComplSave : LocalizedStrings.DifSave;
                var jobString = $"{jobId}{spacing}{job.Name}{spacing}{backupType}";

                BackupList.Items[i] = jobString;
                i++;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            OpenUpdateWorkJobWindow(-1);

            updateWind.ViewModel.IsAdd = true;
        }

        private void SetBackupInfoForUpdateWin(int id)
        {
            if (id == -1)
                updateWind.ViewModel.Id = userInteract.BackupJobsData.Count;
            else
            {
                updateWind.ViewModel.Id = userInteract.BackupJobsData[id].Id;
                updateWind.ViewModel.SaveName = userInteract.BackupJobsData[id].Name;
                updateWind.ViewModel.Source = userInteract.BackupJobsData[id].Source;
                updateWind.ViewModel.Dest = userInteract.BackupJobsData[id].Destination;
                updateWind.ViewModel.Type = userInteract.BackupJobsData[id].Type;
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
                    DeleteItems();
                }
                UpdateUI();
            }            
        }
        private void DeleteItems()
        {
            for (int i = BackupList.SelectedItems.Count - 1; i >= 0; i--)
            {
                var index = BackupList.SelectedItems[i].ToString()[0] - '0' - 1;
                if (BackupList.SelectedItems[i].ToString()[0] - '0' - 1 == userInteract.BackupJobsData.Count)
                {
                    index--;
                }
                Error = userInteract.DeleteJobVM(index);
                BackupList.Items.RemoveAt(index);
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (BackupList.SelectedIndex > -1)
            {
                OpenUpdateWorkJobWindow(BackupList.SelectedIndex);

                updateWind.ViewModel.IsAdd = false;
            } // Update UI of main window
        }
        private void OpenUpdateWorkJobWindow(int id)
        {
            if (!IsOpen(updateWind))
                updateWind = new UpdateWorkJobWindow(this);

            updateWind.Show();
            updateWind.Activate();
            SetBackupInfoForUpdateWin(id);
            updateWind.UpdateUI();
        }
        private void Execut_Click(object sender, RoutedEventArgs e)
        {
            if (BackupList.SelectedItems.Count <= 0 || SaveWin != null || userInteract.IsSaving)
            {
                return;
            }
            userInteract.IsSetup = false;
            var msboxAnswer = MessageBox.Show(LocalizedStrings.Crypt, "IsSaveCrypted", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (msboxAnswer == MessageBoxResult.Yes)
                userInteract.IsCrypt = true;
            else if (msboxAnswer == MessageBoxResult.No)
                userInteract.IsCrypt = false;
            else
                return;

            List<int> jobsToExec = new();
            foreach (var item in BackupList.SelectedItems)
            {
                jobsToExec.Add((item.ToString()[0] - '0') - 1);
            }

            //thread et lancer la page

            Thread thread = new Thread (() => Error = userInteract.ExecuteJob(jobsToExec));
            thread.Start();
            //instance de la classe qui contient la page
            var saveWinThread = new Thread(() =>
            {
                SaveWin = new Save(this, jobsToExec);
                System.Windows.Threading.Dispatcher.Run();
            });
            saveWinThread.SetApartmentState(ApartmentState.STA);
            saveWinThread.IsBackground = true;
            saveWinThread.Start();
        }
        private void RadioExt_Checked(object sender, RoutedEventArgs e)
        {
            string ext = ((RadioButton)sender).Tag.ToString();
            userInteract.ChangeExtensionLog(ext);
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }
        public static bool IsOpen(Window window)
        {
            return Application.Current.Windows.Cast<Window>().Any(x => x == window);
        }
    }
}
