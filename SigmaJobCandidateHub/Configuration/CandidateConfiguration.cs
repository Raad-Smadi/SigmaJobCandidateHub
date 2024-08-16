using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SigmaJobCandidateHub.Models;

namespace SigmaJobCandidateHub.Configuration
{
    public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(e => e.Id).UseIdentityColumn();

            builder.HasIndex(e => e.Email).IsUnique();

            //Required Fields
            builder.Property(e => e.FirstName).IsRequired();
            builder.Property(e => e.LastName).IsRequired();
            builder.Property(e => e.Email).IsRequired();
            builder.Property(e => e.Comment).IsRequired();

            builder.ToTable("Candidates");
        }
    }
}
