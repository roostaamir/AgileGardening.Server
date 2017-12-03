using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileGardeningApp.Models;
using AgileGardeningApp.MonoDBClient;
using AgileGardeningApp.ResultModels;

namespace AgileGardeningApp.ServiceRepository.Interface
{
    public interface IPlantInfoService
    {
        PlantResult GetPlantInfo(string imageData, string userToekn);
        List<PlantResult> GetPlantHistory(string userToken);
        List<PlantResult>GetPlantFavorites(string userToken);
        void AddToFavorite(int plantId, string userToken);
        void RemoveFromFavorite(int plantId, string userToken);
    }
}
