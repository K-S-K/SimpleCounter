namespace SimpleCounter.Data
{
    public class CounterData : ICounterData
    {
        private readonly PagedCounters counters = new();

        public int GetCounterValue(Guid pageId)
        {
            return counters.Increment(pageId);
        }
    }
}