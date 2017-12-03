using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileGardeningApp.Models;
using AgileGardeningApp.ServiceRepository.Interface;
using AgileGardeningApp.ServiceRepository.SqlDbImpl.Helper;
using AgileGardeningApp.ExteranlApis.VisionApi;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Reflection.Emit;
using AgileGardeningApp.AppConstants;
using AgileGardeningApp.ResultModels;

namespace AgileGardeningApp.ServiceRepository.SqlDbImpl
{
    public class PlantsInfoServiceRepository : IPlantInfoService
    {
        private readonly IVisionService _visionService;

        public PlantsInfoServiceRepository(IVisionService visionService)
        {
            _visionService = visionService;
        }

        public PlantResult GetPlantInfo(string imageData, string userToken)
        {
            var keywords = _visionService.GetQueryKeywords(imageData);
           
            using (var context = new AppDbContext())
            {
                var searchParam = new SqlParameter("@SearchTerm", keywords);
                var plantId = context.Database.SqlQuery<int>("FindPlant @SearchTerm", searchParam)
                    .ToList().FirstOrDefault();

                var plant = (from x in context.Plants
                    where x.PlantId == plantId
                    select x).FirstOrDefault();

                if (plant == null) return null;

                var plantHistory = (from x in context.UsersPlantsInfos
                        .Include(e => e.User)
                        .Include(e => e.Plant)
                    where x.User.LoginToken == userToken && x.Plant.PlantId == plant.PlantId
                    select x).FirstOrDefault();

                var isFlowerFavorite = false;
                if (plantHistory == null)
                {
                    var user = (from x in context.Users
                        where x.LoginToken == userToken
                        select x).FirstOrDefault();

                    context.UsersPlantsInfos.Add(new UsersPlantsInfo()
                    {
                        User = user,
                        Plant = plant,
                        IsFavourite = false
                    });

                    context.SaveChanges();
                }
                else
                    isFlowerFavorite = plantHistory.IsFavourite;

                return new PlantResult()
                {
                    PlantId = plant.PlantId,
                    Name = plant.Name,
                    ImageUrl = Appconstants.DomainUrl + plant.Imageurl,
                    Info = plant.Description,
                    IsFavorite = isFlowerFavorite
                };
            }
        }

        public List<PlantResult> GetPlantHistory(string userToken)
        {
            using (var context = new AppDbContext())
            {
                return (from x in context.UsersPlantsInfos.Include(x => x.User).Include(x => x.Plant)
                    where x.User.LoginToken == userToken
                    select new PlantResult()
                    {
                        PlantId = x.Plant.PlantId,
                        Name = x.Plant.Name,
                        ImageUrl = Appconstants.DomainUrl + x.Plant.Imageurl,
                        Info = x.Plant.Description,
                        IsFavorite = x.IsFavourite
                    }).ToList();
            }
        }

        public List<PlantResult> GetPlantFavorites(string userToken)
        {
            using (var context = new AppDbContext())
            {
                return (from x in context.UsersPlantsInfos.Include(x => x.User).Include(x => x.Plant)
                    where x.User.LoginToken == userToken && x.IsFavourite
                    select new PlantResult()
                    {
                        PlantId = x.Plant.PlantId,
                        Name = x.Plant.Name,
                        ImageUrl = Appconstants.DomainUrl + x.Plant.Imageurl,
                        Info = x.Plant.Description,
                        IsFavorite = x.IsFavourite
                    }).ToList();
            }
        }

        public void AddToFavorite(int plantId, string userToken)
        {
            using (var context = new AppDbContext())
            {
                var plantHistory = (from x in context.UsersPlantsInfos
                        .Include(e => e.User)
                        .Include(e => e.Plant)
                    where x.User.LoginToken == userToken && x.Plant.PlantId == plantId
                    select x).FirstOrDefault();
                if (plantHistory != null) plantHistory.IsFavourite = true;
                context.SaveChanges();
            }
        }

        public void RemoveFromFavorite(int plantId, string userToken)
        {
            using (var context = new AppDbContext())
            {
                var plantHistory = (from x in context.UsersPlantsInfos
                        .Include(e => e.User)
                        .Include(e => e.Plant)
                    where x.User.LoginToken == userToken && x.Plant.PlantId == plantId
                    select x).FirstOrDefault();
                if (plantHistory != null) plantHistory.IsFavourite = false;
                context.SaveChanges();
            }
        }
    }
}
