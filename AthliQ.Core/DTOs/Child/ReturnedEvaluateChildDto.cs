using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.DTOs.Sport;

namespace AthliQ.Core.DTOs.Child
{
    public class ReturnedEvaluateChildDto
    {
        public List<ChildResultIntegratedDto> ChildResultIntegratedDto { get; set; }
        public List<ChildResultWithPercentagesDto> ChildResultWithPercentagesDtos { get; set; }
        public string FinalResult { get; set; }
        public List<ResultedSportDto> MatchedSports { get; set; }
    }
}
