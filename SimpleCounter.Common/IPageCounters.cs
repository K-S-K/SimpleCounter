namespace SimpleCounter.Common
{
    public interface IPageCounters
    {
        IEnumerable<ICounterItem> Counters { get; }

        int Increment(Guid pageId);
    }
}