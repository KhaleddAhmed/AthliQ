using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace AthliQ.Core.Entities
{
    public class AthliQUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public bool IsAccepted { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ModifiedAt { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public virtual ICollection<UserClub> UserClubs { get; set; }
        public virtual ICollection<Child> Childs { get; set; }
    }
}
