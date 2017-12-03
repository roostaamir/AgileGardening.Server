using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileGardeningApp.ExteranlApis.VisionApi
{
    public interface IVisionService
    {
        string GetQueryKeywords(string imageData);
    }
}
