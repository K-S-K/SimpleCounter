namespace SimpleCounter.Mongo
{
    public class CountersDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string CountersCollectionName { get; set; } = null!;
    }
}
