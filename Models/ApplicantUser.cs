namespace RecruitFlow.Models
{
    public class ApplicantUser
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
    }
}
