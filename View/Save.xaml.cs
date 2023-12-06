using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PROGRAMMATION_SYST_ME.View
{
    /// <summary>
    /// Interaction logic for Save.xaml
    /// </summary>
    public partial class Save : Window
    {
        // JB: Ajouter un "_" pour les variables privées
        private readonly MainWindow Mhandle;
        private readonly List<int> jobs;
        public Save(MainWindow handleMain, List<int> jobsToExec)
        {
            this.jobs = jobsToExec;
            this.Mhandle = handleMain;
            InitializeComponent();
        }
        
    }
}
