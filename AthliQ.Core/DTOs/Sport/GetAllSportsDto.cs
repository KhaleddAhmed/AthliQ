using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.Sport
{
    public class GetAllSportsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ArabicName { get; set; }
		public string Description { get; set; }
		public string DescriptionAr { get; set; }
        public int CategoryId { get; set; }
    }
}
