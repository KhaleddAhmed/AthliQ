using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.Test
{
    public class CreateTestDto
    {
        [Required(ErrorMessage = "Test Name is Required")]
        [MaxLength(70, ErrorMessage = "This is Above the Max length of test")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Test arabic Name is Required")]
        [MaxLength(70, ErrorMessage = "This is Above the Max length of test")]
        public string ArabicName { get; set; }

        [Required(ErrorMessage = "Test Description is Required")]
        [MaxLength(100, ErrorMessage = "This is Above the Max length of test Description")]
        public string Description { get; set; }

		[Required(ErrorMessage = "Test Description is Required")]
		[MaxLength(300, ErrorMessage = "This is Above the Max length of test Description")]
		public string DescriptionAr { get; set; }

		[Required(ErrorMessage = "Every test must has Category")]
        public int CategoryId { get; set; }
    }
}
