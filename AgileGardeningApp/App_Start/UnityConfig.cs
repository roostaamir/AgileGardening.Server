using Microsoft.Practices.Unity;
using System.Web.Http;
using AgileGardeningApp.ExteranlApis.VisionApi;
using AgileGardeningApp.ServiceRepository.Interface;
using AgileGardeningApp.ServiceRepository.SqlDbImpl;
using Unity.WebApi;

namespace AgileGardeningApp
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IVisionService, ClarifaiVisionApi>();
            container.RegisterType<IPlantInfoService, PlantsInfoServiceRepository>();
            container.RegisterType<IUserInfoService, UserInfoService>();
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}