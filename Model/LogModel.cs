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
            Xml.SelectSingleNode("/root/ExtLog").InnerText = ExtLog;
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
            var newXml = new XmlDocument();
            if (Xml.SelectSingleNode("/root/ExtLog").InnerText == "xml")
            {
                XmlElement root = newXml.CreateElement("root");
                newXml.AppendChild(root);
                XmlElement log = newXml.CreateElement("log");
                root.AppendChild(log);
                XmlElement id = newXml.CreateElement("id");
                id.InnerText = logData.Id.ToString();
                log.AppendChild(id);
                XmlElement name = newXml.CreateElement("name");
                name.InnerText = logData.Name;
                log.AppendChild(name);
                XmlElement source = newXml.CreateElement("source");
                source.InnerText = logData.Source;
                log.AppendChild(source);
                XmlElement destination = newXml.CreateElement("destination");
                destination.InnerText = logData.Destination;
                log.AppendChild(destination);
                XmlElement type = newXml.CreateElement("type");
                type.InnerText = logData.Type.ToString();
                log.AppendChild(type);
                XmlElement elapsedTimeElement = newXml.CreateElement("elapsedTime");
                elapsedTimeElement.InnerText = elapsedTime.ToString();
                log.AppendChild(elapsedTimeElement);
                XmlElement saveSizeElement = newXml.CreateElement("saveSize");
                saveSizeElement.InnerText = saveSize.ToString();
                log.AppendChild(saveSizeElement);
                XmlElement time = newXml.CreateElement("time");
                time.InnerText = DateTime.Now.ToString();
                log.AppendChild(time);
                newXml.Save(logFile);
            }
            else if (Xml.SelectSingleNode("/root/ExtLog").InnerText == "json")
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
    class LogDataModel
    {
        public BackupJobDataModel LogData { get; set;}
        public long ElapsedTime { get; set;}
        public long SaveSize { get; set;}
        public string Time { get; set;}
    }


}
