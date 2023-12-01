using System.Collections.Generic;
using PROGRAMMATION_SYST_ME.Model;
using System.IO;
using System.Text.RegularExpressions;
using PROGRAMMATION_SYST_ME;
using System.Diagnostics;


namespace PROGRAMMATION_SYST_ME.ViewModel
{
    public class UserInteractionViewModel
    { 
        public List<BackupJobDataModel> BackupJobsData { set; get; } = new List<BackupJobDataModel>();
        public BackupJobModel BackupJobs { set; get; }
        public List<RealTimeDataModel> RealTimeData { set; get; } = new List<RealTimeDataModel>();
        public RealTimeModel RealTime { set; get; } = new RealTimeModel();
        public LogModel LogFile { set; get; } = new LogModel();
        //private readonly StatusView statusView = new StatusView();
        private long totalSaveSize = 0;
        private long totalNbFile = 0;
        private long NbFilesCopied = 0;
        private int indRTime = 0;
        private delegate void CopyType(FileInfo file, string destination);
        CopyType delegCopy;
        private string businessSoft = "CalculatorApp";
        public UserInteractionViewModel()
        {
            BackupJobs = new BackupJobModel(BackupJobsData);
            delegCopy = CopyFile;
        }
        public bool ChangeExtensionLog(string extLog)
        {
            BackupJobs.ChangeExtensionLog(extLog);
            LogFile.CreateLogFile();
            RealTime.CreateRealTimeFile();
            return true;
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
        public errorCode DeleteJobVM(int job)
        {
            BackupJobsData.RemoveAt(job);
            BackupJobs.DestroyNode(job);
            BackupJobs.SaveParam(BackupJobsData);
            BackupJobs.UpdateList(BackupJobsData);
            return errorCode.SUCCESS;
        }
        public errorCode UpdateJob(int jobChoice, string name, string source, string dest, int type)
        {   
            BackupJobsData[jobChoice].Name = name;
            BackupJobsData[jobChoice].Source = source;
            BackupJobsData[jobChoice].Destination = dest;
            BackupJobsData[jobChoice].Type = type;
            BackupJobs.SaveParam(BackupJobsData);
            return errorCode.SUCCESS;
        }
        /// <summary>
        /// Method to execute backup jobs
        /// </summary>
        /// <param name="selection">input user</param>
        /// <returns>error code BUSINESS_SOFT_LAUNCHED or INPUT_USER or SOURCE_ERROR or SUCCESS</returns>
        public errorCode ExecuteJob(List<int> jobsToExec)
        {
            errorCode error = errorCode.SUCCESS;
            Process[] processes = Process.GetProcessesByName(businessSoft);
            if (processes.Length != 0)
            {
                error = errorCode.BUSINESS_SOFT_LAUNCHED;
                return error;
            }

            SetupRealTime(jobsToExec);
            indRTime = 0;
            foreach (int i in jobsToExec)
            {
                NbFilesCopied = 0;
                if (error == errorCode.SUCCESS)
                {
                    RealTimeData[indRTime].State = "ACTIVE";
                    RealTime.WriteRealTimeFile(RealTimeData);

                    //statusView.JobStart(BackupJobsData[i].Name);
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
                    {
                        //statusView.JobStop(BackupJobsData[i].Name, watch.ElapsedMilliseconds);
                        RealTimeData[indRTime].State = "SUCCESSFUL";
                    }
                    else
                        RealTimeData[indRTime].State = "ERROR";
                    RealTime.WriteRealTimeFile(RealTimeData);
                }
                else
                    break;
                indRTime++;
            }
            if (error == errorCode.SUCCESS) { }
            //statusView.JobsComplete();
            return error;
        }
        /// <summary>
        /// Get the file size and length
        /// </summary>
        /// <param name="file">a file</param>
        /// <param name="destination">null by default (only here for delegate)</param>
        private void GetDirectoryInfo(FileInfo file, string destination = null)
        {
            totalSaveSize += file.Length;
            totalNbFile++;
        }
        /// <summary>
        /// Go through a directory recursively and use a delegate 
        /// to choose what to do on each file
        /// </summary>
        /// <param name="source">Source directory</param>
        /// <param name="destination">Destination directory</param>
        /// <returns>error code SUCCESS or SOURCE_ERROR</returns>
        private errorCode SaveDir(string source, string destination)
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
        /// <summary>
        /// Copy a file while updating total copy info
        /// </summary>
        /// <param name="file">a file</param>
        /// <param name="destination">destination directory</param>
        private void CopyFile(FileInfo file, string destination)
        {
            file.CopyTo(Path.Combine(destination, file.Name), true);
            NbFilesCopied++;
            RealTimeData[indRTime].NbFilesLeftToDo = NbFilesCopied - RealTimeData[indRTime].TotalFilesToCopy;
            RealTimeData[indRTime].Progression = NbFilesCopied / RealTimeData[indRTime].TotalFilesToCopy;
            RealTime.WriteRealTimeFile(RealTimeData);
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
            indRTime = 0;
            foreach (int i in jobsToExec)
            {
                RealTimeData.Add(new RealTimeDataModel());
                RealTimeData[indRTime].SaveData = BackupJobsData[i];
                RealTimeData[indRTime].State = "WAITING";
                if (Directory.Exists(BackupJobsData[i].Source))
                {
                    totalNbFile = 0;
                    totalSaveSize = 0;
                    delegCopy = GetDirectoryInfo;
                    SaveDir(BackupJobsData[i].Source, BackupJobsData[i].Destination);
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
        }
    }
}
