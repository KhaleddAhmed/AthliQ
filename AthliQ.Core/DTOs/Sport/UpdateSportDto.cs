using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.Sport
{
    public class UpdateSportDto
    {
        [Required]
        public int Id { get; set; }

        [StringLength(20, ErrorMessage = "Sport Name Can Not Contain More Than 20 Charachter")]
        public string Name { get; set; }

        [StringLength(40, ErrorMessage = "Sport Arabic Name Can Not Contain More Than 40 Charachter")]
        public string ArabicName { get; set; }

        [StringLength(200, ErrorMessage = "Sport Description Can Not Contain More Than 200 Charachter")]
        public string Description { get; set; }

        public int CategoryId { get; set; }
    }
}
