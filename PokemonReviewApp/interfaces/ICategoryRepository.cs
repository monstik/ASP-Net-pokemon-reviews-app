using PokemonReviewApp.Models;

namespace PokemonReviewApp.interfaces;

public interface ICategoryRepository
{
    ICollection<Category> GetCategories();

    Category GetCategory(int id);

    ICollection<Pokemon> GetPokemonByCategory(int id);

    bool CategoryExists(int id);

    bool CreateCategory(Category category);
    bool UpdateCategory(Category category);
    bool DeleteCategory(Category category);
    bool Save();
}