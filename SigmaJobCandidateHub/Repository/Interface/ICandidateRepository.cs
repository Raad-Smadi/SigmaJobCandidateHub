using SigmaJobCandidateHub.Models;

namespace SigmaJobCandidateHub.Repository.Interface
{
    public interface ICandidateRepository
    {
        Task<Candidate> CreateUpdateCandidate(Candidate candidate);
    }
}
