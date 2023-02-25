namespace SimpleCounter.Data
{
    public class PageCounterItem
    {
        private int _value = 0;

        public readonly Guid Id;
        public int Value => _value;

        public int Increment()
        {
            Interlocked.Increment(ref _value);
            return _value;
        }

        public override string ToString() => $"{_value}";

        public PageCounterItem(Guid id, int value = 0)
        {
            Id = id;
            _value = value;
        }
    }
}
