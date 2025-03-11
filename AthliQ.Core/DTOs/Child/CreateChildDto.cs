using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AthliQ.Core.DTOs.Child
{
    public class CreateChildDto
    {
        [Required(ErrorMessage = "Name is Required")]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Gender is Required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Date of birth is Required")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "School is Required")]
        public string SchoolName { get; set; }

        public string? ClubName { get; set; }

        [Required(ErrorMessage = "Doctor Approval is Required")]
        public bool IsAgreeDoctorApproval { get; set; }

        [Required(ErrorMessage = "Blood Test is Required")]
        public bool IsNormalBloodTest { get; set; }

        public int SportHistoryId { get; set; }

        [Required]
        public int ParentSportHistoryId { get; set; }

        [Required]
        public int SportPreferenceId { get; set; }

        [Required(ErrorMessage = "Height is Required")]
        public double Height { get; set; }

        [Required(ErrorMessage = "Weight is Required")]
        public double Weight { get; set; }

        public IFormFile FrontImage { get; set; }

        public string? FrontImageName { get; set; }

        public IFormFile SideImage { get; set; }

        public string? SideImageName { get; set; }

        public List<CreateChildTestDto> CreateChildTestDtos { get; set; }
    }
}
