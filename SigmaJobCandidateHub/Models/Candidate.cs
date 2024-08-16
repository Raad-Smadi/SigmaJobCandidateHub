using System.ComponentModel.DataAnnotations;

namespace SigmaJobCandidateHub.Models
{
    public class Candidate
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [RegularExpression(@"^(\+?[1-9]\d{0,2})?[\s.-]?(\(?\d{1,4}\)?[\s.-]?)?(\d{1,4}[\s.-]?)\d{1,9}$",
        ErrorMessage = "The phone number is not valid.")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public TimeOnly? PreferredCallStartTime { get; set; }

        public TimeOnly? PreferredCallEndTime { get; set; }

        [RegularExpression(@"^https:\/\/(www\.)?linkedin\.com\/in\/[a-zA-Z0-9_-]+$",
        ErrorMessage = "The LinkedIn profile URL is not valid.")]
        public string LinkedInProfileUrl { get; set; }

        [RegularExpression(@"^https:\/\/(www\.)?github\.com\/[a-zA-Z0-9_-]+$",
        ErrorMessage = "The GitHub profile URL is not valid.")]
        public string GitHubProfileUrl { get; set; }

        [Required]
        public string Comment { get; set; }
    }
}
