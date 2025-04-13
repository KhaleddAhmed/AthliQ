using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.Category
{
    public class UpdateCategoryDto
    {
        [Required]
        public int Id { get; set; }

        [StringLength(20, ErrorMessage = "Category Name Can Not Contain More Than 20 Character")]
        public string Name { get; set; }

        [StringLength(40, ErrorMessage = "Category Arabic Name Can Not Contain More Than 40 Character")]
        public string ArabicName { get; set; }

        [StringLength(200, ErrorMessage = "Category Description Can Not Contain More Than 200 Character")]
        public string Description { get; set; }
		
        [StringLength(400, ErrorMessage = "Category Description Can Not Contain More Than 400 Character")]
		public string DescriptionAr { get; set; }
	}
}
