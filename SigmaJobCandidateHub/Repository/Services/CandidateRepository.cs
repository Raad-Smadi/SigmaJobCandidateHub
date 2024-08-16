using Microsoft.EntityFrameworkCore;
using SigmaJobCandidateHub.Data;
using SigmaJobCandidateHub.Models;
using SigmaJobCandidateHub.Repository.Interface;

namespace SigmaJobCandidateHub.Repository.Services
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly AppDbContext appDbContext;

        public CandidateRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<Candidate> CreateUpdateCandidate(Candidate candidate)
        {
            var result = await appDbContext.Candidates
                .SingleOrDefaultAsync(e => e.Id == candidate.Id || e.Email == candidate.Email);

            if (result != null)
            {
                result.FirstName = candidate.FirstName;
                result.LastName = candidate.LastName;
                result.PhoneNumber = candidate.PhoneNumber;
                result.Email = candidate.Email;
                result.PreferredCallStartTime = candidate.PreferredCallStartTime;
                result.PreferredCallEndTime = candidate.PreferredCallEndTime;
                result.LinkedInProfileUrl = candidate.LinkedInProfileUrl;
                result.GitHubProfileUrl = candidate.GitHubProfileUrl;
                result.Comment = candidate.Comment;

                await appDbContext.SaveChangesAsync();
                return result;
            }
            else
            {
                var candidateEntity = await appDbContext.Candidates.AddAsync(candidate);
                await appDbContext.SaveChangesAsync();
                candidate.Id = candidateEntity.Entity.Id;
                return candidate;
            }
        }
    }
}
