namespace SimpleCounter.Data
{
    public class CounterData : ICounterData
    {
        private readonly PageCounters counters = new();

        public IEnumerable<CounterItem> Counters => counters.Counters;

        public int GetCounterValue(Guid pageId)
        {
            return counters.Increment(pageId);
        }
    }
}