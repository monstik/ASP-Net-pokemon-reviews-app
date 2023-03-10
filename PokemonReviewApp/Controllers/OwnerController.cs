using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OwnerController : Controller
{
    private readonly ICountryRepository _countryRepository;
    private readonly IMapper _mapper;
    private readonly IOwnerRepository _ownerRepository;

    public OwnerController(
        IOwnerRepository ownerRepository,
        ICountryRepository countryRepository,
        IMapper mapper
    )
    {
        _ownerRepository = ownerRepository;
        _countryRepository = countryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
    public IActionResult GetOwners()
    {
        var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(owners);
    }

    [HttpGet("{ownerId}")]
    [ProducesResponseType(200, Type = typeof(Owner))]
    [ProducesResponseType(400)]
    public IActionResult GetOwner(int ownerId)
    {
        if (!_ownerRepository.OwnerExists(ownerId))
            return NotFound();

        var owner = _ownerRepository.GetOwner(ownerId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(owner);
    }

    [HttpGet("{ownerId}/pokemon")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemonByOwner(int ownerId)
    {
        if (!_ownerRepository.OwnerExists(ownerId))
            return NotFound();

        var pokemons = _mapper.Map<List<PokemonDto>>(_ownerRepository.GetPokemonByOwner(ownerId));

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(pokemons);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateOwner([FromBody] OwnerDto ownerCreate, [FromQuery] int countryId)
    {
        if (ownerCreate == null)
            return BadRequest();

        var owners = _ownerRepository.GetOwners()
            .Where(o => o.LastName.Trim().ToUpper() == ownerCreate.LastName.Trim().ToUpper()).FirstOrDefault();

        if (owners != null)
        {
            ModelState.AddModelError("", "Owner already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownersMap = _mapper.Map<Owner>(ownerCreate);

        ownersMap.Country = _countryRepository.GetCountry(countryId);

        if (!_ownerRepository.CreateOwner(ownersMap))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }

    [HttpPut("{ownerId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult UpdateOwner(int ownerId, OwnerDto updateOwner)
    {
        if (updateOwner == null)
            return BadRequest();

        if (ownerId != updateOwner.Id)
            return BadRequest();

        if (!_ownerRepository.OwnerExists(ownerId))
            return NotFound();

        if (!ModelState.IsValid)
            return BadRequest();

        var ownerMap = _mapper.Map<Owner>(updateOwner);

        if (!_ownerRepository.UpdateOwner(ownerMap))
        {
            ModelState.AddModelError("", "Something went wrong while updating owner");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully updated");
    }

    [HttpDelete("{ownerId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult RemoveOwner(int ownerId)
    {
        if (!_ownerRepository.OwnerExists(ownerId))
            return NotFound();

        var ownerToDelete = _ownerRepository.GetOwner(ownerId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_ownerRepository.DeleteOwner(ownerToDelete))
        {
            ModelState.AddModelError("", "Something went wrong while removing owner");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully deleted");
    }
}