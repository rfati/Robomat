using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using DAL.Migrations;
using Model;

namespace DAL
{
    public class RobomatContext : DbContext
    {
        //public RobomatContext() : base("RobomatContext")
        //{
        //    Database.SetInitializer<RobomatContext>(new CreateDatabaseIfNotExists<RobomatContext>());
        //    this.Configuration.LazyLoadingEnabled = false;
        //}


        public RobomatContext() : base("data source=localhost;initial catalog=ROBOMAT;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework")
        {
            //Database.SetInitializer<RobomatContext>(new CreateDatabaseIfNotExists<RobomatContext>());
            this.Configuration.LazyLoadingEnabled = false;
        }


        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<RafBolme> RafBolmes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

        }

    }
}
