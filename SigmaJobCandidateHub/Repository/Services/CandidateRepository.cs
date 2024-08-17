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
            // Define cache key
            var emailCacheKey = $"{CandidateCacheKeyPrefix}{candidate.Email}";

            Candidate result = memoryCache.Get<Candidate>(emailCacheKey);

            if (result == null)
            {
                // If not found, query database
                result = await appDbContext.Candidates
                    .SingleOrDefaultAsync(e => e.Id == candidate.Id || e.Email == candidate.Email);

                if (result == null)
                {
                    // Add new candidate
                    var candidateEntity = await appDbContext.Candidates.AddAsync(candidate);
                    await appDbContext.SaveChangesAsync();
                    candidate.Id = candidateEntity.Entity.Id;

                    // Update cache with the latest data
                    memoryCache.Set(emailCacheKey, candidate, GetCacheEntryOptions());
                    return candidate;
                }
            }

            // Attach entity to the DbContext in case of get result for cache
            if (appDbContext.Entry(result).State == EntityState.Detached)
            {
                appDbContext.Attach(result);
            }

            // Update existing candidate
            UpdateCandidateProperties(result, candidate);

            try
            {
                await appDbContext.SaveChangesAsync();

                // Update cache with the latest data
                memoryCache.Set(emailCacheKey, result, GetCacheEntryOptions());
            }
            catch (DbUpdateException ex)
            {
                // Log or handle exception
                throw new ApplicationException("An error occurred while saving the candidate.", ex);
            }

            return result;
        }

        private void UpdateCandidateProperties(Candidate target, Candidate source)
        {
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.PhoneNumber = source.PhoneNumber;
            target.Email = source.Email;
            target.PreferredCallStartTime = source.PreferredCallStartTime;
            target.PreferredCallEndTime = source.PreferredCallEndTime;
            target.LinkedInProfileUrl = source.LinkedInProfileUrl;
            target.GitHubProfileUrl = source.GitHubProfileUrl;
            target.Comment = source.Comment;
        }

        private MemoryCacheEntryOptions GetCacheEntryOptions()
        {
            return new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };
        }
    }
}
