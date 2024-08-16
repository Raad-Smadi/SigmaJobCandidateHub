using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SigmaJobCandidateHub.Data;
using SigmaJobCandidateHub.Models;
using SigmaJobCandidateHub.Repository.Services;

namespace SigmaJobCandidateHub.Tests.Repository.Services
{
    public class CandidateRepositoryTests
    {
        private readonly CandidateRepository _repository;
        private readonly IMemoryCache _memoryCache;
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        private const string CandidateCacheKeyPrefix = "Candidate_";

        public CandidateRepositoryTests()
        {
            // Setup for the in-memory database
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "SigmaDB")
                .Options;

            // Instantiate the DbContext
            var dbContext = new AppDbContext(_dbContextOptions);

            // Instantiate the real IMemoryCache
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            // Instantiate the CandidateRepository with both DbContext and IMemoryCache
            _repository = new CandidateRepository(dbContext, _memoryCache);
        }

        [Fact]
        public async Task CreateUpdateCandidate_AddsNewCandidate()
        {
            // Arrange
            var candidate = new Candidate
            {
                Id = 0, // New candidate
                Email = "new@example.com",
                FirstName = "NewName",
                LastName = "NewLastName",
                Comment = "New Comment"
            };

            // Act
            var result = await _repository.CreateUpdateCandidate(candidate);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0); // Verify that Id is generated

            // Check if cache was updated
            var cacheKey = $"{CandidateCacheKeyPrefix}{candidate.Email}";
            var cachedCandidate = _memoryCache.Get<Candidate>(cacheKey);

            Assert.NotNull(cachedCandidate);
            Assert.Equal(candidate.Email, cachedCandidate.Email);
            Assert.Equal(candidate.FirstName, cachedCandidate.FirstName);
            Assert.Equal(candidate.LastName, cachedCandidate.LastName);
            Assert.Equal(candidate.Comment, cachedCandidate.Comment);
        }

        [Fact]
        public async Task CreateUpdateCandidate_UpdatesExistingCandidate()
        {
            // Arrange
            var dbContext = new AppDbContext(_dbContextOptions);
            dbContext.Candidates.Add(new Candidate
            {
                Id = 3,
                Email = "test@example.com",
                FirstName = "OldName",
                LastName = "LastName",
                Comment = "Old Comment"
            });
            await dbContext.SaveChangesAsync();

            var candidate = new Candidate
            {
                Id = 3,
                Email = "test@example.com",
                FirstName = "UpdatedName",
                LastName = "UpdatedLastName",
                Comment = "Updated Comment"
            };

            // Act
            var result = await _repository.CreateUpdateCandidate(candidate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(candidate.FirstName, result.FirstName);
            Assert.Equal(candidate.Email, result.Email);
            Assert.Equal("UpdatedName", result.FirstName);
            Assert.Equal("UpdatedLastName", result.LastName);
            Assert.Equal("Updated Comment", result.Comment);

            var cacheKey = $"{CandidateCacheKeyPrefix}{candidate.Email}";
            var cachedCandidate = _memoryCache.Get<Candidate>(cacheKey);

            Assert.NotNull(cachedCandidate);
            Assert.Equal(candidate.Email, cachedCandidate.Email);
            Assert.Equal(candidate.FirstName, cachedCandidate.FirstName);
            Assert.Equal(candidate.LastName, cachedCandidate.LastName);
            Assert.Equal(candidate.Comment, cachedCandidate.Comment);
        }

        [Fact]
        public async Task CreateUpdateCandidate_ThrowsExceptionForMissingRequiredFields()
        {
            // Arrange
            var candidate = new Candidate
            {
                Id = 0, // New candidate
                Email = null, // Missing Email
                FirstName = null, // Missing FirstName
                LastName = null, // Missing LastName
                Comment = null // Missing Comment
            };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _repository.CreateUpdateCandidate(candidate));
        }

        [Fact]
        public async Task CreateUpdateCandidate_DoesNotCacheIfNoChange()
        {
            // Arrange
            var dbContext = new AppDbContext(_dbContextOptions);
            var existingCandidate = new Candidate
            {
                Id = 4,
                Email = "nocache@example.com",
                FirstName = "NoChange",
                LastName = "NoChange",
                Comment = "No Change"
            };
            dbContext.Candidates.Add(existingCandidate);
            await dbContext.SaveChangesAsync();

            var candidateToCheck = new Candidate
            {
                Id = 4,
                Email = "nocache@example.com",
                FirstName = "NoChange",
                LastName = "NoChange",
                Comment = "No Change"
            };

            // Act
            var result = await _repository.CreateUpdateCandidate(candidateToCheck);

            // Assert
            var cacheKey = $"{CandidateCacheKeyPrefix}{candidateToCheck.Email}";

            // Ensure the cache was updated correctly
            var cachedCandidate = _memoryCache.Get<Candidate>(cacheKey);

            // Check if the cache contains the same value as before
            Assert.NotNull(cachedCandidate); // Cache should not be null
            Assert.Equal(candidateToCheck.Email, cachedCandidate.Email);
            Assert.Equal(candidateToCheck.FirstName, cachedCandidate.FirstName);
            Assert.Equal(candidateToCheck.LastName, cachedCandidate.LastName);
            Assert.Equal(candidateToCheck.Comment, cachedCandidate.Comment);
        }

        [Fact]
        public async Task CreateUpdateCandidate_CachesUpdatedCandidateData()
        {
            // Arrange
            var dbContext = new AppDbContext(_dbContextOptions);
            var existingCandidate = new Candidate
            {
                Id = 5,
                Email = "updatecache@example.com",
                FirstName = "OldName",
                LastName = "OldLastName",
                Comment = "Old Comment"
            };
            dbContext.Candidates.Add(existingCandidate);
            await dbContext.SaveChangesAsync();

            var updatedCandidate = new Candidate
            {
                Id = 5,
                Email = "updatecache@example.com",
                FirstName = "NewName",
                LastName = "NewLastName",
                Comment = "New Comment"
            };

            // Act
            var result = await _repository.CreateUpdateCandidate(updatedCandidate);

            // Assert
            var cacheKey = $"{CandidateCacheKeyPrefix}{updatedCandidate.Email}";
            var cachedCandidate = _memoryCache.Get<Candidate>(cacheKey);

            Assert.NotNull(cachedCandidate);
            Assert.Equal(updatedCandidate.FirstName, cachedCandidate.FirstName);
            Assert.Equal(updatedCandidate.LastName, cachedCandidate.LastName);
            Assert.Equal(updatedCandidate.Comment, cachedCandidate.Comment);
        }

        [Fact]
        public async Task CreateUpdateCandidate_CachesNewCandidate()
        {
            // Arrange
            var candidate = new Candidate
            {
                Id = 0, // New candidate
                Email = "newcache@example.com",
                FirstName = "NewName",
                LastName = "NewLastName",
                Comment = "New Comment"
            };

            // Act
            var result = await _repository.CreateUpdateCandidate(candidate);

            // Assert
            var cacheKey = $"{CandidateCacheKeyPrefix}{candidate.Email}";
            var cachedCandidate = _memoryCache.Get<Candidate>(cacheKey);

            Assert.NotNull(cachedCandidate);
            Assert.Equal(candidate.FirstName, cachedCandidate.FirstName);
            Assert.Equal(candidate.LastName, cachedCandidate.LastName);
            Assert.Equal(candidate.Comment, cachedCandidate.Comment);
        }
    }
}
