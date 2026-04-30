using DaymapInventory.Models;

namespace DaymapInventory.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetByName(string name);
    }
}
