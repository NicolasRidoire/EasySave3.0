using System.IO;
using System;
using PROGRAMMATION_SYST_ME.Model;
using System.Text.Json;
using System.Collections.Generic;

namespace PROGRAMMATION_SYST_ME.Model
{
    internal class RealTimeModel
    {
        private readonly string realTimeFile;
        public RealTimeModel()
        {
            realTimeFile = Path.Combine(Environment.CurrentDirectory, @"logs", @"State.json");
        }
        public void WriteRealTimeFile(List<RealTimeDataModel> RealTime)
        {
            var jsonString = JsonSerializer.Serialize(RealTime, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(realTimeFile, jsonString);
        }
    }
    internal class RealTimeDataModel
    {
        public BackupJobDataModel SaveData { get; set; }
        public string State { get; set; }
        public long TotalFilesToCopy { get; set; }
        public long TotalFilesSize { get; set; }
        public long NbFilezLeftToDo { get; set; }
        public double Progression { get; set; }
    }
}
