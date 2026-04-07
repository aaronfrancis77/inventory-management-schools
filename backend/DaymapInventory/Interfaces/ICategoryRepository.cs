using DaymapInventory.Models;

namespace DaymapInventory.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Category? GetByName(string name);
    }
}
