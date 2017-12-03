using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgileGardeningApp.Models
{
    public class Plant
    {
        public Plant()
        {
            this.UsersPlantsInfos = new HashSet<UsersPlantsInfo>();
        }

        [Required]
        public int PlantId { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public string Types { get; set; }

        [Required]
        public string Keywords { get; set; }

        public string Imageurl { get; set; }

        public string Description { get; set; }

        public ICollection<UsersPlantsInfo> UsersPlantsInfos { get; set; }
    }
}