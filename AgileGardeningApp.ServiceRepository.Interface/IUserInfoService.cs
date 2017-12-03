using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileGardeningApp.ServiceRepository.Interface
{
    public interface IUserInfoService
    {
        string LoginUser(string email, string password);
        void SignUpUser(string email, string password);
    }
}
