using AgileGardeningApp.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AgileGardeningApp.ServiceRepository.Interface;
using AgileGardeningApp.Helpers;
using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;

namespace AgileGardeningApp.Controllers
{
    public class Inputs
    {
        public class SearchInput
        {
            public string Query { get; set; }
        }

        public class ImageSeachInput
        {
            public string ImageUrl { get; set; }
        }
    }

    public class VisionInput
    {
        public class UrlHolder
        {
            public string Url { get; set; }
        }

        public class ImageDataHolder
        {
            public UrlHolder Image { get; set; }
        }

        public class DataHolder
        {
            public ImageDataHolder Data { get; set; }
        }

        public class RootInput
        {
            public List<DataHolder> Inputs { get; set; }
        }
    } 

    public class ConceptList
    {
        public List<Concept> Concepts { get; set; }
    }

    public class Concept
    {
        public string id { get; set; }
        public string name { get; set; }
        public double value { get; set; }
        public string app_id { get; set; }
    }

    [RoutePrefix("api/v1")]
    public class PlantsController : ApiController
    {
        private const string TEMP_FILE_PATH = "Img/Temp/";

        private readonly IPlantInfoService _plantInfoServiceRepository;
        private const int RANDOM_STRING_LENGTH = 5;

        public PlantsController(IPlantInfoService plantInfoServiceRepository)
        {
            _plantInfoServiceRepository = plantInfoServiceRepository;
        }

