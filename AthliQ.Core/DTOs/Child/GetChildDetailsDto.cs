using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.DTOs.Test;

namespace AthliQ.Core.DTOs.Child
{
    public class GetChildDetailsDto
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string SchoolName { get; set; }
        public string? ClubName { get; set; }

        public List<string> PreferredSports { get; set; }
        public List<string> ParentSportsHistory { get; set; }

        public double Height { get; set; }
        public double Weight { get; set; }

        public List<TestWithValueDto> Tests { get; set; }

        public string Category { get; set; }
        public string CategoryAr { get; set; }
        public List<string> Sports { get; set; }
        public List<string> SportsAr { get; set; }
    }
}
