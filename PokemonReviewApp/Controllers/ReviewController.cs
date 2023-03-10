using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : Controller
{
    private readonly IMapper _mapper;
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IReviewersRepository _reviewersRepository;
    private readonly IReviewRepository _reviewRepository;

    public ReviewController(IReviewRepository reviewRepository, IReviewersRepository reviewersRepository,
        IPokemonRepository pokemonRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _reviewersRepository = reviewersRepository;
        _pokemonRepository = pokemonRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
    public IActionResult GetReviews()
    {
        // var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());
        var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(reviews);
    }

    [HttpGet("{reviewId}")]
    [ProducesResponseType(200, Type = typeof(Review))]
    [ProducesResponseType(400)]
    public IActionResult GetReview(int reviewId)
    {
        if (!_reviewRepository.ReviewExists(reviewId))
            return NotFound();

        var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(review);
    }

    [HttpGet("pokemon/{pokemonId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
    [ProducesResponseType(400)]
    public IActionResult GetReviewOfAPokemon(int pokemonId)
    {
        var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfAPokemon(pokemonId));

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(reviews);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokemonId,
        [FromBody] ReviewDto reviewCreate)
    {
        if (reviewCreate == null)
            return BadRequest();

        var reviews = _reviewRepository.GetReviews()
            .Where(r => r.Title.Trim().ToUpper() == reviewCreate.Title.Trim().ToUpper()).FirstOrDefault();

        if (reviews != null)
        {
            ModelState.AddModelError("", "Review already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var reviewMap = _mapper.Map<Review>(reviewCreate);
        var pokemon = _pokemonRepository.GetPokemon(pokemonId);

        if (pokemon == null)
            return BadRequest();

        reviewMap.Pokemon = pokemon;

        var reviewer = _reviewersRepository.GetReviewer(reviewerId);

        if (reviewer == null)
            return BadRequest();

        reviewMap.Reviewer = reviewer;


        if (!_reviewRepository.CreateReview(reviewMap))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }

    [HttpPut("{reviewId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto updateReview)
    {
        if (updateReview == null)
            return BadRequest();

        if (reviewId != updateReview.Id)
            return BadRequest();

        if (!_reviewRepository.ReviewExists(reviewId))
            return NotFound();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var reviewMap = _mapper.Map<Review>(updateReview);

        if (!_reviewRepository.UpdateReview(reviewMap))
        {
            ModelState.AddModelError("", "Something went wrong while updating review");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully updating");
    }

    [HttpDelete("{reviewId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult RemoveReview(int reviewId)
    {
        if (!_reviewRepository.ReviewExists(reviewId))
            return NotFound();

        var reviewToDelete = _reviewRepository.GetReview(reviewId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_reviewRepository.DeleteReview(reviewToDelete))
        {
            ModelState.AddModelError("", "Something went wrong when removing review");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully deleted");
    }
}