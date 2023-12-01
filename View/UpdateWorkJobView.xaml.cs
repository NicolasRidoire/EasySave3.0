using PROGRAMMATION_SYST_ME.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace PROGRAMMATION_SYST_ME.View
{
    /// <summary>
    /// Interaction logic for UpdateWorkJobView.xaml
    /// </summary>
    public partial class UpdateWorkJobView : Window
    {
        private readonly MainWindow handleWin;
        public int Id { get; set; }
        public string SaveName { get; set; }
        public string Source { get; set; }
        public string Dest { get; set; }
        public int Type { get; set; }
        public bool IsAdd { get; set; }
        public UpdateWorkJobView(MainWindow handleWin)
        {
            this.handleWin = handleWin;
            InitializeComponent();
        }
        public void UpdateUI()
        {
            BoxId.Text = (Id + 1).ToString();
            BoxName.Text = SaveName;
            BoxSource.Text = Source;
            BoxDest.Text = Dest;
            ComboType.SelectedIndex = Type;
        }
        private void ButtonSource_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonValidate_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdd)
            {
                handleWin.BackupList.Items.Add("");
                handleWin.userInteract.CreateJob(
                    BoxName.Text,
                    BoxSource.Text,
                    BoxDest.Text,
                    ComboType.SelectedIndex);
            }
            else
            {
                handleWin.userInteract.UpdateJob(
                    Id,
                    BoxName.Text,
                    BoxSource.Text,
                    BoxDest.Text,
                    ComboType.SelectedIndex);
            }
            handleWin.UpdateUI();
            Close();
        }
    }
}
