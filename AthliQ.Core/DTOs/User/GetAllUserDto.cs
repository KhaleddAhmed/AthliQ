using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.User
{
    public class GetAllUserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Gender { get; set; }
        public bool IsAccepted { get; set; }
        public string Role { get; set; } = "Parent";
    }
}
