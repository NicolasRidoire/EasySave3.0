using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace PROGRAMMATION_SYST_ME.Model
{
    /// <summary>
    /// Model class for a list of 5 backup jobs
    /// </summary>
    public class BackupJobModel
    {
        // JB: On a plusieurs responsabilités ici:
        // 1) Le stockage des données au format XML
        // 2) La lecture des données au format XML
        // 3) La lecture et la mise à jour du modèle avec les propriétés
        // On pourrait avoir une classe XmlService qui contiendrait le traitement des données XML pour alléger le modèle
        public XmlDocument Xml { set; get; } = new XmlDocument();
        private readonly string xmlPath;
        public BackupJobModel(List<BackupJobDataModel> jobList)
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
            UpdateList(jobList);
        }
        
        public void UpdateList(List<BackupJobDataModel> jobList)
        {
            jobList.Clear();
            foreach (XmlNode node in Xml.DocumentElement.SelectNodes("//saveJob"))
            {
                BackupJobDataModel data = new BackupJobDataModel();
                data.Id = int.Parse(node.ChildNodes[0].InnerText);
                data.Name = node.ChildNodes[1].InnerText;
                data.Source = node.ChildNodes[2].InnerText;
                data.Destination = node.ChildNodes[3].InnerText;
                data.Type = int.Parse(node.ChildNodes[4].InnerText);
                jobList.Add(data);
            }
        }
        public void DestroyNode(int index)
        {
            var nodeToDestroy = Xml.DocumentElement;
            nodeToDestroy.RemoveChild(nodeToDestroy.SelectNodes("//saveJob")[index]);
        }
        public void UpdateNode(List<BackupJobDataModel> jobList, XmlNode node, int id)
        {
            node.ChildNodes[0].InnerText = id.ToString();
            node.ChildNodes[1].InnerText = jobList[id].Name;
            node.ChildNodes[2].InnerText = jobList[id].Source;
            node.ChildNodes[3].InnerText = jobList[id].Destination;
            node.ChildNodes[4].InnerText = jobList[id].Type.ToString();
            Xml.Save(xmlPath);
        }

        /// <summary>
        /// Method that allows for edits in the backup jobs
        /// </summary>
        public void SaveParam(List<BackupJobDataModel> jobList)
        {
            var i = 0;
            foreach (BackupJobDataModel job in jobList)
            {
                var node = Xml.DocumentElement.SelectNodes("//saveJob")[i];
                if (node == null) // If we need to create a new node to add a job
                {
                    Xml.DocumentElement.AppendChild(Xml.DocumentElement.AppendChild(Xml.CreateElement("//saveJob")));
                    node = Xml.DocumentElement.SelectNodes("//saveJob")[i];
                    node.AppendChild(Xml.CreateElement("//id"));
                    node.AppendChild(Xml.CreateElement("//name"));
                    node.AppendChild(Xml.CreateElement("//source"));
                    node.AppendChild(Xml.CreateElement("//destination"));
                    node.AppendChild(Xml.CreateElement("//type"));
                }
                UpdateNode(jobList, node, i++);
            }
        }

        /// <summary>
        /// Change the extension file
        /// </summary>
        /// <param name="extLog">extension name</param>
        public void ChangeExtensionLog(string extLog)
        {
            Xml.SelectSingleNode("/root/ExtLog").InnerText = extLog;
            Xml.Save(xmlPath);
        }

        public void CreateJob(List<BackupJobDataModel> jobList)
        {
            Xml.DocumentElement.AppendChild(Xml.CreateElement("saveJob"));
            XmlNode saveJob = Xml.DocumentElement.SelectNodes("saveJob")[jobList.Count-1];
            saveJob.AppendChild(Xml.CreateElement("Id"));
            saveJob.AppendChild(Xml.CreateElement("Name"));
            saveJob.AppendChild(Xml.CreateElement("Source"));
            saveJob.AppendChild(Xml.CreateElement("Destination"));
            saveJob.AppendChild(Xml.CreateElement("Type"));
            UpdateNode(jobList, saveJob, jobList.Count-1);
        }
    }
}
