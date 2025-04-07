using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.Child
{
    public class TestGradesDto
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public double Grade { get; set; }
        public string Evaluation { get; set; }
    }
}
