﻿using PROGRAMMATION_SYST_ME.Ressources;
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
                ProgressListView.Items.Add(new Item
                {
                    Name = mhandle.userInteract.BackupJobsData[job].Name,
                    Progr = pro,
                    ProgrStr = pro.ToString() + " %",
                    Status = mhandle.userInteract.RealTimeData[i].State
                });
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
                        if (pro == 100 && canEnd)
                            End = true;
                        else
                        {
                            End = false;
                            canEnd = false;
                        }

                        this.Dispatcher.Invoke(() => ProgressListView.Items[y] = new Item
                        {
                            Name = mhandle.userInteract.BackupJobsData[job].Name,
                            Progr = pro,
                            ProgrStr = pro.ToString() + " %",
                            Status = mhandle.userInteract.RealTimeData[y].State
                        });
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

            mhandle.SaveWin = null;
        }
    }
    public class Item()
    {
        public string Name { get; set; }
        public int Progr { get; set; }
        public string ProgrStr { get; set; }
        public string Status { get; set; }
    }
}
