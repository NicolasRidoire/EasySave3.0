using Microsoft.Win32;
using PROGRAMMATION_SYST_ME.Ressources;
using PROGRAMMATION_SYST_ME.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PROGRAMMATION_SYST_ME.View
{
    /// <summary>
    /// Interaction logic for UpdateWorkJobView.xaml
    /// </summary>
    public partial class UpdateWorkJobWindow : Window
    {
        private readonly MainWindow handleWin;
        public UpdateWorkJobViewModel ViewModel { get; set; } = new UpdateWorkJobViewModel();
        public UpdateWorkJobWindow(MainWindow handleWin)
        {
            this.handleWin = handleWin;
            InitializeComponent();
        }
        public void ChangeLang()
        {
            LabelId.Content = LocalizedStrings.LabId;
            LabelName.Content = LocalizedStrings.LabName;
            LabelSource.Content = LocalizedStrings.LabSource;
            LabelDest.Content = LocalizedStrings.LabDest;
            LabelType.Content = LocalizedStrings.LabSaveT;
            ButtonCancel.Content = LocalizedStrings.ButCancel;
            ButtonValidate.Content = LocalizedStrings.ButValidate;
        }
        public void UpdateUI()
        {
            IdTextBox.Text = (ViewModel.Id + 1).ToString();
            NameTextBox.Text = ViewModel.SaveName;
            SourceTextBox.Text = ViewModel.Source;
            DestTextBox.Text = ViewModel.Dest;
            TypeComboBox.SelectedIndex = ViewModel.Type;
        }
        public bool IsInputValid()
        {
            bool isValid = true;
            if (NameTextBox.Text == "")
            {
                isValid = false;
                NameWrongLabel.Visibility = Visibility.Visible;
            }
            if (SourceTextBox.Text == "")
            {
                isValid = false;
                SourceWrongLabel.Visibility = Visibility.Visible;
            }
            if (DestTextBox.Text == "") 
            {
                isValid = false;
                DestWrongLabel.Visibility = Visibility.Visible;
            }
            return isValid;
        }
        private void ButtonSource_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {
                Title = "Select Source Folder"
            };

            if (folderDialog.ShowDialog() is true)
            {
                SourceTextBox.Text = folderDialog.FolderName;
            }
        }
        private void ButtonDest_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {
                Title = "Select Destination Folder"
            };

            if (folderDialog.ShowDialog() == true)
            {
                DestTextBox.Text = folderDialog.FolderName;
            }
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonValidate_Click(object sender, RoutedEventArgs e)
        {
            if (!IsInputValid()){
                return;
            }

            foreach (var name in handleWin.userInteract.BackupJobsData)
            {
                bool wrongInp = false;
                if (name.Name == NameTextBox.Text)
                {
                    if (ViewModel.IsAdd)
                        wrongInp = true;
                    else
                        if (NameTextBox.Text != handleWin.userInteract.BackupJobsData[int.Parse(IdTextBox.Text) - 1].Name)
                            wrongInp = true;
                }
                if (wrongInp)
                {
                    NameWrongLabel.Visibility = Visibility.Visible;
                    NameAlreadyUsedLabel.Visibility = Visibility.Visible;
                    return;
                }
            }

            if (ViewModel.IsAdd)
            {
                handleWin.BackupList.Items.Add("");
                handleWin.userInteract.CreateJob(
                    NameTextBox.Text,
                    SourceTextBox.Text,
                    DestTextBox.Text,
                    TypeComboBox.SelectedIndex);
            }
            else
            {
                handleWin.userInteract.UpdateJob(
                    ViewModel.Id,
                    NameTextBox.Text,
                    SourceTextBox.Text,
                    DestTextBox.Text,
                    TypeComboBox.SelectedIndex);
            }
            handleWin.UpdateUI();
            Close();
        }

       
    }
}
