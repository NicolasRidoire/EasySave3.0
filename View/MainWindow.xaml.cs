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

// JB: Renommer errorCode en ErrorCode + déplacer l'enum dans un dossier
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
        // JB: On peut avoir une propriété ErrorCode dans la ViewModel
        // Idéalement MainWindow a accès à la ViewModel et aux événements de la View (comme vous êtes en Code-Behind)
        public errorCode error { set; get; }
        public readonly UserInteractionViewModel userInteract = new();
        // JB: Pourquoi avoir un "_" ici et pas au niveau de updateWind? 
        private double _infoSaveWidth;
        // JB: En WPF, les fenêtres sont suffixées par Window => UpdateWorkJobWindow
        private UpdateWorkJobView updateWind;

        public MainWindow()
        {
            // JB: On peut avoir une méthode pour les lignes 43-50
            // (Voir une classe car ce n'est pas la responsabilité de la View de gérer les processus)
            // JB: Renommer proc en currentProcess
            Process proc = Process.GetCurrentProcess();
            // JB: On peut utiliser: int count = Process.GetProcessesByName(proc.ProcessName).Length;
            int count = Process.GetProcesses()
                .Where(p => p.ProcessName == proc.ProcessName)
                .Count();

            if (count > 1)
            {
                App.Current.Shutdown();
            }

            InitializeComponent();

            // JB: On peut avoir une méthode privée pour les lignes 55-58
            foreach (BackupJobDataModel job in userInteract.BackupJobsData)
            {
                BackupList.Items.Add("");
            }

            // JB: On peut avoir une méthode privée pour les lignes 61-66
            EN.IsChecked = true;

            if (userInteract.LogFile.ExtLog == "json")
                json.IsChecked = true;
            else
                xml.IsChecked = true;

            iconLoad.Visibility = Visibility.Hidden;

            UpdateUI();
        }
        // JB: Peut être dans la ViewModel
        // Si besoin, une ViewModel peut hériter d'une autre ViewModel OU
        // peut être composée d'une autre ViewModel
        // Se renseigner sur: Héritage vs Composition
        // De plus, si vous êtes en Code-Behind vous n'avez pas besoin d'utiliser OnPropertyChanged ;)
        // Autre point, je n'ai pas l'impression que la propriété InfoSaveWidth soit utilisée actuellement
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
            // JB: Concernant le nom des controles, c'est mieux d'avoir cette convention:
            // WelcomeTextBlock, InfoSaveTextBlock, ExecuteButton, DeleteButton etc..
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
                // JB: On peut avoir quelque chose du style:
                /*
                    var jobId = (job.Id + 1).ToString();
                    var spacing = "    -    ";
                    var backupType = job.Type == 0 ? LocalizedStrings.ComplSave : LocalizedStrings.DifSave;
                    var jobString = $"{jobId}{spacing}{job.Name}{spacing}{backupType}";
                 */
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
                    // JB: On peut avoir une méthode privée pour les lignes 190-199
                    for (int i = BackupList.SelectedItems.Count - 1; i >= 0; i--)
                    {
                        // JB: Un peu risqué le fait de mettre à jour les identifiants lors de la suppression de sauvegarde
                        // On peut se perdre au niveau des logs
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
            // JB: Ici on Update l'UI dans tous les cas?
            UpdateUI();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            // JB: SelectedIndex > -1
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

        // JB: Renommer l'event
        private void Execut_Click(object sender, RoutedEventArgs e)
        {
            // JB: Il existe une bonne pratique en programmation qui consiste
            // à éviter les "if" imbriqués pour gagner en lisibilité
            // ex:
            /*
               if (BackupList.SelectedItems.Count <= 0)
               {
                 return;
               }
              
               List<int> jobsToExec = new();
               foreach (var item in BackupList.SelectedItems)
               {
                   jobsToExec.Add((item.ToString()[0] - '0') - 1);
               }
                ...
             */
            if (BackupList.SelectedItems.Count > 0)
            {
                //JB: On peut avoir une méthode privée pour les lignes 253 - 276
                List<int> jobsToExec = new();
                foreach (var item in BackupList.SelectedItems)
                {
                    jobsToExec.Add((item.ToString()[0] - '0') - 1);
                }

                iconLoad.Visibility = Visibility.Visible;

                UpdateLayout();

                // Used to wait for iconLoad to show
                Dispatcher.Invoke(() => { error = userInteract.ExecuteJob(jobsToExec); }, DispatcherPriority.ContextIdle);
                if (error == errorCode.SUCCESS)
                {
                    MessageBox.Show(LocalizedStrings.BackupEnd, "SaveFinished",
                                MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show(LocalizedStrings.BackupError + error, "BackupProblem",
                                MessageBoxButton.YesNo);
                }

                iconLoad.Visibility = Visibility.Hidden;
            }
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
