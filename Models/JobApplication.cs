namespace RecruitFlow.Models
{
    public class JobApplication
    {
        public int Id { get; set; }

        public int ApplicantUserId { get; set; }
        public ApplicantUser? ApplicantUser { get; set; }

        public string FullName { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public decimal CurrentCTC { get; set; }
        public decimal ExpectedCTC { get; set; }
        public string JobRole { get; set; } = string.Empty;

        public string ResumeFileName { get; set; } = string.Empty;
        public string ResumeFilePath { get; set; } = string.Empty;

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public DateTime? InterviewDate { get; set; }
        public TimeSpan? InterviewTime { get; set; }
        public string? InterviewVenue { get; set; }

        public DateTime AppliedOn { get; set; } = DateTime.Now;
    }
}
