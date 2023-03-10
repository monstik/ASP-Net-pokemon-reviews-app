using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PokemonController : Controller
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IReviewRepository _reviewRepository;

    public PokemonController(
        IPokemonRepository pokemonRepository,
        ICategoryRepository categoryRepository,
        IOwnerRepository ownerRepository,
        IReviewRepository reviewRepository,
        IMapper mapper)
    {
        _pokemonRepository = pokemonRepository;
        _categoryRepository = categoryRepository;
        _ownerRepository = ownerRepository;
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
    public IActionResult GetPokemons()
    {
        var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(pokemons);
    }

    [HttpGet("{pokemonId}")]
    [ProducesResponseType(200, Type = typeof(Pokemon))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemon(int pokemonId)
    {
        if (!_pokemonRepository.PokemonExists(pokemonId))
            return NotFound();

        var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokemonId));

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(pokemon);
    }

    [HttpGet("{pokemonId}/rating")]
    [ProducesResponseType(200, Type = typeof(decimal))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemonRating(int pokemonId)
    {
        if (!_pokemonRepository.PokemonExists(pokemonId))
            return NotFound();

        var pokemonRating = _pokemonRepository.GetPokemonRating(pokemonId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(pokemonRating);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int categoryId,
        [FromBody] PokemonDto pokemonCreate)
    {
        if (pokemonCreate == null)
            return BadRequest();

        var pokemons = _pokemonRepository.GetPokemons()
            .Where(p => p.Name.Trim().ToUpper() == pokemonCreate.Name.Trim().ToUpper()).FirstOrDefault();

        if (pokemons != null)
        {
            ModelState.AddModelError("", "Pokemon already exists");
            return StatusCode(422, ModelState);
        }

        var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }

    [HttpPut("{pokemonId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult UpdatePokemon(int pokemonId, [FromBody] PokemonDto updatePokemon)
    {
        if (updatePokemon == null)
            return BadRequest();

        if (pokemonId != updatePokemon.Id)
            return BadRequest();

        if (!_pokemonRepository.PokemonExists(pokemonId))
            return NotFound();

        var pokemonMap = _mapper.Map<Pokemon>(updatePokemon);


        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_pokemonRepository.UpdatePokemon(pokemonMap))
        {
            ModelState.AddModelError("", "Something went wrong while updating pokemon");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully updated");
    }

    [HttpDelete("{pokemonId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult RemovePokemon(int pokemonId)
    {
        if (!_pokemonRepository.PokemonExists(pokemonId))
            return NotFound();
        var reviewsToDelete = _reviewRepository.GetReviewsOfAPokemon(pokemonId).ToList();
        var pokemonToDelete = _pokemonRepository.GetPokemon(pokemonId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_reviewRepository.DeleteReviews(reviewsToDelete))
        {
            ModelState.AddModelError("", "Something went wrong when removing reviews");
            return StatusCode(500, ModelState);
        }

        if (!_pokemonRepository.DeletePokemon(pokemonToDelete))
        {
            ModelState.AddModelError("", "Something went wrong when removing pokemon");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully deleted");
    }
}