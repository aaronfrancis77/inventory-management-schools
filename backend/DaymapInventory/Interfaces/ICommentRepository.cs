using DaymapInventory.Models;

namespace DaymapInventory.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetByItemId(int itemId);
    }
}