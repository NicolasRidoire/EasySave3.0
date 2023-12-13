using System.IO;
using System;
using PROGRAMMATION_SYST_ME.Model;
using System.Text.Json;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualBasic.FileIO;

namespace PROGRAMMATION_SYST_ME.Model
{
    public class RealTimeModel
    {
        private string realTimeFile;
        public string ExtRealTime { get; set; }
        public XmlDocument Xml { set; get; } = new XmlDocument();
        private readonly string xmlPath;
        public RealTimeModel()
        {
            xmlPath = Path.Combine(Environment.CurrentDirectory, @"SaveJobsConfig.xml");
            // if the selected path is found, proceed. otherwise raise an error.
            CreateRealTimeFile();
        }
        /// <summary>
        /// Create the log file
        /// </summary>
        public void CreateRealTimeFile()
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
            ExtRealTime = Xml.SelectSingleNode("/root/ExtLog").InnerText;
            realTimeFile = Path.Combine(Environment.CurrentDirectory, @"logs", @"State." + ExtRealTime);
        }
        /// <summary>
        /// Write the real time log file
        /// </summary>
        /// <param name="RealTime">List of data to write in the file</param>
        public void WriteRealTimeFile(List<RealTimeDataModel> RealTime)
        {
            if (ExtRealTime == "json") 
            {
                var jsonString = JsonSerializer.Serialize(RealTime, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(realTimeFile, jsonString);
            }
            else if (ExtRealTime == "xml")
            {
                var newXml = new XmlDocument();
                newXml.RemoveAll();
                XmlElement root = newXml.CreateElement("root");
                newXml.AppendChild(root);
                var i = 0;
                foreach (RealTimeDataModel data in RealTime)
                {
                    var job = CreateXmlElement(newXml, root, "job", "");
                    CreateXmlElement(newXml, job, "id", data.SaveData.Id.ToString());
                    CreateXmlElement(newXml, job, "name", data.SaveData.Name);
                    CreateXmlElement(newXml, job, "source", data.SaveData.Source);
                    CreateXmlElement(newXml, job, "destination", data.SaveData.Destination);
                    CreateXmlElement(newXml, job, "type", data.SaveData.Type.ToString());
                    CreateXmlElement(newXml, job, "state", data.State);
                    CreateXmlElement(newXml, job, "totalFilesToCopy", data.TotalFilesToCopy.ToString());
                    CreateXmlElement(newXml, job, "totalFilesSize", data.TotalFilesSize.ToString());
                    CreateXmlElement(newXml, job, "nbFilesLeftToDo", data.NbFilesLeftToDo.ToString());
                    CreateXmlElement(newXml, job, "progression", data.Progression.ToString());
                    i++;
                }
                newXml.Save(realTimeFile);
            }
        }
        private static XmlElement CreateXmlElement(XmlDocument xml, XmlElement father, string name, string data)
        {
            XmlElement element = xml.CreateElement(name);
            element.InnerText = data;
            father.AppendChild(element);
            return element;
        }
    }
}
