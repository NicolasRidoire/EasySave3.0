﻿using System.Collections.Generic;
using PROGRAMMATION_SYST_ME.Model;
using System.IO;
using System.Text.RegularExpressions;
using PROGRAMMATION_SYST_ME;
using System.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PROGRAMMATION_SYST_ME.ViewModel
{
    public class MainWindowViewModel
    { 
        // TODO : Add socket server and client to save page
        private long totalSaveSize = 0;
        private long totalNbFile = 0;
        private List<long> NbFilesCopied = new();
        private int indRTime = 0;
        private delegate void CopyType(FileInfo file, string destination);
        CopyType delegCopy;
        private List<Stopwatch> watch = new ();
        private List<string> filePrioList = new();
        private List<string> fileCopied = new();
        public Mutex Mut { set; get; } = new();
        public bool IsSetup { set; get; } = false;
        public bool IsSaving { set; get; } = false;
        public long SizeThreshold { set; get; } = 100000; // En bytes
        public List<bool> ThreadPause { set; get; } = new List<bool>();
        public List<bool> ThreadStop { set; get; } = new List<bool>();

        private readonly string businessSoft = "CalculatorApp";
        public MainWindowViewModel()
        {
            BackupJobs = new BackupJobModel(BackupJobsData);
            delegCopy = CopyFile;
            Threads = new List<Thread>();
        }
        public List<Thread> Threads { set; get; }
        public List<BackupJobDataModel> BackupJobsData { set; get; } = new List<BackupJobDataModel>();
        public BackupJobModel BackupJobs { set; get; }
        public List<RealTimeDataModel> RealTimeData { set; get; } = new List<RealTimeDataModel>();
        public RealTimeModel RealTime { set; get; } = new RealTimeModel();
        public LogModel LogFile { set; get; } = new LogModel();
        private List<string> extPrioList = new List<string>();
        public bool? IsCrypt { set; get; }

        public void ChangeExtensionLog(string extLog)
        {
            BackupJobs.ChangeExtensionLog(extLog);
            LogFile.CreateLogFile();
            RealTime.CreateRealTimeFile();
        }
        public int CreateJob(string name, string source, string Destination, int type)
        {
            BackupJobDataModel newJob = new BackupJobDataModel
            {
                Id = BackupJobsData.Count,
                Name = name,
                Source = source,
                Destination = Destination,
                Type = type
            };

            BackupJobsData.Add(newJob);
            BackupJobs.CreateJob(BackupJobsData);

            return newJob.Id;
        }
        public ErrorCode DeleteJobVM(int job)
        {
            BackupJobsData.RemoveAt(job);
            BackupJobs.DestroyNode(job);
            BackupJobs.SaveParam(BackupJobsData);
            BackupJobs.UpdateList(BackupJobsData);

            return ErrorCode.SUCCESS;
        }

        public ErrorCode UpdateJob(int jobChoice, string name, string source, string dest, int type)
        {   
            BackupJobsData[jobChoice].Name = name;
            BackupJobsData[jobChoice].Source = source;
            BackupJobsData[jobChoice].Destination = dest;
            BackupJobsData[jobChoice].Type = type;

            BackupJobs.SaveParam(BackupJobsData);
            return ErrorCode.SUCCESS;
        }
        private ErrorCode IsBusinessSoftLaunched()
        {
            Process[] processes = Process.GetProcessesByName(businessSoft);

            if (processes.Length != 0)
            {
                return ErrorCode.BUSINESS_SOFT_LAUNCHED;
            }
            return ErrorCode.SUCCESS;
        }
        /// <summary>
        /// Method to execute backup jobs
        /// </summary>
        /// <param name="selection">input user</param>
        /// <returns>error code BUSINESS_SOFT_LAUNCHED or INPUT_USER or SOURCE_ERROR or SUCCESS</returns>
        public ErrorCode ExecuteJob(List<int> jobsToExec, string extPrioString)
        {
            IsSaving = true;
            ErrorCode error = ErrorCode.SUCCESS;
            extPrioList.Clear();
            filePrioList.Clear();
            fileCopied.Clear();
            ThreadPause.Clear();
            ThreadStop.Clear();
            if (extPrioString.Length > 3)
            {
                foreach (string ext in extPrioString.Split(';'))
                {
                    extPrioList.Add(ext);
                }
            }
            else if (extPrioString.Length == 3)
            {
                extPrioList.Add(extPrioString);
            }

            if (IsBusinessSoftLaunched() == ErrorCode.BUSINESS_SOFT_LAUNCHED)
                return ErrorCode.BUSINESS_SOFT_LAUNCHED;

            foreach (int i in jobsToExec)
            {
                GetCopyDeleg(i);
                CreateDir(BackupJobsData[i].Source, BackupJobsData[i].Destination, delegCopy);
                ThreadStop.Add(false);
                ThreadPause.Add(false);
            }

            SetupRealTime(jobsToExec);

            indRTime = 0;
            foreach (int i in jobsToExec)
            {
                delegCopy = CopyPriorityFiles;
                SaveDir(BackupJobsData[i].Source, BackupJobsData[i].Destination, delegCopy);
                indRTime++;
            }

            indRTime = 0;
            foreach (int i in jobsToExec)
            {
                if (error == ErrorCode.SUCCESS)
                {
                    watch.Add(new Stopwatch());
                    watch[indRTime].Start();

                    GetCopyDeleg(i);

                    error = StartJob(i, indRTime);
                }
                else
                    break;
                indRTime++;
            }
            Process[] cryptProcesses;
            do
            {
                cryptProcesses = Process.GetProcessesByName("Cryptosoft");
                Thread.Sleep(50);
            }
            while (cryptProcesses.Length != 0);
            foreach (Thread t in Threads)
            {
                int j = int.Parse(t.Name);
                t.Join();
                watch[j].Stop();   
                Mut.WaitOne();
                if (error == ErrorCode.SUCCESS)
                {
                    if (RealTimeData[j].Progression == 100)
                        RealTimeData[j].State = "SUCCESSFUL";
                }
                else
                    RealTimeData[j].State = "ERROR";
                RealTime.WriteRealTimeFile(RealTimeData);
                Mut.ReleaseMutex();
                LogFile.WriteLogSave(
                        BackupJobsData[jobsToExec[j]],
                        watch[j].ElapsedMilliseconds,
                        RealTimeData[j].TotalFilesSize
                    );
            }
            IsSaving = false;
            Threads.Clear();
            return error;
        }
        private void GetCopyDeleg(int i)
        {
            if (BackupJobsData[i].Type == 0) // Full backup
            {
                delegCopy = CopyFile;
            }
            else // Differencial backup
            {
                delegCopy = CopyFileDiff;
            }
        }
        private ErrorCode StartJob(int i, int indRTime)
        {
            if (Directory.Exists(BackupJobsData[i].Source))
            {
                Thread task = new(() => SaveDir(BackupJobsData[i].Source, BackupJobsData[i].Destination, delegCopy));
                task.Name = indRTime.ToString();
                task.Start();
                Threads.Add(task);
            }
            else if (File.Exists(BackupJobsData[i].Source))
            {
                Thread task = new(() => delegCopy(new FileInfo(BackupJobsData[i].Source), BackupJobsData[i].Destination));
                task.Name = indRTime.ToString();
                task.Start();
                Threads.Add(task);
            }
            else
                return ErrorCode.SOURCE_ERROR;
            return ErrorCode.SUCCESS;
        }
        private ErrorCode CreateDir(string source, string destination, CopyType deleg)
        {
            var dir = new DirectoryInfo(source);

            if (!dir.Exists)
                return ErrorCode.SOURCE_ERROR;
            DirectoryInfo[] dirs = dir.GetDirectories();

            var dirDest = new DirectoryInfo(destination);
            if (deleg == CopyFile)
                if (dirDest.Exists)
                    Directory.Delete(destination, true);
            Directory.CreateDirectory(destination);

            foreach (DirectoryInfo subDir in dirs)
                CreateDir(subDir.FullName, Path.Combine(destination, subDir.Name), deleg);
            return ErrorCode.SUCCESS;
        }
        /// <summary>
        /// Get the file size and length
        /// </summary>
        /// <param name="file">a file</param>
        /// <param name="destination">null by default (only here for delegate)</param>
        private void GetDirectoryInfo(FileInfo file, string destination)
        {
            totalSaveSize += file.Length;
            totalNbFile++;
        }
        private void CopyPriorityFiles(FileInfo file, string destination)
        {
            bool toCopy = false;
            if (file.Length > SizeThreshold)
            {
                toCopy = true;
            }
            else
            {
                foreach (string ext in extPrioList)
                {
                    if (file.Extension == ('.' + ext))
                    {
                        toCopy = true;
                    }
                }
            }
            if (!toCopy)
                return;
            CopyFile(file, destination);
            fileCopied.Add(file.FullName);
            NbFilesCopied[indRTime]++;
            RealTimeData[indRTime].NbFilesLeftToDo = RealTimeData[indRTime].TotalFilesToCopy - NbFilesCopied[indRTime];
            RealTimeData[indRTime].Progression = (double)NbFilesCopied[indRTime] / (double)RealTimeData[indRTime].TotalFilesToCopy * 100;
            RealTime.WriteRealTimeFile(RealTimeData);
        }
        /// <summary>
        /// Go through a directory recursively and use a delegate 
        /// to choose what to do on each file
        /// </summary>
        /// <param name="source">Source directory</param>
        /// <param name="destination">Destination directory</param>
        /// <returns>error code SUCCESS or SOURCE_ERROR</returns>
        private ErrorCode SaveDir(string source, string destination, CopyType deleg)
        {
            var dir = new DirectoryInfo(source);

            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (FileInfo file in dir.GetFiles())
            {
                if (!fileCopied.Contains(file.FullName))
                    deleg(file, destination);
            }

            foreach (DirectoryInfo subDir in dirs)
            {
                SaveDir(subDir.FullName, Path.Combine(destination, subDir.Name), deleg);
            }
            return ErrorCode.SUCCESS;
        }
        /// <summary>
        /// Copy a file while updating total copy info
        /// </summary>
        /// <param name="file">a file</param>
        /// <param name="destination">destination directory</param>
        private void CopyFile(FileInfo file, string destination)
        {
            while (IsBusinessSoftLaunched() == ErrorCode.BUSINESS_SOFT_LAUNCHED)
                Thread.Sleep(50);
            var ind = 0;
            if (Threads.Count > 0)
            {
                ind = int.Parse(Thread.CurrentThread.Name);
                if (ThreadStop[ind])
                    return;
                while (ThreadPause[ind])
                    Thread.Sleep(100);
            }
            
            if (IsCrypt == true)
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "Cryptosoft.exe";
                process.StartInfo.Arguments = '"' + file.FullName + '"' + " " + '"' + Path.Combine(destination, file.Name) + '"';
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();
            }
            else
            {
                file.CopyTo(Path.Combine(destination, file.Name), true);
            }

            if (Threads.Count < 1)
                return;
            NbFilesCopied[ind]++;
            RealTimeData[ind].NbFilesLeftToDo = RealTimeData[ind].TotalFilesToCopy - NbFilesCopied[ind];
            RealTimeData[ind].Progression = (double)NbFilesCopied[ind] / (double)RealTimeData[ind].TotalFilesToCopy * 100;
            Mut.WaitOne();
            RealTime.WriteRealTimeFile(RealTimeData);
            Mut.ReleaseMutex();
        }
        /// <summary>
        /// Copy a file if a change occured while updating total copy info
        /// </summary>
        /// <param name="file">a file</param>
        /// <param name="destination">destination directory</param>
        private void CopyFileDiff(FileInfo file, string destination)
        {
            var destPath = Path.Combine(destination, file.Name);
            var destFile = new FileInfo(destPath);

            if (file.LastWriteTime != destFile.LastWriteTime) // Condition to see if file changed
            {
                CopyFile(file, destination);
            }
        }
        /// <summary>
        /// Setup the real time log file
        /// </summary>
        /// <param name="jobsToExec">List of index that represent the backup jobs</param>
        private void SetupRealTime(List<int> jobsToExec)
        {
            RealTimeData.Clear();
            NbFilesCopied.Clear();
            indRTime = 0;

            foreach (int i in jobsToExec)
            {
                RealTimeData.Add(new RealTimeDataModel());
                NbFilesCopied.Add(0);
                RealTimeData[indRTime].SaveData = BackupJobsData[i];
                RealTimeData[indRTime].State = "ACTIVE";
                if (Directory.Exists(BackupJobsData[i].Source))
                {
                    totalNbFile = 0;
                    totalSaveSize = 0;
                    delegCopy = GetDirectoryInfo;
                    SaveDir(BackupJobsData[i].Source, BackupJobsData[i].Destination, delegCopy);
                    RealTimeData[indRTime].TotalFilesSize = totalSaveSize;
                    RealTimeData[indRTime].TotalFilesToCopy = totalNbFile;
                }
                else if (File.Exists(BackupJobsData[i].Source))
                {
                    RealTimeData[indRTime].TotalFilesSize = new FileInfo(BackupJobsData[i].Source).Length;
                    RealTimeData[indRTime].TotalFilesToCopy = 1;
                }
                indRTime++;
            }
            RealTime.WriteRealTimeFile(RealTimeData);
            IsSetup = true; 
        }
    }
}
