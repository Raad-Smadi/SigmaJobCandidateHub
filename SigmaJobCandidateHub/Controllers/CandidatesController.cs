using Microsoft.AspNetCore.Mvc;
using SigmaJobCandidateHub.Models;
using SigmaJobCandidateHub.Repository.Interface;

namespace SigmaJobCandidateHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateRepository candidateRepository;

        public CandidatesController(ICandidateRepository candidateRepository)
        {
            this.candidateRepository = candidateRepository;
        }

        [HttpPost("ManageCandidate")]
        public async Task<ActionResult<Candidate>> ManageCandidate(Candidate candidate)
        {
            if (candidate == null)
                return BadRequest("Candidate object is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                return await candidateRepository.CreateUpdateCandidate(candidate);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating or update candidate record");
            }
        }
    }
}
