using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.Child
{
    public class ReturnedEvaluateChildDto
    {
        public List<ChildResultIntegratedDto> ChildResultIntegratedDto { get; set; }
        public string FinalResult { get; set; }
    }
}
