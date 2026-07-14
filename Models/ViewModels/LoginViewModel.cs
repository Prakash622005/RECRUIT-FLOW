using System.ComponentModel.DataAnnotations;

namespace RecruitFlow.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email / Username")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
