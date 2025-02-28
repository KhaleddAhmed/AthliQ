using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.Entities.Models
{
    public class Child : BaseEntity
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string SchoolName { get; set; }
        public string ClubName { get; set; }
        public bool IsAgreeHealthPolicies { get; set; } = false;
        public int SportHistoryId { get; set; }
        public int ParentSportHistoryId { get; set; }
        public int SportPreferenceId { get; set; }
        public double Hieght { get; set; }
        public double Weight { get; set; }
        public string ImageFrontURL { get; set; }
        public string ImageSideURL { get; set; }

        public string AthliQUserId { get; set; }
        public AthliQUser AthliQUser { get; set; }

        public virtual ICollection<ChildTest> ChildTests { get; set; }
        public virtual ICollection<ChildResult> ChildResults { get; set; }
        
    }
}
