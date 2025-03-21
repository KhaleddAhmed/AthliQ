using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.Entities.Models
{
    public class ChildTest : BaseEntity
    {
        public int TestId { get; set; }
        public int ChildId { get; set; }
        public double TestResult { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Test Test { get; set; }
        public Child Child { get; set; }
    }
}
