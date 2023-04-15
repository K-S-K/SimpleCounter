using SimpleCounter.Common;

namespace SimpleCounter.Data
{
    public class CounterData : ICounterData
    {
        private readonly IPageCounters counters;

        public CounterData(IPageCounters counters)
        {
            this.counters = counters;
        }

        public IEnumerable<ICounterItem> Counters => counters.Counters;

        public int GetCounterValue(Guid pageId)
        {
            return counters.Increment(pageId);
        }
    }
}