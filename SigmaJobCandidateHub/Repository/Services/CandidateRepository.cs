using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SigmaJobCandidateHub.Data;
using SigmaJobCandidateHub.Models;
using SigmaJobCandidateHub.Repository.Interface;

namespace SigmaJobCandidateHub.Repository.Services
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly IMemoryCache memoryCache;
        private const string CandidateCacheKeyPrefix = "Candidate_";

        public CandidateRepository(AppDbContext appDbContext, IMemoryCache memoryCache)
        {
            this.appDbContext = appDbContext;
            this.memoryCache = memoryCache;
        }

        public async Task<Candidate> CreateUpdateCandidate(Candidate candidate)
        {
            // Define cache keys
            var emailCacheKey = $"{CandidateCacheKeyPrefix}{candidate.Email}";

            Candidate result = memoryCache.Get<Candidate>(emailCacheKey);

            // If not found, query database
            if (result == null)
            {
                result = await appDbContext.Candidates
                    .SingleOrDefaultAsync(e => e.Id == candidate.Id || e.Email == candidate.Email);
            }

            if (result != null)
            {
                // Attach entity to the DbContext in case of get result for cache
                if (appDbContext.Entry(result).State == EntityState.Detached)
                {
                    appDbContext.Attach(result);
                }

                // Update existing candidate
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

                // Update cache with the latest data
                memoryCache.Set(emailCacheKey, result); // Cache by Email
                return result;
            }
            else
            {
                // Add new candidate
                var candidateEntity = await appDbContext.Candidates.AddAsync(candidate);
                await appDbContext.SaveChangesAsync();
                candidate.Id = candidateEntity.Entity.Id;

                // Update cache with the new candidate
                memoryCache.Set(emailCacheKey, candidate); // Cache by Email
                return candidate;
            }
        }

    }
}
