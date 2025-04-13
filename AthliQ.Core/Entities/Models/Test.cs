using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.Entities.Models
{
    public class Test : BaseEntity
    {
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public string Description { get; set; }
        public string DescriptionAr { get; set; }

        public ICollection<ChildTest> TestChilderen { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
