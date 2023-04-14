namespace SimpleCounter.Data
{
    public class CounterItem
    {
        private int _value = 0;

        public Guid CounterId { get; private set; }
        public int CounterValue => _value;

        public int Increment()
        {
            Interlocked.Increment(ref _value);
            return _value;
        }

        public override string ToString() => $"{_value}";

        public CounterItem(Guid id, int value = 0)
        {
            CounterId = id;
            _value = value;
        }
    }
}
