using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.Entities.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public string Description { get; set; }
        public string DescriptionAr { get; set; }
        public virtual ICollection<Sport> Sports { get; set; }
        public virtual ICollection<ChildResult> ChildCategories { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
    }
}
