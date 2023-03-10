using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewerController : Controller
{
    private readonly IMapper _mapper;
    private readonly IReviewersRepository _reviewersRepository;


    public ReviewerController(
        IReviewersRepository reviewersRepository,
        IMapper mapper)
    {
        _reviewersRepository = reviewersRepository;


        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
    public IActionResult GetReviewers()
    {
        var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewersRepository.GetReviewers());

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(reviewers);
    }

    [HttpGet("{reviewerId}")]
    [ProducesResponseType(200, Type = typeof(Reviewer))]
    [ProducesResponseType(400)]
    public IActionResult GetReviewer(int reviewerId)
    {
        if (!_reviewersRepository.ReviewerExists(reviewerId))
            return NotFound();

        var reviewer = _mapper.Map<ReviewerWithReviewsDto>(_reviewersRepository.GetReviewer(reviewerId));

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(reviewer);
    }

    [HttpGet("{reviewerId}/reviews")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
    [ProducesResponseType(400)]
    public IActionResult GetReviewsByReviewer(int reviewerId)
    {
        if (!_reviewersRepository.ReviewerExists(reviewerId))
            return NotFound();

        var reviews = _mapper.Map<List<ReviewDto>>(_reviewersRepository.GetReviewsByReviewer(reviewerId));

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(reviews);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateReviewer([FromBody] ReviewerDto reviewerCreate)
    {
        if (reviewerCreate == null)
            return BadRequest();

        var reviewerMap = _mapper.Map<Reviewer>(reviewerCreate);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_reviewersRepository.CreateReviewer(reviewerMap))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }

    [HttpPut("{reviewerId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult UpdateReviewer(int reviewerId, [FromBody] ReviewerDto updateReviewer)
    {
        if (updateReviewer == null)
            return BadRequest();

        if (reviewerId != updateReviewer.Id)
            return BadRequest();

        if (!_reviewersRepository.ReviewerExists(reviewerId))
            return NotFound();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var reviewerMap = _mapper.Map<Reviewer>(updateReviewer);

        if (!_reviewersRepository.UpdateReviewer(reviewerMap))
        {
            ModelState.AddModelError("", "Something went wrong while updating reviewer");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully updated");
    }

    [HttpDelete("{reviewerId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult RemoveReviewer(int reviewerId)
    {
        if (!_reviewersRepository.ReviewerExists(reviewerId))
            return NotFound();

        var reviewerToDelete = _reviewersRepository.GetReviewer(reviewerId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_reviewersRepository.DeleteReviewer(reviewerToDelete))
        {
            ModelState.AddModelError("", "Something went wrong when removing reviewer");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
}