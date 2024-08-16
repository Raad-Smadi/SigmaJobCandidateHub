using Microsoft.EntityFrameworkCore;
using SigmaJobCandidateHub.Configuration;
using SigmaJobCandidateHub.Models;

namespace SigmaJobCandidateHub.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public virtual DbSet<Candidate> Candidates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CandidateConfiguration());
        }
    }
}
