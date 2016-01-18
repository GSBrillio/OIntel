using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpsIntel.ViewModel
{
    public class RulesAction
    {

        public int? Id { get; set; }
        public string Name { get; set; }
        public string CPUs { get; set; }
        public decimal? Memory { get; set; }
        public int? ThresholdCPU { get; set; }
        public int? ThresholdMemory { get; set; }
        public int? DiskWrite { get; set; }
        public int? DiskRead { get; set; }
        public int? NetworkIn { get; set; }
        public int? NetworkOut { get; set; }

    }
}