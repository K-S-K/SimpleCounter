namespace SimpleCounter.Data
{
    public interface ICounterData
    {
        public IEnumerable<CounterItem> Counters { get; }
        int GetCounterValue(Guid pageId);
    }
}