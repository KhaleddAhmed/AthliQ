using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.Child
{
    public class ChildToSendDto
    {
        public string Name { get; set; }
        public DateOnly BirthDate { get; set; }

        public string SchoolType { get; set; }

        public string Gender { get; set; }

        public double Height { get; set; }

        public double Weight { get; set; }

        public List<string> PreferredSports { get; set; }
        public List<string> ParentSportsHistory { get; set; }

        public List<double> TestScores { get; set; }
        public bool HasDoctorApproval { get; set; }
        public bool HasNormalBloodTest { get; set; }
    }
}
