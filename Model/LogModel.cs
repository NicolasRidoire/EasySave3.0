using System;
using System.IO;
using System.Text.Json;
using System.Xml;

namespace PROGRAMMATION_SYST_ME.Model
{
    /// <summary>
    /// Log model class
    /// </summary>
    public class LogModel
    {
        public XmlDocument Xml { set; get; } = new XmlDocument();
        public XmlDocument logXml { set; get; } = new XmlDocument();
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
            CreateLogFile();
        }
        /// <summary>
        /// Create the log file
        /// </summary>
        public void CreateLogFile()
        {
            try
            {
                Xml.Load(xmlPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error : {e}");
                Environment.Exit(3);
            }
            ExtLog = Xml.SelectSingleNode("/root/ExtLog").InnerText;
            logFolder = Path.Combine(Environment.CurrentDirectory, "logs");
            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);
            logFile = Path.Combine(logFolder, DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + "." + ExtLog);
        }
        /// <summary>
        /// Method to write our logs' content
        /// </summary>
        /// <param name="info"> Info </param>
        /// <param name="elapsedTime"> Time </param>
        /// <param name="fileSize"> Size of the log file </param>
        public void WriteLogSave(BackupJobDataModel logData, long elapsedTime, long saveSize)
        {
            if (ExtLog == "xml")
            {
                try
                {
                    logXml.Load(logFile);
                }
                catch (Exception e)
                {
                    var createRoot = logXml.CreateElement("root");
                    logXml.AppendChild(createRoot);
                }
                var root = logXml.DocumentElement;
                XmlElement log = logXml.CreateElement("log");
                root.AppendChild(log);
                XmlElement id = logXml.CreateElement("id");
                id.InnerText = logData.Id.ToString();
                log.AppendChild(id);
                XmlElement name = logXml.CreateElement("name");
                name.InnerText = logData.Name;
                log.AppendChild(name);
                XmlElement source = logXml.CreateElement("source");
                source.InnerText = logData.Source;
                log.AppendChild(source);
                XmlElement destination = logXml.CreateElement("destination");
                destination.InnerText = logData.Destination;
                log.AppendChild(destination);
                XmlElement type = logXml.CreateElement("type");
                type.InnerText = logData.Type.ToString();
                log.AppendChild(type);
                XmlElement elapsedTimeElement = logXml.CreateElement("elapsedTime");
                elapsedTimeElement.InnerText = elapsedTime.ToString();
                log.AppendChild(elapsedTimeElement);
                XmlElement saveSizeElement = logXml.CreateElement("saveSize");
                saveSizeElement.InnerText = saveSize.ToString();
                log.AppendChild(saveSizeElement);
                XmlElement time = logXml.CreateElement("time");
                time.InnerText = DateTime.Now.ToString();
                log.AppendChild(time);
                logXml.Save(logFile);
            }
            else if (ExtLog == "json")
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
    }
    public class LogDataModel
    {
        public BackupJobDataModel LogData { get; set;}
        public long ElapsedTime { get; set;}
        public long SaveSize { get; set;}
        public string Time { get; set;}
    }


}
