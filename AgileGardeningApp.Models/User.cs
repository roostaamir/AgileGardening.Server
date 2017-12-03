using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileGardeningApp.Models
{
    public class User
    {
        public User()
        {
            this.UsersPlantsInfos = new HashSet<UsersPlantsInfo>();
        }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }

        public string LoginToken { get; set; }

        public ICollection<UsersPlantsInfo> UsersPlantsInfos { get; set; }
    }
}
