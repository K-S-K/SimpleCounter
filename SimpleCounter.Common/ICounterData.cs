namespace SimpleCounter.Common
{
    public interface ICounterData
    {
        public IEnumerable<ICounterItem> Counters { get; }
        int GetCounterValue(Guid pageId);
    }
}