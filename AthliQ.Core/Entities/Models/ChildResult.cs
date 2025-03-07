using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.Entities.Models
{
    public class ChildResult : BaseEntity
    {
        public int ChildId { get; set; }
        public int CategoryId { get; set; }
        public DateTime ResultDate { get; set; } = DateTime.Now;

        public Child Child { get; set; }
        public Category Category { get; set; }
    }
}
