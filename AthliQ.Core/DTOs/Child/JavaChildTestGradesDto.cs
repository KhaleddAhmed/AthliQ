using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.Child
{
    public class JavaChildTestGradesDto
    {
        public string Name { get; set; }
        public Dictionary<string, double> TestScores { get; set; }
    }
}
