using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROGRAMMATION_SYST_ME.Model
{
    public class BackupJobDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public int Type { get; set; }// O is full backup and 1 is differential backup
    }
}
