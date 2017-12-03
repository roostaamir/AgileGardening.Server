using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileGardeningApp.ExteranlApis.VisionApi;
using AgileGardeningApp.Models;
using AgileGardeningApp.MonoDBClient;
using AgileGardeningApp.ServiceRepository.Interface;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace AgileGardeningApp.ServiceRepository.MongoDbImpl
{
    public class ImageProcessingServiceRepository : IImageProcessingService
    {
        private readonly IVisionService _visionService;
        private static readonly IMongoDatabase Db = MongoDbClient.Db;

        public ImageProcessingServiceRepository(IVisionService visionService)
        {
            _visionService = visionService;
        }

        public Plant GetPlantInfo(string imageData)
        {
            var keywords = _visionService.GetQueryKeywords(imageData);

            var filter = Builders<Plant>.Filter.Text(keywords);
            var projection = Builders<Plant>.Projection.MetaTextScore("score");
            var sort = Builders<Plant>.Sort.MetaTextScore("score");

            var plants = Db.GetCollection<Plant>("plants")
                .Find(filter)
                .Project<Plant>(projection)
                .Sort(sort)
                .ToList();

            return plants.FirstOrDefault(p => p.RelevanceScore >= 4.0f);
        }
    }
}
