namespace SimpleCounter.Common
{
    public interface ICounterStorage
    {
        Task CreateAsync(ICounterItem newItem);
        Task<List<ICounterItem>> GetAsync();
        Task<ICounterItem?> GetAsync(Guid id);
        Task RemoveAsync(Guid id);
        Task UpdateAsync(Guid id, ICounterItem updatedItem);
    }
}