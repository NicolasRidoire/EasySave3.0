using PROGRAMMATION_SYST_ME.Ressources;
using PROGRAMMATION_SYST_ME.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
    public partial class SaveWindow : Window
    {
        private readonly MainWindow mhandle;
        private readonly List<int> jobs;
        private SaveWindowViewModel viewModel = new();
        private List<Item> itemList = new();
        public bool End { set; get; } = false;
        public SaveWindow(MainWindow handleMain, List<int> jobsToExec)
        {
            jobs = jobsToExec;
            mhandle = handleMain;
            InitializeComponent();
            ProgressListView.Items.Clear();
            while (!mhandle.userInteract.IsSetup) { Thread.Sleep(100); }
            int i = 0;
            foreach (var job in jobs)
            {
                int pro = (int)(mhandle.userInteract.RealTimeData[i].Progression * 100);
                itemList.Add(new Item
                {
                    Id = i,
                    Name = mhandle.userInteract.BackupJobsData[job].Name,
                    Progr = pro,
                    ProgrStr = pro.ToString() + " %",
                    Status = mhandle.userInteract.RealTimeData[i].State
                });
                ProgressListView.Items.Add(itemList[i]);
                viewModel.SendInfoToSocket(itemList);
                i++;
            }
            Thread backThread = new Thread(() =>
            {
                while (!End)
                {
                    int y = 0;
                    bool canEnd = true;
                    foreach (var job in jobs)
                    {
                        int pro = (int)mhandle.userInteract.RealTimeData[y].Progression;
                        if ((pro == 100 && canEnd) || mhandle.userInteract.ThreadStop[y])
                            End = true;
                        else
                        {
                            End = false;
                            canEnd = false;
                        }
                        itemList[y] = new Item
                        {
                            Id = y,
                            Name = mhandle.userInteract.BackupJobsData[job].Name,
                            Progr = pro,
                            ProgrStr = pro.ToString() + " %",
                            Status = mhandle.userInteract.RealTimeData[y].State
                        };
                        this.Dispatcher.Invoke(() => ProgressListView.Items[y] = itemList[y]);
                        viewModel.SendInfoToSocket(itemList);
                        y++;
                    }
                    Thread.Sleep(100);
                }
            });
            backThread.Start();

            Show();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!End)
            {
                e.Cancel = true;
                return;
            }
            itemList.Clear();
            viewModel.StopConnexion = true;
            viewModel.serv.Disconnect(viewModel.Connected);
            viewModel.socket1.Dispose();
            mhandle.SaveWin = null;
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            if (End)
                return;
            int id = (int)((Button)sender).Tag;
            mhandle.userInteract.ThreadPause[id] = true;
            mhandle.userInteract.RealTimeData[id].State = "PAUSED";
            UpdateRealTime();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (End)
                return;
            int id = (int)((Button)sender).Tag;
            mhandle.userInteract.ThreadStop[id] = true;
            mhandle.userInteract.RealTimeData[id].State = "CANCELED";
            UpdateRealTime();
        }

        private void ButtonContinue_Click(object sender, RoutedEventArgs e)
        {
            if (End)
                return;
            int id = (int)((Button)sender).Tag;
            mhandle.userInteract.ThreadPause[id] = false;
            mhandle.userInteract.RealTimeData[id].State = "ACTIVE";
            UpdateRealTime();
        }
        private void UpdateRealTime()
        {
            mhandle.userInteract.Mut.WaitOne();
            mhandle.userInteract.RealTime.WriteRealTimeFile(mhandle.userInteract.RealTimeData);
            mhandle.userInteract.Mut.ReleaseMutex();
        }
    }
    public class Item()
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Progr { get; set; }
        public string ProgrStr { get; set; }
        public string Status { get; set; }
    }
}
