namespace SimpleCounter.Common
{
    public interface ICounterItem
    {
        Guid CounterId { get; }
        int CounterValue { get; }
    }
}