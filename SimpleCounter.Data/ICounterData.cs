namespace SimpleCounter.Data
{
    public interface ICounterData
    {
        public IEnumerable<PageCounterItem> Counters { get; }
        int GetCounterValue(Guid pageId);
    }
}