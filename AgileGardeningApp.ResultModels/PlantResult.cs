using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileGardeningApp.ResultModels
{
    public class PlantResult
    {
        public int PlantId { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Info { get; set; }

        public bool IsFavorite { get; set; }
    }
}
