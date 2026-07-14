using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RecruitFlow.Models.ViewModels
{
    public class JobApplicationViewModel
    {
        [Required, StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, Range(0, 50)]
        [Display(Name = "Experience (Years)")]
        public int ExperienceYears { get; set; }

        [Required, Range(0, 10000000)]
        [Display(Name = "Current CTC (per annum)")]
        public decimal CurrentCTC { get; set; }

        [Required, Range(0, 10000000)]
        [Display(Name = "Expected CTC (per annum)")]
        public decimal ExpectedCTC { get; set; }

        [Required]
        [Display(Name = "Job Role")]
        public string JobRole { get; set; } = string.Empty;

        [Display(Name = "Resume (PDF)")]
        public IFormFile? Resume { get; set; }
    }
}
