using SimpleCounter.Common;

namespace SimpleCounter.Data
{
    public class PageCounters : IPageCounters
    {
        private readonly SemaphoreSlim semaphore = new(1, 1);
        private readonly ICounterStorage? counterStorage = null;
        private readonly Dictionary<Guid, ICounterItem> _data = new();

        public PageCounters(ICounterStorage storage)
        {
            counterStorage = storage;

            List<ICounterItem> data = counterStorage.GetAsync().Result;

            foreach(var item in data)
            {
                _data[item.CounterId] = new CounterItem(item.CounterId, item.CounterValue);
            }
        }

        public int Increment(Guid pageId)
        {
            semaphore.Wait();

            if (!_data.TryGetValue(pageId, out ICounterItem? item))
            {
                item = new CounterItem(pageId);
                _data.Add(pageId, item);
            }

            int counterValue = 0;
            if (item is CounterItem realItem)
            {
                counterValue = realItem.Increment();
            }

            semaphore.Release();

            return counterValue;
        }

        public IEnumerable<ICounterItem> Counters => _data.Values;
    }
}
