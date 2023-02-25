namespace SimpleCounter.Data
{
    public interface ICounterData
    {
        int GetCounterValue(Guid pageId);
    }
}