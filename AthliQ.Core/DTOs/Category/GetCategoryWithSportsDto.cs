using AthliQ.Core.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.Category
{
    public class GetCategoryWithSportsDto
    {
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public string Description { get; set; }
        public string DescriptionAr { get; set; }
        public ICollection<string> SportsName { get; set; }
        public ICollection<string> SportsNameAr { get; set; }
    }
}
