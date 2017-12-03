using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileGardeningApp.Models
{
    public class UsersPlantsInfo
    {
        [Required]
        public int UsersPlantsInfoId { get; set; }

        public User User { get; set; }

        public Plant Plant { get; set; }

        public bool IsFavourite { get; set; }
    }
}
