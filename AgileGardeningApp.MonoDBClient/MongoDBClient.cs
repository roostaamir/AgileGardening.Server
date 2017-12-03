using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileGardeningApp.MonoDBClient
{
    public class MongoDbClient
    {
        private MongoDbClient() { }

        private const string CONNECTION_STRING = "mongodb://localhost:27017";
        private const string DEFAULT_DB = "agile_gardening_db";
        private static readonly MongoClient _mongoClient = new MongoClient(CONNECTION_STRING);

        public static IMongoDatabase Db => _mongoClient.GetDatabase(DEFAULT_DB);
    }
}
