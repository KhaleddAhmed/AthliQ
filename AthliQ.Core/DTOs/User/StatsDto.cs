using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.User
{
    public class StatsDto
    {
        public int NumberOfPendingUsers { get; set; }
        public int NumberOfUsersApprovedToday { get; set; }
        public int NumberOfTests { get; set; }
        public int NumberOfCategories { get; set; }
        public int NumberOfSports { get; set; }
    }
}
