using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.Sport
{
    public class CreateSportDto
    {
        [Required(ErrorMessage = "Sport Name Is Required")]
        [StringLength(20 , ErrorMessage = "Sport Name Can Not Contain More Than 20 Charachter")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Sport Arabic Name Is Required")]
        [StringLength(40, ErrorMessage = "Sport Arabic Name Can Not Contain More Than 30 Charachter")]
        public string ArabicName { get; set; }

        [Required(ErrorMessage = "Sport Description Is Required")]
        [StringLength(200, ErrorMessage = "Sport Description Can Not Contain More Than 200 Charachter")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The Sport Must Belongs To A Category")]
        public int CategoryId { get; set; }
    }
}
