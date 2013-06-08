using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TchillrREST.Data
{
    public class TchillrDBContext : DbContext
    {
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Occurence> Occurences { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public TchillrDBContext(string connectionString)
            : base(connectionString)
        {
            Database.CreateIfNotExists();
        }

    }
}