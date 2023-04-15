using MongoDB.Driver;
using Microsoft.Extensions.Options;

using SimpleCounter.Common;

namespace SimpleCounter.Mongo
{
    public class CounterStorage : ICounterStorage
    {
        private readonly IMongoCollection<Counter> _countersCollection;

        public async Task<List<ICounterItem>> GetAsync()
        {
            var itemsIn = await _countersCollection
                .Find(_ => true).ToListAsync();

            List<ICounterItem> itemsOut = new();

            foreach (var itemIn in itemsIn)
            {
                itemsOut.Add(itemIn);
            }

            return itemsOut;
        }

        public async Task<ICounterItem?> GetAsync(Guid id)
        {
            return await _countersCollection
                .Find(x => x.CounterId == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(ICounterItem newItem)
        {
            if (newItem is Counter counter)
            {
                await _countersCollection.InsertOneAsync(counter);
            }
        }

        public async Task UpdateAsync(Guid id, ICounterItem updatedItem)
        {
            if (updatedItem is Counter counter)
            {
                await _countersCollection
                    .ReplaceOneAsync(x => x.CounterId == id, counter);
            }
        }

        public async Task RemoveAsync(Guid id)
        {
            await _countersCollection
                .DeleteOneAsync(x => x.CounterId == id);
        }

        public CounterStorage(IOptions<CountersDatabaseSettings> settings)
        {
            var mongoClient = new MongoClient(
                        settings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                settings.Value.DatabaseName);

            _countersCollection = mongoDatabase.GetCollection<Counter>(
                settings.Value.CountersCollectionName);
        }
    }
}
