using System.ComponentModel.DataAnnotations;

namespace RecruitFlow.Models.ViewModels
{
    public class ScheduleInterviewViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime InterviewDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan InterviewTime { get; set; }

        [Required, StringLength(200)]
        public string InterviewVenue { get; set; } = string.Empty;
    }
}
