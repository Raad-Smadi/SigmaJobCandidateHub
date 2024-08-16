namespace SigmaJobCandidateHub.Models
{
    public class Candidate
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public TimeOnly? PreferredCallStartTime { get; set; }
        public TimeOnly? PreferredCallEndTime { get; set; }
        public string LinkedInProfileUrl { get; set; }
        public string GitHubProfileUrl { get; set; }
        public string Comment { get; set; }
    }
}
