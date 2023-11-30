// Importez les espaces de noms nécessaires
using System.Threading;
using System.Windows;

using System;
using System.Globalization;  // Assurez-vous que cette ligne est présente
using System.Windows.Controls;
using System.ComponentModel;

namespace EasySaveV2._0
{
    public partial class MainWindow : Window
    {
        private double _infoSaveWidth;
        public MainWindow()
        {
            
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
            InfoSaveWidth = (Thread.CurrentThread.CurrentUICulture.Name == "fr-FR") ? 700 : 489;
        }
    }
}
