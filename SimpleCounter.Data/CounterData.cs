namespace SimpleCounter.Data
{
    public class CounterData : ICounterData
    {
        private int count = 0;

        public int GetCounterValue(Guid pageId)
        {
            return ++count;
        }
    }
}