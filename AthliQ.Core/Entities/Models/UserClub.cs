using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.Entities.Models
{
    public class UserClub : BaseEntity
    {
        public string AthliQUserId { get; set; }
        public string ClubName { get; set; }
        public AthliQUser User { get; set; }
    }
}
