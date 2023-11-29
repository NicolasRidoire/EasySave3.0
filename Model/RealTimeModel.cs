using System.IO;
using System;
using PROGRAMMATION_SYST_ME.Model;
using System.Text.Json;

namespace PROGRAMMATION_SYST_ME.Model
{
    internal class RealTimeModel
    {
        public DataJSON RealTime { get; set; } = new DataJSON();
        private readonly string realTimeFile;
        RealTimeModel()
        {
            realTimeFile = Path.Combine(Environment.CurrentDirectory, @"logs", @"State.json");
            if (!Directory.Exists(realTimeFile))
                Directory.CreateDirectory(realTimeFile);
        }
        public void WriteRealTimeFile()
        {
            var jsonString = JsonSerializer.Serialize(RealTime, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(realTimeFile, jsonString);
        }
    }
    internal class DataJSON
    {
        public BackupJobDataModel RealTimeData { get; set; }
        public string State { get; set; }
        public int TotalFilesToCopy { get; set; }
        public int TotalFilesSize { get; set; }
        public int NbFilezLeftToDo { get; set; }
        public float Progression { get; set; }
    }
}