        [Route("GetPlantByImage")]
        [HttpPost]
        public async Task<HttpResponseMessage> GetDetails()
        {
            try
            {
                //getting the user's token if exists
                var userToken = Request.Headers.FirstOrDefault(x => x.Key == "auth_token").Value?.FirstOrDefault();
                if (userToken == null)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);

                var provider = await Request.Content.ReadAsMultipartAsync<InMemoryMultipartFormDataStreamProvider>(new InMemoryMultipartFormDataStreamProvider());
                NameValueCollection formData = provider.FormData;
                //access files  
                IList<HttpContent> files = provider.Files;
                HttpContent file = files[0];
                string originalFileName = file.Headers.ContentDisposition.FileName.Trim('\"');
                string extension = Path.GetExtension(originalFileName);

                if (extension.ToLower() != ".jpg" && extension.ToLower() != ".png" && extension.ToLower() != ".jpeg")
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                string directory = HostingEnvironment.MapPath("~/" + TEMP_FILE_PATH);
                string newFileName = GenerateRandomString(RANDOM_STRING_LENGTH) + extension;
                string FullPath = Path.Combine(directory, newFileName);
                while (File.Exists(FullPath))
                {
                    newFileName = GenerateRandomString(RANDOM_STRING_LENGTH) + extension;
                    FullPath = Path.Combine(directory, newFileName);
                }
                Stream input = await file.ReadAsStreamAsync();
                using (Stream s = File.OpenWrite(FullPath))
                {
                    input.CopyTo(s);
                    s.Close();
                }

                var base64Image = ConvertToBase64(FullPath, extension);
                var plantResult = _plantInfoServiceRepository.GetPlantInfo(base64Image, userToken);

                return plantResult != null ? Request.CreateResponse(HttpStatusCode.OK, plantResult) 
                    : Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { ex_message = ex.Message, ex_stack = ex.StackTrace, ex_inner_ex = ex.InnerException?.Message });
            }
        }

        [Route("GetPlantsHistory")]
        [HttpGet]
        public HttpResponseMessage GetPlantHistory()
        {
            try
            {
                var userToken = Request.Headers.FirstOrDefault(x => x.Key == "auth_token").Value?.FirstOrDefault();
                if (userToken == null)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                return Request.CreateResponse(HttpStatusCode.OK,
                    _plantInfoServiceRepository.GetPlantHistory(userToken));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { ex_message = ex.Message, ex_stack = ex.StackTrace, ex_inner_ex = ex.InnerException?.Message });
            }
        }

        [Route("GetFavoritesHistory")]
        [HttpGet]
        public HttpResponseMessage GetFavoritesHistory()
        {
            try
            {
                var userToken = Request.Headers.FirstOrDefault(x => x.Key == "auth_token").Value?.FirstOrDefault();
                if (userToken == null)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                return Request.CreateResponse(HttpStatusCode.OK,
                    _plantInfoServiceRepository.GetPlantFavorites(userToken));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { ex_message = ex.Message, ex_stack = ex.StackTrace, ex_inner_ex = ex.InnerException?.Message });
            }
        }

        [Route("AddPlantToFavorite/{plantId}")]
        [HttpPost]
        public HttpResponseMessage AddPlantToFavorite(int plantId)
        {
            try
            {
                var userToken = Request.Headers.FirstOrDefault(x => x.Key == "auth_token").Value?.FirstOrDefault();
                if (userToken == null)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                _plantInfoServiceRepository.AddToFavorite(plantId, userToken);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { ex_message = ex.Message, ex_stack = ex.StackTrace, ex_inner_ex = ex.InnerException?.Message });
            }
        }

        [Route("RemovePlantFromFavorite/{plantId}")]
        [HttpPost]
        public HttpResponseMessage RemovePlantFromFavorite(int plantId)
        {
            try
            {
                var userToken = Request.Headers.FirstOrDefault(x => x.Key == "auth_token").Value?.FirstOrDefault();
                if (userToken == null)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                _plantInfoServiceRepository.RemoveFromFavorite(plantId, userToken);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { ex_message = ex.Message, ex_stack = ex.StackTrace, ex_inner_ex = ex.InnerException?.Message });
            }
        }

        //-----------------------------------------//

        //[Route("GetPlantName")]
        //[HttpPost]
        //public HttpResponseMessage GetDetails([FromBody] Inputs.ImageSeachInput searchInput)
        //{
        //    var plant = _plantInfoServiceRepository.GetPlantInfo(searchInput.ImageUrl);
        //    return Request.CreateResponse(HttpStatusCode.OK,
        //        $"This plant is most probably a '{plant?.Name ?? "nothing"}'");

        //}

        //[Route("GetPlantName")]
        //[HttpPost]
        //public HttpResponseMessage GetDetails([FromBody] Inputs.ImageSeachInput searchInput)
        //{
        //    var client = new MongoClient("mongodb://localhost:27017");
        //    var db = client.GetDatabase("agile_gardeing_db");

        //    var restClient = new RestClient("https://api.clarifai.com/v2/");
        //    var request = new RestRequest("models/aaa03c23b3724a16a56b629203edc62c/outputs", Method.POST);

        //    //creating the input object
        //    var visionInput = new VisionInput.RootInput()
        //    {
        //        Inputs = new List<VisionInput.DataHolder>()
        //        {
        //            new VisionInput.DataHolder()
        //            {
        //                Data = new VisionInput.ImageDataHolder()
        //                {
        //                    Image = new VisionInput.UrlHolder()
        //                    {
        //                        Url = searchInput.ImageUrl
        //                    }
        //                }
        //            }
        //        }
        //    };
        //    //end creating the input object

        //    request.AddJsonBody(visionInput);
        //    request.AddHeader("Authorization", "Key c6e6c6adb1e049829cd03e0e9a0b8c19");
        //    request.AddHeader("Content-Type", "application/json");

        //    IRestResponse response = restClient.Execute(request);
        //    JObject responseRoot = JObject.Parse(response.Content);
        //    string concepts = responseRoot["outputs"][0]["data"].ToString();
        //    var result = new JavaScriptSerializer().Deserialize<ConceptList>(concepts);

        //    var searchQueryBuilder = new StringBuilder();
        //    foreach (var concept in result.Concepts)
        //    {
        //        searchQueryBuilder.Append(concept.name).Append(" ");
        //    }
        //    searchQueryBuilder.Length -= 1;

        //    var filter = Builders<Plant>.Filter.Text(searchQueryBuilder.ToString()) &
        //        Builders<Plant>.Filter.Gt("score", 4.0f);
        //    var projection = Builders<Plant>.Projection.MetaTextScore("score");
        //    var sort = Builders<Plant>.Sort.MetaTextScore("score");
        //    List<Plant> plants = db.GetCollection<Plant>("plants")
        //        .Find(filter)
        //        .Project<Plant>(projection)
        //        .Sort(sort)
        //        .ToList();
        //    return Request.CreateResponse(HttpStatusCode.OK,
        //        $"This plant is most probably a '{plants.FirstOrDefault()?.Name ?? "nothing"}'");

        //}

        //[Route("SubmitImage")]
        //[HttpPost]
        //public async Task<HttpResponseMessage> SubmitImage()
        //{
        //    Check if the request contains multipart / form - data.
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }

        //    using (var context = new SuperOnlineDbContext())
        //    {
        //        try
        //        {
        //            string token = Request.Headers.Single(h => h.Key == "token").Value.FirstOrDefault();

        //            var shop = (from s in context.Shops
        //                        where s.APIKey == token && !s.IsBanned
        //                        select s).FirstOrDefault();
        //            if (shop == null)
        //            {
        //                var blockPhoneNumber = (from x in context.AppConfigs
        //                                        select x.BlockedAccountPhoneNumber).FirstOrDefault();

        //                return Request.CreateResponse(HttpStatusCode.Unauthorized, new { block_phone_number = blockPhoneNumber });
        //            }

        //            var provider = await Request.Content.ReadAsMultipartAsync<InMemoryMultipartFormDataStreamProvider>(new InMemoryMultipartFormDataStreamProvider());
        //            //access form data  
        //            NameValueCollection formData = provider.FormData;
        //            //access files  
        //            IList<HttpContent> files = provider.Files;

        //            string firstName = formData.Get("name");
        //            string lastName = formData.Get("family");
        //            string shopName = formData.Get("shop_name");
        //            string email = formData.Get("email");
        //            int region = int.Parse(formData.Get("area_code"));

        //            string geoLatString = formData.Get("lat");
        //            string geoLongString = formData.Get("long");

        //            double geoLat = (string.IsNullOrEmpty(geoLatString) || geoLatString == "0") ? 0 : double.Parse(geoLatString);
        //            double geoLong = (string.IsNullOrEmpty(geoLongString) || geoLongString == "0") ? 0 : double.Parse(geoLongString);
        //            int minOrder = int.Parse(formData.Get("min_order"));
        //            int deliveryPrice = int.Parse(formData.Get("courier_fee"));

        //            HttpContent avatarFile = files[0];
        //            string originalFileName = avatarFile.Headers.ContentDisposition.FileName.Trim('\"');
        //            string extension = Path.GetExtension(originalFileName);

        //            if (extension.ToLower() != ".jpg" && extension.ToLower() != ".png" && extension.ToLower() != ".jpeg")
        //            {
        //                return Request.CreateResponse(HttpStatusCode.BadRequest, new { err_code = ApplicationConstants.HttpErrors.UNSUPPORTED_FILE_EXTENSION });
        //            }
        //            else
        //            {
        //                List<ProductInput> allProducts = (from p in context.Products
        //                                                  select new ProductInput { ProductID = p.ProductID, Price = p.InitialPrice }).ToList();
        //                //saving the image file
        //                string directory = HostingEnvironment.MapPath("~/" + AVATAR_FOLDER_PATH);
        //                string newFileName = GenerateRandomString(RANDOM_STRING_LENGTH) + extension;
        //                string FullPath = Path.Combine(directory, newFileName);
        //                while (File.Exists(FullPath))
        //                {
        //                    newFileName = GenerateRandomString(RANDOM_STRING_LENGTH) + extension;
        //                    FullPath = Path.Combine(directory, newFileName);
        //                }
        //                Stream input = await avatarFile.ReadAsStreamAsync();
        //                using (Stream file = File.OpenWrite(FullPath))
        //                {
        //                    input.CopyTo(file);
        //                    file.Close();
        //                }

        //                shop.FirstName = firstName;
        //                shop.LastName = lastName;
        //                shop.FullName = firstName + " " + lastName;
        //                shop.Name = shopName;
        //                shop.Email = email == string.Empty ? null : email;
        //                shop.Region = region;
        //                shop.GeoLat = geoLat;
        //                shop.GeoLong = geoLong;
        //                shop.ImageUrl = "/" + AVATAR_FOLDER_PATH + newFileName;
        //                shop.DeliveryPrice = deliveryPrice;
        //                shop.MinDelivery = minOrder;
        //                shop.IsInActivationQueue = true;
        //                shop.Credit = 0;
        //                context.Stocks.AddRange(
        //                    allProducts.Select(ap => new Stock { ShopID = shop.ShopID, ProductID = ap.ProductID, Price = ap.Price, IsAvailable = false })
        //                    );
        //                context.SaveChanges();

        //                return Request.CreateResponse(HttpStatusCode.OK, new { message = "اطلاعات با موفقیت ثبت شد" });
        //            }
        //        }
        //        catch (InvalidOperationException)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.BadRequest);
        //        }
        //        catch
        //        {
        //            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { err_code = ApplicationConstants.HttpErrors.DB_RETRIVAL_ERROR });
        //        }
        //    }
        //}

        //[Route("test")]
        //[HttpPost]
        //public HttpResponseMessage Test([FromBody] Inputs.SearchInput searchInput)
        //{
        //    var client = new MongoClient("mongodb://localhost:27017");
        //    var db = client.GetDatabase("agile_gardeing_db");
        //    var filter = Builders<Plant>.Filter.Text(searchInput.Query);
        //    var projection = Builders<Plant>.Projection.MetaTextScore("score");
        //    var sort = Builders<Plant>.Sort.MetaTextScore("score");
        //    List<Plant> plants = db.GetCollection<Plant>("plants")
        //        .Find(filter)
        //        .Project<Plant>(projection)
        //        .Sort(sort)
        //        .ToList();
        //    return Request.CreateResponse(HttpStatusCode.OK, plants);

        //}

        public string GenerateRandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string ConvertToBase64(string path, string extension)
        {
            var file = File.OpenRead(path);
            BinaryReader br = new BinaryReader(file);
            Byte[] bytes = br.ReadBytes((Int32)file.Length);
            return Convert.ToBase64String(bytes, 0, bytes.Length);
            //return $"data:image/{extension.TrimStart('.')};base64,{base64String}";
        }
    }
}
