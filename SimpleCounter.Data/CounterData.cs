namespace SimpleCounter.Data
{
    public class CounterData : ICounterData
    {
        private readonly PageCounters counters = new();

        public IEnumerable<PageCounterItem> Counters => counters.Counters;

        public int GetCounterValue(Guid pageId)
        {
            return counters.Increment(pageId);
        }
    }
}