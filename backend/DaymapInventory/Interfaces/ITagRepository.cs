using DaymapInventory.Models;

namespace DaymapInventory.Interfaces
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<Tag?> GetByName(string name);
        Task<IEnumerable<Tag>> GetDefaults();

        // Item tag assignment (SCRUM-93)
        Task<bool> IsAssignedToItem(int itemId, int tagId);
        Task AssignToItem(int itemId, int tagId);
        Task RemoveFromItem(int itemId, int tagId);
        Task<IEnumerable<Tag>> GetTagsForItem(int itemId);
    }
}