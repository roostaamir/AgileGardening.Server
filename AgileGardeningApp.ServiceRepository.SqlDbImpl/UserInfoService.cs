using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AgileGardeningApp.ServiceRepository.Interface;
using AgileGardeningApp.ServiceRepository.SqlDbImpl.Helper;

namespace AgileGardeningApp.ServiceRepository.SqlDbImpl
{
    public class UserInfoService : IUserInfoService
    {
        public string LoginUser(string email, string password)
        {
            using (var context = new AppDbContext())
            {
                var user = (from x in context.Users
                    where x.EmailAddress == email && x.Password == password
                    select x).FirstOrDefault();

                if (user == null) return null;

                var newLoginToken = GenerateApiKey(email);
                user.LoginToken = newLoginToken;
                context.SaveChanges();

                return newLoginToken;
            }
        }

        public void SignUpUser(string email, string password)
        {
            throw new NotImplementedException();
        }

        private string GenerateApiKey(string salt)
        {
            Guid guid = Guid.NewGuid();
            var hash = new HMACMD5(Encoding.UTF8.GetBytes(salt));
            byte[] hashBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(guid.ToString()));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
