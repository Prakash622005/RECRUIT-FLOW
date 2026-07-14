namespace RecruitFlow.Models
{
    // Companies are pre-provisioned (seeded) - there is no public registration flow for them.
    public class Company
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
