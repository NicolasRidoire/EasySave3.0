using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROGRAMMATION_SYST_ME.Model
{
    public class RealTimeDataModel
    {
        public BackupJobDataModel SaveData { get; set; }
        public string State { get; set; }
        public long TotalFilesToCopy { get; set; }
        public long TotalFilesSize { get; set; }
        public long NbFilesLeftToDo { get; set; }
        public double Progression { get; set; }
    }
}
