using System.Collections.Generic;
using PROGRAMMATION_SYST_ME.Model;
using System.IO;
using System.Text.RegularExpressions;
using PROGRAMMATION_SYST_ME.View;
using System.Diagnostics;

namespace PROGRAMMATION_SYST_ME.ViewModel
{
    class UserInteractionViewModel
    {
        public List<BackupJobDataModel> BackupJobsData { set; get; } = new List<BackupJobDataModel>();
        public BackupJobModel BackupJobs { set; get; }
        public LogModel LogFile { set; get; } = new LogModel();
        private readonly StatusView statusView = new StatusView();
        private long totalSaveSize = 0;
        private delegate void CopyType(FileInfo file, string destination);
        CopyType delegCopy;
        public UserInteractionViewModel() 
        {
            BackupJobs = new BackupJobModel(BackupJobsData);
            delegCopy = CopyFile;
        }
        /// <summary>
        /// Method to update backup jobs
        /// </summary>
        /// <param name="jobChoice"></param>
        /// <param name="change"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public errorCode UpdateJob(int jobChoice, string change, string newValue) 
        {   // Utilisation d'un switch case
            switch (change)
            {
                case "N":
                    BackupJobsData[jobChoice].Name = newValue;
                    break;
                case "S":
                    BackupJobsData[jobChoice].Source = newValue;
                    break;
                case "D":
                    BackupJobsData[jobChoice].Destination = newValue;
                    break;
                case "T":
                    BackupJobsData[jobChoice].Type = int.Parse(newValue);
                    break;
            }
            BackupJobs.SaveParam(BackupJobsData);
            return errorCode.SUCCESS;
        }
        /// <summary>
        /// Method to execute backup jobs
        /// </summary>
        /// <param name="selection"></param>
        /// <returns></returns>
        public errorCode ExecuteJob(string selection) // execute save job
        {
            errorCode error = errorCode.SUCCESS;
            List<int> jobsToExec = new List<int>();
            if (Regex.IsMatch(selection, @"^[1-4]-[2-5]\z"))
            {
                var start = selection[0];
                var end = selection[2];
                for (var i = start; i < end + 1; i++)
                {
                    jobsToExec.Add(i - '0' - 1);
                }
            }
            else if (Regex.IsMatch(selection, @"^[1-5](;[1-5]){0,3};[1-5]\z"))
            {
                foreach (char c in selection)
                {
                    if (c != ';')
                        jobsToExec.Add(c - '0' - 1);
                }
            }
            else if (Regex.IsMatch(selection, @"^[1-5]\z"))
            {
                jobsToExec.Add(int.Parse(selection) - 1);
            }
            else if (selection == "Q")
                return global::errorCode.SUCCESS;
            else
            {
                return global::errorCode.INPUT_ERROR;
            }
            foreach (int i in jobsToExec) // Utilisation de foreach
            { 
                if (error == errorCode.SUCCESS)
                {
                    statusView.JobStart(BackupJobsData[i].Name);
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    totalSaveSize = 0;
                    if (BackupJobsData[i].Type == 0) // Full backup
                    {
                        delegCopy = CopyFile;
                    }
                    else // Differencial backup
                    {
                        delegCopy = CopyFileDiff;
                    }
                    if (Directory.Exists(BackupJobsData[i].Source))
                    {
                        error = SaveDir(BackupJobsData[i].Source, BackupJobsData[i].Destination);
                    }
                    else if (File.Exists(BackupJobsData[i].Source))
                    {
                        delegCopy(new FileInfo(BackupJobsData[i].Source), BackupJobsData[i].Destination);
                    }
                    else
                        error = errorCode.SOURCE_ERROR;
                    watch.Stop();

                    LogFile.WriteLogSave(
                        BackupJobsData[i],
                        watch.ElapsedMilliseconds, 
                        totalSaveSize
                    );

                    if (error == errorCode.SUCCESS)
                        statusView.JobStop(BackupJobsData[i].Name, watch.ElapsedMilliseconds);
                }
                else 
                    break;
            }
            if (error == errorCode.SUCCESS)
                statusView.JobsComplete();
            return error;
        }
        public errorCode SaveDir(string source, string destination)
        {
            var dir = new DirectoryInfo(source);
            if (!dir.Exists)
                return errorCode.SOURCE_ERROR;
            DirectoryInfo[] dirs = dir.GetDirectories();
            var dirDest = new DirectoryInfo(destination);
            if (dirDest.Exists)
                Directory.Delete(destination, true);
            Directory.CreateDirectory(destination);
            foreach (FileInfo file in dir.GetFiles())
            {
                delegCopy(file, destination);
            }
            foreach (DirectoryInfo subDir in dirs)
            {
                SaveDir(subDir.FullName, Path.Combine(destination, subDir.Name));
            }
            return errorCode.SUCCESS;
        }
        private void CopyFile(FileInfo file, string destination)
        {
            file.CopyTo(Path.Combine(destination, file.Name), true);
            totalSaveSize += file.Length;
        }
        private void CopyFileDiff(FileInfo file, string destination)
        {
            var destPath = Path.Combine(destination, file.Name);
            var destFile = new FileInfo(destPath);
            if (file.LastWriteTime != destFile.LastWriteTime) // Condition to see if file changed
            {
                CopyFile(file, destination);
            }
        }
    }
}
