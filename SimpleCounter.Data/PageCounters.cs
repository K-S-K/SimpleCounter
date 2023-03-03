namespace SimpleCounter.Data
{
    public class PageCounters
    {
        private readonly SemaphoreSlim semaphore = new(1, 1);
        private readonly Dictionary<Guid, PageCounterItem> _data = new();

        public int Increment(Guid pageId)
        {
            semaphore.Wait();

            if (!_data.TryGetValue(pageId, out PageCounterItem? item))
            {
                item = new(pageId);
                _data.Add(pageId, item);
            }

            int counterValue = item.Increment();

            semaphore.Release();

            return counterValue;
        }

        public IEnumerable<PageCounterItem> Counters => _data.Values;
    }
}
