using System;
using System.IO;
using System.Text.Json;
using System.Xml;

namespace PROGRAMMATION_SYST_ME.Model
{
    /// <summary>
    /// Log model class
    /// </summary>
    class LogModel
    {
        public XmlDocument Xml { set; get; } = new XmlDocument();
        private string logFolder;
        private string logFile;
        private readonly string xmlPath;
        public string ExtLog { get; set; }
        public int NbJobs { set; get; }
        /// <summary>
        /// When creating a .json log file, the name is automatimacally generated to match the current date
        /// </summary>
        public LogModel()
        {
            xmlPath = Path.Combine(Environment.CurrentDirectory, @"SaveJobsConfig.xml");
            // if the selected path is found, proceed. otherwise raise an error.
            try
            {
                Xml.Load(xmlPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error : {e}");
                Environment.Exit(3);
            }
            createLogFile();
        }

        public void createLogFile()
        {
            
            string ExtLog = Xml.SelectSingleNode("/root/ExtLog").InnerText;
            logFolder = Path.Combine(Environment.CurrentDirectory, "logs");
            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);
            logFile = Path.Combine(logFolder, DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + "." + ExtLog);
        }

        public void ChangeExtensionLog(string extLog)
        {
            
            ExtLog = extLog;
            Console.WriteLine($"Extension of the log file : {ExtLog}");
            
            Xml.SelectSingleNode("/root/ExtLog").InnerText = ExtLog;
            Console.WriteLine($"extension : {Xml.SelectSingleNode("/root/ExtLog").InnerText}");
            Xml.Save(xmlPath);
            createLogFile();
        }
        /// <summary>
        /// Method to write our logs' content
        /// </summary>
        /// <param name="info"> Info </param>
        /// <param name="elapsedTime"> Time </param>
        /// <param name="fileSize"> Size of the log file </param>
        public void WriteLogSave(BackupJobDataModel logData, long elapsedTime, long saveSize)
        {
            LogDataModel data = new LogDataModel()
            {
                LogData = logData,
                ElapsedTime = elapsedTime,
                SaveSize = saveSize,
                Time = DateTime.Now.ToString()
            };
            var jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.AppendAllText(logFile, jsonString);
        }
    }
    class LogDataModel
    {
        public BackupJobDataModel LogData { get; set;}
        public long ElapsedTime { get; set;}
        public long SaveSize { get; set;}
        public string Time { get; set;}
    }


}
