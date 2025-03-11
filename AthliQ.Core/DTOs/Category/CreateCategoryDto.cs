using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.Category
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category Name Is Required")]
        [StringLength(20 , ErrorMessage = "Category Name Can Not Contain More Than 20 Character")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Category Arabic Name Is Required")]
        [StringLength(40, ErrorMessage = "Category Name Can Not Contain More Than 40 Character")]
        public string ArabicName { get; set; }

        [Required(ErrorMessage = "Category Description Is Required")]
        [StringLength(200, ErrorMessage = "Category Name Can Not Contain More Than 200 Character")]
        public string Description { get; set; }
    }
}
