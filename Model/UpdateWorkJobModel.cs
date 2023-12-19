using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROGRAMMATION_SYST_ME.Model
{
    public class UpdateWorkJobModel
    {
        public int Id { get; set; }
        public string SaveName { get; set; }
        public string Source { get; set; }
        public string Dest { get; set; }
        public int Type { get; set; }
        public bool IsAdd { get; set; }
    }
}
