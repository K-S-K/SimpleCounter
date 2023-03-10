namespace SimpleCounter.Data
{
    public class CounterItem
    {
        private int _value = 0;

        public Guid Id { get; private set; }
        public int Value => _value;

        public int Increment()
        {
            Interlocked.Increment(ref _value);
            return _value;
        }

        public override string ToString() => $"{_value}";

        public CounterItem(Guid id, int value = 0)
        {
            Id = id;
            _value = value;
        }
    }
}
