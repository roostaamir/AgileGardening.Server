using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace AgileGardeningApp.ExteranlApis.VisionApi
{
    public class ClarifaiVisionApi : IVisionService
    {
        internal class ConceptList
        {
            public List<Concept> Concepts { get; set; }
        }

        internal class Concept
        {
            public string id { get; set; }
            public string name { get; set; }
            public double value { get; set; }
            public string app_id { get; set; }
        }

        internal class VisionInput
        {
            public class UrlHolder
            {
                public string base64 { get; set; }
            }

            public class ImageDataHolder
            {
                public UrlHolder image { get; set; }
            }

            public class DataHolder
            {
                public ImageDataHolder data { get; set; }
            }

            public class RootInput
            {
                public List<DataHolder> inputs { get; set; }
            }
        }

        private const string API_ROUTE_PREFIX = "https://api.clarifai.com/v2/";
        private const string API_ROUTE = "models/aaa03c23b3724a16a56b629203edc62c/outputs";
        private const string HEADER_AUTH_KEY = "Authorization";
        private const string HEADER_AUTH_VALUE = "Key <CLARIFAI_PRIVATE_KEY>";
        private const string HEADER_CONTENT_TYPE_KEY = "Content-Type";
        private const string HEADER_CONTENT_TYPE_VALUE_JSON = "application/json";

        public string GetQueryKeywords(string imageData)
        {
            var conceptList = GetConceptList(imageData);
            return CreateKeywordQueryBuilder(conceptList).ToString();
        }

        private StringBuilder CreateKeywordQueryBuilder(ConceptList conceptList)
        {
            var searchQueryBuilder = new StringBuilder();
            foreach (var concept in conceptList.Concepts)
            {
                searchQueryBuilder.Append(concept.name).Append(" ");
            }
            searchQueryBuilder.Length -= 1;
            return searchQueryBuilder;
        }

        private ConceptList GetConceptList(string imageData)
        {
            var restClient = new RestClient(API_ROUTE_PREFIX);
            var request = new RestRequest(API_ROUTE, Method.POST);

            var visionInput = CreateVisionInput(imageData);
            request.AddJsonBody(visionInput);
            request.AddHeader(HEADER_AUTH_KEY, HEADER_AUTH_VALUE);
            request.AddHeader(HEADER_CONTENT_TYPE_KEY, HEADER_CONTENT_TYPE_VALUE_JSON);

            IRestResponse response = restClient.Execute(request);
            JObject responseRoot = JObject.Parse(response.Content);
            string concepts = responseRoot["outputs"][0]["data"].ToString();
            var result = JsonConvert.DeserializeObject<ConceptList>(concepts);
            return result;
        }

        private VisionInput.RootInput CreateVisionInput(string imageData)
        {
            VisionInput.RootInput visionInput = new VisionInput.RootInput()
            {
                inputs = new List<VisionInput.DataHolder>()
                {
                    new VisionInput.DataHolder()
                    {
                        data = new VisionInput.ImageDataHolder()
                        {
                            image = new VisionInput.UrlHolder()
                            {
                                base64 = imageData
                            }
                        }
                    }
                }
            };

            return visionInput;
        }
    }
}
