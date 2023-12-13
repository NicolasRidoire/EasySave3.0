using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROGRAMMATION_SYST_ME.Model
{
    public class LogDataModel
    {
        public BackupJobDataModel LogData { get; set; }
        public long ElapsedTime { get; set; }
        public long SaveSize { get; set; }
        public string Time { get; set; }
    }
}
