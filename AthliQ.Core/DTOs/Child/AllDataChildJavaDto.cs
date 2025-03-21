using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.Child
{
    public class AllDataChildJavaDto
    {
        public Dictionary<string, string> CategoryPercentages { get; set; }
        public Dictionary<string, double> CategoryScores { get; set; }
        public string Name { get; set; }
        public string BestCategory { get; set; }
        public List<string> Errors { get; set; }
    }
}
