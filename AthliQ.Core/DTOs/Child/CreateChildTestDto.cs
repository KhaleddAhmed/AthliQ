using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.Child
{
    public class CreateChildTestDto
    {
        [Required]
        public int TestId { get; set; }

        [Required]
        public double TestResult { get; set; }
    }
}
