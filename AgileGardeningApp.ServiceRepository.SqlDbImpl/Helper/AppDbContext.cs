using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileGardeningApp.Models;

namespace AgileGardeningApp.ServiceRepository.SqlDbImpl.Helper
{
    class AppDbContext : DbContext
    {
        public AppDbContext() : base ("name=db_connection") { }
        public DbSet<User> Users { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<UsersPlantsInfo> UsersPlantsInfos { get; set; }
    }
}
