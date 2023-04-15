// use SimpleCounters
db.createCollection("Counters")
db.Counters.insertMany([{ "CounterId": "863e8de9-6ad7-4f00-84c1-8bacb998e26c", "CounterValue": 20 }, { "CounterId": "863e8de9-6ad7-4f00-84c1-8bacb998e26d", "CounterValue": 20 }]);
db.Counters.find().pretty()
