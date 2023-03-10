﻿using PokemonReviewApp.Models;

namespace PokemonReviewApp.interfaces;

public interface IOwnerRepository
{
    ICollection<Owner> GetOwners();

    Owner GetOwner(int ownerId);

    ICollection<Owner> GetOwnerOfAPokemon(int pokemonId);

    ICollection<Pokemon> GetPokemonByOwner(int ownerId);

    bool OwnerExists(int ownerId);

    bool CreateOwner(Owner owner);
    bool UpdateOwner(Owner owner);
    bool DeleteOwner(Owner owner);

    bool Save();
}