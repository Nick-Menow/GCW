using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WorkDB.Models;


namespace WorkDB.DAL
{
    public class TournamentContext : DbContext
    {
       public TournamentContext() : base("TournamentContext")
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        
    }
}