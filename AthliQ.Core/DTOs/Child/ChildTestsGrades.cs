using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.Child
{
    public class ChildTestsGrades
    {
        public string Name { get; set; }
        public List<TestGradesDto> TestGradesDtos { get; set; }
    }
}
